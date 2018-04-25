using System;
using System.Linq;
using Zidium.Api.Others;
using Zidium.Core.AccountsDb.Classes;
using Zidium.Core.Common;

namespace Zidium.Core.AccountsDb
{
    internal class DefectService : IDefectService
    {
        private AccountDbContext _accountDbContext;

        public DefectService(AccountDbContext accountDbContext)
        {
            if (accountDbContext == null)
            {
                throw new ArgumentNullException("accountDbContext");
            }
            _accountDbContext = accountDbContext;
        }

        protected int GetNextDefectNumber()
        {
            using (var accountDbContext = _accountDbContext.Clone())
            {
                var count = accountDbContext.Defects.Count();
                return count + 1;
            }
        }

        public Defect GetOrCreateDefectForEventType(
            Guid accountId,
            EventType eventType, 
            User createUser, 
            User responsibleUser, 
            string comment)
        {
            lock (typeof(DefectService))
            {
                if (eventType.Defect == null)
                {
                    var title = eventType.DisplayName;
                    StringHelper.SetMaxLength(ref title, 500);
                    var defect2 = CreateDefectInternal(accountId, title, createUser, comment, responsibleUser, null);
                    //defect2.EventType = eventType;
                    defect2.EventTypeId = eventType.Id;
                    //eventType.Defect = defect2;
                    eventType.DefectId = defect2.Id;
                    _accountDbContext.SaveChanges();
                }
                var defect = eventType.Defect;
                if (defect != null && defect.LastChange == null)
                {
                    ChangeStatus(accountId, defect, DefectStatus.Open, createUser, comment);
                }
                return defect;
            }
        }

        public DefectChange ChangeStatus(
            Guid accountId,
            Defect defect, 
            DefectStatus status, 
            User user, 
            string comment)
        {
            var change = new DefectChange()
            {
                Id = Guid.NewGuid(),
                Status = status,
                //Defect = defect,
                //User = user,
                Comment = comment,
                Date = DateTime.Now,
                UserId = user.Id,
                DefectId = defect.Id
            };
            defect.LastChange = change;

            // Если дефект закрывается, то нужно внести информацию о версии в тип события
            if (status == DefectStatus.Closed && defect.EventType != null)
            {
                string version;
                using (var context = AccountDbContext.CreateFromAccountId(accountId))
                {
                    var repository = context.GetEventRepository();
                    var lastEvent = repository.GetLastEventByEndDate(defect.EventType.Id);
                    version = lastEvent != null ? lastEvent.Version : null;
                }

                if (!string.IsNullOrEmpty(version))
                {
                    defect.EventType.OldVersion = version;
                    defect.EventType.ImportanceForOld = Api.EventImportance.Unknown;
                }
            }

            return change;
        }

        public DefectChange AutoReopen(Guid accountId, Defect defect)
        {
            var userRepository = _accountDbContext.GetUserRepository();
            var user = userRepository.GetAccountAdmin();
            return ChangeStatus(accountId, defect, DefectStatus.ReOpen, user, "Автоматически переоткрыт из-за новой ошибки");
        }

        public Defect CreateAndCloseDefectForEventType(Guid accountId, EventType eventType, User user)
        {
            var defect = GetOrCreateDefectForEventType(accountId, eventType, user, user, null);
            ChangeStatus(accountId, defect, DefectStatus.Closed, user, null);
            return defect;
        }

        private Defect CreateDefectInternal(Guid accountId, string title, User createUser, string statusComment, User responsibleUser, string notes)
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
                var defect = new Defect()
                {
                    Id = Guid.NewGuid(),
                    Number = number,
                    Title = title,
                    Notes = notes,
                    ResponsibleUserId = responsibleUser.Id,
                    //ResponsibleUser = responsibleUser
                };
                _accountDbContext.Defects.Add(defect);
                _accountDbContext.SaveChanges();
                ChangeStatus(accountId, defect, DefectStatus.Open, createUser, statusComment);
                _accountDbContext.SaveChanges();
                return defect;
            }
        }

        public Defect CreateDefect(Guid accountId, string title, User createUser, User responsibleUser, string notes)
        {
            var defect = CreateDefectInternal(accountId, title, createUser, notes, responsibleUser, null);
            return defect;
        }
    }
}
