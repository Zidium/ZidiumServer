using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Zidium.Api;

namespace Zidium.Core.Common.Helpers
{
    /// <summary>
    /// Используется для самопроверки компонента по таймеру
    /// </summary>
    public class ComponentSelfTest
    {
        private Dictionary<string, Action> _unitTests = new Dictionary<string, Action>();
        private Timer _timer;
        private IComponentControl _componentControl;

        public ComponentSelfTest(IComponentControl componentControl)
        {
            if (componentControl == null)
            {
                throw new ArgumentNullException("componentControl");
            }
            _componentControl = componentControl;
        }

        public void AddUnitTest(string name, Action action)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }
            _unitTests.Add(name, action);
        }

        public ComponentSelfTestResult Validate()
        {
            var result = new ComponentSelfTestResult()
            {
                Success = true,
                UnitTests = new Dictionary<string, Exception>()
            };
            foreach (var pair in _unitTests)
            {
                var name = pair.Key;
                var action = pair.Value;
                try
                {
                    action();
                    result.UnitTests.Add(name, null);
                }
                catch (Exception exception)
                {
                    result.UnitTests.Add(name, exception);
                    result.Success = false;
                }
            }
            var log = new StringBuilder();
            var unitTestControl = _componentControl.GetOrCreateUnitTestControl("SelfTest");
            if (_unitTests.Count == 0)
            {
                result.Success = false;
                log.AppendLine("Нет ни одного юнит-теста");
                unitTestControl.SendResult(UnitTestResult.Alarm, TimeSpan.FromMinutes(5), "Нет ни одного юнит-теста");
                
            }
            else
            {
                if (result.Success)
                {
                    unitTestControl.SendResult(UnitTestResult.Success, TimeSpan.FromMinutes(5));
                }
                else
                {
                    var resultData = new SendUnitTestResultData()
                    {
                        ActualInterval = TimeSpan.FromMinutes(5),
                        Result = UnitTestResult.Alarm
                    };
                    foreach (var pair in result.UnitTests)
                    {
                        var name = pair.Key;
                        var exception = pair.Value;
                        var message = (exception == null) ? "success" : exception.Message;
                        resultData.Properties.Set(name, message);
                    }
                    unitTestControl.SendResult(resultData);
                }
            }
            string title = result.Success ? "### SUCCESS ###" : "### ERROR ###";
            foreach (var pair in result.UnitTests)
            {
                var name = pair.Key;
                var exception = pair.Value;
                var message = (exception == null) ? "success" : exception.Message;
                log.AppendLine(name + ": " + message);
            }

            result.Log = title + Environment.NewLine
                   + DateTime.Now + Environment.NewLine
                   + "-----------------------------------------------" + Environment.NewLine
                   + log;

            return result;
        }

        public void StartTimer(TimeSpan period)
        {
            _timer = new Timer((obj)=>Validate(), null, TimeSpan.Zero, period);
            
        }

        public void StopTimer()
        {
            if (_timer != null)
            {
                _timer.Dispose();
                _timer = null;
            }
        }
    }
}

