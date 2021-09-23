using System;
using Zidium.Api.Dto;
using Zidium.Api.Others;
using Zidium.Common;
using Zidium.Storage;

namespace Zidium.Core.AccountsDb
{
    public class DefectService : IDefectService
    {
        public DefectService(IStorage storage)
        {
            _storage = storage;
        }

        private readonly IStorage _storage;

        protected int GetNextDefectNumber()
        {
            var count = _storage.Defects.GetCount();
            return count + 1;
        }

        public DefectForRead GetOrCreateDefectForEventType(EventTypeForRead eventType,
            UserForRead createUser,
            UserForRead responsibleUser,
            string comment)
        {
            lock (typeof(DefectService))
            {
                using (var transaction = _storage.BeginTransaction())
                {
                    var defectId = eventType.DefectId;
                    if (defectId == null)
                    {
                        var title = eventType.DisplayName;
                        StringHelper.SetMaxLength(ref title, 500);
                        defectId = CreateDefectInternal(title, createUser, comment, responsibleUser, null);

                        var defectForUpdate = new DefectForUpdate(defectId.Value);
                        defectForUpdate.EventTypeId.Set(eventType.Id);
                        _storage.Defects.Update(defectForUpdate);

                        var eventTypeForUpdate = eventType.GetForUpdate();
                        eventTypeForUpdate.DefectId.Set(defectId);
                        _storage.EventTypes.Update(eventTypeForUpdate);
                    }

                    var defect = _storage.Defects.GetOneById(defectId.Value);
                    if (defect != null && defect.LastChangeId == null)
                    {
                        ChangeStatus(defect, DefectStatus.Open, createUser, comment);
                    }

                    transaction.Commit();

                    return defect;
                }
            }
        }

        public Guid ChangeStatus(DefectForRead defect,
            DefectStatus status,
            UserForRead user,
            string comment)
        {
            var change = new DefectChangeForAdd()
            {
                Id = Ulid.NewUlid(),
                Status = status,
                Comment = comment,
                Date = DateTime.Now,
                UserId = user.Id,
                DefectId = defect.Id
            };
            _storage.DefectChanges.Add(change);

            var defectForUpdate = defect.GetForUpdate();
            defectForUpdate.LastChangeId.Set(change.Id);
            _storage.Defects.Update(defectForUpdate);

            // Если дефект закрывается, то нужно внести информацию о версии в тип события
            if (status == DefectStatus.Closed && defect.EventTypeId != null)
            {
                var lastEvent = _storage.Events.GetLastEventByEndDate(defect.EventTypeId.Value);
                var version = lastEvent?.Version;

                if (!string.IsNullOrEmpty(version))
                {
                    var eventTypeForUpdate = new EventTypeForUpdate(defect.EventTypeId.Value);
                    eventTypeForUpdate.OldVersion.Set(version);
                    eventTypeForUpdate.ImportanceForOld.Set(EventImportance.Unknown);
                    _storage.EventTypes.Update(eventTypeForUpdate);
                }
            }

            return change.Id;
        }

        public Guid AutoReopen(DefectForRead defect)
        {
            var userService = new UserService(_storage);
            var user = userService.GetAccountAdmin();
            return ChangeStatus(defect, DefectStatus.Reopened, user, "Автоматически переоткрыт из-за новой ошибки");
        }

        public DefectStatus GetStatus(Guid defectId)
        {
            return _storage.DefectChanges.GetLastByDefectId(defectId)?.Status ?? DefectStatus.Unknown;
        }

        public Guid CreateAndCloseDefectForEventType(EventTypeForRead eventType, UserForRead user)
        {
            var defect = GetOrCreateDefectForEventType(eventType, user, user, null);
            ChangeStatus(defect, DefectStatus.Closed, user, null);
            return defect.Id;
        }

        private Guid CreateDefectInternal(string title, UserForRead createUser, string statusComment, UserForRead responsibleUser, string notes)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                throw new UserFriendlyException("Укажите название дефекта");
            }
            if (title.Length > 500)
            {
                throw new UserFriendlyException("Название дефекта не должно быть длиннее 500 символов");
            }
            lock (typeof(DefectService))
            {
                int number = GetNextDefectNumber();
                var defectForAdd = new DefectForAdd()
                {
                    Id = Ulid.NewUlid(),
                    Number = number,
                    Title = title,
                    Notes = notes,
                    ResponsibleUserId = responsibleUser.Id,
                };

                _storage.Defects.Add(defectForAdd);

                var defect = _storage.Defects.GetOneById(defectForAdd.Id);
                ChangeStatus(defect, DefectStatus.Open, createUser, statusComment);

                return defectForAdd.Id;
            }
        }

        public Guid CreateDefect(string title, UserForRead createUser, UserForRead responsibleUser, string notes)
        {
            var defect = CreateDefectInternal(title, createUser, notes, responsibleUser, null);
            return defect;
        }
    }
}
