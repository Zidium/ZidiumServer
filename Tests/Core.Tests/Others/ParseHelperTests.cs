using System;
using Xunit;
using Zidium.Core.Common;

namespace Zidium.Core.Tests.Others
{
    public class ParseHelperTests : BaseTest
    {
        [Fact]
        public void DateTimeTest()
        {
            var date = new DateTime(2015, 12, 29, 23, 40, 59);
            Assert.Equal(date, ParseHelper.ParseDateTime("29.12.2015 23:40:59"));
            Assert.Equal(date, ParseHelper.ParseDateTime("29.12.2015.23.40.59")); // старый формат веб-форм
            Assert.Equal(date, ParseHelper.ParseDateTime("29.12.2015_23.40.59")); // новый формат веб-форм
        }
    }
}
