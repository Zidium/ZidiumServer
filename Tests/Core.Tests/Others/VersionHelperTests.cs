using Zidium.Core.Common;
using Xunit;

namespace Zidium.Core.Tests.Others
{
    
    public class VersionHelperTests
    {
        [Fact]
        public void NullVersionTest()
        {
            string version = null;
            var versionLong = VersionHelper.FromString(version);
            Assert.Null(versionLong);
        }

        [Fact]
        public void EmptyVersionTest()
        {
            var version = string.Empty;
            var versionLong = VersionHelper.FromString(version);
            Assert.Null(versionLong);
        }

        [Fact]
        public void NormalVersionTest()
        {
            var version = "1.2.3.4";
            var versionLong = VersionHelper.FromString(version);
            Assert.NotNull(versionLong);
            Assert.Equal(281483566841860, versionLong.Value);
        }

        [Fact]
        public void ZeroVersionTest()
        {
            var version = "0.0.0.0";
            var versionLong = VersionHelper.FromString(version);
            Assert.NotNull(versionLong);
            Assert.Equal(0, versionLong.Value);
        }

        [Fact]
        public void MaxVersionTest()
        {
            var version = "32767.65535.65535.65535";
            var versionLong = VersionHelper.FromString(version);
            Assert.NotNull(versionLong);
            Assert.Equal(9223372036854775807, versionLong.Value);
        }

        [Fact]
        public void TooMaxVersionTest()
        {
            var version = "65535.65535.65535.65535";
            var versionLong = VersionHelper.FromString(version);
            Assert.NotNull(versionLong);
            Assert.Equal(9223372036854775807, versionLong.Value);
        }

        [Fact]
        public void OverflowVersionTest()
        {
            var version = "65535.65535.65535.100000";
            var versionLong = VersionHelper.FromString(version);
            Assert.Null(versionLong);
        }

        [Fact]
        public void NonNumberVersionTest()
        {
            var version = "a.b.c.d";
            var versionLong = VersionHelper.FromString(version);
            Assert.Null(versionLong);
        }

        [Fact]
        public void CompareSameMajorTest()
        {
            var version1Long = VersionHelper.FromString("1.0.0.0");
            var version2Long = VersionHelper.FromString("1.1.0.0");
            Assert.True(version2Long > version1Long);
        }

        [Fact]
        public void CompareDiffMajorTest()
        {
            var version1Long = VersionHelper.FromString("1.1.0.0");
            var version2Long = VersionHelper.FromString("2.0.0.0");
            Assert.True(version2Long > version1Long);
        }

    }
}
