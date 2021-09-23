using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Zidium.Api.Dto;
using Zidium.Common;
using Zidium.Core;
using Zidium.Core.AccountsDb;
using Zidium.Core.Api;
using Zidium.Core.Common.Helpers;
using Zidium.Storage;
using Zidium.UserAccount.Models.Controls;

namespace Zidium.UserAccount.Models.CheckModels
{
    public abstract class UnitTestCommonSettingsModel
    {
        public Guid? Id { get; set; }

        [Display(Name = "Название")]
        public string CheckName { get; set; }

        public Guid? ComponentId { get; set; }

        [Display(Name = "Компонент")]
        public ComponentForRead Component { get; set; }

        [Display(Name = "Период")]
        public TimeSpan? Period { get; set; }

        [Display(Name = "Цвет проверки при ошибке")]
        public ColorStatusSelectorValue ErrorColor { get; set; }

        [Display(Name = "Цвет, если нет сигнала")]
        public ColorStatusSelectorValue NoSignalColor { get; set; }

        [Display(Name = "Время актуальности")]
        public TimeSpan? ActualTime { get; set; }

        public string Action { get; set; }

        public bool Enable;

        public void Load(Guid? id, Guid? componentId, IStorage storage)
        {
            if (id.HasValue)
            {
                Id = id;
                var test = storage.UnitTests.GetOneById(id.Value);
                CheckName = test.DisplayName;
                ComponentId = test.ComponentId;
                Enable = test.Enable;
                Period = TimeSpan.FromSeconds(test.PeriodSeconds ?? 0);
                ActualTime = TimeSpanHelper.FromSeconds(test.ActualTimeSecs);
                NoSignalColor = ColorStatusSelectorValue.FromColor(test.NoSignalColor);
                ErrorColor = ColorStatusSelectorValue.FromUnitTestResultStatus(test.ErrorColor);
                if (ErrorColor.NotChecked)
                {
                    ErrorColor.RedChecked = true;
                }
            }
            else
            {
                Period = TimeSpan.FromMinutes(10);
                ActualTime = TimeSpan.FromMinutes(20);
                ComponentId = componentId;
                CheckName = DefaultCheckName;
                ErrorColor = ColorStatusSelectorValue.FromUnitTestResultStatus(UnitTestResult.Alarm);
                NoSignalColor = ColorStatusSelectorValue.FromUnitTestResultStatus(UnitTestResult.Alarm);
                Enable = true;
            }
        }

        public abstract string ComponentLabelText { get; }

        public abstract string DefaultCheckName { get; }

        public abstract UserFolder NewComponentFolder { get; }

        public abstract Guid NewComponentTypeId { get; }

        public abstract Guid UnitTestTypeId { get; }

        protected void ValidateCommonSettings()
        {
            // проверим данные
            if (string.IsNullOrEmpty(CheckName))
            {
                throw new UserFriendlyException("Укажите название проверки");
            }
            if (SystemUnitTestType.CanEditPeriod(UnitTestTypeId))
            {
                // например для доменной проверки НЕльзя указывать период
                if (Period == null)
                {
                    throw new UserFriendlyException("Укажите период проверки");
                }
                if (Period.Value.TotalMinutes < 1)
                {
                    throw new UserFriendlyException("Период проверки должен быть >= 1 минуты");
                }
            }
            if (ComponentId == null)
            {
                throw new UserFriendlyException("Укажите название компонента");
            }
        }

        protected abstract void ValidateRule();

        public void Validate()
        {
            ValidateCommonSettings();
            ValidateRule();
        }

        public void SaveCommonSettings()
        {
            var dispatcher = DispatcherHelper.GetDispatcherClient();

            // получим юнит-тест
            if (Id == null)
            {
                var unitTestResponse = dispatcher.GetOrCreateUnitTest(
                    new GetOrCreateUnitTestRequestDataDto()
                    {
                        ComponentId = ComponentId,
                        SystemName = "GUID_" + Ulid.NewUlid(),
                        DisplayName = CheckName,
                        UnitTestTypeId = UnitTestTypeId,
                        AttempMax = 2
                    });

                Id = unitTestResponse.Data.Id;
            }

            var isSystem = SystemUnitTestType.IsSystem(UnitTestTypeId);

            // обновим данные юнит-теста
            var updateResponse = dispatcher.UpdateUnitTest(
                new UpdateUnitTestRequestData()
                {
                    ComponentId = ComponentId,
                    DisplayName = CheckName,
                    PeriodSeconds = Period?.TotalSeconds,
                    ActualTime = !isSystem ? TimeSpanHelper.GetSeconds(ActualTime) : null,
                    ErrorColor = ErrorColor.GetSelectedUnitTestResultStatuses().FirstOrDefault(),
                    NoSignalColor = !isSystem ? NoSignalColor.GetSelectedColors().FirstOrDefault() : ObjectColor.Gray,
                    UnitTestId = Id,
                    SimpleMode = false
                });

            updateResponse.Check();

            var setNextTimeRequestData = new SetUnitTestNextTimeRequestData()
            {
                UnitTestId = Id
            };
            dispatcher.SetUnitTestNextTime(setNextTimeRequestData).Check();
        }
    }
}