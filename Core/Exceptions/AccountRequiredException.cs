﻿using Zidium.Common;

namespace Zidium.Core
{
    public class AccountRequiredException : UserFriendlyException
    {
        public AccountRequiredException() : base ("Требуется явное указание аккаунта")
        {
            
        }
    }
}
