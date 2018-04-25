using Xunit;
using Zidium.Core.Common.Helpers;

namespace Zidium.Core.Tests.Others
{
    public class DataSizeHelperTests
    {
        [Fact]
        public void GetDataSizeTest()
        {
            // целые размеры
            Assert.Equal("0 байт", DataSizeHelper.GetSizeText(0));
            Assert.Equal("10 байт", DataSizeHelper.GetSizeText(10));
            Assert.Equal("100 байт", DataSizeHelper.GetSizeText(100));
            Assert.Equal("1 000 байт", DataSizeHelper.GetSizeText(1000));

            Assert.Equal("1 КБ", DataSizeHelper.GetSizeText(1024));
            Assert.Equal("10 КБ", DataSizeHelper.GetSizeText(10240));
            Assert.Equal("100 КБ", DataSizeHelper.GetSizeText(102400));
            Assert.Equal("1 000 КБ", DataSizeHelper.GetSizeText(1024000));

            Assert.Equal("1 МБ", DataSizeHelper.GetSizeText(1048576));
            Assert.Equal("10 МБ", DataSizeHelper.GetSizeText(10485760));
            Assert.Equal("100 МБ", DataSizeHelper.GetSizeText(104857600));
            Assert.Equal("1 000 МБ", DataSizeHelper.GetSizeText(1048576000));

            Assert.Equal("1 ГБ", DataSizeHelper.GetSizeText(1073741824));
            Assert.Equal("10 ГБ", DataSizeHelper.GetSizeText(10737418240));
            Assert.Equal("100 ГБ", DataSizeHelper.GetSizeText(107374182400));
            Assert.Equal("1 000 ГБ", DataSizeHelper.GetSizeText(1073741824000));
            Assert.Equal("1 024 ГБ", DataSizeHelper.GetSizeText(1099511627776));
            Assert.Equal("2 048 ГБ", DataSizeHelper.GetSizeText(1099511627776 * 2));

            // дробные размеры
            Assert.Equal("1.1 КБ", DataSizeHelper.GetSizeText(1024 + 102));
            Assert.Equal("1.15 КБ", DataSizeHelper.GetSizeText(1024 + 150));
            Assert.Equal("100 КБ", DataSizeHelper.GetSizeText(1024*100 + 102));
        }
    }
}
