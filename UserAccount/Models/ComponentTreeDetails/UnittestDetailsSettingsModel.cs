using System;
using Zidium.Api.Dto;

namespace Zidium.UserAccount.Models.ComponentTreeDetails
{
    public class UnittestDetailsSettingsModel
    {
        public Guid Id { get; set; }

        public string TypeName { get; set; }

        public string Name { get; set; }

        public TimeSpan? ExecutionInterval { get; set; }

        public TimeSpan? ActualInterval { get; set; }

        public bool IsSystem { get; set; }

        public Guid TypeId { get; set; }

        /// <summary>
        /// Цвет проверки, если нет сигнала
        /// </summary>
        public ObjectColor NoSignalColor { get; set; }

        public string HttpUrl { get; set; }

        public TimeSpan HttpTimeout { get; set; }

        public string PingHost { get; set; }

        public TimeSpan PingTimeout { get; set; }

        public string DomainName { get; set; }

        public string SslHost { get; set; }

        public string SqlQuery { get; set; }

        public int TcpPort { get; set; }
    }
}