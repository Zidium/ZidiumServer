﻿using System;
using Zidium.Common;
using Zidium.Core;

namespace Zidium.Agent
{
    internal class Configuration : JsonConfiguration<Options>,
        IDebugConfiguration, IDatabaseConfiguration, IDispatcherConfiguration, IAgentConfiguration
    {
        public bool DebugMode => Get().DebugMode;

        public string ProviderName => Get().Database.ProviderName;

        public string ConnectionString => Get().Database.ConnectionString;

        public bool UseLocalDispatcher => Get().UseLocalDispatcher;

        public Uri DispatcherUrl => new Uri(Get().DispatcherUrl);

        public string ServiceName => Get().Install.ServiceName;

        public string ServiceDescription => Get().Install.ServiceDescription;

        public bool DummyMode => Get().DummyMode;

        public string MaximumOfflineInterval => Get().MaximumOfflineInterval;

        public string SmtpServer => Get().Smtp.SmtpServer;

        public int SmtpPort => Get().Smtp.SmtpPort;

        public string SmtpLogin => Get().Smtp.SmtpLogin;

        public string SmtpPassword => Get().Smtp.SmtpPassword;

        public string SmtpFrom => Get().Smtp.SmtpFrom;

        public bool SmtpUseMailKit => Get().Smtp.SmtpUseMailKit;

        public bool SmtpUseSsl => Get().Smtp.SmtpUseSsl;

        public string SmsRuApiId => Get().Sms.SmsRuApiId;

        public string SmsRuFrom => Get().Sms.SmsRuFrom;

        public string TelegramBotToken => Get().TelegramBotToken;

        public string VKontakteAuthToken => Get().VKontakteAuthToken;

        public int? EventsMaxDeleteCount => Get().EventsMaxDeleteCount;
    }
}