using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Zidium.Storage.Ef.MsSql.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AccountSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountSettings", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "ComponentTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    SystemName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    IsSystem = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComponentTypes", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "LimitDatas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BeginDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    EventsRequests = table.Column<int>(type: "int", nullable: false),
                    LogSize = table.Column<long>(type: "bigint", nullable: false),
                    UnitTestsRequests = table.Column<int>(type: "int", nullable: false),
                    MetricsRequests = table.Column<int>(type: "int", nullable: false),
                    EventsSize = table.Column<long>(type: "bigint", nullable: false),
                    UnitTestsSize = table.Column<long>(type: "bigint", nullable: false),
                    MetricsSize = table.Column<long>(type: "bigint", nullable: false),
                    SmsCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LimitDatas", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "MetricTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SystemName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ConditionAlarm = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ConditionWarning = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ConditionSuccess = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ConditionElseColor = table.Column<int>(type: "int", nullable: true),
                    NoSignalColor = table.Column<int>(type: "int", nullable: true),
                    ActualTimeSecs = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MetricTypes", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SystemName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DisplayName = table.Column<string>(type: "nchar(50)", fixedLength: true, maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "SendEmailCommand",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    From = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    To = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Subject = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Body = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsHtml = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ErrorMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SendDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReferenceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SendEmailCommand", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "SendMessageCommand",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Channel = table.Column<int>(type: "int", nullable: false),
                    To = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Body = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ErrorMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SendDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReferenceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SendMessageCommand", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "SendSmsCommand",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Body = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ErrorMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SendDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReferenceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ExternalId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SendSmsCommand", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "TimeZones",
                columns: table => new
                {
                    OffsetMinutes = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimeZones", x => x.OffsetMinutes)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "UnitTestTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SystemName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    IsSystem = table.Column<bool>(type: "bit", nullable: false),
                    NoSignalColor = table.Column<int>(type: "int", nullable: true),
                    ActualTimeSecs = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnitTestTypes", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Login = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    MiddleName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DisplayName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Post = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    InArchive = table.Column<bool>(type: "bit", nullable: false),
                    SecurityStamp = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "Tokens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Purpose = table.Column<int>(type: "int", nullable: false),
                    SecurityStamp = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsUsed = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tokens", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
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
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserContacts", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
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
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
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
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSettings", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
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
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    SystemName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ParentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ComponentTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InternalStatusId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExternalStatusId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UnitTestsStatusId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EventsStatusId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MetricsStatusId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChildComponentsStatusId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Version = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Enable = table.Column<bool>(type: "bit", nullable: false),
                    DisableToDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DisableComment = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ParentEnable = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Components", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
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
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ComponentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", maxLength: 8000, nullable: true),
                    DataType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComponentProperties", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
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
                    ComponentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastUpdateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Enabled = table.Column<bool>(type: "bit", nullable: false),
                    IsDebugEnabled = table.Column<bool>(type: "bit", nullable: false),
                    IsTraceEnabled = table.Column<bool>(type: "bit", nullable: false),
                    IsInfoEnabled = table.Column<bool>(type: "bit", nullable: false),
                    IsWarningEnabled = table.Column<bool>(type: "bit", nullable: false),
                    IsErrorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    IsFatalEnabled = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogConfigs", x => x.ComponentId)
                        .Annotation("SqlServer:Clustered", false);
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
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ComponentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Level = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", maxLength: 8000, nullable: true),
                    ParametersCount = table.Column<int>(type: "int", nullable: false),
                    Context = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Logs", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
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
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ComponentTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ComponentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Object = table.Column<int>(type: "int", nullable: false),
                    Channel = table.Column<int>(type: "int", nullable: false),
                    IsEnabled = table.Column<bool>(type: "bit", nullable: false),
                    NotifyBetterStatus = table.Column<bool>(type: "bit", nullable: false),
                    Importance = table.Column<int>(type: "int", nullable: false),
                    DurationMinimumInSeconds = table.Column<int>(type: "int", nullable: true),
                    ResendTimeInSeconds = table.Column<int>(type: "int", nullable: true),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SendOnlyInInterval = table.Column<bool>(type: "bit", nullable: false),
                    SendIntervalFromHour = table.Column<int>(type: "int", nullable: true),
                    SendIntervalFromMinute = table.Column<int>(type: "int", nullable: true),
                    SendIntervalToHour = table.Column<int>(type: "int", nullable: true),
                    SendIntervalToMinute = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subscriptions", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
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
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LogId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DataType = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogParameters", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
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
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ComponentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MetricTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DisableToDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DisableComment = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Enable = table.Column<bool>(type: "bit", nullable: false),
                    ParentEnable = table.Column<bool>(type: "bit", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Value = table.Column<double>(type: "float", nullable: true),
                    BeginDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ActualDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ActualTimeSecs = table.Column<int>(type: "int", nullable: true),
                    NoSignalColor = table.Column<int>(type: "int", nullable: true),
                    ConditionAlarm = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ConditionWarning = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ConditionSuccess = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ConditionElseColor = table.Column<int>(type: "int", nullable: true),
                    StatusDataId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Metrics", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
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
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SystemName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    TypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PeriodSeconds = table.Column<int>(type: "int", nullable: true),
                    ComponentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StatusDataId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NextExecutionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NextStepProcessDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastExecutionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DisableToDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DisableComment = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Enable = table.Column<bool>(type: "bit", nullable: false),
                    ParentEnable = table.Column<bool>(type: "bit", nullable: false),
                    SimpleMode = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ErrorColor = table.Column<int>(type: "int", nullable: true),
                    NoSignalColor = table.Column<int>(type: "int", nullable: true),
                    ActualTimeSecs = table.Column<int>(type: "int", nullable: true),
                    AttempCount = table.Column<int>(type: "int", nullable: false),
                    AttempMax = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnitTests", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
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
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ComponentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UnitTestId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    MetricId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EventCategory = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    PreviousStatus = table.Column<int>(type: "int", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Count = table.Column<int>(type: "int", nullable: false),
                    ActualDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", maxLength: 8000, nullable: true),
                    FirstEventId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastEventId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastChildStatusDataId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    StatusEventId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpTimeStartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpTimeLengthMs = table.Column<long>(type: "bigint", nullable: false),
                    UpTimeSuccessMs = table.Column<long>(type: "bigint", nullable: false),
                    HasSignal = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bulbs", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
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
                    UnitTestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HttpRequestUnitTests", x => x.UnitTestId)
                        .Annotation("SqlServer:Clustered", false);
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
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LimitDataId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UnitTestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ResultsCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LimitDatasForUnitTests", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
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
                    UnitTestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Domain = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AlarmDaysCount = table.Column<int>(type: "int", nullable: false),
                    WarningDaysCount = table.Column<int>(type: "int", nullable: false),
                    LastRunErrorCode = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnitTestDomainNamePaymentPeriodRules", x => x.UnitTestId)
                        .Annotation("SqlServer:Clustered", false);
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
                    UnitTestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Host = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TimeoutMs = table.Column<int>(type: "int", nullable: false),
                    LastRunErrorCode = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnitTestPingRules", x => x.UnitTestId)
                        .Annotation("SqlServer:Clustered", false);
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
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UnitTestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", maxLength: 8000, nullable: true),
                    DataType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnitTestProperties", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
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
                    UnitTestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Provider = table.Column<int>(type: "int", nullable: false),
                    ConnectionString = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    OpenConnectionTimeoutMs = table.Column<int>(type: "int", nullable: false),
                    CommandTimeoutMs = table.Column<int>(type: "int", nullable: false),
                    Query = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnitTestSqlRules", x => x.UnitTestId)
                        .Annotation("SqlServer:Clustered", false);
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
                    UnitTestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Url = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    AlarmDaysCount = table.Column<int>(type: "int", nullable: false),
                    WarningDaysCount = table.Column<int>(type: "int", nullable: false),
                    LastRunErrorCode = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnitTestSslCertificateExpirationDateRules", x => x.UnitTestId)
                        .Annotation("SqlServer:Clustered", false);
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
                    UnitTestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Host = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TimeoutMs = table.Column<int>(type: "int", nullable: false),
                    Port = table.Column<int>(type: "int", nullable: false),
                    Opened = table.Column<bool>(type: "bit", nullable: false),
                    LastRunErrorCode = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnitTestTcpPortRules", x => x.UnitTestId)
                        .Annotation("SqlServer:Clustered", false);
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
                    UnitTestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Url = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    NextStep = table.Column<int>(type: "int", nullable: false),
                    ScanTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ScanId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    LastRunErrorCode = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnitTestVirusTotalRules", x => x.UnitTestId)
                        .Annotation("SqlServer:Clustered", false);
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
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HttpRequestUnitTestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SortNumber = table.Column<int>(type: "int", nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Url = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Method = table.Column<int>(type: "int", nullable: false),
                    Body = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ResponseCode = table.Column<int>(type: "int", nullable: true),
                    MaxResponseSize = table.Column<int>(type: "int", nullable: false),
                    SuccessHtml = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ErrorHtml = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    TimeoutSeconds = table.Column<int>(type: "int", nullable: true),
                    LastRunErrorCode = table.Column<int>(type: "int", nullable: true),
                    LastRunErrorMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastRunTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastRunDurationMs = table.Column<int>(type: "int", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HttpRequestUnitTestRules", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
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
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RuleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Key = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HttpRequestUnitTestRuleDatas", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
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
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ComponentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    EventImportance = table.Column<int>(type: "int", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EventId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NotificationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LastComponentNotifications", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
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
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ComponentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MetricTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BeginDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ActualDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Value = table.Column<double>(type: "float", nullable: true),
                    Color = table.Column<int>(type: "int", nullable: false),
                    StatusEventId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    HasSignal = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MetricHistory", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
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
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Number = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    LastChangeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ResponsibleUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EventTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Defects", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
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
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DefectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DefectChanges", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
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
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Category = table.Column<int>(type: "int", nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    SystemName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    JoinIntervalSeconds = table.Column<int>(type: "int", nullable: true),
                    IsSystem = table.Column<bool>(type: "bit", nullable: false),
                    OldVersion = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ImportanceForOld = table.Column<int>(type: "int", nullable: true),
                    ImportanceForNew = table.Column<int>(type: "int", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DefectId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventTypes", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
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
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OwnerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EventTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", maxLength: 8000, nullable: true),
                    Importance = table.Column<int>(type: "int", nullable: false),
                    PreviousImportance = table.Column<int>(type: "int", nullable: false),
                    Count = table.Column<int>(type: "int", nullable: false),
                    JoinKeyHash = table.Column<long>(type: "bigint", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ActualDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsSpace = table.Column<bool>(type: "bit", nullable: false),
                    LastUpdateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastNotificationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsUserHandled = table.Column<bool>(type: "bit", nullable: false),
                    Version = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    VersionLong = table.Column<long>(type: "bigint", nullable: true),
                    Category = table.Column<int>(type: "int", nullable: false),
                    LastStatusEventId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    FirstReasonEventId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Events", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
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
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EventId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
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
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EventId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DataType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventParameters", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
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
                    EventId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StatusId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
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
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EventId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    SendError = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SendDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SubscriptionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Reason = table.Column<int>(type: "int", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    SendEmailCommandId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SendMessageCommandId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
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
                    NotificationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Json = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationsHttp", x => x.NotificationId)
                        .Annotation("SqlServer:Clustered", false);
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
