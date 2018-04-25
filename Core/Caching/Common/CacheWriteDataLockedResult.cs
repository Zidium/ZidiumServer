//using System;

//namespace Zidium.Core.Caching
//{
//    public class CacheWriteDataLockedResult<TRequest, TResponse, TReadData, TWriteData> : IDisposable
//        //where TRequest : class 
//        where TResponse : CacheResponse<TRequest, TResponse, TReadData, TWriteData>
//        where TReadData : class 
//        where TWriteData : class, ICacheWriteObjectT<TResponse, TWriteData> 
//    {
//        private TResponse _response;

//        public TWriteData Data { get; private set; }

//        public TRequest Request
//        {
//            get { return _response.Request; }
//        }

//        public TResponse Response
//        {
//            get { return _response; }
//        }

//        public CacheWriteDataLockedResult(TWriteData writeData, TResponse response)
//        {
//            if (response == null)
//            {
//                throw new ArgumentNullException("response");
//            }
//            Data = writeData;
//            _response = response; // response нужен на случай, когда writeData null
//        }

//        public void Dispose()
//        {
//            _response.Lock.Exit();
//        }
//    }
//}
