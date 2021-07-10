using System;
using Microsoft.AspNetCore.Mvc;

namespace Zidium.UserAccount
{
    public class CanEditPrivateDataAttribute : TypeFilterAttribute
    {
        public CanEditPrivateDataAttribute() : base(typeof(CanEditPrivateDataFilter))
        {
        }
    }
}
