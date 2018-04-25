using System;
using Xunit;
using Zidium.Core.Common.Helpers;

namespace Zidium.Core.Tests.Others
{
    public class DateTimeHelperTests
    {
        [Fact]
        public void ParseFromUrl()
        {
            Assert.Equal(new DateTime(2001, 10, 7), DateTimeHelper.FromUrlFormat("2001-10-07"));
            Assert.Equal(new DateTime(2001, 10, 7), DateTimeHelper.FromUrlFormat("2001-10-07_00.00.00"));
            Assert.Equal(new DateTime(2001, 10, 7, 23, 24, 25), DateTimeHelper.FromUrlFormat("2001-10-07_23.24.25"));
        }

        [Fact]
        public void ToUrlFormat()
        {
            Assert.Equal("2001-10-07_00.00.00", DateTimeHelper.ToUrlFormat(new DateTime(2001, 10, 7)));
            Assert.Equal("2001-10-07_23.24.25", DateTimeHelper.ToUrlFormat(new DateTime(2001, 10, 7, 23, 24, 25)));
        }
    }
}
