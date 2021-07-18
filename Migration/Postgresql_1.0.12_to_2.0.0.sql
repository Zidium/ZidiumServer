-- This script converts Zidium Postgresql database from version 1.0.12 to 2.0.0
-- 
-- Script was generated by Devart dbForge Schema Compare for PostgreSQL
-- Product Home Page: http://www.devart.com/dbforge/postgresql/schemacompare
-- 

--
-- Drop foreign key
--
ALTER TABLE dbo."Components" 
   DROP CONSTRAINT "FK_dbo.Components_dbo.Bulbs_ChildComponentsStatusId";

--
-- Drop foreign key
--
ALTER TABLE dbo."Components" 
   DROP CONSTRAINT "FK_dbo.Components_dbo.Bulbs_EventsStatusId";

--
-- Drop foreign key
--
ALTER TABLE dbo."Components" 
   DROP CONSTRAINT "FK_dbo.Components_dbo.Bulbs_ExternalStatusId";

--
-- Drop foreign key
--
ALTER TABLE dbo."Components" 
   DROP CONSTRAINT "FK_dbo.Components_dbo.Bulbs_InternalStatusId";

--
-- Drop foreign key
--
ALTER TABLE dbo."Components" 
   DROP CONSTRAINT "FK_dbo.Components_dbo.Bulbs_MetricsStatusId";

--
-- Drop foreign key
--
ALTER TABLE dbo."Components" 
   DROP CONSTRAINT "FK_dbo.Components_dbo.Bulbs_UnitTestsStatusId";

--
-- Drop foreign key
--
ALTER TABLE dbo."Metrics" 
   DROP CONSTRAINT "FK_dbo.Metrics_dbo.Bulbs_StatusDataId";

--
-- Drop foreign key
--
ALTER TABLE dbo."UnitTests" 
   DROP CONSTRAINT "FK_dbo.UnitTests_dbo.Bulbs_StatusDataId";

--
-- Drop foreign key
--
ALTER TABLE dbo."LogParameters" 
   DROP CONSTRAINT "FK_dbo.LogParameters_dbo.Logs_LogId";

--
-- Drop foreign key
--
ALTER TABLE dbo."Bulbs" 
   DROP CONSTRAINT "FK_dbo.Bulbs_dbo.Metrics_MetricId";

--
-- Drop foreign key
--
ALTER TABLE dbo."LastComponentNotifications" 
   DROP CONSTRAINT "FK_dbo.LastComponentNotifications_dbo.Notifications_Notificatio";

--
-- Drop foreign key
--
ALTER TABLE dbo."NotificationsHttp" 
   DROP CONSTRAINT "FK_dbo.NotificationsHttp_dbo.Notifications_NotificationId";

--
-- Drop foreign key
--
ALTER TABLE dbo."Notifications" 
   DROP CONSTRAINT "FK_dbo.Notifications_dbo.Subscriptions_SubscriptionId";

--
-- Drop foreign key
--
ALTER TABLE dbo."HttpRequestUnitTestRuleDatas" 
   DROP CONSTRAINT "FK_dbo.HttpRequestUnitTestRuleDatas_dbo.HttpRequestUnitTestRule";

--
-- Drop foreign key
--
ALTER TABLE dbo."HttpRequestUnitTestRules" 
   DROP CONSTRAINT "FK_dbo.HttpRequestUnitTestRules_dbo.HttpRequestUnitTests_HttpRe";

--
-- Drop foreign key
--
ALTER TABLE dbo."Bulbs" 
   DROP CONSTRAINT "FK_dbo.Bulbs_dbo.UnitTests_UnitTestId";

--
-- Drop foreign key
--
ALTER TABLE dbo."HttpRequestUnitTests" 
   DROP CONSTRAINT "FK_dbo.HttpRequestUnitTests_dbo.UnitTests_UnitTestId";

--
-- Drop foreign key
--
ALTER TABLE dbo."LimitDatasForUnitTests" 
   DROP CONSTRAINT "FK_dbo.LimitDatasForUnitTests_dbo.UnitTests_UnitTestId";

--
-- Drop foreign key
--
ALTER TABLE dbo."UnitTestDomainNamePaymentPeriodRules" 
   DROP CONSTRAINT "FK_dbo.UnitTestDomainNamePaymentPeriodRules_dbo.UnitTests_UnitT";

--
-- Drop foreign key
--
ALTER TABLE dbo."UnitTestPingRules" 
   DROP CONSTRAINT "FK_dbo.UnitTestPingRules_dbo.UnitTests_UnitTestId";

--
-- Drop foreign key
--
ALTER TABLE dbo."UnitTestProperties" 
   DROP CONSTRAINT "FK_dbo.UnitTestProperties_dbo.UnitTests_UnitTestId";

--
-- Drop foreign key
--
ALTER TABLE dbo."UnitTestSqlRules" 
   DROP CONSTRAINT "FK_dbo.UnitTestSqlRules_dbo.UnitTests_UnitTestId";

--
-- Drop foreign key
--
ALTER TABLE dbo."UnitTestSslCertificateExpirationDateRules" 
   DROP CONSTRAINT "FK_dbo.UnitTestSslCertificateExpirationDateRules_dbo.UnitTests_";

--
-- Drop foreign key
--
ALTER TABLE dbo."UnitTestTcpPortRules" 
   DROP CONSTRAINT "FK_dbo.UnitTestTcpPortRules_dbo.UnitTests_UnitTestId";

--
-- Drop foreign key
--
ALTER TABLE dbo."UnitTestVirusTotalRules" 
   DROP CONSTRAINT "FK_dbo.UnitTestVirusTotalRules_dbo.UnitTests_UnitTestId";

--
-- Drop foreign key
--
ALTER TABLE dbo."Bulbs" 
   DROP CONSTRAINT "FK_dbo.Bulbs_dbo.Components_ComponentId";

--
-- Drop foreign key
--
ALTER TABLE dbo."ComponentProperties" 
   DROP CONSTRAINT "FK_dbo.ComponentProperties_dbo.Components_ComponentId";

--
-- Drop foreign key
--
ALTER TABLE dbo."LastComponentNotifications" 
   DROP CONSTRAINT "FK_dbo.LastComponentNotifications_dbo.Components_ComponentId";

--
-- Drop foreign key
--
ALTER TABLE dbo."LogConfigs" 
   DROP CONSTRAINT "FK_dbo.LogConfigs_dbo.Components_ComponentId";

--
-- Drop foreign key
--
ALTER TABLE dbo."Logs" 
   DROP CONSTRAINT "FK_dbo.Logs_dbo.Components_ComponentId";

--
-- Drop foreign key
--
ALTER TABLE dbo."MetricHistory" 
   DROP CONSTRAINT "FK_dbo.MetricHistory_dbo.Components_ComponentId";

--
-- Drop foreign key
--
ALTER TABLE dbo."Metrics" 
   DROP CONSTRAINT "FK_dbo.Metrics_dbo.Components_ComponentId";

--
-- Drop foreign key
--
ALTER TABLE dbo."Subscriptions" 
   DROP CONSTRAINT "FK_dbo.Subscriptions_dbo.Components_ComponentId";

--
-- Drop foreign key
--
ALTER TABLE dbo."UnitTests" 
   DROP CONSTRAINT "FK_dbo.UnitTests_dbo.Components_ComponentId";

--
-- Drop foreign key
--
ALTER TABLE dbo."ArchivedStatuses" 
   DROP CONSTRAINT "FK_dbo.ArchivedStatuses_dbo.Events_EventId";

--
-- Drop foreign key
--
ALTER TABLE dbo."EventParameters" 
   DROP CONSTRAINT "FK_dbo.EventParameters_dbo.Events_EventId";

--
-- Drop foreign key
--
ALTER TABLE dbo."EventStatuses" 
   DROP CONSTRAINT "FK_dbo.EventStatuses_dbo.Events_EventId";

--
-- Drop foreign key
--
ALTER TABLE dbo."EventStatuses" 
   DROP CONSTRAINT "FK_dbo.EventStatuses_dbo.Events_StatusId";

--
-- Drop foreign key
--
ALTER TABLE dbo."LastComponentNotifications" 
   DROP CONSTRAINT "FK_dbo.LastComponentNotifications_dbo.Events_EventId";

--
-- Drop foreign key
--
ALTER TABLE dbo."MetricHistory" 
   DROP CONSTRAINT "FK_dbo.MetricHistory_dbo.Events_StatusEventId";

--
-- Drop foreign key
--
ALTER TABLE dbo."Notifications" 
   DROP CONSTRAINT "FK_dbo.Notifications_dbo.Events_EventId";

--
-- Drop foreign key
--
ALTER TABLE dbo."Defects" 
   DROP CONSTRAINT "FK_dbo.Defects_dbo.EventTypes_EventTypeId";

--
-- Drop foreign key
--
ALTER TABLE dbo."Events" 
   DROP CONSTRAINT "FK_dbo.Events_dbo.EventTypes_EventTypeId";

--
-- Drop foreign key
--
ALTER TABLE dbo."Defects" 
   DROP CONSTRAINT "FK_dbo.Defects_dbo.DefectChanges_LastChangeId";

--
-- Drop foreign key
--
ALTER TABLE dbo."DefectChanges" 
   DROP CONSTRAINT "FK_dbo.DefectChanges_dbo.Defects_DefectId";

--
-- Drop foreign key
--
ALTER TABLE dbo."EventTypes" 
   DROP CONSTRAINT "FK_dbo.EventTypes_dbo.Defects_DefectId";

--
-- Drop table "dbo"."TariffLimits"
--
DROP TABLE dbo."TariffLimits";

--
-- Drop table "dbo"."__MigrationHistory"
--
DROP TABLE dbo."__MigrationHistory";

--
-- Drop foreign key
--
ALTER TABLE dbo."UserSettings" 
   DROP CONSTRAINT "FK_dbo.UserSettings_dbo.Users_UserId";

--
-- Create foreign key
--
ALTER TABLE dbo."UserSettings" 
  ADD CONSTRAINT "FK_UserSettings_Users_UserId" FOREIGN KEY ("UserId")
    REFERENCES dbo."Users"("Id") ON DELETE NO ACTION ON UPDATE NO ACTION INITIALLY IMMEDIATE;

--
-- Drop foreign key
--
ALTER TABLE dbo."UserContacts" 
   DROP CONSTRAINT "FK_dbo.UserContacts_dbo.Users_UserId";

--
-- Create foreign key
--
ALTER TABLE dbo."UserContacts" 
  ADD CONSTRAINT "FK_UserContacts_Users_UserId" FOREIGN KEY ("UserId")
    REFERENCES dbo."Users"("Id") ON DELETE NO ACTION ON UPDATE NO ACTION INITIALLY IMMEDIATE;

--
-- Drop foreign key
--
ALTER TABLE dbo."Tokens" 
   DROP CONSTRAINT "FK_dbo.Tokens_dbo.Users_UserId";

--
-- Create foreign key
--
ALTER TABLE dbo."Tokens" 
  ADD CONSTRAINT "FK_Tokens_Users_UserId" FOREIGN KEY ("UserId")
    REFERENCES dbo."Users"("Id") ON DELETE NO ACTION ON UPDATE NO ACTION INITIALLY IMMEDIATE;

--
-- Drop foreign key
--
ALTER TABLE dbo."Defects" 
   DROP CONSTRAINT "FK_dbo.Defects_dbo.Users_ResponsibleUserId";

--
-- Create foreign key
--
ALTER TABLE dbo."Defects" 
  ADD CONSTRAINT "FK_Defects_Users_ResponsibleUserId" FOREIGN KEY ("ResponsibleUserId")
    REFERENCES dbo."Users"("Id") ON DELETE NO ACTION ON UPDATE NO ACTION INITIALLY IMMEDIATE;

--
-- Drop foreign key
--
ALTER TABLE dbo."DefectChanges" 
   DROP CONSTRAINT "FK_dbo.DefectChanges_dbo.Users_UserId";

--
-- Create foreign key
--
ALTER TABLE dbo."DefectChanges" 
  ADD CONSTRAINT "FK_DefectChanges_Defects_DefectId" FOREIGN KEY ("DefectId")
    REFERENCES dbo."Defects"("Id") ON DELETE NO ACTION ON UPDATE NO ACTION INITIALLY IMMEDIATE;

--
-- Create foreign key
--
ALTER TABLE dbo."DefectChanges" 
  ADD CONSTRAINT "FK_DefectChanges_Users_UserId" FOREIGN KEY ("UserId")
    REFERENCES dbo."Users"("Id") ON DELETE NO ACTION ON UPDATE NO ACTION INITIALLY IMMEDIATE;

--
-- Create foreign key
--
ALTER TABLE dbo."Defects" 
  ADD CONSTRAINT "FK_Defects_DefectChanges_LastChangeId" FOREIGN KEY ("LastChangeId")
    REFERENCES dbo."DefectChanges"("Id") ON DELETE NO ACTION ON UPDATE NO ACTION INITIALLY IMMEDIATE;

--
-- Drop foreign key
--
ALTER TABLE dbo."UserRoles" 
   DROP CONSTRAINT "FK_dbo.UserRoles_dbo.Roles_RoleId";

--
-- Drop foreign key
--
ALTER TABLE dbo."UserRoles" 
   DROP CONSTRAINT "FK_dbo.UserRoles_dbo.Users_UserId";

--
-- Create foreign key
--
ALTER TABLE dbo."UserRoles" 
  ADD CONSTRAINT "FK_UserRoles_Roles_RoleId" FOREIGN KEY ("RoleId")
    REFERENCES dbo."Roles"("Id") ON DELETE NO ACTION ON UPDATE NO ACTION INITIALLY IMMEDIATE;

--
-- Create foreign key
--
ALTER TABLE dbo."UserRoles" 
  ADD CONSTRAINT "FK_UserRoles_Users_UserId" FOREIGN KEY ("UserId")
    REFERENCES dbo."Users"("Id") ON DELETE NO ACTION ON UPDATE NO ACTION INITIALLY IMMEDIATE;

--
-- Create foreign key
--
ALTER TABLE dbo."EventTypes" 
  ADD CONSTRAINT "FK_EventTypes_Defects_DefectId" FOREIGN KEY ("DefectId")
    REFERENCES dbo."Defects"("Id") ON DELETE NO ACTION ON UPDATE NO ACTION INITIALLY IMMEDIATE;

--
-- Create foreign key
--
ALTER TABLE dbo."Defects" 
  ADD CONSTRAINT "FK_Defects_EventTypes_EventTypeId" FOREIGN KEY ("EventTypeId")
    REFERENCES dbo."EventTypes"("Id") ON DELETE NO ACTION ON UPDATE NO ACTION INITIALLY IMMEDIATE;

--
-- Drop foreign key
--
ALTER TABLE dbo."Events" 
   DROP CONSTRAINT "FK_dbo.Events_dbo.Events_LastStatusEventId";

--
-- Create foreign key
--
ALTER TABLE dbo."Events" 
  ADD CONSTRAINT "FK_Events_EventTypes_EventTypeId" FOREIGN KEY ("EventTypeId")
    REFERENCES dbo."EventTypes"("Id") ON DELETE NO ACTION ON UPDATE NO ACTION INITIALLY IMMEDIATE;

--
-- Create foreign key
--
ALTER TABLE dbo."Events" 
  ADD CONSTRAINT "FK_Events_Events_LastStatusEventId" FOREIGN KEY ("LastStatusEventId")
    REFERENCES dbo."Events"("Id") ON DELETE NO ACTION ON UPDATE NO ACTION INITIALLY IMMEDIATE;

--
-- Create foreign key
--
ALTER TABLE dbo."EventStatuses" 
  ADD CONSTRAINT "FK_EventStatuses_Events_EventId" FOREIGN KEY ("EventId")
    REFERENCES dbo."Events"("Id") ON DELETE NO ACTION ON UPDATE NO ACTION INITIALLY IMMEDIATE;

--
-- Create foreign key
--
ALTER TABLE dbo."EventStatuses" 
  ADD CONSTRAINT "FK_EventStatuses_Events_StatusId" FOREIGN KEY ("StatusId")
    REFERENCES dbo."Events"("Id") ON DELETE NO ACTION ON UPDATE NO ACTION INITIALLY IMMEDIATE;

--
-- Create foreign key
--
ALTER TABLE dbo."EventParameters" 
  ADD CONSTRAINT "FK_EventParameters_Events_EventId" FOREIGN KEY ("EventId")
    REFERENCES dbo."Events"("Id") ON DELETE NO ACTION ON UPDATE NO ACTION INITIALLY IMMEDIATE;

--
-- Drop primary key
--
ALTER TABLE dbo."ArchivedStatuses" 
  DROP CONSTRAINT "PK_dbo.ArchivedStatuses";

--
-- Alter column "Id" on table "dbo"."ArchivedStatuses"
--
ALTER TABLE dbo."ArchivedStatuses" 
  ALTER COLUMN "Id" TYPE bigint;

--
-- Create primary key
--
ALTER TABLE dbo."ArchivedStatuses" 
  ADD PRIMARY KEY ("Id");

--
-- Create foreign key
--
ALTER TABLE dbo."ArchivedStatuses" 
  ADD CONSTRAINT "FK_ArchivedStatuses_Events_EventId" FOREIGN KEY ("EventId")
    REFERENCES dbo."Events"("Id") ON DELETE NO ACTION ON UPDATE NO ACTION INITIALLY IMMEDIATE;

--
-- Drop foreign key
--
ALTER TABLE dbo."Components" 
   DROP CONSTRAINT "FK_dbo.Components_dbo.ComponentTypes_ComponentTypeId";

--
-- Drop foreign key
--
ALTER TABLE dbo."Components" 
   DROP CONSTRAINT "FK_dbo.Components_dbo.Components_ParentId";

--
-- Create foreign key
--
ALTER TABLE dbo."Components" 
  ADD CONSTRAINT "FK_Components_ComponentTypes_ComponentTypeId" FOREIGN KEY ("ComponentTypeId")
    REFERENCES dbo."ComponentTypes"("Id") ON DELETE NO ACTION ON UPDATE NO ACTION INITIALLY IMMEDIATE;

--
-- Create foreign key
--
ALTER TABLE dbo."Components" 
  ADD CONSTRAINT "FK_Components_Components_ParentId" FOREIGN KEY ("ParentId")
    REFERENCES dbo."Components"("Id") ON DELETE NO ACTION ON UPDATE NO ACTION INITIALLY IMMEDIATE;

--
-- Drop foreign key
--
ALTER TABLE dbo."UnitTests" 
   DROP CONSTRAINT "FK_dbo.UnitTests_dbo.UnitTestTypes_TypeId";

--
-- Create foreign key
--
ALTER TABLE dbo."UnitTests" 
  ADD CONSTRAINT "FK_UnitTests_Components_ComponentId" FOREIGN KEY ("ComponentId")
    REFERENCES dbo."Components"("Id") ON DELETE NO ACTION ON UPDATE NO ACTION INITIALLY IMMEDIATE;

--
-- Create foreign key
--
ALTER TABLE dbo."UnitTests" 
  ADD CONSTRAINT "FK_UnitTests_UnitTestTypes_TypeId" FOREIGN KEY ("TypeId")
    REFERENCES dbo."UnitTestTypes"("Id") ON DELETE NO ACTION ON UPDATE NO ACTION INITIALLY IMMEDIATE;

--
-- Create foreign key
--
ALTER TABLE dbo."UnitTestVirusTotalRules" 
  ADD CONSTRAINT "FK_UnitTestVirusTotalRules_UnitTests_UnitTestId" FOREIGN KEY ("UnitTestId")
    REFERENCES dbo."UnitTests"("Id") ON DELETE NO ACTION ON UPDATE NO ACTION INITIALLY IMMEDIATE;

--
-- Create foreign key
--
ALTER TABLE dbo."UnitTestTcpPortRules" 
  ADD CONSTRAINT "FK_UnitTestTcpPortRules_UnitTests_UnitTestId" FOREIGN KEY ("UnitTestId")
    REFERENCES dbo."UnitTests"("Id") ON DELETE NO ACTION ON UPDATE NO ACTION INITIALLY IMMEDIATE;

--
-- Create foreign key
--
ALTER TABLE dbo."UnitTestSslCertificateExpirationDateRules" 
  ADD CONSTRAINT "FK_UnitTestSslCertificateExpirationDateRules_UnitTests_UnitTes~" FOREIGN KEY ("UnitTestId")
    REFERENCES dbo."UnitTests"("Id") ON DELETE NO ACTION ON UPDATE NO ACTION INITIALLY IMMEDIATE;

--
-- Create foreign key
--
ALTER TABLE dbo."UnitTestSqlRules" 
  ADD CONSTRAINT "FK_UnitTestSqlRules_UnitTests_UnitTestId" FOREIGN KEY ("UnitTestId")
    REFERENCES dbo."UnitTests"("Id") ON DELETE NO ACTION ON UPDATE NO ACTION INITIALLY IMMEDIATE;

--
-- Create foreign key
--
ALTER TABLE dbo."UnitTestProperties" 
  ADD CONSTRAINT "FK_UnitTestProperties_UnitTests_UnitTestId" FOREIGN KEY ("UnitTestId")
    REFERENCES dbo."UnitTests"("Id") ON DELETE NO ACTION ON UPDATE NO ACTION INITIALLY IMMEDIATE;

--
-- Create foreign key
--
ALTER TABLE dbo."UnitTestPingRules" 
  ADD CONSTRAINT "FK_UnitTestPingRules_UnitTests_UnitTestId" FOREIGN KEY ("UnitTestId")
    REFERENCES dbo."UnitTests"("Id") ON DELETE NO ACTION ON UPDATE NO ACTION INITIALLY IMMEDIATE;

--
-- Create foreign key
--
ALTER TABLE dbo."UnitTestDomainNamePaymentPeriodRules" 
  ADD CONSTRAINT "FK_UnitTestDomainNamePaymentPeriodRules_UnitTests_UnitTestId" FOREIGN KEY ("UnitTestId")
    REFERENCES dbo."UnitTests"("Id") ON DELETE NO ACTION ON UPDATE NO ACTION INITIALLY IMMEDIATE;

--
-- Drop foreign key
--
ALTER TABLE dbo."LimitDatasForUnitTests" 
   DROP CONSTRAINT "FK_dbo.LimitDatasForUnitTests_dbo.LimitDatas_LimitDataId";

--
-- Create foreign key
--
ALTER TABLE dbo."LimitDatasForUnitTests" 
  ADD CONSTRAINT "FK_LimitDatasForUnitTests_LimitDatas_LimitDataId" FOREIGN KEY ("LimitDataId")
    REFERENCES dbo."LimitDatas"("Id") ON DELETE NO ACTION ON UPDATE NO ACTION INITIALLY IMMEDIATE;

--
-- Create foreign key
--
ALTER TABLE dbo."LimitDatasForUnitTests" 
  ADD CONSTRAINT "FK_LimitDatasForUnitTests_UnitTests_UnitTestId" FOREIGN KEY ("UnitTestId")
    REFERENCES dbo."UnitTests"("Id") ON DELETE NO ACTION ON UPDATE NO ACTION INITIALLY IMMEDIATE;

--
-- Drop column "HasBanner" from table "dbo"."HttpRequestUnitTests"
--
ALTER TABLE dbo."HttpRequestUnitTests" 
  DROP COLUMN "HasBanner";

--
-- Drop column "LastBannerCheck" from table "dbo"."HttpRequestUnitTests"
--
ALTER TABLE dbo."HttpRequestUnitTests" 
  DROP COLUMN "LastBannerCheck";

--
-- Create foreign key
--
ALTER TABLE dbo."HttpRequestUnitTests" 
  ADD CONSTRAINT "FK_HttpRequestUnitTests_UnitTests_UnitTestId" FOREIGN KEY ("UnitTestId")
    REFERENCES dbo."UnitTests"("Id") ON DELETE NO ACTION ON UPDATE NO ACTION INITIALLY IMMEDIATE;

--
-- Create foreign key
--
ALTER TABLE dbo."HttpRequestUnitTestRules" 
  ADD CONSTRAINT "FK_HttpRequestUnitTestRules_HttpRequestUnitTests_HttpRequestUn~" FOREIGN KEY ("HttpRequestUnitTestId")
    REFERENCES dbo."HttpRequestUnitTests"("UnitTestId") ON DELETE NO ACTION ON UPDATE NO ACTION INITIALLY IMMEDIATE;

--
-- Create foreign key
--
ALTER TABLE dbo."HttpRequestUnitTestRuleDatas" 
  ADD CONSTRAINT "FK_HttpRequestUnitTestRuleDatas_HttpRequestUnitTestRules_RuleId" FOREIGN KEY ("RuleId")
    REFERENCES dbo."HttpRequestUnitTestRules"("Id") ON DELETE NO ACTION ON UPDATE NO ACTION INITIALLY IMMEDIATE;

--
-- Drop foreign key
--
ALTER TABLE dbo."Subscriptions" 
   DROP CONSTRAINT "FK_dbo.Subscriptions_dbo.ComponentTypes_ComponentTypeId";

--
-- Drop foreign key
--
ALTER TABLE dbo."Subscriptions" 
   DROP CONSTRAINT "FK_dbo.Subscriptions_dbo.Users_UserId";

--
-- Create foreign key
--
ALTER TABLE dbo."Subscriptions" 
  ADD CONSTRAINT "FK_Subscriptions_ComponentTypes_ComponentTypeId" FOREIGN KEY ("ComponentTypeId")
    REFERENCES dbo."ComponentTypes"("Id") ON DELETE NO ACTION ON UPDATE NO ACTION INITIALLY IMMEDIATE;

--
-- Create foreign key
--
ALTER TABLE dbo."Subscriptions" 
  ADD CONSTRAINT "FK_Subscriptions_Components_ComponentId" FOREIGN KEY ("ComponentId")
    REFERENCES dbo."Components"("Id") ON DELETE NO ACTION ON UPDATE NO ACTION INITIALLY IMMEDIATE;

--
-- Create foreign key
--
ALTER TABLE dbo."Subscriptions" 
  ADD CONSTRAINT "FK_Subscriptions_Users_UserId" FOREIGN KEY ("UserId")
    REFERENCES dbo."Users"("Id") ON DELETE NO ACTION ON UPDATE NO ACTION INITIALLY IMMEDIATE;

--
-- Drop foreign key
--
ALTER TABLE dbo."Notifications" 
   DROP CONSTRAINT "FK_dbo.Notifications_dbo.SendEmailCommand_SendEmailCommandId";

--
-- Drop foreign key
--
ALTER TABLE dbo."Notifications" 
   DROP CONSTRAINT "FK_dbo.Notifications_dbo.SendMessageCommand_SendMessageCommandI";

--
-- Drop foreign key
--
ALTER TABLE dbo."Notifications" 
   DROP CONSTRAINT "FK_dbo.Notifications_dbo.Users_UserId";

--
-- Create foreign key
--
ALTER TABLE dbo."Notifications" 
  ADD CONSTRAINT "FK_Notifications_Events_EventId" FOREIGN KEY ("EventId")
    REFERENCES dbo."Events"("Id") ON DELETE NO ACTION ON UPDATE NO ACTION INITIALLY IMMEDIATE;

--
-- Create foreign key
--
ALTER TABLE dbo."Notifications" 
  ADD CONSTRAINT "FK_Notifications_SendEmailCommand_SendEmailCommandId" FOREIGN KEY ("SendEmailCommandId")
    REFERENCES dbo."SendEmailCommand"("Id") ON DELETE NO ACTION ON UPDATE NO ACTION INITIALLY IMMEDIATE;

--
-- Create foreign key
--
ALTER TABLE dbo."Notifications" 
  ADD CONSTRAINT "FK_Notifications_SendMessageCommand_SendMessageCommandId" FOREIGN KEY ("SendMessageCommandId")
    REFERENCES dbo."SendMessageCommand"("Id") ON DELETE NO ACTION ON UPDATE NO ACTION INITIALLY IMMEDIATE;

--
-- Create foreign key
--
ALTER TABLE dbo."Notifications" 
  ADD CONSTRAINT "FK_Notifications_Subscriptions_SubscriptionId" FOREIGN KEY ("SubscriptionId")
    REFERENCES dbo."Subscriptions"("Id") ON DELETE NO ACTION ON UPDATE NO ACTION INITIALLY IMMEDIATE;

--
-- Create foreign key
--
ALTER TABLE dbo."Notifications" 
  ADD CONSTRAINT "FK_Notifications_Users_UserId" FOREIGN KEY ("UserId")
    REFERENCES dbo."Users"("Id") ON DELETE NO ACTION ON UPDATE NO ACTION INITIALLY IMMEDIATE;

--
-- Create foreign key
--
ALTER TABLE dbo."NotificationsHttp" 
  ADD CONSTRAINT "FK_NotificationsHttp_Notifications_NotificationId" FOREIGN KEY ("NotificationId")
    REFERENCES dbo."Notifications"("Id") ON DELETE NO ACTION ON UPDATE NO ACTION INITIALLY IMMEDIATE;

--
-- Create foreign key
--
ALTER TABLE dbo."LastComponentNotifications" 
  ADD CONSTRAINT "FK_LastComponentNotifications_Components_ComponentId" FOREIGN KEY ("ComponentId")
    REFERENCES dbo."Components"("Id") ON DELETE NO ACTION ON UPDATE NO ACTION INITIALLY IMMEDIATE;

--
-- Create foreign key
--
ALTER TABLE dbo."LastComponentNotifications" 
  ADD CONSTRAINT "FK_LastComponentNotifications_Events_EventId" FOREIGN KEY ("EventId")
    REFERENCES dbo."Events"("Id") ON DELETE NO ACTION ON UPDATE NO ACTION INITIALLY IMMEDIATE;

--
-- Create foreign key
--
ALTER TABLE dbo."LastComponentNotifications" 
  ADD CONSTRAINT "FK_LastComponentNotifications_Notifications_NotificationId" FOREIGN KEY ("NotificationId")
    REFERENCES dbo."Notifications"("Id") ON DELETE NO ACTION ON UPDATE NO ACTION INITIALLY IMMEDIATE;

--
-- Drop foreign key
--
ALTER TABLE dbo."Metrics" 
   DROP CONSTRAINT "FK_dbo.Metrics_dbo.MetricTypes_MetricTypeId";

--
-- Create foreign key
--
ALTER TABLE dbo."Metrics" 
  ADD CONSTRAINT "FK_Metrics_Components_ComponentId" FOREIGN KEY ("ComponentId")
    REFERENCES dbo."Components"("Id") ON DELETE NO ACTION ON UPDATE NO ACTION INITIALLY IMMEDIATE;

--
-- Create foreign key
--
ALTER TABLE dbo."Metrics" 
  ADD CONSTRAINT "FK_Metrics_MetricTypes_MetricTypeId" FOREIGN KEY ("MetricTypeId")
    REFERENCES dbo."MetricTypes"("Id") ON DELETE NO ACTION ON UPDATE NO ACTION INITIALLY IMMEDIATE;

--
-- Drop foreign key
--
ALTER TABLE dbo."MetricHistory" 
   DROP CONSTRAINT "FK_dbo.MetricHistory_dbo.MetricTypes_MetricTypeId";

--
-- Create foreign key
--
ALTER TABLE dbo."MetricHistory" 
  ADD CONSTRAINT "FK_MetricHistory_Components_ComponentId" FOREIGN KEY ("ComponentId")
    REFERENCES dbo."Components"("Id") ON DELETE NO ACTION ON UPDATE NO ACTION INITIALLY IMMEDIATE;

--
-- Create foreign key
--
ALTER TABLE dbo."MetricHistory" 
  ADD CONSTRAINT "FK_MetricHistory_Events_StatusEventId" FOREIGN KEY ("StatusEventId")
    REFERENCES dbo."Events"("Id") ON DELETE NO ACTION ON UPDATE NO ACTION INITIALLY IMMEDIATE;

--
-- Create foreign key
--
ALTER TABLE dbo."MetricHistory" 
  ADD CONSTRAINT "FK_MetricHistory_MetricTypes_MetricTypeId" FOREIGN KEY ("MetricTypeId")
    REFERENCES dbo."MetricTypes"("Id") ON DELETE NO ACTION ON UPDATE NO ACTION INITIALLY IMMEDIATE;

--
-- Create foreign key
--
ALTER TABLE dbo."Logs" 
  ADD CONSTRAINT "FK_Logs_Components_ComponentId" FOREIGN KEY ("ComponentId")
    REFERENCES dbo."Components"("Id") ON DELETE NO ACTION ON UPDATE NO ACTION INITIALLY IMMEDIATE;

--
-- Create foreign key
--
ALTER TABLE dbo."LogParameters" 
  ADD CONSTRAINT "FK_LogParameters_Logs_LogId" FOREIGN KEY ("LogId")
    REFERENCES dbo."Logs"("Id") ON DELETE NO ACTION ON UPDATE NO ACTION INITIALLY IMMEDIATE;

--
-- Create foreign key
--
ALTER TABLE dbo."LogConfigs" 
  ADD CONSTRAINT "FK_LogConfigs_Components_ComponentId" FOREIGN KEY ("ComponentId")
    REFERENCES dbo."Components"("Id") ON DELETE NO ACTION ON UPDATE NO ACTION INITIALLY IMMEDIATE;

--
-- Alter column "Value" on table "dbo"."ComponentProperties"
--
ALTER TABLE dbo."ComponentProperties" 
  ALTER COLUMN "Value" TYPE character varying(8000);

--
-- Create foreign key
--
ALTER TABLE dbo."ComponentProperties" 
  ADD CONSTRAINT "FK_ComponentProperties_Components_ComponentId" FOREIGN KEY ("ComponentId")
    REFERENCES dbo."Components"("Id") ON DELETE NO ACTION ON UPDATE NO ACTION INITIALLY IMMEDIATE;

--
-- Create foreign key
--
ALTER TABLE dbo."Bulbs" 
  ADD CONSTRAINT "FK_Bulbs_Components_ComponentId" FOREIGN KEY ("ComponentId")
    REFERENCES dbo."Components"("Id") ON DELETE NO ACTION ON UPDATE NO ACTION INITIALLY IMMEDIATE;

--
-- Create foreign key
--
ALTER TABLE dbo."Bulbs" 
  ADD CONSTRAINT "FK_Bulbs_Metrics_MetricId" FOREIGN KEY ("MetricId")
    REFERENCES dbo."Metrics"("Id") ON DELETE NO ACTION ON UPDATE NO ACTION INITIALLY IMMEDIATE;

--
-- Create foreign key
--
ALTER TABLE dbo."Bulbs" 
  ADD CONSTRAINT "FK_Bulbs_UnitTests_UnitTestId" FOREIGN KEY ("UnitTestId")
    REFERENCES dbo."UnitTests"("Id") ON DELETE NO ACTION ON UPDATE NO ACTION INITIALLY IMMEDIATE;

--
-- Create foreign key
--
ALTER TABLE dbo."Components" 
  ADD CONSTRAINT "FK_Components_Bulbs_ChildComponentsStatusId" FOREIGN KEY ("ChildComponentsStatusId")
    REFERENCES dbo."Bulbs"("Id") ON DELETE NO ACTION ON UPDATE NO ACTION INITIALLY IMMEDIATE;

--
-- Create foreign key
--
ALTER TABLE dbo."Components" 
  ADD CONSTRAINT "FK_Components_Bulbs_EventsStatusId" FOREIGN KEY ("EventsStatusId")
    REFERENCES dbo."Bulbs"("Id") ON DELETE NO ACTION ON UPDATE NO ACTION INITIALLY IMMEDIATE;

--
-- Create foreign key
--
ALTER TABLE dbo."Components" 
  ADD CONSTRAINT "FK_Components_Bulbs_ExternalStatusId" FOREIGN KEY ("ExternalStatusId")
    REFERENCES dbo."Bulbs"("Id") ON DELETE NO ACTION ON UPDATE NO ACTION INITIALLY IMMEDIATE;

--
-- Create foreign key
--
ALTER TABLE dbo."Components" 
  ADD CONSTRAINT "FK_Components_Bulbs_InternalStatusId" FOREIGN KEY ("InternalStatusId")
    REFERENCES dbo."Bulbs"("Id") ON DELETE NO ACTION ON UPDATE NO ACTION INITIALLY IMMEDIATE;

--
-- Create foreign key
--
ALTER TABLE dbo."Components" 
  ADD CONSTRAINT "FK_Components_Bulbs_MetricsStatusId" FOREIGN KEY ("MetricsStatusId")
    REFERENCES dbo."Bulbs"("Id") ON DELETE NO ACTION ON UPDATE NO ACTION INITIALLY IMMEDIATE;

--
-- Create foreign key
--
ALTER TABLE dbo."Components" 
  ADD CONSTRAINT "FK_Components_Bulbs_UnitTestsStatusId" FOREIGN KEY ("UnitTestsStatusId")
    REFERENCES dbo."Bulbs"("Id") ON DELETE NO ACTION ON UPDATE NO ACTION INITIALLY IMMEDIATE;

--
-- Create foreign key
--
ALTER TABLE dbo."Metrics" 
  ADD CONSTRAINT "FK_Metrics_Bulbs_StatusDataId" FOREIGN KEY ("StatusDataId")
    REFERENCES dbo."Bulbs"("Id") ON DELETE NO ACTION ON UPDATE NO ACTION INITIALLY IMMEDIATE;

--
-- Create foreign key
--
ALTER TABLE dbo."UnitTests" 
  ADD CONSTRAINT "FK_UnitTests_Bulbs_StatusDataId" FOREIGN KEY ("StatusDataId")
    REFERENCES dbo."Bulbs"("Id") ON DELETE NO ACTION ON UPDATE NO ACTION INITIALLY IMMEDIATE;

--
-- Create table "public"."__EFMigrationsHistory"
--
CREATE TABLE public."__EFMigrationsHistory"(
  "MigrationId" character varying(150) NOT NULL,
  "ProductVersion" character varying(32) NOT NULL)
;

--
-- Create primary key
--
ALTER TABLE public."__EFMigrationsHistory" 
  ADD PRIMARY KEY ("MigrationId");
  
--
-- Migrations history data
---

INSERT INTO public."__EFMigrationsHistory"
("MigrationId", "ProductVersion") VALUES
(E'20210529185033_Initial', E'5.0.4');
  