using System.Collections.Generic;
using System.Linq;
using Zidium.Core.AccountsDb;
using Zidium.Core.Common;

namespace Zidium.Core
{
    public static class UserHelper
    {
        public static List<UserContact> GetUserContactsOfType(User user, UserContactType contactType)
        {
            var result = user
                .UserContacts.Where(x => x.Type == contactType && !string.IsNullOrEmpty(x.Value))
                .ToList();

            if (result.Count == 0 && contactType == UserContactType.Email)
            {
                var login = user.Login;
                if (ValidationHelper.IsEmail(login))
                {
                    result.Add(new UserContact()
                    {
                        Type = UserContactType.Email,
                        Value = login
                    });
                }
            }

            return result;
        }
    }
}
