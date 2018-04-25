using System;
using Zidium.Core.Api;
using Zidium.Core.Common;

namespace Zidium.Core.ConfigDb
{
    public class PaymentService : IPaymentService
    {
        public PaymentService(DatabasesContext context)
        {
            Context = context;
        }

        protected DatabasesContext Context;

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
