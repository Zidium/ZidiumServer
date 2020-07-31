using System;
using System.Linq;
using Zidium.Common;

namespace Zidium.Storage.Ef
{
    internal class SendSmsCommandRepository : ISendSmsCommandRepository
    {
        public SendSmsCommandRepository(Storage storage)
        {
            _storage = storage;
        }

        private readonly Storage _storage;

        public void Add(SendSmsCommandForAdd entity)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                contextWrapper.Context.SendSmsCommands.Add(new DbSendSmsCommand()
                {
                    Id = entity.Id,
                    Status = entity.Status,
                    CreateDate = entity.CreateDate,
                    Body = entity.Body,
                    ReferenceId = entity.ReferenceId,
                    SendDate = entity.SendDate,
                    ErrorMessage = entity.ErrorMessage,
                    Phone = entity.Phone,
                    ExternalId = entity.ExternalId
                });
                contextWrapper.Context.SaveChanges();
            }
        }

        public SendSmsCommandForRead[] GetForSend(int maxCount)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                return contextWrapper.Context.SendSmsCommands.AsNoTracking()
                    .Where(x => x.Status == SmsStatus.InQueue)
                    .OrderBy(x => x.CreateDate)
                    .Take(maxCount)
                    .AsEnumerable()
                    .Select(DbToEntity)
                    .ToArray();
            }
        }

        public void MarkAsSendSuccessed(Guid id, DateTime now, string externalId)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                var command = DbGetOneById(id);
                command.Status = SmsStatus.Sent;
                command.SendDate = now;
                command.ErrorMessage = null;
                command.ExternalId = externalId;
                contextWrapper.Context.SaveChanges();
            }
        }

        public void MarkAsSendFail(Guid id, DateTime now, string error)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                var command = DbGetOneById(id);
                command.Status = SmsStatus.Error;
                command.SendDate = now;
                command.ErrorMessage = error;
                contextWrapper.Context.SaveChanges();
            }
        }

        private DbSendSmsCommand DbGetOneOrNullById(Guid id)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                return contextWrapper.Context.SendSmsCommands.Find(id);
            }
        }

        private DbSendSmsCommand DbGetOneById(Guid id)
        {
            var result = DbGetOneOrNullById(id);

            if (result == null)
                throw new ObjectNotFoundException($"Команда отправки sms {id} не найдена");

            return result;
        }

        private SendSmsCommandForRead DbToEntity(DbSendSmsCommand entity)
        {
            if (entity == null)
                return null;

            return new SendSmsCommandForRead(entity.Id, entity.Phone, entity.Body, entity.Status, entity.ErrorMessage,
                entity.CreateDate, entity.SendDate, entity.ReferenceId, entity.ExternalId);
        }
    }
}
