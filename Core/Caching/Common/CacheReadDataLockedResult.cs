//using System;

//namespace Zidium.Core.Caching
//{
//    public class CacheReadDataLockedResult<TRequest, TResponse, TReadData, TWriteData> : IDisposable
//       // where TRequest : class 
//        where TResponse : CacheResponse<TRequest, TResponse, TReadData, TWriteData>
//        where TReadData : class 
//        where TWriteData : class, ICacheWriteObjectT<TResponse, TWriteData> 
//    {
//        private TResponse _response;

//        public TReadData Data { get; private set; }

//        public TResponse Response
//        {
//            get { return _response; }
//        }

//        public CacheReadDataLockedResult(TReadData readData, TResponse response)
//        {
//            if (response == null)
//            {
//                throw new ArgumentNullException("response");
//            }
//            Data = readData;
//            _response = response;
//        }

//        public void Dispose()
//        {
//            _response.Lock.Exit();
//        }
//    }
//}
