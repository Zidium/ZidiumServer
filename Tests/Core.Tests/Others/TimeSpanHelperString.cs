using System;
using Xunit;
using Zidium.Core.Common.Helpers;

namespace Zidium.Core.Tests.Others
{
    public class TimeSpanHelperString : BaseTest
    {
        [Fact]
        public void Test()
        {
            Assert.Equal("5 дней", TimeSpanHelper.Get2UnitsString(TimeSpan.FromDays(5.1)));
            Assert.Equal("2 дня 2 часа", TimeSpanHelper.Get2UnitsString(TimeSpan.FromHours(50)));
            Assert.Equal("10 часов", TimeSpanHelper.Get2UnitsString(TimeSpan.FromHours(10)));
            Assert.Equal("2 часа 30 мин", TimeSpanHelper.Get2UnitsString(TimeSpan.FromHours(2.5)));
            Assert.Equal("30 мин", TimeSpanHelper.Get2UnitsString(TimeSpan.FromMinutes(30)));
            Assert.Equal("2 мин 30 сек", TimeSpanHelper.Get2UnitsString(TimeSpan.FromMinutes(2.5)));
            Assert.Equal("30 сек", TimeSpanHelper.Get2UnitsString(TimeSpan.FromSeconds(30)));
        }
    }
}
