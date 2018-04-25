using System;
using Zidium.Core.Api;

namespace Zidium.Core.ConfigDb
{
    public interface IPaymentService
    {
        void AddYandexKassaRefillPayment(Guid accountId, AddYandexKassaPaymentRequestData data);

        void ProcessPartnerPayments(DateTime fromDate);
    }
}
