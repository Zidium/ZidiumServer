using System;
using System.Collections.Generic;
using Xunit;

namespace Zidium.Core.Tests.Services
{
    
    public class ConditionCheckerTests
    {
        [Fact]
        public void SatisfiedConditionTest()
        {
            var parameters = new[]
            {
                new KeyValuePair<string, double?>("HDD", 100),
                new KeyValuePair<string, double?>("MEM", 500),
            };
            var condition = @"HDD <= 100 || MEM <= 100";
            var result = ConditionChecker.Check(parameters, condition);
            Assert.True(result);
        }

        [Fact]
        public void UnSatisfiedConditionTest()
        {
            var parameters = new[]
            {
                new KeyValuePair<string, double?>("HDD", 100),
                new KeyValuePair<string, double?>("MEM", 500),
            };
            var condition = @"HDD <= 100 && MEM <= 100";
            var result = ConditionChecker.Check(parameters, condition);
            Assert.False(result);
        }

        [Fact]
        public void IsNullConditionTest()
        {
            var parameters = new[]
            {
                new KeyValuePair<string, double?>("HDD", null)
            };
            var condition = @"HDD == null";
            var result = ConditionChecker.Check(parameters, condition);
            Assert.True(result);
        }

        [Fact]
        public void EmptyConditionTest()
        {
            var parameters = new[]
            {
                new KeyValuePair<string, double?>("HDD", null)
            };
            var condition = @"";
            var result = ConditionChecker.Check(parameters, condition);
            Assert.False(result);
        }

        [Fact]
        public void NoCLRAccessTest()
        {
            bool exceptionFlag = false;
            var parameters = new[]
            {
                new KeyValuePair<string, double?>("HDD", null)
            };
            var condition = @"(new System.Text.StringBuilder()).Count == 0";
            try
            {
                var result = ConditionChecker.Check(parameters, condition);
            }
            catch (Exception)
            {
                exceptionFlag = true;
            }
            Assert.True(exceptionFlag);
        }

        [Fact]
        public void NoFilesAccessTest()
        {
            bool exceptionFlag = false;
            var parameters = new[]
            {
                new KeyValuePair<string, double?>("HDD", null)
            };
            var condition = @"(new ActiveXObject('Scripting.FileSystemObject').CreateTextFile('C:\\test.txt', true).WriteLine('TEST') == 0)";
            try
            {
                var result = ConditionChecker.Check(parameters, condition);
            }
            catch (Exception)
            {
                exceptionFlag = true;
            }
            Assert.True(exceptionFlag);
        }

        [Fact]
        public void PerformanceTest()
        {
            var startDate = DateTime.Now;
            for (var i = 0; i < 10000; i++)
            {
                var parameters = new[]
                {
                    new KeyValuePair<string, double?>("HDD", 1)
                };
                var condition = @"HDD > 100";
                var result = ConditionChecker.Check(parameters, condition);
            }
            var endDate = DateTime.Now;
            var duration = (int)(endDate - startDate).TotalMilliseconds;
            Console.WriteLine(duration + " ms");
        }

        [Fact]
        public void NullAndZero1Test()
        {
            var parameters = new[]
            {
                new KeyValuePair<string, double?>("value", null),
            };
            var condition = @"value < 10";
            var result = ConditionChecker.Check(parameters, condition);
            Assert.False(result);            
        }

        [Fact]
        public void NullAndZero2Test()
        {
            var parameters = new[]
            {
                new KeyValuePair<string, double?>("value", null),
            };
            var condition = @"value > 10";
            var result = ConditionChecker.Check(parameters, condition);
            Assert.False(result);
        }

    }
}
