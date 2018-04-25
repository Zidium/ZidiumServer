using System;
using System.Collections.Generic;
using Zidium.Api;
using Xunit;

namespace ApiTests_1._0
{
    
    public class ExtentionPropertyCollectionTests
    {
        [Fact]
        public void BooleanTest()
        {
            string key = "key";
            var properties = new ExtentionPropertyCollection();

            // проверим GetBooleanOrNull когда нет значения
            bool? value = properties.GetBooleanOrNull(key);
            Assert.Null(value);

            // проверим GetBoolean когда нет значения
            Assert.Throws<KeyNotFoundException>(() => properties.GetBoolean(key));

            // проверим GetBooleanOrNull когда значение есть
            properties.Set(key, true);
            value = properties.GetBooleanOrNull(key);
            Assert.True(value.HasValue);
            Assert.True(value == true);

            // проверим GetBoolean когда значение есть
            value = null;
            value = properties.GetBoolean(key);
            Assert.True(value.HasValue);
            Assert.True(value == true);
        }

        [Fact]
        public void Int32Test()
        {
            string key = "key";
            var properties = new ExtentionPropertyCollection();

            // проверим GetInt32OrNull когда нет значения
            int? value = properties.GetInt32OrNull(key);
            Assert.Null(value);

            // проверим GetInt32 когда нет значения
            Assert.Throws<KeyNotFoundException>(() => properties.GetInt32(key));

            // проверим GetInt32OrNull когда значение есть
            properties.Set(key, int.MaxValue);
            value = properties.GetInt32OrNull(key);
            Assert.True(value.HasValue);
            Assert.True(value == int.MaxValue);

            // проверим GetInt32 когда значение есть
            value = null;
            value = properties.GetInt32(key);
            Assert.True(value.HasValue);
            Assert.True(value == int.MaxValue);
        }

        [Fact]
        public void Int64Test()
        {
            string key = "key";
            var properties = new ExtentionPropertyCollection();

            // проверим GetInt64OrNull когда нет значения
            long? value = properties.GetInt64OrNull(key);
            Assert.Null(value);

            // проверим GetInt64 когда нет значения
            Assert.Throws<KeyNotFoundException>(() => properties.GetInt64(key));

            // проверим GetInt64OrNull когда значение есть
            properties.Set(key, long.MaxValue);
            value = properties.GetInt64OrNull(key);
            Assert.True(value.HasValue);
            Assert.True(value == long.MaxValue);

            // проверим GetInt64 когда значение есть
            value = null;
            value = properties.GetInt64(key);
            Assert.True(value.HasValue);
            Assert.True(value == long.MaxValue);
        }

        [Fact]
        public void DoubleTest()
        {
            string key = "key";
            var properties = new ExtentionPropertyCollection();

            // проверим GetDoubleOrNull когда нет значения
            double? value = properties.GetDoubleOrNull(key);
            Assert.Null(value);

            // проверим GetDouble когда нет значения
            Assert.Throws<KeyNotFoundException>(() => properties.GetDouble(key));

            // проверим GetDoubleOrNull когда значение есть
            double setValue = 1121212.2323;
            properties.Set(key, setValue);
            value = properties.GetDoubleOrNull(key);
            Assert.True(value.HasValue);
            Assert.True(value == setValue);

            // проверим GetDouble когда значение есть
            value = null;
            value = properties.GetDouble(key);
            Assert.True(value.HasValue);
            Assert.True(value == setValue);
        }

        [Fact]
        public void DateTimeTest()
        {
            string key = "key";
            var properties = new ExtentionPropertyCollection();

            // проверим GetDateTimeOrNull когда нет значения
            DateTime? value = properties.GetDateTimeOrNull(key);
            Assert.Null(value);

            // проверим GetDateTime когда нет значения
            Assert.Throws<KeyNotFoundException>(() => properties.GetDateTime(key));

            // проверим GetDateTimeOrNull когда значение есть
            DateTime now = DateTime.Now;
            now = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second); // потому что милесекунды отбрасываются !
            properties.Set(key, now);
            value = properties.GetDateTimeOrNull(key);
            Assert.True(value.HasValue);
            Assert.True(value == now);

            // проверим GetDateTime когда значение есть
            value = null;
            value = properties.GetDateTime(key);
            Assert.True(value.HasValue);
            Assert.True(value == now);
        }

        [Fact]
        public void StringTest()
        {
            string key = "key";
            var properties = new ExtentionPropertyCollection();

            // проверим GetStringOrNull когда нет значения
            string value = properties.GetStringOrNull(key);
            Assert.Null(value);

            // проверим GetString когда нет значения
            Assert.Throws<KeyNotFoundException>(() => properties.GetString(key));

            // проверим GetStringOrNull когда значение есть
            properties.Set(key, "value");
            value = properties.GetStringOrNull(key);
            Assert.True(value == "value");

            // проверим GetString когда значение есть
            value = null;
            value = properties.GetString(key);
            Assert.True(value == "value");
        }

        [Fact]
        public void SetNullTest()
        {
            var properties = new ExtentionPropertyCollection();
            string value = null;
            properties.Set("key", value);
            string value2 = properties["key"];
            Assert.Null(value2);
        }

        [Fact]
        public void ComplextTest()
        {
            string key = "key";
            var properties = new ExtentionPropertyCollection();
            Assert.Equal(0, properties.Count);
            
            // проверим методы GetXxxOrNull когда значения нет
            Assert.Null(properties.GetBooleanOrNull(key));
            Assert.Null(properties.GetInt32OrNull(key));
            Assert.Null(properties.GetInt64OrNull(key));
            Assert.Null(properties.GetDoubleOrNull(key));
            Assert.Null(properties.GetDateTimeOrNull(key));
            Assert.Null(properties.GetStringOrNull(key));

            // проверим методы Get когда значения есть
            properties.Set(key, true);
            Assert.True(properties.GetBooleanOrNull(key)==true);
            Assert.True(properties.GetBoolean(key) == true);
            
            properties.Set(key, int.MaxValue);
            Assert.True(properties.GetInt32OrNull(key)==int.MaxValue);
            Assert.True(properties.GetInt32(key) == int.MaxValue);

            properties.Set(key, long.MaxValue);
            Assert.True(properties.GetInt64OrNull(key) == long.MaxValue);
            Assert.True(properties.GetInt64(key) == long.MaxValue);

            properties.Set(key, 1000.05);
            Assert.True(properties.GetDoubleOrNull(key)==1000.05);
            Assert.True(properties.GetDouble(key) == 1000.05);

            var now = DateTime.Now;
            now = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second);
            properties.Set(key, now);
            Assert.True(properties.GetDateTimeOrNull(key)==now);
            Assert.True(properties.GetDateTime(key) == now);

            properties.Set(key, "value");
            Assert.True(properties.GetStringOrNull(key) == "value");
            Assert.True(properties.GetString(key) == "value");

            Assert.Equal(1, properties.Count);
        }
    }
}
