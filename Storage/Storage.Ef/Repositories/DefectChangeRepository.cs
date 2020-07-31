using System;
using System.Linq;
using Zidium.Common;

namespace Zidium.Storage.Ef
{
    internal class DefectChangeRepository : IDefectChangeRepository
    {
        public DefectChangeRepository(Storage storage)
        {
            _storage = storage;
        }

        private readonly Storage _storage;

        public void Add(DefectChangeForAdd entity)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                contextWrapper.Context.DefectChanges.Add(new DbDefectChange()
                {
                    Id = entity.Id,
                    Status = entity.Status,
                    UserId = entity.UserId,
                    Comment = entity.Comment,
                    Date = entity.Date,
                    DefectId = entity.DefectId
                });
                contextWrapper.Context.SaveChanges();
            }
        }

        public DefectChangeForRead GetOneById(Guid id)
        {
            return DbToEntity(DbGetOneById(id));
        }

        public DefectChangeForRead GetLastByDefectId(Guid defectId)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                return DbToEntity(contextWrapper.Context.Defects.AsNoTracking().First(t => t.Id == defectId).LastChange);
            }
        }

        public DefectChangeForRead[] GetByDefectId(Guid defectId)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                return contextWrapper.Context.DefectChanges.AsNoTracking()
                    .Where(t => t.DefectId == defectId)
                    .AsEnumerable()
                    .Select(DbToEntity)
                    .ToArray();
            }
        }

        private DbDefectChange DbGetOneOrNullById(Guid id)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                return contextWrapper.Context.DefectChanges.Find(id);
            }
        }

        private DbDefectChange DbGetOneById(Guid id)
        {
            var result = DbGetOneOrNullById(id);

            if (result == null)
                throw new ObjectNotFoundException($"Изменение дефекта {id} не найдено");

            return result;
        }

        private DefectChangeForRead DbToEntity(DbDefectChange entity)
        {
            if (entity == null)
                return null;

            return new DefectChangeForRead(entity.Id, entity.DefectId, entity.Date, entity.Status, entity.Comment, entity.UserId);
        }
    }
}
