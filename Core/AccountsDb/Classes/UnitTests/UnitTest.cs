using System;
using System.Collections.Generic;
using Zidium.Core.AccountsDb.Classes.UnitTests;
using Zidium.Core.Api;
using Zidium.Core.Common;

namespace Zidium.Core.AccountsDb
{
    /// <summary>
    /// Проверка
    /// </summary>
    public class UnitTest
    {
        public UnitTest()
        {
            Properties = new HashSet<UnitTestProperty>();    
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
        public virtual UnitTestType Type { get; set; }

        /// <summary>
        /// Период выполнения. Используется для проверок, которые запускает через определенный интервал сам Зидиум (например проверка открытия страницы или пинг)
        /// </summary>
        public int? PeriodSeconds { get; set; }

        /// <summary>
        /// Компонент
        /// </summary>
        public virtual Component Component { get; set; }

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
        public virtual Bulb Bulb { get; set; }

        /// <summary>
        /// Время следующего запуска
        /// </summary>
        public DateTime? NextExecutionDate { get; set; }

        /// <summary>
        /// Время последнего выполнения
        /// </summary>
        public DateTime? LastExecutionDate { get; set; }

        /// <summary>
        /// Настройки http-проверки, если есть
        /// </summary>
        public virtual HttpRequestUnitTest HttpRequestUnitTest { get; set; }

        /// <summary>
        /// Правило проверки ping
        /// </summary>
        public virtual UnitTestPingRule PingRule { get; set; }

        /// <summary>
        /// Правило проверки Sql
        /// </summary>
        public virtual UnitTestSqlRule SqlRule { get; set; }

        /// <summary>
        /// Правило проверки Domain
        /// </summary>
        public virtual UnitTestDomainNamePaymentPeriodRule DomainNamePaymentPeriodRule { get; set; }

        /// <summary>
        /// Правило проверки Ssl
        /// </summary>
        public virtual UnitTestSslCertificateExpirationDateRule SslCertificateExpirationDateRule { get; set; }

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
        /// Проверка и компонент включены
        /// </summary>
        public bool CanProcess
        {
            get { return Enable && ParentEnable; }
        }

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
        /// Проверяет что тип проверки системный
        /// </summary>
        public bool IsSystemType
        {
            get { return SystemUnitTestTypes.IsSystem(TypeId); }
        }

        /// <summary>
        /// Свойства
        /// </summary>
        public virtual ICollection<UnitTestProperty> Properties { get; set; }

        /// <summary>
        /// Количество выполненных неуспешных попыток
        /// </summary>
        public int AttempCount { get; set; }

        /// <summary>
        /// Максимальное количество неуспешных попыток, после которого проверка провалится
        /// </summary>
        public int AttempMax { get; set; }

        public string GetFullDisplayName()
        {
            return Component.GetFullDisplayName() + " / " + DisplayName;
        }
    }
}
