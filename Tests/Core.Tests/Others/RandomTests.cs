using System;
using System.Collections.Generic;
using System.Globalization;
using Zidium.Core.Common;
using Xunit;

namespace Zidium.Core.Tests.Others
{
    public class RandomTests
    {
        [Fact]
        public void GetRandomItemFromListTest()
        {
            var rr = DateTime.Now.ToString(CultureInfo.InvariantCulture);
            var list = new List<int>() {10, 20, 30};
            bool hasMin = false;
            bool hasMax = false;
            
            for (int i = 0; i < 50; i++)
            {
                int value = RandomHelper.GetRandomItemFromList(list);
                if (value == 10)
                {
                    hasMin = true;
                }
                if (value == 30)
                {
                    hasMax = true;
                }
            }

            Assert.True(hasMax);
            Assert.True(hasMin);
        }
    }
}
