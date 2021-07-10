using System;
using System.Collections.Generic;
using Zidium.Api.Dto;

namespace Zidium.Storage.Ef
{
    /// <summary>
    /// Проверка
    /// </summary>
    public class DbUnitTest
    {
        public DbUnitTest()
        {
            Properties = new HashSet<DbUnitTestProperty>();    
        }

        /// <summary>
        /// Id
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Системное имя
        /// </summary>
        public string SystemName { get; set; }

        /// <summary>
        /// Отображаемое имя
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Ссылка на тип проверки
        /// </summary>
        public Guid TypeId { get; set; }

        /// <summary>
        /// Тип проверки
        /// </summary>
        public virtual DbUnitTestType Type { get; set; }

        /// <summary>
        /// Период выполнения. Используется для проверок, которые запускает через определенный интервал сам Зидиум (например проверка открытия страницы или пинг)
        /// </summary>
        public int? PeriodSeconds { get; set; }

        /// <summary>
        /// Компонент
        /// </summary>
        public virtual DbComponent Component { get; set; }

        /// <summary>
        /// Ссылка на компонент
        /// </summary>
        public Guid ComponentId { get; set; }

        /// <summary>
        /// Ссылка на данные статуса
        /// </summary>
        public Guid StatusDataId { get; set; }

        /// <summary>
        /// Данные статуса
        /// </summary>
        public virtual DbBulb Bulb { get; set; }

        /// <summary>
        /// Время следующего запуска
        /// </summary>
        public DateTime? NextExecutionDate { get; set; }

        public DateTime? NextStepProcessDate { get; set; }

        /// <summary>
        /// Время последнего выполнения
        /// </summary>
        public DateTime? LastExecutionDate { get; set; }

        /// <summary>
        /// Настройки http-проверки, если есть
        /// </summary>
        public virtual DbHttpRequestUnitTest HttpRequestUnitTest { get; set; }

        /// <summary>
        /// Правило проверки ping
        /// </summary>
        public virtual DbUnitTestPingRule PingRule { get; set; }

        /// <summary>
        /// Правило проверки virus total
        /// </summary>
        public virtual DbUnitTestVirusTotalRule VirusTotalRule { get; set; }

        /// <summary>
        /// Правило проверки tcp порта
        /// </summary>
        public virtual DbUnitTestTcpPortRule TcpPortRule { get; set; }

        /// <summary>
        /// Правило проверки Sql
        /// </summary>
        public virtual DbUnitTestSqlRule SqlRule { get; set; }

        /// <summary>
        /// Правило проверки Domain
        /// </summary>
        public virtual DbUnitTestDomainNamePaymentPeriodRule DomainNamePaymentPeriodRule { get; set; }

        /// <summary>
        /// Правило проверки Ssl
        /// </summary>
        public virtual DbUnitTestSslCertificateExpirationDateRule SslCertificateExpirationDateRule { get; set; }

        /// <summary>
        /// Дата, до которой выключена проверка
        /// </summary>
        public DateTime? DisableToDate { get; set; }

        /// <summary>
        /// Комментарий к выключению (указывается при выключении)
        /// </summary>
        public string DisableComment { get; set; }

        /// <summary>
        /// Признак что проверка включена
        /// </summary>
        public bool Enable { get; set; }

        /// <summary>
        /// Признак, что родитель включен
        /// </summary>
        public bool ParentEnable { get; set; }

        /// <summary>
        /// Признак редактирования в упрощённом режиме в ЛК
        /// </summary>
        public bool SimpleMode { get; set; }

        /// <summary>
        /// Признак удалённого
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// Дата создания
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// Переопределение цвета ошибки
        /// Это цвет, который будет у ошибки в случае провала проверки
        /// </summary>
        public UnitTestResult? ErrorColor { get; set; }

        /// <summary>
        /// Цвет проверки, если нет сигнала
        /// </summary>
        public ObjectColor? NoSignalColor { get; set; }

        /// <summary>
        /// Время актуальности проверки
        /// </summary>
        public int? ActualTimeSecs { get; set; }

        /// <summary>
        /// Свойства
        /// </summary>
        public virtual ICollection<DbUnitTestProperty> Properties { get; set; }

        /// <summary>
        /// Количество выполненных неуспешных попыток
        /// </summary>
        public int AttempCount { get; set; }

        /// <summary>
        /// Максимальное количество неуспешных попыток, после которого проверка провалится
        /// </summary>
        public int AttempMax { get; set; }        

    }
}
