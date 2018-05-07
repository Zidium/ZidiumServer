using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Zidium.Core;
using Zidium.Core.AccountsDb;
using Zidium.Core.Api;
using Zidium.Core.Common.Helpers;
using Zidium.UserAccount.Models.Controls;

namespace Zidium.UserAccount.Models.CheckModels
{
    public abstract class UnitTestCommonSettingsModel
    {
        public Guid? Id { get; set; }

        public UnitTest UnitTest { get; set; }

        [Display(Name = "Название")]
        public string CheckName { get; set; }

        public Guid? ComponentId { get; set; }

        [Display(Name = "Компонент")]
        public Component Component { get; set; }

        [Display(Name = "Период")]
        public TimeSpan? Period { get; set; }

        [Display(Name = "Цвет проверки при ошибке")]
        public ColorStatusSelectorValue ErrorColor { get; set; }

        [Display(Name = "Цвет, если нет сигнала")]
        public ColorStatusSelectorValue NoSignalColor { get; set; }

        [Display(Name = "Время актуальности")]
        public TimeSpan? ActualTime { get; set; }

        [Display(Name = "Количество попыток")]
        public int AttempMax { get; set; }

        public string Action { get; set; }

        protected AccountDbContext AccountDbContext
        {
            get { return FullRequestContext.Current.AccountDbContext; }
        }

        protected UserInfo User
        {
            get { return FullRequestContext.Current.CurrentUser; }
        }

        public UnitTest GetUnitTest(Guid id)
        {
            var repository = AccountDbContext.GetUnitTestRepository();
            var result = repository.GetById(id);
            UnitTest = result;
            Id = id;
            return result;
        }

        public void Load(Guid? id, Guid? componentId)
        {
            if (id.HasValue)
            {
                var test = GetUnitTest(id.Value);
                CheckName = test.DisplayName;
                ComponentId = test.ComponentId;
                Period = TimeSpan.FromSeconds(test.PeriodSeconds ?? 0);
                ActualTime = TimeSpanHelper.FromSeconds(test.ActualTimeSecs);
                NoSignalColor = ColorStatusSelectorValue.FromColor(test.NoSignalColor);
                ErrorColor = ColorStatusSelectorValue.FromUnitTestResultStatus(test.ErrorColor);
                if (ErrorColor.NotChecked)
                {
                    ErrorColor.RedChecked = true;
                }
                AttempMax = test.AttempMax;
            }
            else
            {
                Period = TimeSpan.FromMinutes(10);
                ActualTime = TimeSpan.FromMinutes(20);
                ComponentId = componentId;
                CheckName = DefaultCheckName;
                ErrorColor = ColorStatusSelectorValue.FromUnitTestResultStatus(UnitTestResult.Alarm);
                NoSignalColor = ColorStatusSelectorValue.FromUnitTestResultStatus(UnitTestResult.Alarm);
                AttempMax = 2;
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
            if (SystemUnitTestTypes.CanEditPeriod(UnitTestTypeId))
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
            // инициализруем свойство UnitTest, иначе если в ValidateCommonSettings будет ошибка, то вьюшка кинет исключение
            if (Id.HasValue)
            {
                GetUnitTest(Id.Value);
            }
            ValidateCommonSettings();
            ValidateRule();
        }

        public void SaveCommonSettings()
        {
            var dispatcher = FullRequestContext.Current.Controller.GetDispatcherClient();

            // получим юнит-тест
            if (Id == null)
            {
                var unitTestResponse = dispatcher.GetOrCreateUnitTest(
                    User.AccountId,
                    new GetOrCreateUnitTestRequestData()
                    {
                        ComponentId = ComponentId,
                        SystemName = "GUID_" + Guid.NewGuid(),
                        DisplayName = CheckName,
                        UnitTestTypeId = UnitTestTypeId
                    });

                unitTestResponse.Check();

                GetUnitTest(unitTestResponse.Data.Id);
            }
            else
            {
                GetUnitTest(Id.Value);
            }

            var isSystem = SystemUnitTestTypes.IsSystem(UnitTest.TypeId);

            // обновим данные юнит-теста
            var updateResponse = dispatcher.UpdateUnitTest(
                User.AccountId,
                new UpdateUnitTestRequestData()
                {
                    ComponentId = ComponentId,
                    DisplayName = CheckName,
                    PeriodSeconds = Period != null ? Period.Value.TotalSeconds : (double?)null,
                    ActualTime = !isSystem ? TimeSpanHelper.GetSeconds(ActualTime) : null,
                    ErrorColor = ErrorColor.GetSelectedUnitTestResultStatuses().FirstOrDefault(),
                    NoSignalColor = !isSystem ? NoSignalColor.GetSelectedColors().FirstOrDefault() : Core.Common.ObjectColor.Gray,
                    UnitTestId = UnitTest.Id,
                    SimpleMode = false,
                    AttempMax = AttempMax
                });

            updateResponse.Check();

            var setNextTime = new SetUnitTestNextTimeRequestData()
            {
                UnitTestId = UnitTest.Id
            };
            dispatcher.SetUnitTestNextTime(User.AccountId, setNextTime).Check();

            GetUnitTest(UnitTest.Id);
        }
    }
}