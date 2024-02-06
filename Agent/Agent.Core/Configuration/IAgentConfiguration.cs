namespace Zidium.Agent
{
    public interface IAgentConfiguration
    {
        bool DummyMode { get; }

        string MaximumOfflineInterval { get; }

        string SmtpServer { get; }

        int SmtpPort { get; }

        string SmtpLogin { get; }

        string SmtpPassword { get; }

        string SmtpFrom { get; }

        string SmtpFromEmail { get; }

        bool SmtpUseMailKit { get; }

        bool SmtpUseSsl { get; }

        string SmtpLocalServerName { get; }

        string SmsRuApiId { get; }

        string SmsRuFrom { get; }

        string TelegramBotToken { get; }

        string VKontakteAuthToken { get; }

        int? EventsMaxDeleteCount { get; }

    }
}
