using System;
using Zidium.Core.Api;

namespace Zidium.Core.ConfigDb
{
    public class PaymentService : IPaymentService
    {

        public void AddYandexKassaRefillPayment(Guid accountId, AddYandexKassaPaymentRequestData data)
        {
            throw new NotImplementedException();
        }

        public void ProcessPartnerPayments(DateTime fromDate)
        {
            throw new NotImplementedException();
        }
    }
}
