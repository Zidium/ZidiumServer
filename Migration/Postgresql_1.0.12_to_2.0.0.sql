-- This script converts Zidium Postgresql database from version 1.0.12 to 2.0.0

DROP INDEX dbo."UnitTestVirusTotalRules_IX_UnitTestId";

DROP INDEX dbo."Notifications_IX_SendMessageCommandId";

DROP INDEX dbo."Notifications_IX_SendEmailCommandId";

DROP INDEX dbo."SendMessageCommand_IX_ForSend";

DROP INDEX dbo."UnitTestTcpPortRules_IX_UnitTestId";

DROP INDEX dbo."Logs_IX_Date";

DROP INDEX dbo."MetricHistory_IX_BeginDate";

DROP INDEX dbo."EventStatuses_IX_StatusId";

DROP INDEX dbo."EventStatuses_IX_EventId";

DROP INDEX dbo."Tokens_IX_UserId";

DROP INDEX dbo."SendSmsCommand_IX_ForSend";

DROP INDEX dbo."SendEmailCommand_IX_ForSend";

DROP INDEX dbo."MetricHistory_IX_StatusEventId";

DROP INDEX dbo."MetricHistory_IX_ForHistory";

DROP INDEX dbo."Logs_IX_ComponentBased";

DROP INDEX dbo."LogParameters_IX_LogId";

DROP INDEX dbo."LimitDatasForUnitTests_IX_UnitTestId";

DROP INDEX dbo."LimitDatasForUnitTests_IX_LimitDataId";

DROP INDEX dbo."LimitDatas_IX_AccountId";

DROP INDEX dbo."EventParameters_IX_EventId";

DROP INDEX dbo."UserContacts_IX_UserId";

DROP INDEX dbo."ComponentProperties_IX_ComponentId";

DROP INDEX dbo."LogConfigs_IX_ComponentId";

DROP INDEX dbo."NotificationsHttp_IX_NotificationId";

DROP INDEX dbo."Notifications_IX_SubscriptionId";

DROP INDEX dbo."Notifications_IX_Status";

DROP INDEX dbo."Notifications_IX_UserId";

DROP INDEX dbo."Notifications_IX_EventId";

DROP INDEX dbo."LastComponentNotifications_IX_NotificationId";

DROP INDEX dbo."LastComponentNotifications_IX_EventId";

DROP INDEX dbo."LastComponentNotifications_IX_ComponentId";

DROP INDEX dbo."ComponentTypes_IX_SystemName";

DROP INDEX dbo."UnitTestTypes_IX_SystemName";

DROP INDEX dbo."UnitTestSslCertificateExpirationDateRules_IX_UnitTestId";

DROP INDEX dbo."UnitTestSqlRules_IX_UnitTestId";

DROP INDEX dbo."UnitTestProperties_IX_UnitTestId";

DROP INDEX dbo."UnitTestPingRules_IX_UnitTestId";

DROP INDEX dbo."HttpRequestUnitTestRuleDatas_IX_RuleId";

DROP INDEX dbo."HttpRequestUnitTestRules_IX_HttpRequestUnitTestId";

DROP INDEX dbo."HttpRequestUnitTests_IX_UnitTestId";

DROP INDEX dbo."UnitTestDomainNamePaymentPeriodRules_IX_UnitTestId";

DROP INDEX dbo."UnitTests_IX_StatusDataId";

DROP INDEX dbo."UnitTests_IX_ComponentId";

DROP INDEX dbo."UnitTests_IX_TypeId";

DROP INDEX dbo."Metrics_IX_StatusDataId";

DROP INDEX dbo."Metrics_IX_MetricTypeId";

DROP INDEX dbo."Metrics_IX_ComponentId";

DROP INDEX dbo."Bulbs_IX_MetricId";

DROP INDEX dbo."Bulbs_IX_UnitTestId";

DROP INDEX dbo."Bulbs_IX_ComponentId";

DROP INDEX dbo."Components_IX_ChildComponentsStatusId";

DROP INDEX dbo."Components_IX_MetricsStatusId";

DROP INDEX dbo."Components_IX_EventsStatusId";

DROP INDEX dbo."Components_IX_UnitTestsStatusId";

DROP INDEX dbo."Components_IX_ExternalStatusId";

DROP INDEX dbo."Components_IX_InternalStatusId";

DROP INDEX dbo."Components_IX_ComponentTypeId";

DROP INDEX dbo."Components_IX_ParentId";

DROP INDEX dbo."Components_IX_SystemName";

DROP INDEX dbo."Subscriptions_IX_ComponentId";

DROP INDEX dbo."Subscriptions_IX_ComponentTypeId";

DROP INDEX dbo."Subscriptions_IX_UserId";

DROP INDEX dbo."UserSettings_IX_UserId";

DROP INDEX dbo."UserRoles_IX_RoleId";

DROP INDEX dbo."UserRoles_IX_UserId";

DROP INDEX dbo."Users_IX_Login";

DROP INDEX dbo."DefectChanges_IX_UserId";

DROP INDEX dbo."DefectChanges_IX_DefectId";

DROP INDEX dbo."Defects_IX_EventTypeId";

DROP INDEX dbo."Defects_IX_ResponsibleUserId";

DROP INDEX dbo."Defects_IX_LastChangeId";

DROP INDEX dbo."EventTypes_IX_DefectId";

DROP INDEX dbo."EventTypes_IX_SystemName";

DROP INDEX dbo."Events_IX_LastStatusEventId";

DROP INDEX dbo."Events_IX_ForProcessing";

DROP INDEX dbo."Events_IX_ForJoin";

DROP INDEX dbo."Events_IX_OwnerBased";

DROP INDEX dbo."Events_IX_AccountBased";

DROP INDEX dbo."ArchivedStatuses_IX_EventId";

ALTER TABLE dbo."NotificationsHttp"
    DROP CONSTRAINT "FK_dbo.NotificationsHttp_dbo.Notifications_NotificationId";

ALTER TABLE dbo."NotificationsHttp" RENAME CONSTRAINT "PK_dbo.NotificationsHttp" TO "PK_NotificationsHttp";

ALTER TABLE dbo."SendMessageCommand" RENAME CONSTRAINT "PK_dbo.SendMessageCommand" TO "PK_SendMessageCommand";

ALTER TABLE dbo."SendEmailCommand" RENAME CONSTRAINT "PK_dbo.SendEmailCommand" TO "PK_SendEmailCommand";

ALTER TABLE dbo."Notifications"
    DROP CONSTRAINT "FK_dbo.Notifications_dbo.Events_EventId",
    DROP CONSTRAINT "FK_dbo.Notifications_dbo.Users_UserId",
    DROP CONSTRAINT "FK_dbo.Notifications_dbo.Subscriptions_SubscriptionId",
    DROP CONSTRAINT "FK_dbo.Notifications_dbo.SendEmailCommand_SendEmailCommandId",
    DROP CONSTRAINT "FK_dbo.Notifications_dbo.SendMessageCommand_SendMessageCommandI";

ALTER TABLE dbo."Notifications" RENAME CONSTRAINT "PK_dbo.Notifications" TO "PK_Notifications";

ALTER TABLE dbo."EventStatuses"
    DROP CONSTRAINT "FK_dbo.EventStatuses_dbo.Events_StatusId",
    DROP CONSTRAINT "FK_dbo.EventStatuses_dbo.Events_EventId";

ALTER TABLE dbo."EventStatuses" RENAME CONSTRAINT "PK_dbo.EventStatuses" TO "PK_EventStatuses";

ALTER TABLE dbo."EventParameters"
    DROP CONSTRAINT "FK_dbo.EventParameters_dbo.Events_EventId";

ALTER TABLE dbo."EventParameters" RENAME CONSTRAINT "PK_dbo.EventParameters" TO "PK_EventParameters";

ALTER TABLE dbo."ArchivedStatuses"
    DROP CONSTRAINT "FK_dbo.ArchivedStatuses_dbo.Events_EventId";

ALTER TABLE dbo."ArchivedStatuses" RENAME CONSTRAINT "PK_dbo.ArchivedStatuses" TO "PK_ArchivedStatuses";

ALTER TABLE dbo."ArchivedStatuses" ALTER COLUMN "Id" DROP DEFAULT
    , ALTER COLUMN "Id" ADD GENERATED BY DEFAULT AS IDENTITY;

ALTER TABLE dbo."Events"
    DROP CONSTRAINT "FK_dbo.Events_dbo.Events_LastStatusEventId",
    DROP CONSTRAINT "FK_dbo.Events_dbo.EventTypes_EventTypeId";

ALTER TABLE dbo."Events" RENAME CONSTRAINT "PK_dbo.Events" TO "PK_Events";

ALTER TABLE dbo."EventTypes"
    DROP CONSTRAINT "FK_dbo.EventTypes_dbo.Defects_DefectId";

ALTER TABLE dbo."EventTypes" RENAME CONSTRAINT "PK_dbo.EventTypes" TO "PK_EventTypes";

ALTER TABLE dbo."DefectChanges"
    DROP CONSTRAINT "FK_dbo.DefectChanges_dbo.Defects_DefectId",
    DROP CONSTRAINT "FK_dbo.DefectChanges_dbo.Users_UserId";

ALTER TABLE dbo."DefectChanges" RENAME CONSTRAINT "PK_dbo.DefectChanges" TO "PK_DefectChanges";

ALTER TABLE dbo."Defects"
    DROP CONSTRAINT "FK_dbo.Defects_dbo.EventTypes_EventTypeId",
    DROP CONSTRAINT "FK_dbo.Defects_dbo.DefectChanges_LastChangeId",
    DROP CONSTRAINT "FK_dbo.Defects_dbo.Users_ResponsibleUserId";

ALTER TABLE dbo."Defects" RENAME CONSTRAINT "PK_dbo.Defects" TO "PK_Defects";

ALTER TABLE dbo."MetricHistory"
    DROP CONSTRAINT "FK_dbo.MetricHistory_dbo.Events_StatusEventId",
    DROP CONSTRAINT "FK_dbo.MetricHistory_dbo.Components_ComponentId",
    DROP CONSTRAINT "FK_dbo.MetricHistory_dbo.MetricTypes_MetricTypeId";

ALTER TABLE dbo."MetricHistory" RENAME CONSTRAINT "PK_dbo.MetricHistory" TO "PK_MetricHistory";

ALTER TABLE dbo."LastComponentNotifications"
    DROP CONSTRAINT "FK_dbo.LastComponentNotifications_dbo.Events_EventId",
    DROP CONSTRAINT "FK_dbo.LastComponentNotifications_dbo.Components_ComponentId",
    DROP CONSTRAINT "FK_dbo.LastComponentNotifications_dbo.Notifications_Notificatio";

ALTER TABLE dbo."LastComponentNotifications" RENAME CONSTRAINT "PK_dbo.LastComponentNotifications" TO "PK_LastComponentNotifications";

ALTER TABLE dbo."HttpRequestUnitTestRuleDatas"
    DROP CONSTRAINT "FK_dbo.HttpRequestUnitTestRuleDatas_dbo.HttpRequestUnitTestRule";

ALTER TABLE dbo."HttpRequestUnitTestRuleDatas" RENAME CONSTRAINT "PK_dbo.HttpRequestUnitTestRuleDatas" TO "PK_HttpRequestUnitTestRuleDatas";

ALTER TABLE dbo."HttpRequestUnitTestRules"
    DROP CONSTRAINT "FK_dbo.HttpRequestUnitTestRules_dbo.HttpRequestUnitTests_HttpRe";

ALTER TABLE dbo."HttpRequestUnitTestRules" RENAME CONSTRAINT "PK_dbo.HttpRequestUnitTestRules" TO "PK_HttpRequestUnitTestRules";

ALTER TABLE dbo."UnitTestVirusTotalRules"
    DROP CONSTRAINT "FK_dbo.UnitTestVirusTotalRules_dbo.UnitTests_UnitTestId";

ALTER TABLE dbo."UnitTestVirusTotalRules" RENAME CONSTRAINT "PK_dbo.UnitTestVirusTotalRules" TO "PK_UnitTestVirusTotalRules";

ALTER TABLE dbo."UnitTestTcpPortRules"
    DROP CONSTRAINT "FK_dbo.UnitTestTcpPortRules_dbo.UnitTests_UnitTestId";

ALTER TABLE dbo."UnitTestTcpPortRules" RENAME CONSTRAINT "PK_dbo.UnitTestTcpPortRules" TO "PK_UnitTestTcpPortRules";

ALTER TABLE dbo."UnitTestSslCertificateExpirationDateRules"
    DROP CONSTRAINT "FK_dbo.UnitTestSslCertificateExpirationDateRules_dbo.UnitTests_";

ALTER TABLE dbo."UnitTestSslCertificateExpirationDateRules" RENAME CONSTRAINT "PK_dbo.UnitTestSslCertificateExpirationDateRules" TO "PK_UnitTestSslCertificateExpirationDateRules";

ALTER TABLE dbo."UnitTestSqlRules"
    DROP CONSTRAINT "FK_dbo.UnitTestSqlRules_dbo.UnitTests_UnitTestId";

ALTER TABLE dbo."UnitTestSqlRules" RENAME CONSTRAINT "PK_dbo.UnitTestSqlRules" TO "PK_UnitTestSqlRules";

ALTER TABLE dbo."UnitTestProperties"
    DROP CONSTRAINT "FK_dbo.UnitTestProperties_dbo.UnitTests_UnitTestId";

ALTER TABLE dbo."UnitTestProperties" RENAME CONSTRAINT "PK_dbo.UnitTestProperties" TO "PK_UnitTestProperties";

ALTER TABLE dbo."UnitTestPingRules"
    DROP CONSTRAINT "FK_dbo.UnitTestPingRules_dbo.UnitTests_UnitTestId";

ALTER TABLE dbo."UnitTestPingRules" RENAME CONSTRAINT "PK_dbo.UnitTestPingRules" TO "PK_UnitTestPingRules";

ALTER TABLE dbo."UnitTestDomainNamePaymentPeriodRules"
    DROP CONSTRAINT "FK_dbo.UnitTestDomainNamePaymentPeriodRules_dbo.UnitTests_UnitT";

ALTER TABLE dbo."UnitTestDomainNamePaymentPeriodRules" RENAME CONSTRAINT "PK_dbo.UnitTestDomainNamePaymentPeriodRules" TO "PK_UnitTestDomainNamePaymentPeriodRules";

ALTER TABLE dbo."LimitDatasForUnitTests"
    DROP CONSTRAINT "FK_dbo.LimitDatasForUnitTests_dbo.UnitTests_UnitTestId",
    DROP CONSTRAINT "FK_dbo.LimitDatasForUnitTests_dbo.LimitDatas_LimitDataId";

ALTER TABLE dbo."LimitDatasForUnitTests" RENAME CONSTRAINT "PK_dbo.LimitDatasForUnitTests" TO "PK_LimitDatasForUnitTests";

ALTER TABLE dbo."LimitDatas" RENAME CONSTRAINT "PK_dbo.LimitDatas" TO "PK_LimitDatas";

ALTER TABLE dbo."HttpRequestUnitTests"
    DROP CONSTRAINT "FK_dbo.HttpRequestUnitTests_dbo.UnitTests_UnitTestId";

ALTER TABLE dbo."HttpRequestUnitTests" RENAME CONSTRAINT "PK_dbo.HttpRequestUnitTests" TO "PK_HttpRequestUnitTests";

ALTER TABLE dbo."Bulbs"
    DROP CONSTRAINT "FK_dbo.Bulbs_dbo.Components_ComponentId",
    DROP CONSTRAINT "FK_dbo.Bulbs_dbo.Metrics_MetricId",
    DROP CONSTRAINT "FK_dbo.Bulbs_dbo.UnitTests_UnitTestId";

ALTER TABLE dbo."Bulbs" RENAME CONSTRAINT "PK_dbo.Bulbs" TO "PK_Bulbs";

ALTER TABLE dbo."UnitTestTypes" RENAME CONSTRAINT "PK_dbo.UnitTestTypes" TO "PK_UnitTestTypes";

ALTER TABLE dbo."UnitTests"
    DROP CONSTRAINT "FK_dbo.UnitTests_dbo.Components_ComponentId",
    DROP CONSTRAINT "FK_dbo.UnitTests_dbo.Bulbs_StatusDataId",
    DROP CONSTRAINT "FK_dbo.UnitTests_dbo.UnitTestTypes_TypeId";

ALTER TABLE dbo."UnitTests" RENAME CONSTRAINT "PK_dbo.UnitTests" TO "PK_UnitTests";

ALTER TABLE dbo."MetricTypes" RENAME CONSTRAINT "PK_dbo.MetricTypes" TO "PK_MetricTypes";

ALTER TABLE dbo."Metrics"
    DROP CONSTRAINT "FK_dbo.Metrics_dbo.Components_ComponentId",
    DROP CONSTRAINT "FK_dbo.Metrics_dbo.Bulbs_StatusDataId",
    DROP CONSTRAINT "FK_dbo.Metrics_dbo.MetricTypes_MetricTypeId";

ALTER TABLE dbo."Metrics" RENAME CONSTRAINT "PK_dbo.Metrics" TO "PK_Metrics";

ALTER TABLE dbo."LogParameters"
    DROP CONSTRAINT "FK_dbo.LogParameters_dbo.Logs_LogId";

ALTER TABLE dbo."LogParameters" RENAME CONSTRAINT "PK_dbo.LogParameters" TO "PK_LogParameters";

ALTER TABLE dbo."Subscriptions"
    DROP CONSTRAINT "FK_dbo.Subscriptions_dbo.Users_UserId",
    DROP CONSTRAINT "FK_dbo.Subscriptions_dbo.Components_ComponentId",
    DROP CONSTRAINT "FK_dbo.Subscriptions_dbo.ComponentTypes_ComponentTypeId";

ALTER TABLE dbo."Subscriptions" RENAME CONSTRAINT "PK_dbo.Subscriptions" TO "PK_Subscriptions";

ALTER TABLE dbo."Logs"
    DROP CONSTRAINT "FK_dbo.Logs_dbo.Components_ComponentId";

ALTER TABLE dbo."Logs" RENAME CONSTRAINT "PK_dbo.Logs" TO "PK_Logs";

ALTER TABLE dbo."LogConfigs"
    DROP CONSTRAINT "FK_dbo.LogConfigs_dbo.Components_ComponentId";

ALTER TABLE dbo."LogConfigs" RENAME CONSTRAINT "PK_dbo.LogConfigs" TO "PK_LogConfigs";

ALTER TABLE dbo."ComponentProperties"
    DROP CONSTRAINT "FK_dbo.ComponentProperties_dbo.Components_ComponentId";

ALTER TABLE dbo."ComponentProperties" RENAME CONSTRAINT "PK_dbo.ComponentProperties" TO "PK_ComponentProperties";

ALTER TABLE dbo."ComponentProperties"
    ALTER COLUMN "Value" SET DATA TYPE character varying ( 8000 );

ALTER TABLE dbo."ComponentTypes" RENAME CONSTRAINT "PK_dbo.ComponentTypes" TO "PK_ComponentTypes";

ALTER TABLE dbo."Components"
    DROP CONSTRAINT "FK_dbo.Components_dbo.Components_ParentId",
    DROP CONSTRAINT "FK_dbo.Components_dbo.Bulbs_UnitTestsStatusId",
    DROP CONSTRAINT "FK_dbo.Components_dbo.Bulbs_MetricsStatusId",
    DROP CONSTRAINT "FK_dbo.Components_dbo.Bulbs_InternalStatusId",
    DROP CONSTRAINT "FK_dbo.Components_dbo.Bulbs_ExternalStatusId",
    DROP CONSTRAINT "FK_dbo.Components_dbo.Bulbs_EventsStatusId",
    DROP CONSTRAINT "FK_dbo.Components_dbo.Bulbs_ChildComponentsStatusId",
    DROP CONSTRAINT "FK_dbo.Components_dbo.ComponentTypes_ComponentTypeId";

ALTER TABLE dbo."Components" RENAME CONSTRAINT "PK_dbo.Components" TO "PK_Components";

ALTER TABLE dbo."UserSettings"
    DROP CONSTRAINT "FK_dbo.UserSettings_dbo.Users_UserId";

ALTER TABLE dbo."UserSettings" RENAME CONSTRAINT "PK_dbo.UserSettings" TO "PK_UserSettings";

ALTER TABLE dbo."UserRoles"
    DROP CONSTRAINT "FK_dbo.UserRoles_dbo.Users_UserId",
    DROP CONSTRAINT "FK_dbo.UserRoles_dbo.Roles_RoleId";

ALTER TABLE dbo."UserRoles" RENAME CONSTRAINT "PK_dbo.UserRoles" TO "PK_UserRoles";

ALTER TABLE dbo."Roles" RENAME CONSTRAINT "PK_dbo.Roles" TO "PK_Roles";

ALTER TABLE dbo."UserContacts"
    DROP CONSTRAINT "FK_dbo.UserContacts_dbo.Users_UserId";

ALTER TABLE dbo."UserContacts" RENAME CONSTRAINT "PK_dbo.UserContacts" TO "PK_UserContacts";

ALTER TABLE dbo."Tokens"
    DROP CONSTRAINT "FK_dbo.Tokens_dbo.Users_UserId";

ALTER TABLE dbo."Tokens" RENAME CONSTRAINT "PK_dbo.Tokens" TO "PK_Tokens";

ALTER TABLE dbo."Users" RENAME CONSTRAINT "PK_dbo.Users" TO "PK_Users";

ALTER TABLE dbo."TimeZones" RENAME CONSTRAINT "PK_dbo.TimeZones" TO "PK_TimeZones";

ALTER TABLE dbo."SendSmsCommand" RENAME CONSTRAINT "PK_dbo.SendSmsCommand" TO "PK_SendSmsCommand";

ALTER TABLE dbo."AccountSettings" RENAME CONSTRAINT "PK_dbo.AccountSettings" TO "PK_AccountSettings";

DROP TABLE dbo."__MigrationHistory";

DROP TABLE dbo."TariffLimits";

CREATE TABLE public."__EFMigrationsHistory"
(
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ( "MigrationId" )
);

ALTER TABLE dbo."HttpRequestUnitTests"
    DROP COLUMN "HasBanner",
    DROP COLUMN "LastBannerCheck";

ALTER TABLE dbo."NotificationsHttp"
    ADD CONSTRAINT "FK_NotificationsHttp_Notifications_NotificationId" FOREIGN KEY ( "NotificationId" ) REFERENCES dbo."Notifications" ( "Id" ) ON DELETE RESTRICT;

ALTER TABLE dbo."Notifications"
    ADD CONSTRAINT "FK_Notifications_SendEmailCommand_SendEmailCommandId" FOREIGN KEY ( "SendEmailCommandId" ) REFERENCES dbo."SendEmailCommand" ( "Id" ) ON DELETE RESTRICT,
    ADD CONSTRAINT "FK_Notifications_SendMessageCommand_SendMessageCommandId" FOREIGN KEY ( "SendMessageCommandId" ) REFERENCES dbo."SendMessageCommand" ( "Id" ) ON DELETE RESTRICT,
    ADD CONSTRAINT "FK_Notifications_Users_UserId" FOREIGN KEY ( "UserId" ) REFERENCES dbo."Users" ( "Id" ) ON DELETE RESTRICT,
    ADD CONSTRAINT "FK_Notifications_Subscriptions_SubscriptionId" FOREIGN KEY ( "SubscriptionId" ) REFERENCES dbo."Subscriptions" ( "Id" ) ON DELETE RESTRICT,
    ADD CONSTRAINT "FK_Notifications_Events_EventId" FOREIGN KEY ( "EventId" ) REFERENCES dbo."Events" ( "Id" ) ON DELETE RESTRICT;

ALTER TABLE dbo."EventStatuses"
    ADD CONSTRAINT "FK_EventStatuses_Events_StatusId" FOREIGN KEY ( "StatusId" ) REFERENCES dbo."Events" ( "Id" ) ON DELETE RESTRICT,
    ADD CONSTRAINT "FK_EventStatuses_Events_EventId" FOREIGN KEY ( "EventId" ) REFERENCES dbo."Events" ( "Id" ) ON DELETE RESTRICT;

ALTER TABLE dbo."EventParameters"
    ADD CONSTRAINT "FK_EventParameters_Events_EventId" FOREIGN KEY ( "EventId" ) REFERENCES dbo."Events" ( "Id" ) ON DELETE RESTRICT;

ALTER TABLE dbo."ArchivedStatuses"
    ADD CONSTRAINT "FK_ArchivedStatuses_Events_EventId" FOREIGN KEY ( "EventId" ) REFERENCES dbo."Events" ( "Id" ) ON DELETE RESTRICT;

ALTER TABLE dbo."Events"
    ADD CONSTRAINT "FK_Events_EventTypes_EventTypeId" FOREIGN KEY ( "EventTypeId" ) REFERENCES dbo."EventTypes" ( "Id" ) ON DELETE RESTRICT,
    ADD CONSTRAINT "FK_Events_Events_LastStatusEventId" FOREIGN KEY ( "LastStatusEventId" ) REFERENCES dbo."Events" ( "Id" ) ON DELETE RESTRICT;

ALTER TABLE dbo."EventTypes"
    ADD CONSTRAINT "FK_EventTypes_Defects_DefectId" FOREIGN KEY ( "DefectId" ) REFERENCES dbo."Defects" ( "Id" ) ON DELETE RESTRICT;

ALTER TABLE dbo."DefectChanges"
    ADD CONSTRAINT "FK_DefectChanges_Users_UserId" FOREIGN KEY ( "UserId" ) REFERENCES dbo."Users" ( "Id" ) ON DELETE RESTRICT,
    ADD CONSTRAINT "FK_DefectChanges_Defects_DefectId" FOREIGN KEY ( "DefectId" ) REFERENCES dbo."Defects" ( "Id" ) ON DELETE RESTRICT;

ALTER TABLE dbo."Defects"
    ADD CONSTRAINT "FK_Defects_Users_ResponsibleUserId" FOREIGN KEY ( "ResponsibleUserId" ) REFERENCES dbo."Users" ( "Id" ) ON DELETE RESTRICT,
    ADD CONSTRAINT "FK_Defects_DefectChanges_LastChangeId" FOREIGN KEY ( "LastChangeId" ) REFERENCES dbo."DefectChanges" ( "Id" ) ON DELETE RESTRICT,
    ADD CONSTRAINT "FK_Defects_EventTypes_EventTypeId" FOREIGN KEY ( "EventTypeId" ) REFERENCES dbo."EventTypes" ( "Id" ) ON DELETE RESTRICT;

ALTER TABLE dbo."MetricHistory"
    ADD CONSTRAINT "FK_MetricHistory_MetricTypes_MetricTypeId" FOREIGN KEY ( "MetricTypeId" ) REFERENCES dbo."MetricTypes" ( "Id" ) ON DELETE RESTRICT,
    ADD CONSTRAINT "FK_MetricHistory_Components_ComponentId" FOREIGN KEY ( "ComponentId" ) REFERENCES dbo."Components" ( "Id" ) ON DELETE RESTRICT,
    ADD CONSTRAINT "FK_MetricHistory_Events_StatusEventId" FOREIGN KEY ( "StatusEventId" ) REFERENCES dbo."Events" ( "Id" ) ON DELETE RESTRICT;

ALTER TABLE dbo."LastComponentNotifications"
    ADD CONSTRAINT "FK_LastComponentNotifications_Components_ComponentId" FOREIGN KEY ( "ComponentId" ) REFERENCES dbo."Components" ( "Id" ) ON DELETE RESTRICT,
    ADD CONSTRAINT "FK_LastComponentNotifications_Events_EventId" FOREIGN KEY ( "EventId" ) REFERENCES dbo."Events" ( "Id" ) ON DELETE RESTRICT,
    ADD CONSTRAINT "FK_LastComponentNotifications_Notifications_NotificationId" FOREIGN KEY ( "NotificationId" ) REFERENCES dbo."Notifications" ( "Id" ) ON DELETE RESTRICT;

ALTER TABLE dbo."HttpRequestUnitTestRuleDatas"
    ADD CONSTRAINT "FK_HttpRequestUnitTestRuleDatas_HttpRequestUnitTestRules_RuleId" FOREIGN KEY ( "RuleId" ) REFERENCES dbo."HttpRequestUnitTestRules" ( "Id" ) ON DELETE RESTRICT;

ALTER TABLE dbo."HttpRequestUnitTestRules"
    ADD CONSTRAINT "FK_HttpRequestUnitTestRules_HttpRequestUnitTests_HttpRequestUn~" FOREIGN KEY ( "HttpRequestUnitTestId" ) REFERENCES dbo."HttpRequestUnitTests" ( "UnitTestId" ) ON DELETE RESTRICT;

ALTER TABLE dbo."UnitTestVirusTotalRules"
    ADD CONSTRAINT "FK_UnitTestVirusTotalRules_UnitTests_UnitTestId" FOREIGN KEY ( "UnitTestId" ) REFERENCES dbo."UnitTests" ( "Id" ) ON DELETE RESTRICT;

ALTER TABLE dbo."UnitTestTcpPortRules"
    ADD CONSTRAINT "FK_UnitTestTcpPortRules_UnitTests_UnitTestId" FOREIGN KEY ( "UnitTestId" ) REFERENCES dbo."UnitTests" ( "Id" ) ON DELETE RESTRICT;

ALTER TABLE dbo."UnitTestSslCertificateExpirationDateRules"
    ADD CONSTRAINT "FK_UnitTestSslCertificateExpirationDateRules_UnitTests_UnitTes~" FOREIGN KEY ( "UnitTestId" ) REFERENCES dbo."UnitTests" ( "Id" ) ON DELETE RESTRICT;

ALTER TABLE dbo."UnitTestSqlRules"
    ADD CONSTRAINT "FK_UnitTestSqlRules_UnitTests_UnitTestId" FOREIGN KEY ( "UnitTestId" ) REFERENCES dbo."UnitTests" ( "Id" ) ON DELETE RESTRICT;

ALTER TABLE dbo."UnitTestProperties"
    ADD CONSTRAINT "FK_UnitTestProperties_UnitTests_UnitTestId" FOREIGN KEY ( "UnitTestId" ) REFERENCES dbo."UnitTests" ( "Id" ) ON DELETE RESTRICT;

ALTER TABLE dbo."UnitTestPingRules"
    ADD CONSTRAINT "FK_UnitTestPingRules_UnitTests_UnitTestId" FOREIGN KEY ( "UnitTestId" ) REFERENCES dbo."UnitTests" ( "Id" ) ON DELETE RESTRICT;

ALTER TABLE dbo."UnitTestDomainNamePaymentPeriodRules"
    ADD CONSTRAINT "FK_UnitTestDomainNamePaymentPeriodRules_UnitTests_UnitTestId" FOREIGN KEY ( "UnitTestId" ) REFERENCES dbo."UnitTests" ( "Id" ) ON DELETE RESTRICT;

ALTER TABLE dbo."LimitDatasForUnitTests"
    ADD CONSTRAINT "FK_LimitDatasForUnitTests_LimitDatas_LimitDataId" FOREIGN KEY ( "LimitDataId" ) REFERENCES dbo."LimitDatas" ( "Id" ) ON DELETE RESTRICT,
    ADD CONSTRAINT "FK_LimitDatasForUnitTests_UnitTests_UnitTestId" FOREIGN KEY ( "UnitTestId" ) REFERENCES dbo."UnitTests" ( "Id" ) ON DELETE RESTRICT;

ALTER TABLE dbo."HttpRequestUnitTests"
    ADD CONSTRAINT "FK_HttpRequestUnitTests_UnitTests_UnitTestId" FOREIGN KEY ( "UnitTestId" ) REFERENCES dbo."UnitTests" ( "Id" ) ON DELETE RESTRICT;

ALTER TABLE dbo."Bulbs"
    ADD CONSTRAINT "FK_Bulbs_Components_ComponentId" FOREIGN KEY ( "ComponentId" ) REFERENCES dbo."Components" ( "Id" ) ON DELETE RESTRICT,
    ADD CONSTRAINT "FK_Bulbs_Metrics_MetricId" FOREIGN KEY ( "MetricId" ) REFERENCES dbo."Metrics" ( "Id" ) ON DELETE RESTRICT,
    ADD CONSTRAINT "FK_Bulbs_UnitTests_UnitTestId" FOREIGN KEY ( "UnitTestId" ) REFERENCES dbo."UnitTests" ( "Id" ) ON DELETE RESTRICT;

ALTER TABLE dbo."UnitTests"
    ADD CONSTRAINT "FK_UnitTests_UnitTestTypes_TypeId" FOREIGN KEY ( "TypeId" ) REFERENCES dbo."UnitTestTypes" ( "Id" ) ON DELETE RESTRICT,
    ADD CONSTRAINT "FK_UnitTests_Components_ComponentId" FOREIGN KEY ( "ComponentId" ) REFERENCES dbo."Components" ( "Id" ) ON DELETE RESTRICT,
    ADD CONSTRAINT "FK_UnitTests_Bulbs_StatusDataId" FOREIGN KEY ( "StatusDataId" ) REFERENCES dbo."Bulbs" ( "Id" ) ON DELETE RESTRICT;

ALTER TABLE dbo."Metrics"
    ADD CONSTRAINT "FK_Metrics_MetricTypes_MetricTypeId" FOREIGN KEY ( "MetricTypeId" ) REFERENCES dbo."MetricTypes" ( "Id" ) ON DELETE RESTRICT,
    ADD CONSTRAINT "FK_Metrics_Components_ComponentId" FOREIGN KEY ( "ComponentId" ) REFERENCES dbo."Components" ( "Id" ) ON DELETE RESTRICT,
    ADD CONSTRAINT "FK_Metrics_Bulbs_StatusDataId" FOREIGN KEY ( "StatusDataId" ) REFERENCES dbo."Bulbs" ( "Id" ) ON DELETE RESTRICT;

ALTER TABLE dbo."LogParameters"
    ADD CONSTRAINT "FK_LogParameters_Logs_LogId" FOREIGN KEY ( "LogId" ) REFERENCES dbo."Logs" ( "Id" ) ON DELETE RESTRICT;

ALTER TABLE dbo."Subscriptions"
    ADD CONSTRAINT "FK_Subscriptions_ComponentTypes_ComponentTypeId" FOREIGN KEY ( "ComponentTypeId" ) REFERENCES dbo."ComponentTypes" ( "Id" ) ON DELETE RESTRICT,
    ADD CONSTRAINT "FK_Subscriptions_Users_UserId" FOREIGN KEY ( "UserId" ) REFERENCES dbo."Users" ( "Id" ) ON DELETE RESTRICT,
    ADD CONSTRAINT "FK_Subscriptions_Components_ComponentId" FOREIGN KEY ( "ComponentId" ) REFERENCES dbo."Components" ( "Id" ) ON DELETE RESTRICT;

ALTER TABLE dbo."Logs"
    ADD CONSTRAINT "FK_Logs_Components_ComponentId" FOREIGN KEY ( "ComponentId" ) REFERENCES dbo."Components" ( "Id" ) ON DELETE RESTRICT;

ALTER TABLE dbo."LogConfigs"
    ADD CONSTRAINT "FK_LogConfigs_Components_ComponentId" FOREIGN KEY ( "ComponentId" ) REFERENCES dbo."Components" ( "Id" ) ON DELETE RESTRICT;

ALTER TABLE dbo."ComponentProperties"
    ADD CONSTRAINT "FK_ComponentProperties_Components_ComponentId" FOREIGN KEY ( "ComponentId" ) REFERENCES dbo."Components" ( "Id" ) ON DELETE RESTRICT;

ALTER TABLE dbo."Components"
    ADD CONSTRAINT "FK_Components_ComponentTypes_ComponentTypeId" FOREIGN KEY ( "ComponentTypeId" ) REFERENCES dbo."ComponentTypes" ( "Id" ) ON DELETE RESTRICT,
    ADD CONSTRAINT "FK_Components_Components_ParentId" FOREIGN KEY ( "ParentId" ) REFERENCES dbo."Components" ( "Id" ) ON DELETE RESTRICT,
    ADD CONSTRAINT "FK_Components_Bulbs_UnitTestsStatusId" FOREIGN KEY ( "UnitTestsStatusId" ) REFERENCES dbo."Bulbs" ( "Id" ) ON DELETE RESTRICT,
    ADD CONSTRAINT "FK_Components_Bulbs_MetricsStatusId" FOREIGN KEY ( "MetricsStatusId" ) REFERENCES dbo."Bulbs" ( "Id" ) ON DELETE RESTRICT,
    ADD CONSTRAINT "FK_Components_Bulbs_InternalStatusId" FOREIGN KEY ( "InternalStatusId" ) REFERENCES dbo."Bulbs" ( "Id" ) ON DELETE RESTRICT,
    ADD CONSTRAINT "FK_Components_Bulbs_ExternalStatusId" FOREIGN KEY ( "ExternalStatusId" ) REFERENCES dbo."Bulbs" ( "Id" ) ON DELETE RESTRICT,
    ADD CONSTRAINT "FK_Components_Bulbs_EventsStatusId" FOREIGN KEY ( "EventsStatusId" ) REFERENCES dbo."Bulbs" ( "Id" ) ON DELETE RESTRICT,
    ADD CONSTRAINT "FK_Components_Bulbs_ChildComponentsStatusId" FOREIGN KEY ( "ChildComponentsStatusId" ) REFERENCES dbo."Bulbs" ( "Id" ) ON DELETE RESTRICT;

ALTER TABLE dbo."UserSettings"
    ADD CONSTRAINT "FK_UserSettings_Users_UserId" FOREIGN KEY ( "UserId" ) REFERENCES dbo."Users" ( "Id" ) ON DELETE RESTRICT;

ALTER TABLE dbo."UserRoles"
    ADD CONSTRAINT "FK_UserRoles_Roles_RoleId" FOREIGN KEY ( "RoleId" ) REFERENCES dbo."Roles" ( "Id" ) ON DELETE RESTRICT,
    ADD CONSTRAINT "FK_UserRoles_Users_UserId" FOREIGN KEY ( "UserId" ) REFERENCES dbo."Users" ( "Id" ) ON DELETE RESTRICT;

ALTER TABLE dbo."UserContacts"
    ADD CONSTRAINT "FK_UserContacts_Users_UserId" FOREIGN KEY ( "UserId" ) REFERENCES dbo."Users" ( "Id" ) ON DELETE RESTRICT;

ALTER TABLE dbo."Tokens"
    ADD CONSTRAINT "FK_Tokens_Users_UserId" FOREIGN KEY ( "UserId" ) REFERENCES dbo."Users" ( "Id" ) ON DELETE RESTRICT;

CREATE INDEX "IX_UserSettings_UserId"
    ON dbo."UserSettings"
    ( "UserId" );

CREATE INDEX "IX_Users_Login"
    ON dbo."Users"
    ( "Login" );

CREATE INDEX "IX_UserRoles_UserId"
    ON dbo."UserRoles"
    ( "UserId" );

CREATE INDEX "IX_UserRoles_RoleId"
    ON dbo."UserRoles"
    ( "RoleId" );

CREATE INDEX "IX_UserContacts_UserId"
    ON dbo."UserContacts"
    ( "UserId" );

CREATE INDEX "IX_UnitTestTypes_SystemName"
    ON dbo."UnitTestTypes"
    ( "SystemName" );

CREATE INDEX "IX_UnitTests_TypeId"
    ON dbo."UnitTests"
    ( "TypeId" );

CREATE INDEX "IX_UnitTests_StatusDataId"
    ON dbo."UnitTests"
    ( "StatusDataId" );

CREATE INDEX "IX_UnitTests_ComponentId"
    ON dbo."UnitTests"
    ( "ComponentId" );

CREATE INDEX "IX_UnitTestProperties_UnitTestId"
    ON dbo."UnitTestProperties"
    ( "UnitTestId" );

CREATE INDEX "IX_Tokens_UserId"
    ON dbo."Tokens"
    ( "UserId" );

CREATE INDEX "IX_Subscriptions_UserId"
    ON dbo."Subscriptions"
    ( "UserId" );

CREATE INDEX "IX_Subscriptions_ComponentTypeId"
    ON dbo."Subscriptions"
    ( "ComponentTypeId" );

CREATE INDEX "IX_Subscriptions_ComponentId"
    ON dbo."Subscriptions"
    ( "ComponentId" );

CREATE INDEX "IX_ForSend2"
    ON dbo."SendSmsCommand"
    ( "Status" );

CREATE INDEX "IX_ForSend1"
    ON dbo."SendMessageCommand"
    ( "Channel" , "Status" );

CREATE INDEX "IX_ForSend"
    ON dbo."SendEmailCommand"
    ( "Status" );

CREATE INDEX "IX_Notifications_UserId"
    ON dbo."Notifications"
    ( "UserId" );

CREATE INDEX "IX_Notifications_SubscriptionId"
    ON dbo."Notifications"
    ( "SubscriptionId" );

CREATE INDEX "IX_Notifications_Status"
    ON dbo."Notifications"
    ( "Status" );

CREATE INDEX "IX_Notifications_SendMessageCommandId"
    ON dbo."Notifications"
    ( "SendMessageCommandId" );

CREATE INDEX "IX_Notifications_SendEmailCommandId"
    ON dbo."Notifications"
    ( "SendEmailCommandId" );

CREATE INDEX "IX_Notifications_EventId"
    ON dbo."Notifications"
    ( "EventId" );

CREATE INDEX "IX_Metrics_StatusDataId"
    ON dbo."Metrics"
    ( "StatusDataId" );

CREATE INDEX "IX_Metrics_MetricTypeId"
    ON dbo."Metrics"
    ( "MetricTypeId" );

CREATE INDEX "IX_Metrics_ComponentId"
    ON dbo."Metrics"
    ( "ComponentId" );

CREATE INDEX "IX_MetricHistory_StatusEventId"
    ON dbo."MetricHistory"
    ( "StatusEventId" );

CREATE INDEX "IX_MetricHistory_MetricTypeId"
    ON dbo."MetricHistory"
    ( "MetricTypeId" );

CREATE INDEX "IX_MetricHistory_BeginDate"
    ON dbo."MetricHistory"
    ( "BeginDate" );

CREATE INDEX "IX_ForHistory"
    ON dbo."MetricHistory"
    ( "ComponentId" , "MetricTypeId" , "BeginDate" );

CREATE INDEX "IX_Date"
    ON dbo."Logs"
    ( "Date" );

CREATE INDEX "IX_ComponentBased"
    ON dbo."Logs"
    ( "ComponentId" , "Date" , "Order" , "Level" , "Context" );

CREATE INDEX "IX_LogParameters_LogId"
    ON dbo."LogParameters"
    ( "LogId" );

CREATE INDEX "IX_LimitDatasForUnitTests_UnitTestId"
    ON dbo."LimitDatasForUnitTests"
    ( "UnitTestId" );

CREATE INDEX "IX_LimitDatasForUnitTests_LimitDataId"
    ON dbo."LimitDatasForUnitTests"
    ( "LimitDataId" );

CREATE INDEX "IX_AccountId"
    ON dbo."LimitDatas"
    ( "Type" , "BeginDate" );

CREATE INDEX "IX_LastComponentNotifications_NotificationId"
    ON dbo."LastComponentNotifications"
    ( "NotificationId" );

CREATE INDEX "IX_LastComponentNotifications_EventId"
    ON dbo."LastComponentNotifications"
    ( "EventId" );

CREATE INDEX "IX_LastComponentNotifications_ComponentId"
    ON dbo."LastComponentNotifications"
    ( "ComponentId" );

CREATE INDEX "IX_HttpRequestUnitTestRules_HttpRequestUnitTestId"
    ON dbo."HttpRequestUnitTestRules"
    ( "HttpRequestUnitTestId" );

CREATE INDEX "IX_HttpRequestUnitTestRuleDatas_RuleId"
    ON dbo."HttpRequestUnitTestRuleDatas"
    ( "RuleId" );

CREATE INDEX "IX_EventTypes_SystemName"
    ON dbo."EventTypes"
    ( "SystemName" );

CREATE INDEX "IX_EventTypes_DefectId"
    ON dbo."EventTypes"
    ( "DefectId" );

CREATE INDEX "IX_EventStatuses_StatusId"
    ON dbo."EventStatuses"
    ( "StatusId" );

CREATE INDEX "IX_EventStatuses_EventId"
    ON dbo."EventStatuses"
    ( "EventId" );

CREATE INDEX "IX_OwnerBased"
    ON dbo."Events"
    ( "OwnerId" , "Category" , "ActualDate" , "StartDate" );

CREATE INDEX "IX_ForProcessing"
    ON dbo."Events"
    ( "IsUserHandled" , "EventTypeId" , "VersionLong" );

CREATE INDEX "IX_ForJoin"
    ON dbo."Events"
    ( "OwnerId" , "EventTypeId" , "Importance" , "ActualDate" );

CREATE INDEX "IX_Events_LastStatusEventId"
    ON dbo."Events"
    ( "LastStatusEventId" );

CREATE INDEX "IX_Events_EventTypeId"
    ON dbo."Events"
    ( "EventTypeId" );

CREATE INDEX "IX_AccountBased"
    ON dbo."Events"
    ( "Category" , "ActualDate" , "StartDate" , "Id" );

CREATE INDEX "IX_EventParameters_EventId"
    ON dbo."EventParameters"
    ( "EventId" );

CREATE INDEX "IX_Defects_ResponsibleUserId"
    ON dbo."Defects"
    ( "ResponsibleUserId" );

CREATE INDEX "IX_Defects_LastChangeId"
    ON dbo."Defects"
    ( "LastChangeId" );

CREATE INDEX "IX_Defects_EventTypeId"
    ON dbo."Defects"
    ( "EventTypeId" );

CREATE INDEX "IX_DefectChanges_UserId"
    ON dbo."DefectChanges"
    ( "UserId" );

CREATE INDEX "IX_DefectChanges_DefectId"
    ON dbo."DefectChanges"
    ( "DefectId" );

CREATE INDEX "IX_ComponentTypes_SystemName"
    ON dbo."ComponentTypes"
    ( "SystemName" );

CREATE INDEX "IX_Components_UnitTestsStatusId"
    ON dbo."Components"
    ( "UnitTestsStatusId" );

CREATE INDEX "IX_Components_SystemName"
    ON dbo."Components"
    ( "SystemName" );

CREATE INDEX "IX_Components_ParentId"
    ON dbo."Components"
    ( "ParentId" );

CREATE INDEX "IX_Components_MetricsStatusId"
    ON dbo."Components"
    ( "MetricsStatusId" );

CREATE INDEX "IX_Components_InternalStatusId"
    ON dbo."Components"
    ( "InternalStatusId" );

CREATE INDEX "IX_Components_ExternalStatusId"
    ON dbo."Components"
    ( "ExternalStatusId" );

CREATE INDEX "IX_Components_EventsStatusId"
    ON dbo."Components"
    ( "EventsStatusId" );

CREATE INDEX "IX_Components_ComponentTypeId"
    ON dbo."Components"
    ( "ComponentTypeId" );

CREATE INDEX "IX_Components_ChildComponentsStatusId"
    ON dbo."Components"
    ( "ChildComponentsStatusId" );

CREATE INDEX "IX_ComponentProperties_ComponentId"
    ON dbo."ComponentProperties"
    ( "ComponentId" );

CREATE INDEX "IX_Bulbs_UnitTestId"
    ON dbo."Bulbs"
    ( "UnitTestId" );

CREATE INDEX "IX_Bulbs_MetricId"
    ON dbo."Bulbs"
    ( "MetricId" );

CREATE INDEX "IX_Bulbs_ComponentId"
    ON dbo."Bulbs"
    ( "ComponentId" );

CREATE INDEX "IX_ArchivedStatuses_EventId"
    ON dbo."ArchivedStatuses"
    ( "EventId" );

INSERT INTO public."__EFMigrationsHistory"
("MigrationId", "ProductVersion") VALUES
(E'20210529185033_Initial', E'5.0.4');
  