using System;

namespace Zidium.Core.Api
{
    public class AddYandexKassaPaymentRequestData
    {
        public decimal Sum { get; set; }
        
        public int Currency { get; set; }
        
        public long InvoiceId { get; set; }
        
        public string PaymentType { get; set; }

        public string RequestUrl { get; set; }

        public string RequestBody { get; set; }
    }
}
