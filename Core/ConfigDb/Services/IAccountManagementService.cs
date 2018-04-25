using System;
using Zidium.Core.AccountsDb;
using Zidium.Core.Api;

namespace Zidium.Core.ConfigDb
{
    public interface IAccountManagementService
    {
        /// <summary>
        /// Регистрация аккаунта - шаг 1
        /// </summary>
        Guid RegistrationStep1(string accountName, string email, Guid? userAgentTag);

        /// <summary>
        /// Регистрация аккаунта - шаг 2
        /// </summary>
        Guid RegistrationStep2(Guid regId, string companyName, string site, string companyPost);

        /// <summary>
        /// Регистрация аккаунта - шаг 3
        /// </summary>
        Guid RegistrationStep3(Guid regId, string firstName, string lastName, string fatherName, string phone);

        /// <summary>
        /// Регистрация аккаунта - завершение
        /// </summary>
        Guid EndRegistration(string secretKey);

        /// <summary>
        /// Переводит аккаунт на бесплатный тариф
        /// </summary>
        void MakeAccountFree(Guid accountId);

        /// <summary>
        /// Переводит аккаунт на платный тариф и устанавливает лимиты
        /// </summary>
        void MakeAccountPaidAndSetLimits(Guid accountId, TariffConfigurationInfo data);

        /// <summary>
        /// Меняет тип аккаунта на указанный и устанавливает базовые лимиты
        /// </summary>
        void ChangeAccountType(Guid accountId, AccountType accountType);

        /// <summary>
        /// Прямая установка указанных лимитов
        /// Только для юнит-тестов
        /// </summary>
        void SetAccountLimits(Guid accountId, AccountTotalLimitsDataInfo limits, TariffLimitType type);

        /// <summary>
        /// Проверяет, что указанное имя аккаунта можно использовать в качестве домена 4-го уровня
        /// </summary>
        void ValidateAccountName(string name);

    }
}
