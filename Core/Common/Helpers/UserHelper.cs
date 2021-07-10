using System;
using System.Linq;
using Zidium.Core.Common;
using Zidium.Storage;

namespace Zidium.Core
{
    public static class UserHelper
    {
        public static UserContactForRead[] GetUserContactsOfType(Guid userId, string login, DateTime createDate, UserContactType contactType, IStorage storage)
        {
            var result = storage.UserContacts.GetByType(userId, contactType)
                .Where(x => !string.IsNullOrEmpty(x.Value))
                .ToArray();

            if (result.Length == 0 && contactType == UserContactType.Email)
            {
                if (ValidationHelper.IsEmail(login))
                {
                    result = new[]
                    {
                        new UserContactForRead(Guid.Empty, userId, UserContactType.Email, login, createDate)
                    };
                }
            }

            return result;
        }

        public static UserContactForRead[] GetUserContactsOfType(UserForRead user, UserContactType contactType, IStorage storage)
        {
            return GetUserContactsOfType(user.Id, user.Login, user.CreateDate, contactType, storage);
        }
    }
}
