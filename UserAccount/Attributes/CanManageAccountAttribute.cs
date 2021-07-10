using System;
using Microsoft.AspNetCore.Mvc;

namespace Zidium.UserAccount
{
    public class CanManageAccountAttribute : TypeFilterAttribute
    {
        public CanManageAccountAttribute() : base(typeof(CanManageAccountFilter))
        {
        }
    }
}
