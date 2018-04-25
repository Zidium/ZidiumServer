using System;

namespace Zidium.Api
{
    public class SystemComponentType
    {
        public Guid Id { get; protected set; }

        public string SystemName { get; protected set; }

        public static readonly SystemComponentType Root = new SystemComponentType()
        {
            Id = new Guid("14F601AD-2946-4990-8480-517A6D3C6D64"),
            SystemName = "System.ComponentTypes.Root"
        };

        public static readonly SystemComponentType Folder = new SystemComponentType()
        {
            Id = new Guid("05D8D8E8-1050-41D3-B98E-C17EB22378B9"),
            SystemName = "System.ComponentTypes.Folder"
        };

        public static readonly SystemComponentType WebSite = new SystemComponentType()
        {
            Id = new Guid("2F93B751-5995-4ECB-A7FC-25B777D617B9"),
            SystemName = "System.ComponentTypes.WebSite"
        };

        public static readonly SystemComponentType Others = new SystemComponentType()
        {
            Id = new Guid("685678F9-D778-45BD-8AD2-D11C80FDCCAD"),
            SystemName = "System.ComponentTypes.Others"
        };

        public static readonly SystemComponentType DataBase = new SystemComponentType()
        {
            Id = new Guid("17F4D980-8F31-4CC7-8F5F-F8C1B556E561"),
            SystemName = "System.ComponentTypes.DataBase"
        };

        public static readonly SystemComponentType Domain = new SystemComponentType()
        {
            Id = new Guid("C915AA21-BEE1-4930-BD73-30E1FADE4327"),
            SystemName = "System.ComponentTypes.Domain"
        };

    }
}
