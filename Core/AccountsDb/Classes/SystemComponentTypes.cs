using System;

namespace Zidium.Core.AccountsDb
{
    /// <summary>
    /// Справочник ИД типов системных компонентов
    /// </summary>
    public static class SystemComponentTypes
    {
        public static readonly ComponentType Root = new ComponentType()
        {
            Id = new Guid("14F601AD-2946-4990-8480-517A6D3C6D64"),
            SystemName = "System.ComponentTypes.Root",
            DisplayName = "Корень",
            IsSystem = true
        };

        public static readonly ComponentType Folder = new ComponentType()
        {
            Id = new Guid("05D8D8E8-1050-41D3-B98E-C17EB22378B9"),
            SystemName = "System.ComponentTypes.Folder",
            DisplayName = "Папка",
            IsSystem = true
        };

        public static readonly ComponentType WebSite = new ComponentType()
        {
            Id = new Guid("2F93B751-5995-4ECB-A7FC-25B777D617B9"),
            SystemName = "System.ComponentTypes.WebSite",
            DisplayName = "Веб-сайты",
            IsSystem = true
        };

        public static readonly ComponentType Others = new ComponentType()
        {
            Id = new Guid("685678F9-D778-45BD-8AD2-D11C80FDCCAD"),
            SystemName = "System.ComponentTypes.Others",
            DisplayName = "Разные",
            IsSystem = true
        };

        public static readonly ComponentType DataBase = new ComponentType()
        {
            Id = new Guid("17F4D980-8F31-4CC7-8F5F-F8C1B556E561"),
            SystemName = "System.ComponentTypes.DataBase",
            DisplayName = "Базы данных",
            IsSystem = true
        };

        public static readonly ComponentType Domain = new ComponentType()
        {
            Id = new Guid("C915AA21-BEE1-4930-BD73-30E1FADE4327"),
            SystemName = "System.ComponentTypes.Domain",
            DisplayName = "Доменные имена",
            IsSystem = true
        };
    }
}
