using System;
using System.Linq;
using Zidium.Common;

namespace Zidium.Storage.Ef
{
    internal class SendMessageCommandRepository : ISendMessageCommandRepository
    {
        public SendMessageCommandRepository(Storage storage)
        {
            _storage = storage;
        }

        private readonly Storage _storage;

        public void Add(SendMessageCommandForAdd entity)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                contextWrapper.Context.SendMessageCommands.Add(new DbSendMessageCommand()
                {
                    Id = entity.Id,
                    Status = entity.Status,
                    CreateDate = entity.CreateDate,
                    Body = entity.Body,
                    ReferenceId = entity.ReferenceId,
                    To = entity.To,
                    ErrorMessage = entity.ErrorMessage,
                    SendDate = entity.SendDate,
                    Channel = entity.Channel
                });
                contextWrapper.Context.SaveChanges();
            }
        }

        public SendMessageCommandForRead GetOneById(Guid id)
        {
            return DbToEntity(DbGetOneById(id));
        }

        public SendMessageCommandForRead[] GetForSend(SubscriptionChannel channel, int maxCount)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                return contextWrapper.Context.SendMessageCommands.AsNoTracking()
                    .Where(x => x.Channel == channel && x.Status == MessageStatus.InQueue)
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
                command.Status = MessageStatus.Sent;
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
                command.Status = MessageStatus.Error;
                command.SendDate = now;
                command.ErrorMessage = error;
                contextWrapper.Context.SaveChanges();
            }
        }

        private DbSendMessageCommand DbGetOneOrNullById(Guid id)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                return contextWrapper.Context.SendMessageCommands.Find(id);
            }
        }

        private DbSendMessageCommand DbGetOneById(Guid id)
        {
            var result = DbGetOneOrNullById(id);

            if (result == null)
                throw new ObjectNotFoundException($"Команда отправки сообщения {id} не найдена");

            return result;
        }

        private SendMessageCommandForRead DbToEntity(DbSendMessageCommand entity)
        {
            if (entity == null)
                return null;

            return new SendMessageCommandForRead(entity.Id, entity.Channel, entity.To, entity.Body, entity.Status,
                entity.ErrorMessage, entity.CreateDate, entity.SendDate, entity.ReferenceId);
        }
    }
}
