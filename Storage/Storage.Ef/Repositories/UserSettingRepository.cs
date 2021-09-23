using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Zidium.Common;

namespace Zidium.Storage.Ef
{
    internal class UserSettingRepository : IUserSettingRepository
    {
        public UserSettingRepository(Storage storage)
        {
            _storage = storage;
        }

        private readonly Storage _storage;

        public string GetValue(Guid userId, string name)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                var entity = contextWrapper.Context.UserSettings.AsNoTracking()
                    .FirstOrDefault(t => t.UserId == userId && t.Name.ToLower() == name.ToLower());

                return entity?.Value;
            }
        }

        public void SetValue(Guid userId, string name, string value)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                var entity = contextWrapper.Context.UserSettings
                    .FirstOrDefault(t => t.UserId == userId && t.Name.ToLower() == name.ToLower());

                if (entity == null)
                {
                    entity = new DbUserSetting()
                    {
                        Id = Ulid.NewUlid(),
                        UserId = userId,
                        Name = name
                    };
                    contextWrapper.Context.UserSettings.Add(entity);
                }

                entity.Value = value;

                contextWrapper.Context.SaveChanges();
            }
        }
    }
}
