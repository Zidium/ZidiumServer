using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Zidium.Common;

namespace Zidium.Storage.Ef
{
    internal class SendEmailCommandRepository : ISendEmailCommandRepository
    {
        public SendEmailCommandRepository(Storage storage)
        {
            _storage = storage;
        }

        private readonly Storage _storage;

        public void Add(SendEmailCommandForAdd entity)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                contextWrapper.Context.SendEmailCommands.Add(new DbSendEmailCommand()
                {
                    Id = entity.Id,
                    Status = entity.Status,
                    CreateDate = entity.CreateDate,
                    Body = entity.Body,
                    SendDate = entity.SendDate,
                    Subject = entity.Subject,
                    ReferenceId = entity.ReferenceId,
                    To = entity.To,
                    IsHtml = entity.IsHtml,
                    ErrorMessage = entity.ErrorMessage,
                    From = entity.From
                });
                contextWrapper.Context.SaveChanges();
            }
        }

        public SendEmailCommandForRead GetOneById(Guid id)
        {
            return DbToEntity(DbGetOneById(id));
        }

        public SendEmailCommandForRead[] GetForSend(int maxCount)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                return contextWrapper.Context.SendEmailCommands.AsNoTracking()
                    .Where(x => x.Status == EmailStatus.InQueue)
                    .OrderBy(x => x.CreateDate)
                    .Take(maxCount)
                    .AsEnumerable()
                    .Select(DbToEntity)
                    .ToArray();
            }
        }

        public void MarkAsSendSuccessed(Guid id, DateTime now)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                var command = DbGetOneById(id);
                command.Status = EmailStatus.Sent;
                command.SendDate = now;
                command.ErrorMessage = null;
                contextWrapper.Context.SaveChanges();
            }
        }

        public void MarkAsSendFail(Guid id, DateTime now, string error)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                var command = DbGetOneById(id);
                command.Status = EmailStatus.Error;
                command.SendDate = now;
                command.ErrorMessage = error;
                contextWrapper.Context.SaveChanges();
            }
        }

        private DbSendEmailCommand DbGetOneOrNullById(Guid id)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                return contextWrapper.Context.SendEmailCommands.Find(id);
            }
        }

        private DbSendEmailCommand DbGetOneById(Guid id)
        {
            var result = DbGetOneOrNullById(id);

            if (result == null)
                throw new ObjectNotFoundException($"Команда отправки EMail {id} не найдена");

            return result;
        }

        private SendEmailCommandForRead DbToEntity(DbSendEmailCommand entity)
        {
            if (entity == null)
                return null;

            return new SendEmailCommandForRead(entity.Id, entity.From, entity.To, entity.Subject, entity.Body,
                entity.IsHtml, entity.Status, entity.ErrorMessage, entity.CreateDate, entity.SendDate, entity.ReferenceId);
        }
    }
}
