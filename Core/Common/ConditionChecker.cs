using System.Collections.Generic;
using Jint;

namespace Zidium.Core
{
    public static class ConditionChecker
    {
        /// <summary>
        /// Проверяет произвольное условие для указанных параметров.
        /// </summary>
        /// <param name="parameters">Проверяемые параметры</param>
        /// <param name="condition">Условие по правилам JavaScript</param>
        /// <returns></returns>
        public static bool Check(KeyValuePair<string, double?>[] parameters, string condition)
        {
            if (string.IsNullOrEmpty(condition))
                return false;

            var engine = new Engine();
            var function = string.Empty;
            foreach (var item in parameters)
            {
                if (item.Value != null)
                    engine.SetValue(item.Key, item.Value);
                else
                    function += "var " + item.Key + "; ";
            }
            function += @"function getCondition(){if (" + condition + " ) {return true;} else {return false;}}; getCondition();";
            var engineResult = engine.Execute(function);
            var result = engineResult.GetCompletionValue().AsBoolean();
            return result;
        }
    }
}
