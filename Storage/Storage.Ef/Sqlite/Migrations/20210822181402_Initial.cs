using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Zidium.Storage.Ef.Sqlite.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AccountSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false, collation: "UTF8CI"),
                    Value = table.Column<string>(type: "TEXT", nullable: true, collation: "UTF8CI")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ComponentTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    DisplayName = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false, collation: "UTF8CI"),
                    SystemName = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false, collation: "UTF8CI"),
                    IsSystem = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComponentTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LimitDatas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    BeginDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EndDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    EventsRequests = table.Column<int>(type: "INTEGER", nullable: false),
                    LogSize = table.Column<long>(type: "INTEGER", nullable: false),
                    UnitTestsRequests = table.Column<int>(type: "INTEGER", nullable: false),
                    MetricsRequests = table.Column<int>(type: "INTEGER", nullable: false),
                    EventsSize = table.Column<long>(type: "INTEGER", nullable: false),
                    UnitTestsSize = table.Column<long>(type: "INTEGER", nullable: false),
                    MetricsSize = table.Column<long>(type: "INTEGER", nullable: false),
                    SmsCount = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LimitDatas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MetricTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    SystemName = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false, collation: "UTF8CI"),
                    DisplayName = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true, collation: "UTF8CI"),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ConditionAlarm = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true, collation: "UTF8CI"),
                    ConditionWarning = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true, collation: "UTF8CI"),
                    ConditionSuccess = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true, collation: "UTF8CI"),
                    ConditionElseColor = table.Column<int>(type: "INTEGER", nullable: true),
                    NoSignalColor = table.Column<int>(type: "INTEGER", nullable: true),
                    ActualTimeSecs = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MetricTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    SystemName = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false, collation: "UTF8CI"),
                    DisplayName = table.Column<string>(type: "TEXT", fixedLength: true, maxLength: 50, nullable: true, collation: "UTF8CI")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SendEmailCommand",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    From = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true, collation: "UTF8CI"),
                    To = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false, collation: "UTF8CI"),
                    Subject = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false, collation: "UTF8CI"),
                    Body = table.Column<string>(type: "TEXT", nullable: false, collation: "UTF8CI"),
                    IsHtml = table.Column<bool>(type: "INTEGER", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    ErrorMessage = table.Column<string>(type: "TEXT", nullable: true, collation: "UTF8CI"),
                    CreateDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    SendDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ReferenceId = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SendEmailCommand", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SendMessageCommand",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Channel = table.Column<int>(type: "INTEGER", nullable: false),
                    To = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false, collation: "UTF8CI"),
                    Body = table.Column<string>(type: "TEXT", nullable: false, collation: "UTF8CI"),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    ErrorMessage = table.Column<string>(type: "TEXT", nullable: true, collation: "UTF8CI"),
                    CreateDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    SendDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ReferenceId = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SendMessageCommand", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SendSmsCommand",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Phone = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false, collation: "UTF8CI"),
                    Body = table.Column<string>(type: "TEXT", nullable: false, collation: "UTF8CI"),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    ErrorMessage = table.Column<string>(type: "TEXT", nullable: true, collation: "UTF8CI"),
                    CreateDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    SendDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ReferenceId = table.Column<Guid>(type: "TEXT", nullable: true),
                    ExternalId = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true, collation: "UTF8CI")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SendSmsCommand", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TimeZones",
                columns: table => new
                {
                    OffsetMinutes = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false, collation: "UTF8CI")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimeZones", x => x.OffsetMinutes);
                });

            migrationBuilder.CreateTable(
                name: "UnitTestTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    SystemName = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false, collation: "UTF8CI"),
                    DisplayName = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false, collation: "UTF8CI"),
                    CreateDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsSystem = table.Column<bool>(type: "INTEGER", nullable: false),
                    NoSignalColor = table.Column<int>(type: "INTEGER", nullable: true),
                    ActualTimeSecs = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnitTestTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Login = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false, collation: "UTF8CI"),
                    PasswordHash = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true, collation: "UTF8CI"),
                    FirstName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true, collation: "UTF8CI"),
                    LastName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true, collation: "UTF8CI"),
                    MiddleName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true, collation: "UTF8CI"),
                    DisplayName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true, collation: "UTF8CI"),
                    CreateDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Post = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true, collation: "UTF8CI"),
                    InArchive = table.Column<bool>(type: "INTEGER", nullable: false),
                    SecurityStamp = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true, collation: "UTF8CI")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tokens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Purpose = table.Column<int>(type: "INTEGER", nullable: false),
                    SecurityStamp = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true, collation: "UTF8CI"),
                    CreationDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EndDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsUsed = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tokens_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserContacts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    Value = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false, collation: "UTF8CI"),
                    CreateDate = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserContacts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserContacts_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    RoleId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserRoles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false, collation: "UTF8CI"),
                    Value = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true, collation: "UTF8CI")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserSettings_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Components",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DisplayName = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false, collation: "UTF8CI"),
                    SystemName = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false, collation: "UTF8CI"),
                    ParentId = table.Column<Guid>(type: "TEXT", nullable: true),
                    ComponentTypeId = table.Column<Guid>(type: "TEXT", nullable: false),
                    InternalStatusId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ExternalStatusId = table.Column<Guid>(type: "TEXT", nullable: false),
                    UnitTestsStatusId = table.Column<Guid>(type: "TEXT", nullable: false),
                    EventsStatusId = table.Column<Guid>(type: "TEXT", nullable: false),
                    MetricsStatusId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ChildComponentsStatusId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Version = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true, collation: "UTF8CI"),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    Enable = table.Column<bool>(type: "INTEGER", nullable: false),
                    DisableToDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DisableComment = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true, collation: "UTF8CI"),
                    ParentEnable = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Components", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Components_Components_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Components",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Components_ComponentTypes_ComponentTypeId",
                        column: x => x.ComponentTypeId,
                        principalTable: "ComponentTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ComponentProperties",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ComponentId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false, collation: "UTF8CI"),
                    Value = table.Column<string>(type: "TEXT", maxLength: 8000, nullable: true, collation: "UTF8CI"),
                    DataType = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComponentProperties", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ComponentProperties_Components_ComponentId",
                        column: x => x.ComponentId,
                        principalTable: "Components",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LogConfigs",
                columns: table => new
                {
                    ComponentId = table.Column<Guid>(type: "TEXT", nullable: false),
                    LastUpdateDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Enabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsDebugEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsTraceEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsInfoEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsWarningEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsErrorEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsFatalEnabled = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogConfigs", x => x.ComponentId);
                    table.ForeignKey(
                        name: "FK_LogConfigs_Components_ComponentId",
                        column: x => x.ComponentId,
                        principalTable: "Components",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Logs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ComponentId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Level = table.Column<int>(type: "INTEGER", nullable: false),
                    Date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Order = table.Column<int>(type: "INTEGER", nullable: false),
                    Message = table.Column<string>(type: "TEXT", maxLength: 8000, nullable: true, collation: "UTF8CI"),
                    ParametersCount = table.Column<int>(type: "INTEGER", nullable: false),
                    Context = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true, collation: "UTF8CI")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Logs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Logs_Components_ComponentId",
                        column: x => x.ComponentId,
                        principalTable: "Components",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Subscriptions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ComponentTypeId = table.Column<Guid>(type: "TEXT", nullable: true),
                    ComponentId = table.Column<Guid>(type: "TEXT", nullable: true),
                    Object = table.Column<int>(type: "INTEGER", nullable: false),
                    Channel = table.Column<int>(type: "INTEGER", nullable: false),
                    IsEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    NotifyBetterStatus = table.Column<bool>(type: "INTEGER", nullable: false),
                    Importance = table.Column<int>(type: "INTEGER", nullable: false),
                    DurationMinimumInSeconds = table.Column<int>(type: "INTEGER", nullable: true),
                    ResendTimeInSeconds = table.Column<int>(type: "INTEGER", nullable: true),
                    LastUpdated = table.Column<DateTime>(type: "TEXT", nullable: false),
                    SendOnlyInInterval = table.Column<bool>(type: "INTEGER", nullable: false),
                    SendIntervalFromHour = table.Column<int>(type: "INTEGER", nullable: true),
                    SendIntervalFromMinute = table.Column<int>(type: "INTEGER", nullable: true),
                    SendIntervalToHour = table.Column<int>(type: "INTEGER", nullable: true),
                    SendIntervalToMinute = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subscriptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Subscriptions_Components_ComponentId",
                        column: x => x.ComponentId,
                        principalTable: "Components",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Subscriptions_ComponentTypes_ComponentTypeId",
                        column: x => x.ComponentTypeId,
                        principalTable: "ComponentTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Subscriptions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LogParameters",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    LogId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false, collation: "UTF8CI"),
                    DataType = table.Column<int>(type: "INTEGER", nullable: false),
                    Value = table.Column<string>(type: "TEXT", nullable: true, collation: "UTF8CI")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogParameters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LogParameters_Logs_LogId",
                        column: x => x.LogId,
                        principalTable: "Logs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Metrics",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ComponentId = table.Column<Guid>(type: "TEXT", nullable: false),
                    MetricTypeId = table.Column<Guid>(type: "TEXT", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    DisableToDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DisableComment = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true, collation: "UTF8CI"),
                    Enable = table.Column<bool>(type: "INTEGER", nullable: false),
                    ParentEnable = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Value = table.Column<double>(type: "REAL", nullable: true),
                    BeginDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ActualDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ActualTimeSecs = table.Column<int>(type: "INTEGER", nullable: true),
                    NoSignalColor = table.Column<int>(type: "INTEGER", nullable: true),
                    ConditionAlarm = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true, collation: "UTF8CI"),
                    ConditionWarning = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true, collation: "UTF8CI"),
                    ConditionSuccess = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true, collation: "UTF8CI"),
                    ConditionElseColor = table.Column<int>(type: "INTEGER", nullable: true),
                    StatusDataId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Metrics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Metrics_Components_ComponentId",
                        column: x => x.ComponentId,
                        principalTable: "Components",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Metrics_MetricTypes_MetricTypeId",
                        column: x => x.MetricTypeId,
                        principalTable: "MetricTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UnitTests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    SystemName = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false, collation: "UTF8CI"),
                    DisplayName = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false, collation: "UTF8CI"),
                    TypeId = table.Column<Guid>(type: "TEXT", nullable: false),
                    PeriodSeconds = table.Column<int>(type: "INTEGER", nullable: true),
                    ComponentId = table.Column<Guid>(type: "TEXT", nullable: false),
                    StatusDataId = table.Column<Guid>(type: "TEXT", nullable: false),
                    NextExecutionDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    NextStepProcessDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LastExecutionDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DisableToDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DisableComment = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true, collation: "UTF8CI"),
                    Enable = table.Column<bool>(type: "INTEGER", nullable: false),
                    ParentEnable = table.Column<bool>(type: "INTEGER", nullable: false),
                    SimpleMode = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ErrorColor = table.Column<int>(type: "INTEGER", nullable: true),
                    NoSignalColor = table.Column<int>(type: "INTEGER", nullable: true),
                    ActualTimeSecs = table.Column<int>(type: "INTEGER", nullable: true),
                    AttempCount = table.Column<int>(type: "INTEGER", nullable: false),
                    AttempMax = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnitTests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UnitTests_Components_ComponentId",
                        column: x => x.ComponentId,
                        principalTable: "Components",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UnitTests_UnitTestTypes_TypeId",
                        column: x => x.TypeId,
                        principalTable: "UnitTestTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Bulbs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ComponentId = table.Column<Guid>(type: "TEXT", nullable: true),
                    UnitTestId = table.Column<Guid>(type: "TEXT", nullable: true),
                    MetricId = table.Column<Guid>(type: "TEXT", nullable: true),
                    EventCategory = table.Column<int>(type: "INTEGER", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    PreviousStatus = table.Column<int>(type: "INTEGER", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    StartDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EndDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Count = table.Column<int>(type: "INTEGER", nullable: false),
                    ActualDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Message = table.Column<string>(type: "TEXT", maxLength: 8000, nullable: true, collation: "UTF8CI"),
                    FirstEventId = table.Column<Guid>(type: "TEXT", nullable: true),
                    LastEventId = table.Column<Guid>(type: "TEXT", nullable: true),
                    LastChildStatusDataId = table.Column<Guid>(type: "TEXT", nullable: true),
                    StatusEventId = table.Column<Guid>(type: "TEXT", nullable: false),
                    UpTimeStartDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpTimeLengthMs = table.Column<long>(type: "INTEGER", nullable: false),
                    UpTimeSuccessMs = table.Column<long>(type: "INTEGER", nullable: false),
                    HasSignal = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bulbs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Bulbs_Components_ComponentId",
                        column: x => x.ComponentId,
                        principalTable: "Components",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Bulbs_Metrics_MetricId",
                        column: x => x.MetricId,
                        principalTable: "Metrics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Bulbs_UnitTests_UnitTestId",
                        column: x => x.UnitTestId,
                        principalTable: "UnitTests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HttpRequestUnitTests",
                columns: table => new
                {
                    UnitTestId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HttpRequestUnitTests", x => x.UnitTestId);
                    table.ForeignKey(
                        name: "FK_HttpRequestUnitTests_UnitTests_UnitTestId",
                        column: x => x.UnitTestId,
                        principalTable: "UnitTests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LimitDatasForUnitTests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    LimitDataId = table.Column<Guid>(type: "TEXT", nullable: false),
                    UnitTestId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ResultsCount = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LimitDatasForUnitTests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LimitDatasForUnitTests_LimitDatas_LimitDataId",
                        column: x => x.LimitDataId,
                        principalTable: "LimitDatas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LimitDatasForUnitTests_UnitTests_UnitTestId",
                        column: x => x.UnitTestId,
                        principalTable: "UnitTests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UnitTestDomainNamePaymentPeriodRules",
                columns: table => new
                {
                    UnitTestId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Domain = table.Column<string>(type: "TEXT", nullable: true, collation: "UTF8CI"),
                    AlarmDaysCount = table.Column<int>(type: "INTEGER", nullable: false),
                    WarningDaysCount = table.Column<int>(type: "INTEGER", nullable: false),
                    LastRunErrorCode = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnitTestDomainNamePaymentPeriodRules", x => x.UnitTestId);
                    table.ForeignKey(
                        name: "FK_UnitTestDomainNamePaymentPeriodRules_UnitTests_UnitTestId",
                        column: x => x.UnitTestId,
                        principalTable: "UnitTests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UnitTestPingRules",
                columns: table => new
                {
                    UnitTestId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Host = table.Column<string>(type: "TEXT", nullable: true, collation: "UTF8CI"),
                    TimeoutMs = table.Column<int>(type: "INTEGER", nullable: false),
                    LastRunErrorCode = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnitTestPingRules", x => x.UnitTestId);
                    table.ForeignKey(
                        name: "FK_UnitTestPingRules_UnitTests_UnitTestId",
                        column: x => x.UnitTestId,
                        principalTable: "UnitTests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UnitTestProperties",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    UnitTestId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false, collation: "UTF8CI"),
                    Value = table.Column<string>(type: "TEXT", maxLength: 8000, nullable: true, collation: "UTF8CI"),
                    DataType = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnitTestProperties", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UnitTestProperties_UnitTests_UnitTestId",
                        column: x => x.UnitTestId,
                        principalTable: "UnitTests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UnitTestSqlRules",
                columns: table => new
                {
                    UnitTestId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Provider = table.Column<int>(type: "INTEGER", nullable: false),
                    ConnectionString = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true, collation: "UTF8CI"),
                    OpenConnectionTimeoutMs = table.Column<int>(type: "INTEGER", nullable: false),
                    CommandTimeoutMs = table.Column<int>(type: "INTEGER", nullable: false),
                    Query = table.Column<string>(type: "TEXT", nullable: true, collation: "UTF8CI")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnitTestSqlRules", x => x.UnitTestId);
                    table.ForeignKey(
                        name: "FK_UnitTestSqlRules_UnitTests_UnitTestId",
                        column: x => x.UnitTestId,
                        principalTable: "UnitTests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UnitTestSslCertificateExpirationDateRules",
                columns: table => new
                {
                    UnitTestId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Url = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true, collation: "UTF8CI"),
                    AlarmDaysCount = table.Column<int>(type: "INTEGER", nullable: false),
                    WarningDaysCount = table.Column<int>(type: "INTEGER", nullable: false),
                    LastRunErrorCode = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnitTestSslCertificateExpirationDateRules", x => x.UnitTestId);
                    table.ForeignKey(
                        name: "FK_UnitTestSslCertificateExpirationDateRules_UnitTests_UnitTestId",
                        column: x => x.UnitTestId,
                        principalTable: "UnitTests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UnitTestTcpPortRules",
                columns: table => new
                {
                    UnitTestId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Host = table.Column<string>(type: "TEXT", nullable: true, collation: "UTF8CI"),
                    TimeoutMs = table.Column<int>(type: "INTEGER", nullable: false),
                    Port = table.Column<int>(type: "INTEGER", nullable: false),
                    Opened = table.Column<bool>(type: "INTEGER", nullable: false),
                    LastRunErrorCode = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnitTestTcpPortRules", x => x.UnitTestId);
                    table.ForeignKey(
                        name: "FK_UnitTestTcpPortRules_UnitTests_UnitTestId",
                        column: x => x.UnitTestId,
                        principalTable: "UnitTests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UnitTestVirusTotalRules",
                columns: table => new
                {
                    UnitTestId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Url = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: false, collation: "UTF8CI"),
                    NextStep = table.Column<int>(type: "INTEGER", nullable: false),
                    ScanTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ScanId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true, collation: "UTF8CI"),
                    LastRunErrorCode = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnitTestVirusTotalRules", x => x.UnitTestId);
                    table.ForeignKey(
                        name: "FK_UnitTestVirusTotalRules_UnitTests_UnitTestId",
                        column: x => x.UnitTestId,
                        principalTable: "UnitTests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HttpRequestUnitTestRules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    HttpRequestUnitTestId = table.Column<Guid>(type: "TEXT", nullable: false),
                    SortNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    DisplayName = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false, collation: "UTF8CI"),
                    Url = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false, collation: "UTF8CI"),
                    Method = table.Column<int>(type: "INTEGER", nullable: false),
                    Body = table.Column<string>(type: "TEXT", maxLength: 4000, nullable: true, collation: "UTF8CI"),
                    ResponseCode = table.Column<int>(type: "INTEGER", nullable: true),
                    MaxResponseSize = table.Column<int>(type: "INTEGER", nullable: false),
                    SuccessHtml = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true, collation: "UTF8CI"),
                    ErrorHtml = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true, collation: "UTF8CI"),
                    TimeoutSeconds = table.Column<int>(type: "INTEGER", nullable: true),
                    LastRunErrorCode = table.Column<int>(type: "INTEGER", nullable: true),
                    LastRunErrorMessage = table.Column<string>(type: "TEXT", nullable: true, collation: "UTF8CI"),
                    LastRunTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LastRunDurationMs = table.Column<int>(type: "INTEGER", nullable: true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HttpRequestUnitTestRules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HttpRequestUnitTestRules_HttpRequestUnitTests_HttpRequestUnitTestId",
                        column: x => x.HttpRequestUnitTestId,
                        principalTable: "HttpRequestUnitTests",
                        principalColumn: "UnitTestId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HttpRequestUnitTestRuleDatas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    RuleId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    Key = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true, collation: "UTF8CI"),
                    Value = table.Column<string>(type: "TEXT", nullable: true, collation: "UTF8CI")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HttpRequestUnitTestRuleDatas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HttpRequestUnitTestRuleDatas_HttpRequestUnitTestRules_RuleId",
                        column: x => x.RuleId,
                        principalTable: "HttpRequestUnitTestRules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LastComponentNotifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ComponentId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Address = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false, collation: "UTF8CI"),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    EventImportance = table.Column<int>(type: "INTEGER", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EventId = table.Column<Guid>(type: "TEXT", nullable: false),
                    NotificationId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LastComponentNotifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LastComponentNotifications_Components_ComponentId",
                        column: x => x.ComponentId,
                        principalTable: "Components",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MetricHistory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ComponentId = table.Column<Guid>(type: "TEXT", nullable: false),
                    MetricTypeId = table.Column<Guid>(type: "TEXT", nullable: false),
                    BeginDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ActualDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Value = table.Column<double>(type: "REAL", nullable: true),
                    Color = table.Column<int>(type: "INTEGER", nullable: false),
                    StatusEventId = table.Column<Guid>(type: "TEXT", nullable: true),
                    HasSignal = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MetricHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MetricHistory_Components_ComponentId",
                        column: x => x.ComponentId,
                        principalTable: "Components",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MetricHistory_MetricTypes_MetricTypeId",
                        column: x => x.MetricTypeId,
                        principalTable: "MetricTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Defects",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Number = table.Column<int>(type: "INTEGER", nullable: false),
                    Title = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true, collation: "UTF8CI"),
                    LastChangeId = table.Column<Guid>(type: "TEXT", nullable: true),
                    ResponsibleUserId = table.Column<Guid>(type: "TEXT", nullable: true),
                    EventTypeId = table.Column<Guid>(type: "TEXT", nullable: true),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true, collation: "UTF8CI")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Defects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Defects_Users_ResponsibleUserId",
                        column: x => x.ResponsibleUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DefectChanges",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    DefectId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    Comment = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true, collation: "UTF8CI"),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DefectChanges", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DefectChanges_Defects_DefectId",
                        column: x => x.DefectId,
                        principalTable: "Defects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DefectChanges_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EventTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Category = table.Column<int>(type: "INTEGER", nullable: false),
                    DisplayName = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false, collation: "UTF8CI"),
                    SystemName = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false, collation: "UTF8CI"),
                    Code = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true, collation: "UTF8CI"),
                    JoinIntervalSeconds = table.Column<int>(type: "INTEGER", nullable: true),
                    IsSystem = table.Column<bool>(type: "INTEGER", nullable: false),
                    OldVersion = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true, collation: "UTF8CI"),
                    ImportanceForOld = table.Column<int>(type: "INTEGER", nullable: true),
                    ImportanceForNew = table.Column<int>(type: "INTEGER", nullable: true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DefectId = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventTypes_Defects_DefectId",
                        column: x => x.DefectId,
                        principalTable: "Defects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Events",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    OwnerId = table.Column<Guid>(type: "TEXT", nullable: false),
                    EventTypeId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Message = table.Column<string>(type: "TEXT", maxLength: 8000, nullable: true, collation: "UTF8CI"),
                    Importance = table.Column<int>(type: "INTEGER", nullable: false),
                    PreviousImportance = table.Column<int>(type: "INTEGER", nullable: false),
                    Count = table.Column<int>(type: "INTEGER", nullable: false),
                    JoinKeyHash = table.Column<long>(type: "INTEGER", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    StartDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EndDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ActualDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsSpace = table.Column<bool>(type: "INTEGER", nullable: false),
                    LastUpdateDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LastNotificationDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsUserHandled = table.Column<bool>(type: "INTEGER", nullable: false),
                    Version = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true, collation: "UTF8CI"),
                    VersionLong = table.Column<long>(type: "INTEGER", nullable: true),
                    Category = table.Column<int>(type: "INTEGER", nullable: false),
                    LastStatusEventId = table.Column<Guid>(type: "TEXT", nullable: true),
                    FirstReasonEventId = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Events", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Events_Events_LastStatusEventId",
                        column: x => x.LastStatusEventId,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Events_EventTypes_EventTypeId",
                        column: x => x.EventTypeId,
                        principalTable: "EventTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ArchivedStatuses",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EventId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArchivedStatuses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ArchivedStatuses_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EventParameters",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    EventId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false, collation: "UTF8CI"),
                    Value = table.Column<string>(type: "TEXT", nullable: true, collation: "UTF8CI"),
                    DataType = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventParameters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventParameters_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EventStatuses",
                columns: table => new
                {
                    EventId = table.Column<Guid>(type: "TEXT", nullable: false),
                    StatusId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventStatuses", x => new { x.EventId, x.StatusId });
                    table.ForeignKey(
                        name: "FK_EventStatuses_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EventStatuses_Events_StatusId",
                        column: x => x.StatusId,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    EventId = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    SendError = table.Column<string>(type: "TEXT", maxLength: 4000, nullable: true, collation: "UTF8CI"),
                    CreationDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    SendDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    SubscriptionId = table.Column<Guid>(type: "TEXT", nullable: true),
                    Reason = table.Column<int>(type: "INTEGER", nullable: false),
                    Address = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false, collation: "UTF8CI"),
                    SendEmailCommandId = table.Column<Guid>(type: "TEXT", nullable: true),
                    SendMessageCommandId = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notifications_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Notifications_SendEmailCommand_SendEmailCommandId",
                        column: x => x.SendEmailCommandId,
                        principalTable: "SendEmailCommand",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Notifications_SendMessageCommand_SendMessageCommandId",
                        column: x => x.SendMessageCommandId,
                        principalTable: "SendMessageCommand",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Notifications_Subscriptions_SubscriptionId",
                        column: x => x.SubscriptionId,
                        principalTable: "Subscriptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Notifications_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "NotificationsHttp",
                columns: table => new
                {
                    NotificationId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Json = table.Column<string>(type: "TEXT", nullable: true, collation: "UTF8CI")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationsHttp", x => x.NotificationId);
                    table.ForeignKey(
                        name: "FK_NotificationsHttp_Notifications_NotificationId",
                        column: x => x.NotificationId,
                        principalTable: "Notifications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ArchivedStatuses_EventId",
                table: "ArchivedStatuses",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_Bulbs_ComponentId",
                table: "Bulbs",
                column: "ComponentId");

            migrationBuilder.CreateIndex(
                name: "IX_Bulbs_MetricId",
                table: "Bulbs",
                column: "MetricId");

            migrationBuilder.CreateIndex(
                name: "IX_Bulbs_UnitTestId",
                table: "Bulbs",
                column: "UnitTestId");

            migrationBuilder.CreateIndex(
                name: "IX_ComponentProperties_ComponentId",
                table: "ComponentProperties",
                column: "ComponentId");

            migrationBuilder.CreateIndex(
                name: "IX_Components_ChildComponentsStatusId",
                table: "Components",
                column: "ChildComponentsStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Components_ComponentTypeId",
                table: "Components",
                column: "ComponentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Components_EventsStatusId",
                table: "Components",
                column: "EventsStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Components_ExternalStatusId",
                table: "Components",
                column: "ExternalStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Components_InternalStatusId",
                table: "Components",
                column: "InternalStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Components_MetricsStatusId",
                table: "Components",
                column: "MetricsStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Components_ParentId",
                table: "Components",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_Components_SystemName",
                table: "Components",
                column: "SystemName");

            migrationBuilder.CreateIndex(
                name: "IX_Components_UnitTestsStatusId",
                table: "Components",
                column: "UnitTestsStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_ComponentTypes_SystemName",
                table: "ComponentTypes",
                column: "SystemName");

            migrationBuilder.CreateIndex(
                name: "IX_DefectChanges_DefectId",
                table: "DefectChanges",
                column: "DefectId");

            migrationBuilder.CreateIndex(
                name: "IX_DefectChanges_UserId",
                table: "DefectChanges",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Defects_EventTypeId",
                table: "Defects",
                column: "EventTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Defects_LastChangeId",
                table: "Defects",
                column: "LastChangeId");

            migrationBuilder.CreateIndex(
                name: "IX_Defects_ResponsibleUserId",
                table: "Defects",
                column: "ResponsibleUserId");

            migrationBuilder.CreateIndex(
                name: "IX_EventParameters_EventId",
                table: "EventParameters",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountBased",
                table: "Events",
                columns: new[] { "Category", "ActualDate", "StartDate", "Id" });

            migrationBuilder.CreateIndex(
                name: "IX_Events_EventTypeId",
                table: "Events",
                column: "EventTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Events_LastStatusEventId",
                table: "Events",
                column: "LastStatusEventId");

            migrationBuilder.CreateIndex(
                name: "IX_ForJoin",
                table: "Events",
                columns: new[] { "OwnerId", "EventTypeId", "Importance", "ActualDate" });

            migrationBuilder.CreateIndex(
                name: "IX_ForProcessing",
                table: "Events",
                columns: new[] { "IsUserHandled", "EventTypeId", "VersionLong" });

            migrationBuilder.CreateIndex(
                name: "IX_OwnerBased",
                table: "Events",
                columns: new[] { "OwnerId", "Category", "ActualDate", "StartDate" });

            migrationBuilder.CreateIndex(
                name: "IX_EventStatuses_EventId",
                table: "EventStatuses",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_EventStatuses_StatusId",
                table: "EventStatuses",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_EventTypes_DefectId",
                table: "EventTypes",
                column: "DefectId");

            migrationBuilder.CreateIndex(
                name: "IX_EventTypes_SystemName",
                table: "EventTypes",
                column: "SystemName");

            migrationBuilder.CreateIndex(
                name: "IX_HttpRequestUnitTestRuleDatas_RuleId",
                table: "HttpRequestUnitTestRuleDatas",
                column: "RuleId");

            migrationBuilder.CreateIndex(
                name: "IX_HttpRequestUnitTestRules_HttpRequestUnitTestId",
                table: "HttpRequestUnitTestRules",
                column: "HttpRequestUnitTestId");

            migrationBuilder.CreateIndex(
                name: "IX_LastComponentNotifications_ComponentId",
                table: "LastComponentNotifications",
                column: "ComponentId");

            migrationBuilder.CreateIndex(
                name: "IX_LastComponentNotifications_EventId",
                table: "LastComponentNotifications",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_LastComponentNotifications_NotificationId",
                table: "LastComponentNotifications",
                column: "NotificationId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountId",
                table: "LimitDatas",
                columns: new[] { "Type", "BeginDate" });

            migrationBuilder.CreateIndex(
                name: "IX_LimitDatasForUnitTests_LimitDataId",
                table: "LimitDatasForUnitTests",
                column: "LimitDataId");

            migrationBuilder.CreateIndex(
                name: "IX_LimitDatasForUnitTests_UnitTestId",
                table: "LimitDatasForUnitTests",
                column: "UnitTestId");

            migrationBuilder.CreateIndex(
                name: "IX_LogParameters_LogId",
                table: "LogParameters",
                column: "LogId");

            migrationBuilder.CreateIndex(
                name: "IX_ComponentBased",
                table: "Logs",
                columns: new[] { "ComponentId", "Date", "Order", "Level", "Context" });

            migrationBuilder.CreateIndex(
                name: "IX_Date",
                table: "Logs",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "IX_ForHistory",
                table: "MetricHistory",
                columns: new[] { "ComponentId", "MetricTypeId", "BeginDate" });

            migrationBuilder.CreateIndex(
                name: "IX_MetricHistory_BeginDate",
                table: "MetricHistory",
                column: "BeginDate");

            migrationBuilder.CreateIndex(
                name: "IX_MetricHistory_MetricTypeId",
                table: "MetricHistory",
                column: "MetricTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_MetricHistory_StatusEventId",
                table: "MetricHistory",
                column: "StatusEventId");

            migrationBuilder.CreateIndex(
                name: "IX_Metrics_ComponentId",
                table: "Metrics",
                column: "ComponentId");

            migrationBuilder.CreateIndex(
                name: "IX_Metrics_MetricTypeId",
                table: "Metrics",
                column: "MetricTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Metrics_StatusDataId",
                table: "Metrics",
                column: "StatusDataId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_EventId",
                table: "Notifications",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_SendEmailCommandId",
                table: "Notifications",
                column: "SendEmailCommandId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_SendMessageCommandId",
                table: "Notifications",
                column: "SendMessageCommandId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_Status",
                table: "Notifications",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_SubscriptionId",
                table: "Notifications",
                column: "SubscriptionId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserId",
                table: "Notifications",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ForSend",
                table: "SendEmailCommand",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_ForSend1",
                table: "SendMessageCommand",
                columns: new[] { "Channel", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_ForSend2",
                table: "SendSmsCommand",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_ComponentId",
                table: "Subscriptions",
                column: "ComponentId");

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_ComponentTypeId",
                table: "Subscriptions",
                column: "ComponentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_UserId",
                table: "Subscriptions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Tokens_UserId",
                table: "Tokens",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UnitTestProperties_UnitTestId",
                table: "UnitTestProperties",
                column: "UnitTestId");

            migrationBuilder.CreateIndex(
                name: "IX_UnitTests_ComponentId",
                table: "UnitTests",
                column: "ComponentId");

            migrationBuilder.CreateIndex(
                name: "IX_UnitTests_StatusDataId",
                table: "UnitTests",
                column: "StatusDataId");

            migrationBuilder.CreateIndex(
                name: "IX_UnitTests_TypeId",
                table: "UnitTests",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_UnitTestTypes_SystemName",
                table: "UnitTestTypes",
                column: "SystemName");

            migrationBuilder.CreateIndex(
                name: "IX_UserContacts_UserId",
                table: "UserContacts",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_RoleId",
                table: "UserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_UserId",
                table: "UserRoles",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Login",
                table: "Users",
                column: "Login");

            migrationBuilder.CreateIndex(
                name: "IX_UserSettings_UserId",
                table: "UserSettings",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Components_Bulbs_ChildComponentsStatusId",
                table: "Components",
                column: "ChildComponentsStatusId",
                principalTable: "Bulbs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Components_Bulbs_EventsStatusId",
                table: "Components",
                column: "EventsStatusId",
                principalTable: "Bulbs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Components_Bulbs_ExternalStatusId",
                table: "Components",
                column: "ExternalStatusId",
                principalTable: "Bulbs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Components_Bulbs_InternalStatusId",
                table: "Components",
                column: "InternalStatusId",
                principalTable: "Bulbs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Components_Bulbs_MetricsStatusId",
                table: "Components",
                column: "MetricsStatusId",
                principalTable: "Bulbs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Components_Bulbs_UnitTestsStatusId",
                table: "Components",
                column: "UnitTestsStatusId",
                principalTable: "Bulbs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Metrics_Bulbs_StatusDataId",
                table: "Metrics",
                column: "StatusDataId",
                principalTable: "Bulbs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UnitTests_Bulbs_StatusDataId",
                table: "UnitTests",
                column: "StatusDataId",
                principalTable: "Bulbs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LastComponentNotifications_Events_EventId",
                table: "LastComponentNotifications",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LastComponentNotifications_Notifications_NotificationId",
                table: "LastComponentNotifications",
                column: "NotificationId",
                principalTable: "Notifications",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MetricHistory_Events_StatusEventId",
                table: "MetricHistory",
                column: "StatusEventId",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Defects_DefectChanges_LastChangeId",
                table: "Defects",
                column: "LastChangeId",
                principalTable: "DefectChanges",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Defects_EventTypes_EventTypeId",
                table: "Defects",
                column: "EventTypeId",
                principalTable: "EventTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bulbs_Components_ComponentId",
                table: "Bulbs");

            migrationBuilder.DropForeignKey(
                name: "FK_Metrics_Components_ComponentId",
                table: "Metrics");

            migrationBuilder.DropForeignKey(
                name: "FK_UnitTests_Components_ComponentId",
                table: "UnitTests");

            migrationBuilder.DropForeignKey(
                name: "FK_Bulbs_Metrics_MetricId",
                table: "Bulbs");

            migrationBuilder.DropForeignKey(
                name: "FK_Bulbs_UnitTests_UnitTestId",
                table: "Bulbs");

            migrationBuilder.DropForeignKey(
                name: "FK_DefectChanges_Defects_DefectId",
                table: "DefectChanges");

            migrationBuilder.DropForeignKey(
                name: "FK_EventTypes_Defects_DefectId",
                table: "EventTypes");

            migrationBuilder.DropTable(
                name: "AccountSettings");

            migrationBuilder.DropTable(
                name: "ArchivedStatuses");

            migrationBuilder.DropTable(
                name: "ComponentProperties");

            migrationBuilder.DropTable(
                name: "EventParameters");

            migrationBuilder.DropTable(
                name: "EventStatuses");

            migrationBuilder.DropTable(
                name: "HttpRequestUnitTestRuleDatas");

            migrationBuilder.DropTable(
                name: "LastComponentNotifications");

            migrationBuilder.DropTable(
                name: "LimitDatasForUnitTests");

            migrationBuilder.DropTable(
                name: "LogConfigs");

            migrationBuilder.DropTable(
                name: "LogParameters");

            migrationBuilder.DropTable(
                name: "MetricHistory");

            migrationBuilder.DropTable(
                name: "NotificationsHttp");

            migrationBuilder.DropTable(
                name: "SendSmsCommand");

            migrationBuilder.DropTable(
                name: "TimeZones");

            migrationBuilder.DropTable(
                name: "Tokens");

            migrationBuilder.DropTable(
                name: "UnitTestDomainNamePaymentPeriodRules");

            migrationBuilder.DropTable(
                name: "UnitTestPingRules");

            migrationBuilder.DropTable(
                name: "UnitTestProperties");

            migrationBuilder.DropTable(
                name: "UnitTestSqlRules");

            migrationBuilder.DropTable(
                name: "UnitTestSslCertificateExpirationDateRules");

            migrationBuilder.DropTable(
                name: "UnitTestTcpPortRules");

            migrationBuilder.DropTable(
                name: "UnitTestVirusTotalRules");

            migrationBuilder.DropTable(
                name: "UserContacts");

            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropTable(
                name: "UserSettings");

            migrationBuilder.DropTable(
                name: "HttpRequestUnitTestRules");

            migrationBuilder.DropTable(
                name: "LimitDatas");

            migrationBuilder.DropTable(
                name: "Logs");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "HttpRequestUnitTests");

            migrationBuilder.DropTable(
                name: "Events");

            migrationBuilder.DropTable(
                name: "SendEmailCommand");

            migrationBuilder.DropTable(
                name: "SendMessageCommand");

            migrationBuilder.DropTable(
                name: "Subscriptions");

            migrationBuilder.DropTable(
                name: "Components");

            migrationBuilder.DropTable(
                name: "ComponentTypes");

            migrationBuilder.DropTable(
                name: "Metrics");

            migrationBuilder.DropTable(
                name: "MetricTypes");

            migrationBuilder.DropTable(
                name: "UnitTests");

            migrationBuilder.DropTable(
                name: "Bulbs");

            migrationBuilder.DropTable(
                name: "UnitTestTypes");

            migrationBuilder.DropTable(
                name: "Defects");

            migrationBuilder.DropTable(
                name: "DefectChanges");

            migrationBuilder.DropTable(
                name: "EventTypes");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
