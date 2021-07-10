using System;
using Microsoft.AspNetCore.Mvc;

namespace Zidium.UserAccount
{
    public class CanEditAllDataAttribute : TypeFilterAttribute
    {
        public CanEditAllDataAttribute() : base(typeof(CanEditAllDataFilter))
        {
        }
    }
}
