namespace Zidium.Core.AccountsDb.Migrations.MsSql
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AccountTariffs",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        TariffLimitId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id, clustered: false)
                .ForeignKey("dbo.TariffLimits", t => t.TariffLimitId)
                .Index(t => t.TariffLimitId);
            
            CreateTable(
                "dbo.TariffLimits",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(nullable: false, maxLength: 255),
                        Description = c.String(maxLength: 4000),
                        Type = c.Int(nullable: false),
                        Source = c.Int(nullable: false),
                        EventsRequestsPerDay = c.Int(nullable: false),
                        EventsMaxDays = c.Int(nullable: false),
                        LogSizePerDay = c.Long(nullable: false),
                        LogMaxDays = c.Int(nullable: false),
                        UnitTestsRequestsPerDay = c.Int(nullable: false),
                        UnitTestsMaxDays = c.Int(nullable: false),
                        MetricsRequestsPerDay = c.Int(nullable: false),
                        MetricsMaxDays = c.Int(nullable: false),
                        ComponentsMax = c.Int(nullable: false),
                        ComponentTypesMax = c.Int(nullable: false),
                        UnitTestTypesMax = c.Int(nullable: false),
                        HttpUnitTestsMaxNoBanner = c.Int(nullable: false),
                        UnitTestsMax = c.Int(nullable: false),
                        MetricsMax = c.Int(nullable: false),
                        StorageSizeMax = c.Long(nullable: false),
                        SmsPerDay = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id, clustered: false);
            
            CreateTable(
                "dbo.ArchivedStatuses",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        EventId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id, clustered: false)
                .ForeignKey("dbo.Events", t => t.EventId)
                .Index(t => t.EventId);
            
            CreateTable(
                "dbo.Events",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        OwnerId = c.Guid(nullable: false),
                        EventTypeId = c.Guid(nullable: false),
                        Message = c.String(),
                        Importance = c.Int(nullable: false),
                        PreviousImportance = c.Int(nullable: false),
                        Count = c.Int(nullable: false),
                        JoinKeyHash = c.Long(nullable: false),
                        CreateDate = c.DateTime(nullable: false),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                        ActualDate = c.DateTime(nullable: false),
                        IsSpace = c.Boolean(nullable: false),
                        LastUpdateDate = c.DateTime(nullable: false),
                        LastNotificationDate = c.DateTime(),
                        IsUserHandled = c.Boolean(nullable: false),
                        Version = c.String(maxLength: 255),
                        VersionLong = c.Long(),
                        Category = c.Int(nullable: false),
                        LastStatusEventId = c.Guid(),
                        FirstReasonEventId = c.Guid(),
                    })
                .PrimaryKey(t => t.Id, clustered: false)
                .ForeignKey("dbo.EventTypes", t => t.EventTypeId)
                .ForeignKey("dbo.Events", t => t.LastStatusEventId)
                .Index(t => new { t.Category, t.ActualDate, t.StartDate, t.Id }, name: "IX_AccountBased")
                .Index(t => new { t.OwnerId, t.Category, t.ActualDate, t.StartDate }, name: "IX_OwnerBased")
                .Index(t => new { t.OwnerId, t.EventTypeId, t.Importance, t.JoinKeyHash, t.IsSpace, t.Version, t.Category, t.ActualDate }, name: "IX_ForJoin")
                .Index(t => new { t.EventTypeId, t.IsUserHandled, t.VersionLong }, name: "IX_ForProcessing")
                .Index(t => t.LastStatusEventId);
            
            CreateTable(
                "dbo.EventTypes",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Category = c.Int(nullable: false),
                        DisplayName = c.String(nullable: false, maxLength: 255),
                        SystemName = c.String(nullable: false, maxLength: 255),
                        Code = c.String(maxLength: 20),
                        JoinIntervalSeconds = c.Int(),
                        IsSystem = c.Boolean(nullable: false),
                        OldVersion = c.String(maxLength: 255),
                        ImportanceForOld = c.Int(),
                        ImportanceForNew = c.Int(),
                        IsDeleted = c.Boolean(nullable: false),
                        CreateDate = c.DateTime(),
                        DefectId = c.Guid(),
                    })
                .PrimaryKey(t => t.Id, clustered: false)
                .ForeignKey("dbo.Defects", t => t.DefectId)
                .Index(t => t.SystemName)
                .Index(t => t.DefectId);
            
            CreateTable(
                "dbo.Defects",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Number = c.Int(nullable: false),
                        Title = c.String(maxLength: 500),
                        LastChangeId = c.Guid(),
                        ResponsibleUserId = c.Guid(),
                        EventTypeId = c.Guid(),
                        Notes = c.String(maxLength: 1000),
                    })
                .PrimaryKey(t => t.Id, clustered: false)
                .ForeignKey("dbo.EventTypes", t => t.EventTypeId)
                .ForeignKey("dbo.DefectChanges", t => t.LastChangeId)
                .ForeignKey("dbo.Users", t => t.ResponsibleUserId)
                .Index(t => t.LastChangeId)
                .Index(t => t.ResponsibleUserId)
                .Index(t => t.EventTypeId);
            
            CreateTable(
                "dbo.DefectChanges",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        DefectId = c.Guid(nullable: false),
                        Date = c.DateTime(nullable: false),
                        Status = c.Int(nullable: false),
                        Comment = c.String(maxLength: 1000),
                        UserId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id, clustered: false)
                .ForeignKey("dbo.Defects", t => t.DefectId)
                .ForeignKey("dbo.Users", t => t.UserId)
                .Index(t => t.DefectId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Login = c.String(nullable: false, maxLength: 255),
                        PasswordHash = c.String(maxLength: 255),
                        FirstName = c.String(maxLength: 100),
                        LastName = c.String(maxLength: 100),
                        MiddleName = c.String(maxLength: 100),
                        DisplayName = c.String(maxLength: 100),
                        CreateDate = c.DateTime(nullable: false),
                        Post = c.String(maxLength: 100),
                        InArchive = c.Boolean(nullable: false),
                        SecurityStamp = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.Id, clustered: false)
                .Index(t => t.Login);
            
            CreateTable(
                "dbo.UserRoles",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        UserId = c.Guid(nullable: false),
                        RoleId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id, clustered: false)
                .ForeignKey("dbo.Roles", t => t.RoleId)
                .ForeignKey("dbo.Users", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.Roles",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        SystemName = c.String(nullable: false, maxLength: 50),
                        DisplayName = c.String(maxLength: 50, fixedLength: true),
                    })
                .PrimaryKey(t => t.Id, clustered: false);
            
            CreateTable(
                "dbo.UserSettings",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        UserId = c.Guid(nullable: false),
                        Name = c.String(nullable: false, maxLength: 255),
                        Value = c.String(maxLength: 255),
                    })
                .PrimaryKey(t => t.Id, clustered: false)
                .ForeignKey("dbo.Users", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Subscriptions",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        UserId = c.Guid(nullable: false),
                        ComponentTypeId = c.Guid(),
                        ComponentId = c.Guid(),
                        Object = c.Int(nullable: false),
                        Channel = c.Int(nullable: false),
                        IsEnabled = c.Boolean(nullable: false),
                        NotifyBetterStatus = c.Boolean(nullable: false),
                        Importance = c.Int(nullable: false),
                        DurationMinimumInSeconds = c.Int(),
                        ResendTimeInSeconds = c.Int(),
                        LastUpdated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id, clustered: false)
                .ForeignKey("dbo.Components", t => t.ComponentId)
                .ForeignKey("dbo.ComponentTypes", t => t.ComponentTypeId)
                .ForeignKey("dbo.Users", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.ComponentTypeId)
                .Index(t => t.ComponentId);
            
            CreateTable(
                "dbo.Components",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                        DisplayName = c.String(nullable: false, maxLength: 255),
                        SystemName = c.String(nullable: false, maxLength: 255),
                        ParentId = c.Guid(),
                        ComponentTypeId = c.Guid(nullable: false),
                        InternalStatusId = c.Guid(nullable: false),
                        ExternalStatusId = c.Guid(nullable: false),
                        UnitTestsStatusId = c.Guid(nullable: false),
                        EventsStatusId = c.Guid(nullable: false),
                        MetricsStatusId = c.Guid(nullable: false),
                        ChildComponentsStatusId = c.Guid(nullable: false),
                        Version = c.String(maxLength: 255),
                        IsDeleted = c.Boolean(nullable: false),
                        Enable = c.Boolean(nullable: false),
                        DisableToDate = c.DateTime(),
                        DisableComment = c.String(maxLength: 1000),
                        ParentEnable = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id, clustered: false)
                .ForeignKey("dbo.Bulbs", t => t.ChildComponentsStatusId)
                .ForeignKey("dbo.ComponentTypes", t => t.ComponentTypeId)
                .ForeignKey("dbo.Bulbs", t => t.EventsStatusId)
                .ForeignKey("dbo.Bulbs", t => t.ExternalStatusId)
                .ForeignKey("dbo.Bulbs", t => t.InternalStatusId)
                .ForeignKey("dbo.Bulbs", t => t.MetricsStatusId)
                .ForeignKey("dbo.Components", t => t.ParentId)
                .ForeignKey("dbo.Bulbs", t => t.UnitTestsStatusId)
                .Index(t => t.SystemName)
                .Index(t => t.ParentId)
                .Index(t => t.ComponentTypeId)
                .Index(t => t.InternalStatusId)
                .Index(t => t.ExternalStatusId)
                .Index(t => t.UnitTestsStatusId)
                .Index(t => t.EventsStatusId)
                .Index(t => t.MetricsStatusId)
                .Index(t => t.ChildComponentsStatusId);
            
            CreateTable(
                "dbo.Bulbs",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        ComponentId = c.Guid(),
                        UnitTestId = c.Guid(),
                        MetricId = c.Guid(),
                        EventCategory = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        PreviousStatus = c.Int(nullable: false),
                        CreateDate = c.DateTime(nullable: false),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                        Count = c.Int(nullable: false),
                        ActualDate = c.DateTime(nullable: false),
                        Message = c.String(),
                        FirstEventId = c.Guid(),
                        LastEventId = c.Guid(),
                        LastChildStatusDataId = c.Guid(),
                        StatusEventId = c.Guid(nullable: false),
                        UpTimeStartDate = c.DateTime(nullable: false),
                        UpTimeLengthMs = c.Long(nullable: false),
                        UpTimeSuccessMs = c.Long(nullable: false),
                        HasSignal = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id, clustered: false)
                .ForeignKey("dbo.Components", t => t.ComponentId)
                .ForeignKey("dbo.Metrics", t => t.MetricId)
                .ForeignKey("dbo.UnitTests", t => t.UnitTestId)
                .Index(t => t.ComponentId)
                .Index(t => t.UnitTestId)
                .Index(t => t.MetricId);
            
            CreateTable(
                "dbo.Metrics",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        ComponentId = c.Guid(nullable: false),
                        MetricTypeId = c.Guid(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        DisableToDate = c.DateTime(),
                        DisableComment = c.String(maxLength: 1000),
                        Enable = c.Boolean(nullable: false),
                        ParentEnable = c.Boolean(nullable: false),
                        CreateDate = c.DateTime(),
                        Value = c.Double(),
                        BeginDate = c.DateTime(nullable: false),
                        ActualDate = c.DateTime(nullable: false),
                        ActualTimeSecs = c.Int(),
                        NoSignalColor = c.Int(),
                        ConditionAlarm = c.String(maxLength: 500),
                        ConditionWarning = c.String(maxLength: 500),
                        ConditionSuccess = c.String(maxLength: 500),
                        ConditionElseColor = c.Int(),
                        StatusDataId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id, clustered: false)
                .ForeignKey("dbo.Bulbs", t => t.StatusDataId)
                .ForeignKey("dbo.MetricTypes", t => t.MetricTypeId)
                .ForeignKey("dbo.Components", t => t.ComponentId)
                .Index(t => t.ComponentId)
                .Index(t => t.MetricTypeId)
                .Index(t => t.StatusDataId);
            
            CreateTable(
                "dbo.MetricTypes",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        SystemName = c.String(nullable: false, maxLength: 255),
                        DisplayName = c.String(maxLength: 255),
                        IsDeleted = c.Boolean(nullable: false),
                        CreateDate = c.DateTime(),
                        ConditionAlarm = c.String(maxLength: 500),
                        ConditionWarning = c.String(maxLength: 500),
                        ConditionSuccess = c.String(maxLength: 500),
                        ConditionElseColor = c.Int(),
                        NoSignalColor = c.Int(),
                        ActualTimeSecs = c.Int(),
                    })
                .PrimaryKey(t => t.Id, clustered: false);
            
            CreateTable(
                "dbo.UnitTests",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        SystemName = c.String(nullable: false, maxLength: 255),
                        DisplayName = c.String(nullable: false, maxLength: 255),
                        TypeId = c.Guid(nullable: false),
                        PeriodSeconds = c.Int(),
                        ComponentId = c.Guid(nullable: false),
                        StatusDataId = c.Guid(nullable: false),
                        NextExecutionDate = c.DateTime(),
                        LastExecutionDate = c.DateTime(),
                        DisableToDate = c.DateTime(),
                        DisableComment = c.String(maxLength: 1000),
                        Enable = c.Boolean(nullable: false),
                        ParentEnable = c.Boolean(nullable: false),
                        SimpleMode = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        CreateDate = c.DateTime(nullable: false),
                        ErrorColor = c.Int(),
                        NoSignalColor = c.Int(),
                        ActualTimeSecs = c.Int(),
                    })
                .PrimaryKey(t => t.Id, clustered: false)
                .ForeignKey("dbo.Bulbs", t => t.StatusDataId)
                .ForeignKey("dbo.Components", t => t.ComponentId)
                .ForeignKey("dbo.UnitTestTypes", t => t.TypeId)
                .Index(t => t.TypeId)
                .Index(t => t.ComponentId)
                .Index(t => t.StatusDataId);
            
            CreateTable(
                "dbo.UnitTestDomainNamePaymentPeriodRules",
                c => new
                    {
                        UnitTestId = c.Guid(nullable: false),
                        Domain = c.String(),
                        AlarmDaysCount = c.Int(nullable: false),
                        WarningDaysCount = c.Int(nullable: false),
                        LastRunErrorCode = c.Int(),
                    })
                .PrimaryKey(t => t.UnitTestId, clustered: false)
                .ForeignKey("dbo.UnitTests", t => t.UnitTestId)
                .Index(t => t.UnitTestId);
            
            CreateTable(
                "dbo.HttpRequestUnitTests",
                c => new
                    {
                        UnitTestId = c.Guid(nullable: false),
                        ProcessAllRulesOnError = c.Boolean(nullable: false),
                        HasBanner = c.Boolean(nullable: false),
                        LastBannerCheck = c.DateTime(),
                    })
                .PrimaryKey(t => t.UnitTestId, clustered: false)
                .ForeignKey("dbo.UnitTests", t => t.UnitTestId)
                .Index(t => t.UnitTestId);
            
            CreateTable(
                "dbo.HttpRequestUnitTestRules",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        HttpRequestUnitTestId = c.Guid(nullable: false),
                        SortNumber = c.Int(nullable: false),
                        DisplayName = c.String(nullable: false, maxLength: 255),
                        Url = c.String(nullable: false, maxLength: 255),
                        Method = c.Int(nullable: false),
                        ResponseCode = c.Int(),
                        MaxResponseSize = c.Int(nullable: false),
                        SuccessHtml = c.String(maxLength: 255),
                        ErrorHtml = c.String(maxLength: 255),
                        TimeoutSeconds = c.Int(),
                        LastRunErrorCode = c.Int(),
                        LastRunErrorMessage = c.String(),
                        LastRunTime = c.DateTime(),
                        LastRunDurationMs = c.Int(),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id, clustered: false)
                .ForeignKey("dbo.HttpRequestUnitTests", t => t.HttpRequestUnitTestId)
                .Index(t => t.HttpRequestUnitTestId);
            
            CreateTable(
                "dbo.HttpRequestUnitTestRuleDatas",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        RuleId = c.Guid(nullable: false),
                        Type = c.Int(nullable: false),
                        Key = c.String(maxLength: 500),
                        Value = c.String(),
                    })
                .PrimaryKey(t => t.Id, clustered: false)
                .ForeignKey("dbo.HttpRequestUnitTestRules", t => t.RuleId)
                .Index(t => t.RuleId);
            
            CreateTable(
                "dbo.UnitTestPingRules",
                c => new
                    {
                        UnitTestId = c.Guid(nullable: false),
                        Host = c.String(),
                        TimeoutMs = c.Int(nullable: false),
                        Attemps = c.Int(nullable: false),
                        LastRunErrorCode = c.Int(),
                    })
                .PrimaryKey(t => t.UnitTestId, clustered: false)
                .ForeignKey("dbo.UnitTests", t => t.UnitTestId)
                .Index(t => t.UnitTestId);
            
            CreateTable(
                "dbo.UnitTestProperties",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        UnitTestId = c.Guid(nullable: false),
                        Name = c.String(nullable: false, maxLength: 255),
                        Value = c.String(),
                        DataType = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id, clustered: false)
                .ForeignKey("dbo.UnitTests", t => t.UnitTestId)
                .Index(t => t.UnitTestId);
            
            CreateTable(
                "dbo.UnitTestSqlRules",
                c => new
                    {
                        UnitTestId = c.Guid(nullable: false),
                        Provider = c.Int(nullable: false),
                        ConnectionString = c.String(maxLength: 500),
                        OpenConnectionTimeoutMs = c.Int(nullable: false),
                        CommandTimeoutMs = c.Int(nullable: false),
                        Query = c.String(),
                    })
                .PrimaryKey(t => t.UnitTestId, clustered: false)
                .ForeignKey("dbo.UnitTests", t => t.UnitTestId)
                .Index(t => t.UnitTestId);
            
            CreateTable(
                "dbo.UnitTestSslCertificateExpirationDateRules",
                c => new
                    {
                        UnitTestId = c.Guid(nullable: false),
                        Url = c.String(maxLength: 200),
                        AlarmDaysCount = c.Int(nullable: false),
                        WarningDaysCount = c.Int(nullable: false),
                        LastRunErrorCode = c.Int(),
                    })
                .PrimaryKey(t => t.UnitTestId, clustered: false)
                .ForeignKey("dbo.UnitTests", t => t.UnitTestId)
                .Index(t => t.UnitTestId);
            
            CreateTable(
                "dbo.UnitTestTypes",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        SystemName = c.String(nullable: false, maxLength: 255),
                        DisplayName = c.String(nullable: false, maxLength: 255),
                        CreateDate = c.DateTime(),
                        IsDeleted = c.Boolean(nullable: false),
                        IsSystem = c.Boolean(nullable: false),
                        NoSignalColor = c.Int(),
                        ActualTimeSecs = c.Int(),
                    })
                .PrimaryKey(t => t.Id, clustered: false)
                .Index(t => t.SystemName);
            
            CreateTable(
                "dbo.ComponentTypes",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        DisplayName = c.String(nullable: false, maxLength: 255),
                        SystemName = c.String(nullable: false, maxLength: 255),
                        IsSystem = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id, clustered: false)
                .Index(t => t.SystemName);
            
            CreateTable(
                "dbo.LastComponentNotifications",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        ComponentId = c.Guid(nullable: false),
                        Address = c.String(nullable: false, maxLength: 255),
                        Type = c.Int(nullable: false),
                        EventImportance = c.Int(nullable: false),
                        CreateDate = c.DateTime(nullable: false),
                        EventId = c.Guid(nullable: false),
                        NotificationId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id, clustered: false)
                .ForeignKey("dbo.Components", t => t.ComponentId)
                .ForeignKey("dbo.Events", t => t.EventId)
                .ForeignKey("dbo.Notifications", t => t.NotificationId)
                .Index(t => t.ComponentId)
                .Index(t => t.EventId)
                .Index(t => t.NotificationId);
            
            CreateTable(
                "dbo.Notifications",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        EventId = c.Guid(nullable: false),
                        UserId = c.Guid(nullable: false),
                        Type = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        SendError = c.String(maxLength: 4000),
                        CreationDate = c.DateTime(nullable: false),
                        SendDate = c.DateTime(),
                        SubscriptionId = c.Guid(),
                        Reason = c.Int(nullable: false),
                        Address = c.String(nullable: false, maxLength: 1000),
                    })
                .PrimaryKey(t => t.Id, clustered: false)
                .ForeignKey("dbo.Events", t => t.EventId)
                .ForeignKey("dbo.Subscriptions", t => t.SubscriptionId)
                .ForeignKey("dbo.Users", t => t.UserId)
                .Index(t => t.EventId)
                .Index(t => t.UserId)
                .Index(t => t.Status)
                .Index(t => t.SubscriptionId);
            
            CreateTable(
                "dbo.NotificationsHttp",
                c => new
                    {
                        NotificationId = c.Guid(nullable: false),
                        Json = c.String(),
                    })
                .PrimaryKey(t => t.NotificationId, clustered: false)
                .ForeignKey("dbo.Notifications", t => t.NotificationId)
                .Index(t => t.NotificationId);
            
            CreateTable(
                "dbo.LogConfigs",
                c => new
                    {
                        ComponentId = c.Guid(nullable: false),
                        LastUpdateDate = c.DateTime(nullable: false),
                        Enabled = c.Boolean(nullable: false),
                        IsDebugEnabled = c.Boolean(nullable: false),
                        IsTraceEnabled = c.Boolean(nullable: false),
                        IsInfoEnabled = c.Boolean(nullable: false),
                        IsWarningEnabled = c.Boolean(nullable: false),
                        IsErrorEnabled = c.Boolean(nullable: false),
                        IsFatalEnabled = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ComponentId, clustered: false)
                .ForeignKey("dbo.Components", t => t.ComponentId)
                .Index(t => t.ComponentId);
            
            CreateTable(
                "dbo.ComponentProperties",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        ComponentId = c.Guid(nullable: false),
                        Name = c.String(),
                        Value = c.String(),
                        DataType = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id, clustered: false)
                .ForeignKey("dbo.Components", t => t.ComponentId)
                .Index(t => t.ComponentId);
            
            CreateTable(
                "dbo.UserContacts",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        UserId = c.Guid(nullable: false),
                        Type = c.Int(nullable: false),
                        Value = c.String(nullable: false, maxLength: 255),
                        CreateDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id, clustered: false)
                .ForeignKey("dbo.Users", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.EventParameters",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        EventId = c.Guid(nullable: false),
                        Name = c.String(nullable: false, maxLength: 100),
                        Value = c.String(),
                        DataType = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id, clustered: false)
                .ForeignKey("dbo.Events", t => t.EventId)
                .Index(t => t.EventId);
            
            CreateTable(
                "dbo.LimitDatas",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        BeginDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                        Type = c.Int(nullable: false),
                        EventsRequests = c.Int(nullable: false),
                        LogSize = c.Long(nullable: false),
                        UnitTestsRequests = c.Int(nullable: false),
                        MetricsRequests = c.Int(nullable: false),
                        EventsSize = c.Long(nullable: false),
                        UnitTestsSize = c.Long(nullable: false),
                        MetricsSize = c.Long(nullable: false),
                        SmsCount = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id, clustered: false)
                .Index(t => new { t.Type, t.BeginDate }, name: "IX_AccountId");
            
            CreateTable(
                "dbo.LimitDatasForUnitTests",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        LimitDataId = c.Guid(nullable: false),
                        UnitTestId = c.Guid(nullable: false),
                        ResultsCount = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id, clustered: false)
                .ForeignKey("dbo.LimitDatas", t => t.LimitDataId)
                .ForeignKey("dbo.UnitTests", t => t.UnitTestId)
                .Index(t => t.LimitDataId)
                .Index(t => t.UnitTestId);
            
            CreateTable(
                "dbo.LogParameters",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        LogId = c.Guid(nullable: false),
                        Name = c.String(nullable: false, maxLength: 100),
                        DataType = c.Int(nullable: false),
                        Value = c.String(),
                    })
                .PrimaryKey(t => t.Id, clustered: false)
                .ForeignKey("dbo.Logs", t => t.LogId)
                .Index(t => t.LogId);
            
            CreateTable(
                "dbo.Logs",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        ComponentId = c.Guid(nullable: false),
                        Level = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
                        Order = c.Int(nullable: false),
                        Message = c.String(),
                        ParametersCount = c.Int(nullable: false),
                        Context = c.String(maxLength: 255),
                    })
                .PrimaryKey(t => t.Id, clustered: false)
                .ForeignKey("dbo.Components", t => t.ComponentId)
                .Index(t => new { t.ComponentId, t.Date, t.Order, t.Level, t.Context }, name: "IX_ComponentBased");
            
            CreateTable(
                "dbo.MetricHistory",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        ComponentId = c.Guid(nullable: false),
                        MetricTypeId = c.Guid(nullable: false),
                        BeginDate = c.DateTime(nullable: false),
                        ActualDate = c.DateTime(nullable: false),
                        Value = c.Double(),
                        Color = c.Int(nullable: false),
                        StatusEventId = c.Guid(),
                        HasSignal = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id, clustered: false)
                .ForeignKey("dbo.Components", t => t.ComponentId)
                .ForeignKey("dbo.MetricTypes", t => t.MetricTypeId)
                .ForeignKey("dbo.Events", t => t.StatusEventId)
                .Index(t => new { t.ComponentId, t.MetricTypeId, t.BeginDate }, name: "IX_ForHistory")
                .Index(t => new { t.ComponentId, t.ActualDate }, name: "IX_ForHistoryDeletion")
                .Index(t => t.StatusEventId);
            
            CreateTable(
                "dbo.SendEmailCommand",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        From = c.String(maxLength: 100),
                        To = c.String(nullable: false, maxLength: 255),
                        Subject = c.String(nullable: false, maxLength: 500),
                        Body = c.String(nullable: false),
                        IsHtml = c.Boolean(nullable: false),
                        Status = c.Int(nullable: false),
                        ErrorMessage = c.String(),
                        CreateDate = c.DateTime(nullable: false),
                        SendDate = c.DateTime(),
                        ReferenceId = c.Guid(),
                    })
                .PrimaryKey(t => t.Id, clustered: false)
                .Index(t => t.Status, name: "IX_ForSend");
            
            CreateTable(
                "dbo.SendSmsCommand",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Phone = c.String(nullable: false, maxLength: 255),
                        Body = c.String(nullable: false),
                        Status = c.Int(nullable: false),
                        ErrorMessage = c.String(),
                        CreateDate = c.DateTime(nullable: false),
                        SendDate = c.DateTime(),
                        ReferenceId = c.Guid(),
                        ExternalId = c.String(maxLength: 255),
                    })
                .PrimaryKey(t => t.Id, clustered: false)
                .Index(t => t.Status, name: "IX_ForSend");
            
            CreateTable(
                "dbo.Tokens",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        UserId = c.Guid(nullable: false),
                        Purpose = c.Int(nullable: false),
                        SecurityStamp = c.String(maxLength: 50),
                        CreationDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                        IsUsed = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id, clustered: false)
                .ForeignKey("dbo.Users", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.EventStatuses",
                c => new
                    {
                        EventId = c.Guid(nullable: false),
                        StatusId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.EventId, t.StatusId }, clustered: false)
                .ForeignKey("dbo.Events", t => t.EventId)
                .ForeignKey("dbo.Events", t => t.StatusId)
                .Index(t => t.EventId)
                .Index(t => t.StatusId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Tokens", "UserId", "dbo.Users");
            DropForeignKey("dbo.MetricHistory", "StatusEventId", "dbo.Events");
            DropForeignKey("dbo.MetricHistory", "MetricTypeId", "dbo.MetricTypes");
            DropForeignKey("dbo.MetricHistory", "ComponentId", "dbo.Components");
            DropForeignKey("dbo.LogParameters", "LogId", "dbo.Logs");
            DropForeignKey("dbo.Logs", "ComponentId", "dbo.Components");
            DropForeignKey("dbo.LimitDatasForUnitTests", "UnitTestId", "dbo.UnitTests");
            DropForeignKey("dbo.LimitDatasForUnitTests", "LimitDataId", "dbo.LimitDatas");
            DropForeignKey("dbo.ArchivedStatuses", "EventId", "dbo.Events");
            DropForeignKey("dbo.EventStatuses", "StatusId", "dbo.Events");
            DropForeignKey("dbo.EventStatuses", "EventId", "dbo.Events");
            DropForeignKey("dbo.EventParameters", "EventId", "dbo.Events");
            DropForeignKey("dbo.Events", "LastStatusEventId", "dbo.Events");
            DropForeignKey("dbo.Events", "EventTypeId", "dbo.EventTypes");
            DropForeignKey("dbo.EventTypes", "DefectId", "dbo.Defects");
            DropForeignKey("dbo.Defects", "ResponsibleUserId", "dbo.Users");
            DropForeignKey("dbo.Defects", "LastChangeId", "dbo.DefectChanges");
            DropForeignKey("dbo.Defects", "EventTypeId", "dbo.EventTypes");
            DropForeignKey("dbo.DefectChanges", "UserId", "dbo.Users");
            DropForeignKey("dbo.UserContacts", "UserId", "dbo.Users");
            DropForeignKey("dbo.Subscriptions", "UserId", "dbo.Users");
            DropForeignKey("dbo.Subscriptions", "ComponentTypeId", "dbo.ComponentTypes");
            DropForeignKey("dbo.Subscriptions", "ComponentId", "dbo.Components");
            DropForeignKey("dbo.Components", "UnitTestsStatusId", "dbo.Bulbs");
            DropForeignKey("dbo.ComponentProperties", "ComponentId", "dbo.Components");
            DropForeignKey("dbo.Components", "ParentId", "dbo.Components");
            DropForeignKey("dbo.Components", "MetricsStatusId", "dbo.Bulbs");
            DropForeignKey("dbo.Metrics", "ComponentId", "dbo.Components");
            DropForeignKey("dbo.LogConfigs", "ComponentId", "dbo.Components");
            DropForeignKey("dbo.LastComponentNotifications", "NotificationId", "dbo.Notifications");
            DropForeignKey("dbo.Notifications", "UserId", "dbo.Users");
            DropForeignKey("dbo.Notifications", "SubscriptionId", "dbo.Subscriptions");
            DropForeignKey("dbo.NotificationsHttp", "NotificationId", "dbo.Notifications");
            DropForeignKey("dbo.Notifications", "EventId", "dbo.Events");
            DropForeignKey("dbo.LastComponentNotifications", "EventId", "dbo.Events");
            DropForeignKey("dbo.LastComponentNotifications", "ComponentId", "dbo.Components");
            DropForeignKey("dbo.Components", "InternalStatusId", "dbo.Bulbs");
            DropForeignKey("dbo.Components", "ExternalStatusId", "dbo.Bulbs");
            DropForeignKey("dbo.Components", "EventsStatusId", "dbo.Bulbs");
            DropForeignKey("dbo.Components", "ComponentTypeId", "dbo.ComponentTypes");
            DropForeignKey("dbo.Components", "ChildComponentsStatusId", "dbo.Bulbs");
            DropForeignKey("dbo.Bulbs", "UnitTestId", "dbo.UnitTests");
            DropForeignKey("dbo.UnitTests", "TypeId", "dbo.UnitTestTypes");
            DropForeignKey("dbo.UnitTestSslCertificateExpirationDateRules", "UnitTestId", "dbo.UnitTests");
            DropForeignKey("dbo.UnitTestSqlRules", "UnitTestId", "dbo.UnitTests");
            DropForeignKey("dbo.UnitTestProperties", "UnitTestId", "dbo.UnitTests");
            DropForeignKey("dbo.UnitTestPingRules", "UnitTestId", "dbo.UnitTests");
            DropForeignKey("dbo.HttpRequestUnitTests", "UnitTestId", "dbo.UnitTests");
            DropForeignKey("dbo.HttpRequestUnitTestRules", "HttpRequestUnitTestId", "dbo.HttpRequestUnitTests");
            DropForeignKey("dbo.HttpRequestUnitTestRuleDatas", "RuleId", "dbo.HttpRequestUnitTestRules");
            DropForeignKey("dbo.UnitTestDomainNamePaymentPeriodRules", "UnitTestId", "dbo.UnitTests");
            DropForeignKey("dbo.UnitTests", "ComponentId", "dbo.Components");
            DropForeignKey("dbo.UnitTests", "StatusDataId", "dbo.Bulbs");
            DropForeignKey("dbo.Bulbs", "MetricId", "dbo.Metrics");
            DropForeignKey("dbo.Metrics", "MetricTypeId", "dbo.MetricTypes");
            DropForeignKey("dbo.Metrics", "StatusDataId", "dbo.Bulbs");
            DropForeignKey("dbo.Bulbs", "ComponentId", "dbo.Components");
            DropForeignKey("dbo.UserSettings", "UserId", "dbo.Users");
            DropForeignKey("dbo.UserRoles", "UserId", "dbo.Users");
            DropForeignKey("dbo.UserRoles", "RoleId", "dbo.Roles");
            DropForeignKey("dbo.DefectChanges", "DefectId", "dbo.Defects");
            DropForeignKey("dbo.AccountTariffs", "TariffLimitId", "dbo.TariffLimits");
            DropIndex("dbo.EventStatuses", new[] { "StatusId" });
            DropIndex("dbo.EventStatuses", new[] { "EventId" });
            DropIndex("dbo.Tokens", new[] { "UserId" });
            DropIndex("dbo.SendSmsCommand", "IX_ForSend");
            DropIndex("dbo.SendEmailCommand", "IX_ForSend");
            DropIndex("dbo.MetricHistory", new[] { "StatusEventId" });
            DropIndex("dbo.MetricHistory", "IX_ForHistoryDeletion");
            DropIndex("dbo.MetricHistory", "IX_ForHistory");
            DropIndex("dbo.Logs", "IX_ComponentBased");
            DropIndex("dbo.LogParameters", new[] { "LogId" });
            DropIndex("dbo.LimitDatasForUnitTests", new[] { "UnitTestId" });
            DropIndex("dbo.LimitDatasForUnitTests", new[] { "LimitDataId" });
            DropIndex("dbo.LimitDatas", "IX_AccountId");
            DropIndex("dbo.EventParameters", new[] { "EventId" });
            DropIndex("dbo.UserContacts", new[] { "UserId" });
            DropIndex("dbo.ComponentProperties", new[] { "ComponentId" });
            DropIndex("dbo.LogConfigs", new[] { "ComponentId" });
            DropIndex("dbo.NotificationsHttp", new[] { "NotificationId" });
            DropIndex("dbo.Notifications", new[] { "SubscriptionId" });
            DropIndex("dbo.Notifications", new[] { "Status" });
            DropIndex("dbo.Notifications", new[] { "UserId" });
            DropIndex("dbo.Notifications", new[] { "EventId" });
            DropIndex("dbo.LastComponentNotifications", new[] { "NotificationId" });
            DropIndex("dbo.LastComponentNotifications", new[] { "EventId" });
            DropIndex("dbo.LastComponentNotifications", new[] { "ComponentId" });
            DropIndex("dbo.ComponentTypes", new[] { "SystemName" });
            DropIndex("dbo.UnitTestTypes", new[] { "SystemName" });
            DropIndex("dbo.UnitTestSslCertificateExpirationDateRules", new[] { "UnitTestId" });
            DropIndex("dbo.UnitTestSqlRules", new[] { "UnitTestId" });
            DropIndex("dbo.UnitTestProperties", new[] { "UnitTestId" });
            DropIndex("dbo.UnitTestPingRules", new[] { "UnitTestId" });
            DropIndex("dbo.HttpRequestUnitTestRuleDatas", new[] { "RuleId" });
            DropIndex("dbo.HttpRequestUnitTestRules", new[] { "HttpRequestUnitTestId" });
            DropIndex("dbo.HttpRequestUnitTests", new[] { "UnitTestId" });
            DropIndex("dbo.UnitTestDomainNamePaymentPeriodRules", new[] { "UnitTestId" });
            DropIndex("dbo.UnitTests", new[] { "StatusDataId" });
            DropIndex("dbo.UnitTests", new[] { "ComponentId" });
            DropIndex("dbo.UnitTests", new[] { "TypeId" });
            DropIndex("dbo.Metrics", new[] { "StatusDataId" });
            DropIndex("dbo.Metrics", new[] { "MetricTypeId" });
            DropIndex("dbo.Metrics", new[] { "ComponentId" });
            DropIndex("dbo.Bulbs", new[] { "MetricId" });
            DropIndex("dbo.Bulbs", new[] { "UnitTestId" });
            DropIndex("dbo.Bulbs", new[] { "ComponentId" });
            DropIndex("dbo.Components", new[] { "ChildComponentsStatusId" });
            DropIndex("dbo.Components", new[] { "MetricsStatusId" });
            DropIndex("dbo.Components", new[] { "EventsStatusId" });
            DropIndex("dbo.Components", new[] { "UnitTestsStatusId" });
            DropIndex("dbo.Components", new[] { "ExternalStatusId" });
            DropIndex("dbo.Components", new[] { "InternalStatusId" });
            DropIndex("dbo.Components", new[] { "ComponentTypeId" });
            DropIndex("dbo.Components", new[] { "ParentId" });
            DropIndex("dbo.Components", new[] { "SystemName" });
            DropIndex("dbo.Subscriptions", new[] { "ComponentId" });
            DropIndex("dbo.Subscriptions", new[] { "ComponentTypeId" });
            DropIndex("dbo.Subscriptions", new[] { "UserId" });
            DropIndex("dbo.UserSettings", new[] { "UserId" });
            DropIndex("dbo.UserRoles", new[] { "RoleId" });
            DropIndex("dbo.UserRoles", new[] { "UserId" });
            DropIndex("dbo.Users", new[] { "Login" });
            DropIndex("dbo.DefectChanges", new[] { "UserId" });
            DropIndex("dbo.DefectChanges", new[] { "DefectId" });
            DropIndex("dbo.Defects", new[] { "EventTypeId" });
            DropIndex("dbo.Defects", new[] { "ResponsibleUserId" });
            DropIndex("dbo.Defects", new[] { "LastChangeId" });
            DropIndex("dbo.EventTypes", new[] { "DefectId" });
            DropIndex("dbo.EventTypes", new[] { "SystemName" });
            DropIndex("dbo.Events", new[] { "LastStatusEventId" });
            DropIndex("dbo.Events", "IX_ForProcessing");
            DropIndex("dbo.Events", "IX_ForJoin");
            DropIndex("dbo.Events", "IX_OwnerBased");
            DropIndex("dbo.Events", "IX_AccountBased");
            DropIndex("dbo.ArchivedStatuses", new[] { "EventId" });
            DropIndex("dbo.AccountTariffs", new[] { "TariffLimitId" });
            DropTable("dbo.EventStatuses");
            DropTable("dbo.Tokens");
            DropTable("dbo.SendSmsCommand");
            DropTable("dbo.SendEmailCommand");
            DropTable("dbo.MetricHistory");
            DropTable("dbo.Logs");
            DropTable("dbo.LogParameters");
            DropTable("dbo.LimitDatasForUnitTests");
            DropTable("dbo.LimitDatas");
            DropTable("dbo.EventParameters");
            DropTable("dbo.UserContacts");
            DropTable("dbo.ComponentProperties");
            DropTable("dbo.LogConfigs");
            DropTable("dbo.NotificationsHttp");
            DropTable("dbo.Notifications");
            DropTable("dbo.LastComponentNotifications");
            DropTable("dbo.ComponentTypes");
            DropTable("dbo.UnitTestTypes");
            DropTable("dbo.UnitTestSslCertificateExpirationDateRules");
            DropTable("dbo.UnitTestSqlRules");
            DropTable("dbo.UnitTestProperties");
            DropTable("dbo.UnitTestPingRules");
            DropTable("dbo.HttpRequestUnitTestRuleDatas");
            DropTable("dbo.HttpRequestUnitTestRules");
            DropTable("dbo.HttpRequestUnitTests");
            DropTable("dbo.UnitTestDomainNamePaymentPeriodRules");
            DropTable("dbo.UnitTests");
            DropTable("dbo.MetricTypes");
            DropTable("dbo.Metrics");
            DropTable("dbo.Bulbs");
            DropTable("dbo.Components");
            DropTable("dbo.Subscriptions");
            DropTable("dbo.UserSettings");
            DropTable("dbo.Roles");
            DropTable("dbo.UserRoles");
            DropTable("dbo.Users");
            DropTable("dbo.DefectChanges");
            DropTable("dbo.Defects");
            DropTable("dbo.EventTypes");
            DropTable("dbo.Events");
            DropTable("dbo.ArchivedStatuses");
            DropTable("dbo.TariffLimits");
            DropTable("dbo.AccountTariffs");
        }
    }
}
