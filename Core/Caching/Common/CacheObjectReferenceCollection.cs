using System;
using System.Collections.Generic;
using System.Linq;

namespace Zidium.Core.Caching
{
    public class CacheObjectReferenceCollection : IReadOnlyReferenceCollection
    {
        protected Dictionary<string, CacheObjectReference> _all;
        public int Version { get; protected set; }

        public CacheObjectReferenceCollection()
        {
            _all = new Dictionary<string, CacheObjectReference>();
        }

        protected void SetDictionary(IEnumerable<CacheObjectReference> references)
        {
            if (references == null)
            {
                throw new ArgumentNullException("references");
            }
            _all = references.ToDictionary(x => x.SystemName, StringComparer.InvariantCultureIgnoreCase);
            Version++;
        }

        public int Count
        {
            get { return _all.Count; }
        }

        /// <summary>
        /// Кастыль для метрик (у них нет системного имени, но есть ид типа)
        /// </summary>
        /// <param name="typeId"></param>
        /// <returns></returns>
        public CacheObjectReference FindByMetricTypeId(Guid typeId)
        {
            return FindByName(typeId.ToString());
        }

        public CacheObjectReference FindByName(string systemName)
        {
            CacheObjectReference result = null;
            _all.TryGetValue(systemName, out result);
            return result;
        }

        public CacheObjectReference[] GetAll()
        {
            return _all.Values.ToArray();
        }

        public Guid[] GetIds()
        {
            return _all.Values.Select(x=>x.Id).ToArray();
        }

        public void Add(CacheObjectReference reference)
        {
            lock (this)
            {
                var list = _all.Values.ToList();
                list.Add(reference);
                SetDictionary(list);
            }    
        }

        public void AddRange(IEnumerable<CacheObjectReference> references)
        {
            if (references == null)
            {
                throw new ArgumentNullException("references");
            }
            lock (this)
            {
                var list = _all.Values.ToList();
                list.AddRange(references);
                SetDictionary(list);
            }
        }

        public void Rename(string oldName, string newName)
        {
            if (oldName == null)
            {
                throw new ArgumentNullException("oldName");
            }
            if (newName == null)
            {
                throw new ArgumentNullException("newName");
            }
            lock (this)
            {
                var oldObj = FindByName(oldName);
                if (oldObj == null)
                {
                    throw new Exception("Не удалось найти ссылку с именем " + oldName);
                }
                var newObj = new CacheObjectReference(oldObj.Id, newName);
                var list = _all.Values.Where(x => x.Id != oldObj.Id).ToList();
                list.Add(newObj);
                SetDictionary(list);
            }
        }

        public void Delete(Guid id)
        {
            lock (this)
            {
                var newRefs = _all.Values.Where(x => x.Id != id);
                SetDictionary(newRefs);
            }
        }

        public CacheObjectReferenceCollection GetCopy()
        {
            return new CacheObjectReferenceCollection()
            {
                _all = _all,
                Version = Version
            };
        }
    }
}
