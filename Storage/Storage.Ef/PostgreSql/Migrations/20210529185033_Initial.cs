using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Zidium.Storage.Ef.PostgreSql.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "dbo");

            migrationBuilder.CreateTable(
                name: "AccountSettings",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ComponentTypes",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    SystemName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    IsSystem = table.Column<bool>(type: "boolean", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComponentTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LimitDatas",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BeginDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    EventsRequests = table.Column<int>(type: "integer", nullable: false),
                    LogSize = table.Column<long>(type: "bigint", nullable: false),
                    UnitTestsRequests = table.Column<int>(type: "integer", nullable: false),
                    MetricsRequests = table.Column<int>(type: "integer", nullable: false),
                    EventsSize = table.Column<long>(type: "bigint", nullable: false),
                    UnitTestsSize = table.Column<long>(type: "bigint", nullable: false),
                    MetricsSize = table.Column<long>(type: "bigint", nullable: false),
                    SmsCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LimitDatas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MetricTypes",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SystemName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ConditionAlarm = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    ConditionWarning = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    ConditionSuccess = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    ConditionElseColor = table.Column<int>(type: "integer", nullable: true),
                    NoSignalColor = table.Column<int>(type: "integer", nullable: true),
                    ActualTimeSecs = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MetricTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SystemName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    DisplayName = table.Column<string>(type: "character(50)", fixedLength: true, maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SendEmailCommand",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    From = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    To = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Subject = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Body = table.Column<string>(type: "text", nullable: false),
                    IsHtml = table.Column<bool>(type: "boolean", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ErrorMessage = table.Column<string>(type: "text", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    SendDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ReferenceId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SendEmailCommand", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SendMessageCommand",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Channel = table.Column<int>(type: "integer", nullable: false),
                    To = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Body = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ErrorMessage = table.Column<string>(type: "text", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    SendDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ReferenceId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SendMessageCommand", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SendSmsCommand",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Phone = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Body = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ErrorMessage = table.Column<string>(type: "text", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    SendDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ReferenceId = table.Column<Guid>(type: "uuid", nullable: true),
                    ExternalId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SendSmsCommand", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TimeZones",
                schema: "dbo",
                columns: table => new
                {
                    OffsetMinutes = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimeZones", x => x.OffsetMinutes);
                });

            migrationBuilder.CreateTable(
                name: "UnitTestTypes",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SystemName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    CreateDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    IsSystem = table.Column<bool>(type: "boolean", nullable: false),
                    NoSignalColor = table.Column<int>(type: "integer", nullable: true),
                    ActualTimeSecs = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnitTestTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Login = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    PasswordHash = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    FirstName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    LastName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    MiddleName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    DisplayName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CreateDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Post = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    InArchive = table.Column<bool>(type: "boolean", nullable: false),
                    SecurityStamp = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tokens",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Purpose = table.Column<int>(type: "integer", nullable: false),
                    SecurityStamp = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CreationDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    IsUsed = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tokens_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "dbo",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserContacts",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Value = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    CreateDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserContacts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserContacts_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "dbo",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "dbo",
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserRoles_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "dbo",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserSettings",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Value = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserSettings_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "dbo",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Components",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    SystemName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ParentId = table.Column<Guid>(type: "uuid", nullable: true),
                    ComponentTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    InternalStatusId = table.Column<Guid>(type: "uuid", nullable: false),
                    ExternalStatusId = table.Column<Guid>(type: "uuid", nullable: false),
                    UnitTestsStatusId = table.Column<Guid>(type: "uuid", nullable: false),
                    EventsStatusId = table.Column<Guid>(type: "uuid", nullable: false),
                    MetricsStatusId = table.Column<Guid>(type: "uuid", nullable: false),
                    ChildComponentsStatusId = table.Column<Guid>(type: "uuid", nullable: false),
                    Version = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    Enable = table.Column<bool>(type: "boolean", nullable: false),
                    DisableToDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DisableComment = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    ParentEnable = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Components", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Components_Components_ParentId",
                        column: x => x.ParentId,
                        principalSchema: "dbo",
                        principalTable: "Components",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Components_ComponentTypes_ComponentTypeId",
                        column: x => x.ComponentTypeId,
                        principalSchema: "dbo",
                        principalTable: "ComponentTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ComponentProperties",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ComponentId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Value = table.Column<string>(type: "character varying(8000)", maxLength: 8000, nullable: true),
                    DataType = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComponentProperties", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ComponentProperties_Components_ComponentId",
                        column: x => x.ComponentId,
                        principalSchema: "dbo",
                        principalTable: "Components",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LogConfigs",
                schema: "dbo",
                columns: table => new
                {
                    ComponentId = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUpdateDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Enabled = table.Column<bool>(type: "boolean", nullable: false),
                    IsDebugEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    IsTraceEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    IsInfoEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    IsWarningEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    IsErrorEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    IsFatalEnabled = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogConfigs", x => x.ComponentId);
                    table.ForeignKey(
                        name: "FK_LogConfigs_Components_ComponentId",
                        column: x => x.ComponentId,
                        principalSchema: "dbo",
                        principalTable: "Components",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Logs",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ComponentId = table.Column<Guid>(type: "uuid", nullable: false),
                    Level = table.Column<int>(type: "integer", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    Message = table.Column<string>(type: "character varying(8000)", maxLength: 8000, nullable: true),
                    ParametersCount = table.Column<int>(type: "integer", nullable: false),
                    Context = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Logs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Logs_Components_ComponentId",
                        column: x => x.ComponentId,
                        principalSchema: "dbo",
                        principalTable: "Components",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Subscriptions",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ComponentTypeId = table.Column<Guid>(type: "uuid", nullable: true),
                    ComponentId = table.Column<Guid>(type: "uuid", nullable: true),
                    Object = table.Column<int>(type: "integer", nullable: false),
                    Channel = table.Column<int>(type: "integer", nullable: false),
                    IsEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    NotifyBetterStatus = table.Column<bool>(type: "boolean", nullable: false),
                    Importance = table.Column<int>(type: "integer", nullable: false),
                    DurationMinimumInSeconds = table.Column<int>(type: "integer", nullable: true),
                    ResendTimeInSeconds = table.Column<int>(type: "integer", nullable: true),
                    LastUpdated = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    SendOnlyInInterval = table.Column<bool>(type: "boolean", nullable: false),
                    SendIntervalFromHour = table.Column<int>(type: "integer", nullable: true),
                    SendIntervalFromMinute = table.Column<int>(type: "integer", nullable: true),
                    SendIntervalToHour = table.Column<int>(type: "integer", nullable: true),
                    SendIntervalToMinute = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subscriptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Subscriptions_Components_ComponentId",
                        column: x => x.ComponentId,
                        principalSchema: "dbo",
                        principalTable: "Components",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Subscriptions_ComponentTypes_ComponentTypeId",
                        column: x => x.ComponentTypeId,
                        principalSchema: "dbo",
                        principalTable: "ComponentTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Subscriptions_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "dbo",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LogParameters",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    LogId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    DataType = table.Column<int>(type: "integer", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogParameters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LogParameters_Logs_LogId",
                        column: x => x.LogId,
                        principalSchema: "dbo",
                        principalTable: "Logs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Metrics",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ComponentId = table.Column<Guid>(type: "uuid", nullable: false),
                    MetricTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DisableToDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DisableComment = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Enable = table.Column<bool>(type: "boolean", nullable: false),
                    ParentEnable = table.Column<bool>(type: "boolean", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Value = table.Column<double>(type: "double precision", nullable: true),
                    BeginDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ActualDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ActualTimeSecs = table.Column<int>(type: "integer", nullable: true),
                    NoSignalColor = table.Column<int>(type: "integer", nullable: true),
                    ConditionAlarm = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    ConditionWarning = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    ConditionSuccess = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    ConditionElseColor = table.Column<int>(type: "integer", nullable: true),
                    StatusDataId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Metrics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Metrics_Components_ComponentId",
                        column: x => x.ComponentId,
                        principalSchema: "dbo",
                        principalTable: "Components",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Metrics_MetricTypes_MetricTypeId",
                        column: x => x.MetricTypeId,
                        principalSchema: "dbo",
                        principalTable: "MetricTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UnitTests",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SystemName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    TypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    PeriodSeconds = table.Column<int>(type: "integer", nullable: true),
                    ComponentId = table.Column<Guid>(type: "uuid", nullable: false),
                    StatusDataId = table.Column<Guid>(type: "uuid", nullable: false),
                    NextExecutionDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    NextStepProcessDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastExecutionDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DisableToDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DisableComment = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Enable = table.Column<bool>(type: "boolean", nullable: false),
                    ParentEnable = table.Column<bool>(type: "boolean", nullable: false),
                    SimpleMode = table.Column<bool>(type: "boolean", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ErrorColor = table.Column<int>(type: "integer", nullable: true),
                    NoSignalColor = table.Column<int>(type: "integer", nullable: true),
                    ActualTimeSecs = table.Column<int>(type: "integer", nullable: true),
                    AttempCount = table.Column<int>(type: "integer", nullable: false),
                    AttempMax = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnitTests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UnitTests_Components_ComponentId",
                        column: x => x.ComponentId,
                        principalSchema: "dbo",
                        principalTable: "Components",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UnitTests_UnitTestTypes_TypeId",
                        column: x => x.TypeId,
                        principalSchema: "dbo",
                        principalTable: "UnitTestTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Bulbs",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ComponentId = table.Column<Guid>(type: "uuid", nullable: true),
                    UnitTestId = table.Column<Guid>(type: "uuid", nullable: true),
                    MetricId = table.Column<Guid>(type: "uuid", nullable: true),
                    EventCategory = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    PreviousStatus = table.Column<int>(type: "integer", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Count = table.Column<int>(type: "integer", nullable: false),
                    ActualDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Message = table.Column<string>(type: "character varying(8000)", maxLength: 8000, nullable: true),
                    FirstEventId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastEventId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastChildStatusDataId = table.Column<Guid>(type: "uuid", nullable: true),
                    StatusEventId = table.Column<Guid>(type: "uuid", nullable: false),
                    UpTimeStartDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpTimeLengthMs = table.Column<long>(type: "bigint", nullable: false),
                    UpTimeSuccessMs = table.Column<long>(type: "bigint", nullable: false),
                    HasSignal = table.Column<bool>(type: "boolean", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bulbs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Bulbs_Components_ComponentId",
                        column: x => x.ComponentId,
                        principalSchema: "dbo",
                        principalTable: "Components",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Bulbs_Metrics_MetricId",
                        column: x => x.MetricId,
                        principalSchema: "dbo",
                        principalTable: "Metrics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Bulbs_UnitTests_UnitTestId",
                        column: x => x.UnitTestId,
                        principalSchema: "dbo",
                        principalTable: "UnitTests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HttpRequestUnitTests",
                schema: "dbo",
                columns: table => new
                {
                    UnitTestId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HttpRequestUnitTests", x => x.UnitTestId);
                    table.ForeignKey(
                        name: "FK_HttpRequestUnitTests_UnitTests_UnitTestId",
                        column: x => x.UnitTestId,
                        principalSchema: "dbo",
                        principalTable: "UnitTests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LimitDatasForUnitTests",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    LimitDataId = table.Column<Guid>(type: "uuid", nullable: false),
                    UnitTestId = table.Column<Guid>(type: "uuid", nullable: false),
                    ResultsCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LimitDatasForUnitTests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LimitDatasForUnitTests_LimitDatas_LimitDataId",
                        column: x => x.LimitDataId,
                        principalSchema: "dbo",
                        principalTable: "LimitDatas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LimitDatasForUnitTests_UnitTests_UnitTestId",
                        column: x => x.UnitTestId,
                        principalSchema: "dbo",
                        principalTable: "UnitTests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UnitTestDomainNamePaymentPeriodRules",
                schema: "dbo",
                columns: table => new
                {
                    UnitTestId = table.Column<Guid>(type: "uuid", nullable: false),
                    Domain = table.Column<string>(type: "text", nullable: true),
                    AlarmDaysCount = table.Column<int>(type: "integer", nullable: false),
                    WarningDaysCount = table.Column<int>(type: "integer", nullable: false),
                    LastRunErrorCode = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnitTestDomainNamePaymentPeriodRules", x => x.UnitTestId);
                    table.ForeignKey(
                        name: "FK_UnitTestDomainNamePaymentPeriodRules_UnitTests_UnitTestId",
                        column: x => x.UnitTestId,
                        principalSchema: "dbo",
                        principalTable: "UnitTests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UnitTestPingRules",
                schema: "dbo",
                columns: table => new
                {
                    UnitTestId = table.Column<Guid>(type: "uuid", nullable: false),
                    Host = table.Column<string>(type: "text", nullable: true),
                    TimeoutMs = table.Column<int>(type: "integer", nullable: false),
                    LastRunErrorCode = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnitTestPingRules", x => x.UnitTestId);
                    table.ForeignKey(
                        name: "FK_UnitTestPingRules_UnitTests_UnitTestId",
                        column: x => x.UnitTestId,
                        principalSchema: "dbo",
                        principalTable: "UnitTests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UnitTestProperties",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UnitTestId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Value = table.Column<string>(type: "character varying(8000)", maxLength: 8000, nullable: true),
                    DataType = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnitTestProperties", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UnitTestProperties_UnitTests_UnitTestId",
                        column: x => x.UnitTestId,
                        principalSchema: "dbo",
                        principalTable: "UnitTests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UnitTestSqlRules",
                schema: "dbo",
                columns: table => new
                {
                    UnitTestId = table.Column<Guid>(type: "uuid", nullable: false),
                    Provider = table.Column<int>(type: "integer", nullable: false),
                    ConnectionString = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    OpenConnectionTimeoutMs = table.Column<int>(type: "integer", nullable: false),
                    CommandTimeoutMs = table.Column<int>(type: "integer", nullable: false),
                    Query = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnitTestSqlRules", x => x.UnitTestId);
                    table.ForeignKey(
                        name: "FK_UnitTestSqlRules_UnitTests_UnitTestId",
                        column: x => x.UnitTestId,
                        principalSchema: "dbo",
                        principalTable: "UnitTests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UnitTestSslCertificateExpirationDateRules",
                schema: "dbo",
                columns: table => new
                {
                    UnitTestId = table.Column<Guid>(type: "uuid", nullable: false),
                    Url = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    AlarmDaysCount = table.Column<int>(type: "integer", nullable: false),
                    WarningDaysCount = table.Column<int>(type: "integer", nullable: false),
                    LastRunErrorCode = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnitTestSslCertificateExpirationDateRules", x => x.UnitTestId);
                    table.ForeignKey(
                        name: "FK_UnitTestSslCertificateExpirationDateRules_UnitTests_UnitTes~",
                        column: x => x.UnitTestId,
                        principalSchema: "dbo",
                        principalTable: "UnitTests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UnitTestTcpPortRules",
                schema: "dbo",
                columns: table => new
                {
                    UnitTestId = table.Column<Guid>(type: "uuid", nullable: false),
                    Host = table.Column<string>(type: "text", nullable: true),
                    TimeoutMs = table.Column<int>(type: "integer", nullable: false),
                    Port = table.Column<int>(type: "integer", nullable: false),
                    Opened = table.Column<bool>(type: "boolean", nullable: false),
                    LastRunErrorCode = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnitTestTcpPortRules", x => x.UnitTestId);
                    table.ForeignKey(
                        name: "FK_UnitTestTcpPortRules_UnitTests_UnitTestId",
                        column: x => x.UnitTestId,
                        principalSchema: "dbo",
                        principalTable: "UnitTests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UnitTestVirusTotalRules",
                schema: "dbo",
                columns: table => new
                {
                    UnitTestId = table.Column<Guid>(type: "uuid", nullable: false),
                    Url = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    NextStep = table.Column<int>(type: "integer", nullable: false),
                    ScanTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ScanId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    LastRunErrorCode = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnitTestVirusTotalRules", x => x.UnitTestId);
                    table.ForeignKey(
                        name: "FK_UnitTestVirusTotalRules_UnitTests_UnitTestId",
                        column: x => x.UnitTestId,
                        principalSchema: "dbo",
                        principalTable: "UnitTests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HttpRequestUnitTestRules",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    HttpRequestUnitTestId = table.Column<Guid>(type: "uuid", nullable: false),
                    SortNumber = table.Column<int>(type: "integer", nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Url = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Method = table.Column<int>(type: "integer", nullable: false),
                    Body = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    ResponseCode = table.Column<int>(type: "integer", nullable: true),
                    MaxResponseSize = table.Column<int>(type: "integer", nullable: false),
                    SuccessHtml = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    ErrorHtml = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    TimeoutSeconds = table.Column<int>(type: "integer", nullable: true),
                    LastRunErrorCode = table.Column<int>(type: "integer", nullable: true),
                    LastRunErrorMessage = table.Column<string>(type: "text", nullable: true),
                    LastRunTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastRunDurationMs = table.Column<int>(type: "integer", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HttpRequestUnitTestRules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HttpRequestUnitTestRules_HttpRequestUnitTests_HttpRequestUn~",
                        column: x => x.HttpRequestUnitTestId,
                        principalSchema: "dbo",
                        principalTable: "HttpRequestUnitTests",
                        principalColumn: "UnitTestId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HttpRequestUnitTestRuleDatas",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RuleId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Key = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HttpRequestUnitTestRuleDatas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HttpRequestUnitTestRuleDatas_HttpRequestUnitTestRules_RuleId",
                        column: x => x.RuleId,
                        principalSchema: "dbo",
                        principalTable: "HttpRequestUnitTestRules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LastComponentNotifications",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ComponentId = table.Column<Guid>(type: "uuid", nullable: false),
                    Address = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    EventImportance = table.Column<int>(type: "integer", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    EventId = table.Column<Guid>(type: "uuid", nullable: false),
                    NotificationId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LastComponentNotifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LastComponentNotifications_Components_ComponentId",
                        column: x => x.ComponentId,
                        principalSchema: "dbo",
                        principalTable: "Components",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MetricHistory",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ComponentId = table.Column<Guid>(type: "uuid", nullable: false),
                    MetricTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    BeginDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ActualDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Value = table.Column<double>(type: "double precision", nullable: true),
                    Color = table.Column<int>(type: "integer", nullable: false),
                    StatusEventId = table.Column<Guid>(type: "uuid", nullable: true),
                    HasSignal = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MetricHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MetricHistory_Components_ComponentId",
                        column: x => x.ComponentId,
                        principalSchema: "dbo",
                        principalTable: "Components",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MetricHistory_MetricTypes_MetricTypeId",
                        column: x => x.MetricTypeId,
                        principalSchema: "dbo",
                        principalTable: "MetricTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Defects",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Number = table.Column<int>(type: "integer", nullable: false),
                    Title = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    LastChangeId = table.Column<Guid>(type: "uuid", nullable: true),
                    ResponsibleUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    EventTypeId = table.Column<Guid>(type: "uuid", nullable: true),
                    Notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Defects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Defects_Users_ResponsibleUserId",
                        column: x => x.ResponsibleUserId,
                        principalSchema: "dbo",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DefectChanges",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DefectId = table.Column<Guid>(type: "uuid", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Comment = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DefectChanges", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DefectChanges_Defects_DefectId",
                        column: x => x.DefectId,
                        principalSchema: "dbo",
                        principalTable: "Defects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DefectChanges_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "dbo",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EventTypes",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Category = table.Column<int>(type: "integer", nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    SystemName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    JoinIntervalSeconds = table.Column<int>(type: "integer", nullable: true),
                    IsSystem = table.Column<bool>(type: "boolean", nullable: false),
                    OldVersion = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    ImportanceForOld = table.Column<int>(type: "integer", nullable: true),
                    ImportanceForNew = table.Column<int>(type: "integer", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DefectId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventTypes_Defects_DefectId",
                        column: x => x.DefectId,
                        principalSchema: "dbo",
                        principalTable: "Defects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Events",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OwnerId = table.Column<Guid>(type: "uuid", nullable: false),
                    EventTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    Message = table.Column<string>(type: "character varying(8000)", maxLength: 8000, nullable: true),
                    Importance = table.Column<int>(type: "integer", nullable: false),
                    PreviousImportance = table.Column<int>(type: "integer", nullable: false),
                    Count = table.Column<int>(type: "integer", nullable: false),
                    JoinKeyHash = table.Column<long>(type: "bigint", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ActualDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    IsSpace = table.Column<bool>(type: "boolean", nullable: false),
                    LastUpdateDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    LastNotificationDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsUserHandled = table.Column<bool>(type: "boolean", nullable: false),
                    Version = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    VersionLong = table.Column<long>(type: "bigint", nullable: true),
                    Category = table.Column<int>(type: "integer", nullable: false),
                    LastStatusEventId = table.Column<Guid>(type: "uuid", nullable: true),
                    FirstReasonEventId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Events", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Events_Events_LastStatusEventId",
                        column: x => x.LastStatusEventId,
                        principalSchema: "dbo",
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Events_EventTypes_EventTypeId",
                        column: x => x.EventTypeId,
                        principalSchema: "dbo",
                        principalTable: "EventTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ArchivedStatuses",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EventId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArchivedStatuses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ArchivedStatuses_Events_EventId",
                        column: x => x.EventId,
                        principalSchema: "dbo",
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EventParameters",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EventId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true),
                    DataType = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventParameters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventParameters_Events_EventId",
                        column: x => x.EventId,
                        principalSchema: "dbo",
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EventStatuses",
                schema: "dbo",
                columns: table => new
                {
                    EventId = table.Column<Guid>(type: "uuid", nullable: false),
                    StatusId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventStatuses", x => new { x.EventId, x.StatusId });
                    table.ForeignKey(
                        name: "FK_EventStatuses_Events_EventId",
                        column: x => x.EventId,
                        principalSchema: "dbo",
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EventStatuses_Events_StatusId",
                        column: x => x.StatusId,
                        principalSchema: "dbo",
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EventId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    SendError = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    CreationDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    SendDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    SubscriptionId = table.Column<Guid>(type: "uuid", nullable: true),
                    Reason = table.Column<int>(type: "integer", nullable: false),
                    Address = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    SendEmailCommandId = table.Column<Guid>(type: "uuid", nullable: true),
                    SendMessageCommandId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notifications_Events_EventId",
                        column: x => x.EventId,
                        principalSchema: "dbo",
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Notifications_SendEmailCommand_SendEmailCommandId",
                        column: x => x.SendEmailCommandId,
                        principalSchema: "dbo",
                        principalTable: "SendEmailCommand",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Notifications_SendMessageCommand_SendMessageCommandId",
                        column: x => x.SendMessageCommandId,
                        principalSchema: "dbo",
                        principalTable: "SendMessageCommand",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Notifications_Subscriptions_SubscriptionId",
                        column: x => x.SubscriptionId,
                        principalSchema: "dbo",
                        principalTable: "Subscriptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Notifications_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "dbo",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "NotificationsHttp",
                schema: "dbo",
                columns: table => new
                {
                    NotificationId = table.Column<Guid>(type: "uuid", nullable: false),
                    Json = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationsHttp", x => x.NotificationId);
                    table.ForeignKey(
                        name: "FK_NotificationsHttp_Notifications_NotificationId",
                        column: x => x.NotificationId,
                        principalSchema: "dbo",
                        principalTable: "Notifications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ArchivedStatuses_EventId",
                schema: "dbo",
                table: "ArchivedStatuses",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_Bulbs_ComponentId",
                schema: "dbo",
                table: "Bulbs",
                column: "ComponentId");

            migrationBuilder.CreateIndex(
                name: "IX_Bulbs_MetricId",
                schema: "dbo",
                table: "Bulbs",
                column: "MetricId");

            migrationBuilder.CreateIndex(
                name: "IX_Bulbs_UnitTestId",
                schema: "dbo",
                table: "Bulbs",
                column: "UnitTestId");

            migrationBuilder.CreateIndex(
                name: "IX_ComponentProperties_ComponentId",
                schema: "dbo",
                table: "ComponentProperties",
                column: "ComponentId");

            migrationBuilder.CreateIndex(
                name: "IX_Components_ChildComponentsStatusId",
                schema: "dbo",
                table: "Components",
                column: "ChildComponentsStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Components_ComponentTypeId",
                schema: "dbo",
                table: "Components",
                column: "ComponentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Components_EventsStatusId",
                schema: "dbo",
                table: "Components",
                column: "EventsStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Components_ExternalStatusId",
                schema: "dbo",
                table: "Components",
                column: "ExternalStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Components_InternalStatusId",
                schema: "dbo",
                table: "Components",
                column: "InternalStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Components_MetricsStatusId",
                schema: "dbo",
                table: "Components",
                column: "MetricsStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Components_ParentId",
                schema: "dbo",
                table: "Components",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_Components_SystemName",
                schema: "dbo",
                table: "Components",
                column: "SystemName");

            migrationBuilder.CreateIndex(
                name: "IX_Components_UnitTestsStatusId",
                schema: "dbo",
                table: "Components",
                column: "UnitTestsStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_ComponentTypes_SystemName",
                schema: "dbo",
                table: "ComponentTypes",
                column: "SystemName");

            migrationBuilder.CreateIndex(
                name: "IX_DefectChanges_DefectId",
                schema: "dbo",
                table: "DefectChanges",
                column: "DefectId");

            migrationBuilder.CreateIndex(
                name: "IX_DefectChanges_UserId",
                schema: "dbo",
                table: "DefectChanges",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Defects_EventTypeId",
                schema: "dbo",
                table: "Defects",
                column: "EventTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Defects_LastChangeId",
                schema: "dbo",
                table: "Defects",
                column: "LastChangeId");

            migrationBuilder.CreateIndex(
                name: "IX_Defects_ResponsibleUserId",
                schema: "dbo",
                table: "Defects",
                column: "ResponsibleUserId");

            migrationBuilder.CreateIndex(
                name: "IX_EventParameters_EventId",
                schema: "dbo",
                table: "EventParameters",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountBased",
                schema: "dbo",
                table: "Events",
                columns: new[] { "Category", "ActualDate", "StartDate", "Id" });

            migrationBuilder.CreateIndex(
                name: "IX_Events_EventTypeId",
                schema: "dbo",
                table: "Events",
                column: "EventTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Events_LastStatusEventId",
                schema: "dbo",
                table: "Events",
                column: "LastStatusEventId");

            migrationBuilder.CreateIndex(
                name: "IX_ForJoin",
                schema: "dbo",
                table: "Events",
                columns: new[] { "OwnerId", "EventTypeId", "Importance", "ActualDate" });

            migrationBuilder.CreateIndex(
                name: "IX_ForProcessing",
                schema: "dbo",
                table: "Events",
                columns: new[] { "IsUserHandled", "EventTypeId", "VersionLong" });

            migrationBuilder.CreateIndex(
                name: "IX_OwnerBased",
                schema: "dbo",
                table: "Events",
                columns: new[] { "OwnerId", "Category", "ActualDate", "StartDate" });

            migrationBuilder.CreateIndex(
                name: "IX_EventStatuses_EventId",
                schema: "dbo",
                table: "EventStatuses",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_EventStatuses_StatusId",
                schema: "dbo",
                table: "EventStatuses",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_EventTypes_DefectId",
                schema: "dbo",
                table: "EventTypes",
                column: "DefectId");

            migrationBuilder.CreateIndex(
                name: "IX_EventTypes_SystemName",
                schema: "dbo",
                table: "EventTypes",
                column: "SystemName");

            migrationBuilder.CreateIndex(
                name: "IX_HttpRequestUnitTestRuleDatas_RuleId",
                schema: "dbo",
                table: "HttpRequestUnitTestRuleDatas",
                column: "RuleId");

            migrationBuilder.CreateIndex(
                name: "IX_HttpRequestUnitTestRules_HttpRequestUnitTestId",
                schema: "dbo",
                table: "HttpRequestUnitTestRules",
                column: "HttpRequestUnitTestId");

            migrationBuilder.CreateIndex(
                name: "IX_LastComponentNotifications_ComponentId",
                schema: "dbo",
                table: "LastComponentNotifications",
                column: "ComponentId");

            migrationBuilder.CreateIndex(
                name: "IX_LastComponentNotifications_EventId",
                schema: "dbo",
                table: "LastComponentNotifications",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_LastComponentNotifications_NotificationId",
                schema: "dbo",
                table: "LastComponentNotifications",
                column: "NotificationId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountId",
                schema: "dbo",
                table: "LimitDatas",
                columns: new[] { "Type", "BeginDate" });

            migrationBuilder.CreateIndex(
                name: "IX_LimitDatasForUnitTests_LimitDataId",
                schema: "dbo",
                table: "LimitDatasForUnitTests",
                column: "LimitDataId");

            migrationBuilder.CreateIndex(
                name: "IX_LimitDatasForUnitTests_UnitTestId",
                schema: "dbo",
                table: "LimitDatasForUnitTests",
                column: "UnitTestId");

            migrationBuilder.CreateIndex(
                name: "IX_LogParameters_LogId",
                schema: "dbo",
                table: "LogParameters",
                column: "LogId");

            migrationBuilder.CreateIndex(
                name: "IX_ComponentBased",
                schema: "dbo",
                table: "Logs",
                columns: new[] { "ComponentId", "Date", "Order", "Level", "Context" });

            migrationBuilder.CreateIndex(
                name: "IX_Date",
                schema: "dbo",
                table: "Logs",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "IX_ForHistory",
                schema: "dbo",
                table: "MetricHistory",
                columns: new[] { "ComponentId", "MetricTypeId", "BeginDate" });

            migrationBuilder.CreateIndex(
                name: "IX_MetricHistory_BeginDate",
                schema: "dbo",
                table: "MetricHistory",
                column: "BeginDate");

            migrationBuilder.CreateIndex(
                name: "IX_MetricHistory_MetricTypeId",
                schema: "dbo",
                table: "MetricHistory",
                column: "MetricTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_MetricHistory_StatusEventId",
                schema: "dbo",
                table: "MetricHistory",
                column: "StatusEventId");

            migrationBuilder.CreateIndex(
                name: "IX_Metrics_ComponentId",
                schema: "dbo",
                table: "Metrics",
                column: "ComponentId");

            migrationBuilder.CreateIndex(
                name: "IX_Metrics_MetricTypeId",
                schema: "dbo",
                table: "Metrics",
                column: "MetricTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Metrics_StatusDataId",
                schema: "dbo",
                table: "Metrics",
                column: "StatusDataId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_EventId",
                schema: "dbo",
                table: "Notifications",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_SendEmailCommandId",
                schema: "dbo",
                table: "Notifications",
                column: "SendEmailCommandId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_SendMessageCommandId",
                schema: "dbo",
                table: "Notifications",
                column: "SendMessageCommandId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_Status",
                schema: "dbo",
                table: "Notifications",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_SubscriptionId",
                schema: "dbo",
                table: "Notifications",
                column: "SubscriptionId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserId",
                schema: "dbo",
                table: "Notifications",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ForSend",
                schema: "dbo",
                table: "SendEmailCommand",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_ForSend1",
                schema: "dbo",
                table: "SendMessageCommand",
                columns: new[] { "Channel", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_ForSend2",
                schema: "dbo",
                table: "SendSmsCommand",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_ComponentId",
                schema: "dbo",
                table: "Subscriptions",
                column: "ComponentId");

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_ComponentTypeId",
                schema: "dbo",
                table: "Subscriptions",
                column: "ComponentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_UserId",
                schema: "dbo",
                table: "Subscriptions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Tokens_UserId",
                schema: "dbo",
                table: "Tokens",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UnitTestProperties_UnitTestId",
                schema: "dbo",
                table: "UnitTestProperties",
                column: "UnitTestId");

            migrationBuilder.CreateIndex(
                name: "IX_UnitTests_ComponentId",
                schema: "dbo",
                table: "UnitTests",
                column: "ComponentId");

            migrationBuilder.CreateIndex(
                name: "IX_UnitTests_StatusDataId",
                schema: "dbo",
                table: "UnitTests",
                column: "StatusDataId");

            migrationBuilder.CreateIndex(
                name: "IX_UnitTests_TypeId",
                schema: "dbo",
                table: "UnitTests",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_UnitTestTypes_SystemName",
                schema: "dbo",
                table: "UnitTestTypes",
                column: "SystemName");

            migrationBuilder.CreateIndex(
                name: "IX_UserContacts_UserId",
                schema: "dbo",
                table: "UserContacts",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_RoleId",
                schema: "dbo",
                table: "UserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_UserId",
                schema: "dbo",
                table: "UserRoles",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Login",
                schema: "dbo",
                table: "Users",
                column: "Login");

            migrationBuilder.CreateIndex(
                name: "IX_UserSettings_UserId",
                schema: "dbo",
                table: "UserSettings",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Components_Bulbs_ChildComponentsStatusId",
                schema: "dbo",
                table: "Components",
                column: "ChildComponentsStatusId",
                principalSchema: "dbo",
                principalTable: "Bulbs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Components_Bulbs_EventsStatusId",
                schema: "dbo",
                table: "Components",
                column: "EventsStatusId",
                principalSchema: "dbo",
                principalTable: "Bulbs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Components_Bulbs_ExternalStatusId",
                schema: "dbo",
                table: "Components",
                column: "ExternalStatusId",
                principalSchema: "dbo",
                principalTable: "Bulbs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Components_Bulbs_InternalStatusId",
                schema: "dbo",
                table: "Components",
                column: "InternalStatusId",
                principalSchema: "dbo",
                principalTable: "Bulbs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Components_Bulbs_MetricsStatusId",
                schema: "dbo",
                table: "Components",
                column: "MetricsStatusId",
                principalSchema: "dbo",
                principalTable: "Bulbs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Components_Bulbs_UnitTestsStatusId",
                schema: "dbo",
                table: "Components",
                column: "UnitTestsStatusId",
                principalSchema: "dbo",
                principalTable: "Bulbs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Metrics_Bulbs_StatusDataId",
                schema: "dbo",
                table: "Metrics",
                column: "StatusDataId",
                principalSchema: "dbo",
                principalTable: "Bulbs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UnitTests_Bulbs_StatusDataId",
                schema: "dbo",
                table: "UnitTests",
                column: "StatusDataId",
                principalSchema: "dbo",
                principalTable: "Bulbs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LastComponentNotifications_Events_EventId",
                schema: "dbo",
                table: "LastComponentNotifications",
                column: "EventId",
                principalSchema: "dbo",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LastComponentNotifications_Notifications_NotificationId",
                schema: "dbo",
                table: "LastComponentNotifications",
                column: "NotificationId",
                principalSchema: "dbo",
                principalTable: "Notifications",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MetricHistory_Events_StatusEventId",
                schema: "dbo",
                table: "MetricHistory",
                column: "StatusEventId",
                principalSchema: "dbo",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Defects_DefectChanges_LastChangeId",
                schema: "dbo",
                table: "Defects",
                column: "LastChangeId",
                principalSchema: "dbo",
                principalTable: "DefectChanges",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Defects_EventTypes_EventTypeId",
                schema: "dbo",
                table: "Defects",
                column: "EventTypeId",
                principalSchema: "dbo",
                principalTable: "EventTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bulbs_Components_ComponentId",
                schema: "dbo",
                table: "Bulbs");

            migrationBuilder.DropForeignKey(
                name: "FK_Metrics_Components_ComponentId",
                schema: "dbo",
                table: "Metrics");

            migrationBuilder.DropForeignKey(
                name: "FK_UnitTests_Components_ComponentId",
                schema: "dbo",
                table: "UnitTests");

            migrationBuilder.DropForeignKey(
                name: "FK_Bulbs_Metrics_MetricId",
                schema: "dbo",
                table: "Bulbs");

            migrationBuilder.DropForeignKey(
                name: "FK_Bulbs_UnitTests_UnitTestId",
                schema: "dbo",
                table: "Bulbs");

            migrationBuilder.DropForeignKey(
                name: "FK_DefectChanges_Defects_DefectId",
                schema: "dbo",
                table: "DefectChanges");

            migrationBuilder.DropForeignKey(
                name: "FK_EventTypes_Defects_DefectId",
                schema: "dbo",
                table: "EventTypes");

            migrationBuilder.DropTable(
                name: "AccountSettings",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "ArchivedStatuses",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "ComponentProperties",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "EventParameters",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "EventStatuses",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "HttpRequestUnitTestRuleDatas",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "LastComponentNotifications",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "LimitDatasForUnitTests",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "LogConfigs",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "LogParameters",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "MetricHistory",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "NotificationsHttp",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "SendSmsCommand",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "TimeZones",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Tokens",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "UnitTestDomainNamePaymentPeriodRules",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "UnitTestPingRules",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "UnitTestProperties",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "UnitTestSqlRules",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "UnitTestSslCertificateExpirationDateRules",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "UnitTestTcpPortRules",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "UnitTestVirusTotalRules",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "UserContacts",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "UserRoles",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "UserSettings",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "HttpRequestUnitTestRules",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "LimitDatas",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Logs",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Notifications",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Roles",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "HttpRequestUnitTests",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Events",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "SendEmailCommand",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "SendMessageCommand",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Subscriptions",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Components",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "ComponentTypes",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Metrics",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "MetricTypes",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "UnitTests",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Bulbs",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "UnitTestTypes",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Defects",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "DefectChanges",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "EventTypes",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Users",
                schema: "dbo");
        }
    }
}
