//using System;
//using System.Collections.Generic;

//namespace Zidium.Api
//{
//    public class ControlCache<TControl> : IControlCache<TControl>
//        where TControl : class, IObjectControl
//    {
//        protected Dictionary<string, TControl> Controls = new Dictionary<string, TControl>();

//        public TControl GetOneOrNull(string systemName)
//        {
//            if (systemName == null)
//            {
//                throw new ArgumentNullException("systemName");
//            }
//            string key = systemName.ToLowerInvariant();
//            lock (this)
//            {
//                TControl result;
//                if (Controls.TryGetValue(key, out result))
//                {
//                    return result;
//                }
//                return null;
//            }
//        }

//        public TControl Add(TControl control)
//        {
//            if (control == null)
//            {
//                throw new ArgumentNullException("control");
//            }
//            string key = control.SystemName.ToLowerInvariant();
//            lock (this)
//            {
//                TControl result;
//                if (Controls.TryGetValue(key, out result))
//                {
//                    return result;
//                }
//                Controls.Add(key, control);
//                return control;
//            }
//        }

//        public bool Remove(string systemName)
//        {
//            if (systemName == null)
//            {
//                throw new ArgumentNullException("systemName");
//            }
//            string key = systemName.ToLowerInvariant();
//            lock (this)
//            {
//                return Controls.Remove(key);
//            }
//        }

//        public void Clear()
//        {
//            lock (this)
//            {
//                Controls.Clear();
//            }
//        }
//    }
//}
