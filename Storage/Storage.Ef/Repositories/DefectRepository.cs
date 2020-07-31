using System;
using System.Linq;
using Zidium.Common;

namespace Zidium.Storage.Ef
{
    internal class DefectRepository : IDefectRepository
    {
        public DefectRepository(Storage storage)
        {
            _storage = storage;
        }

        private readonly Storage _storage;

        public void Add(DefectForAdd entity)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                contextWrapper.Context.Defects.Add(new DbDefect()
                {
                    Id = entity.Id,
                    EventTypeId = entity.EventTypeId,
                    Number = entity.Number,
                    LastChangeId = entity.LastChangeId,
                    Notes = entity.Notes,
                    ResponsibleUserId = entity.ResponsibleUserId,
                    Title = entity.Title
                });
                contextWrapper.Context.SaveChanges();
            }
        }

        public void Update(DefectForUpdate entity)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                var defect = DbGetOneById(entity.Id);

                if (entity.Number.Changed())
                    defect.Number = entity.Number.Get();

                if (entity.Title.Changed())
                    defect.Title = entity.Title.Get();

                if (entity.LastChangeId.Changed())
                    defect.LastChangeId = entity.LastChangeId.Get();

                if (entity.ResponsibleUserId.Changed())
                    defect.ResponsibleUserId = entity.ResponsibleUserId.Get();

                if (entity.EventTypeId.Changed())
                    defect.EventTypeId = entity.EventTypeId.Get();

                if (entity.Notes.Changed())
                    defect.Notes = entity.Notes.Get();

                contextWrapper.Context.SaveChanges();
            }
        }

        public DefectForRead GetOneById(Guid id)
        {
            return DbToEntity(DbGetOneById(id));
        }

        public DefectForRead GetOneOrNullById(Guid id)
        {
            return DbToEntity(DbGetOneOrNullById(id));
        }

        public int GetCount()
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                return contextWrapper.Context.Defects.Count();
            }
        }

        private DbDefect DbGetOneOrNullById(Guid id)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                return contextWrapper.Context.Defects.Find(id);
            }
        }

        private DbDefect DbGetOneById(Guid id)
        {
            var result = DbGetOneOrNullById(id);

            if (result == null)
                throw new ObjectNotFoundException($"Дефект {id} не найден");

            return result;
        }

        private DefectForRead DbToEntity(DbDefect entity)
        {
            if (entity == null)
                return null;

            return new DefectForRead(entity.Id, entity.Number, entity.Title, entity.LastChangeId, entity.ResponsibleUserId,
                entity.EventTypeId, entity.Notes);
        }
    }
}
