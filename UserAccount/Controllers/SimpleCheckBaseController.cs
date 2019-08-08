using System;
using Zidium.Core;
using Zidium.Core.AccountsDb;
using Zidium.Core.Api;
using Zidium.Core.Common;
using Zidium.Core.Common.Helpers;
using Zidium.UserAccount.Helpers;
using Zidium.UserAccount.Models;

namespace Zidium.UserAccount.Controllers
{
    public abstract class SimpleCheckBaseController<T> : ContextController
        where T : CheckSimpleBaseModel, new()
    {
        protected abstract UnitTest FindSimpleCheck(T model);

        protected abstract string GetOldReplacementPart(UnitTest unitTest);

        protected abstract string GetNewReplacementPart(T model);

        protected abstract string GetUnitTestDisplayName(T model);

        protected abstract void SetUnitTestParams(UnitTest unitTest, T model);

        protected abstract string GetComponentDisplayName(T model);

        protected string GetComponentSystemName(Guid componentId)
        {
            return ComponentHelper.GetDynamicSystemName(componentId);
        }

        protected abstract string GetFolderDisplayName(T model);

        protected abstract string GetFolderSystemName(T model);

        protected abstract string GetTypeDisplayName(T model);

        protected abstract string GetTypeSystemName(T model);

        protected abstract void SetModelParams(T model, UnitTest unitTest);

        protected abstract Guid GetUnitTestTypeId();

        public T LoadSimpleCheck(Guid? id, Guid? componentId)
        {
            UnitTest unitTest = null;

            if (id.HasValue)
            {
                var unitTestRepository = CurrentAccountDbContext.GetUnitTestRepository();
                unitTest = unitTestRepository.GetByIdOrNull(id.Value);
            }

            if (unitTest == null)
            {
                return new T()
                {
                    Id = null,
                    ComponentId = componentId,
                    Period = TimeSpan.FromMinutes(10)
                };
            }

            var model = new T()
            {
                Id = unitTest.Id,
                ComponentId = unitTest.ComponentId,
                Period = unitTest.PeriodSeconds.HasValue ? TimeSpan.FromSeconds(unitTest.PeriodSeconds.Value) : TimeSpan.FromMinutes(10)
            };

            SetModelParams(model, unitTest);

            return model;
        }

        [CanEditAllData]
        public UnitTest SaveSimpleCheck(T model)
        {
            var dispatcher = GetDispatcherClient();
            var unitTestDisplayName = GetUnitTestDisplayName(model);

            if (model.Id.HasValue)
            {
                var unitTest = GetUnitTestById(model.Id.Value);
                var updateData = new UpdateUnitTestRequestData()
                {
                    ComponentId = unitTest.ComponentId,
                    DisplayName = unitTestDisplayName,
                    PeriodSeconds = TimeSpanHelper.GetSeconds(model.Period),
                    UnitTestId = unitTest.Id,
                    ErrorColor = unitTest.ErrorColor,
                    NoSignalColor = ObjectColor.Gray,
                    SystemName = unitTest.SystemName,
                    SimpleMode = true
                };
                dispatcher.UpdateUnitTest(CurrentUser.AccountId, updateData).Check();
            
                // Зем: название компонента решили не менять, чтобы не было неожиданных сюрпризов, 
                // меняешь проверку, а меняется компонент, если нужно изменить название компонента, 
                // пусть пользователь сделает это явно вручную

                // Обновим параметры
                SetUnitTestParams(unitTest, model);
                CurrentAccountDbContext.SaveChanges();

                if (!Request.IsSmartBlocksRequest())
                {
                    this.SetTempMessage(TempMessageType.Success, string.Format("Обновлена проверка <a href='{1}' class='alert-link'>{0}</a>", unitTest.DisplayName, Url.Action("Edit", "Checks", new { id = unitTest.Id })));
                }

                return unitTest;
            }
            else // создание проверки
            {
                // Создаём компонент только если его ещё нет
                ComponentInfo component;
                if (!model.ComponentId.HasValue)
                {
                    // Создадим папку для компонента
                    var componentRepository = CurrentAccountDbContext.GetComponentRepository();
                    var root = componentRepository.GetRoot();

                    var createFolderResponse = dispatcher.GetOrCreateComponent(CurrentUser.AccountId, new GetOrCreateComponentRequestData()
                    {
                        SystemName = GetFolderSystemName(model),
                        DisplayName = GetFolderDisplayName(model),
                        TypeId = SystemComponentTypes.Folder.Id,
                        ParentComponentId = root.Id
                    });

                    if (!createFolderResponse.Success)
                        throw new UserFriendlyException("Ошибка создания папки для проверки: " + createFolderResponse.ErrorMessage);

                    var folder = createFolderResponse.Data.Component;

                    // Создадим тип компонента
                    var createComponentTypeResponse = dispatcher.GetOrCreateComponentType(CurrentUser.AccountId, new GetOrCreateComponentTypeRequestData()
                    {
                        SystemName = GetTypeSystemName(model),
                        DisplayName = GetTypeDisplayName(model)
                    });

                    if (!createComponentTypeResponse.Success)
                        throw new UserFriendlyException("Ошибка создания типа компонента для проверки: " + createComponentTypeResponse.ErrorMessage);

                    var componentType = createComponentTypeResponse.Data;

                    // Создадим компонент
                    var componentId = Guid.NewGuid();
                    var createComponentResponse = dispatcher.GetOrCreateComponent(CurrentUser.AccountId, new GetOrCreateComponentRequestData()
                    {
                        NewId = componentId,
                        SystemName = GetComponentSystemName(componentId),
                        DisplayName = GetComponentDisplayName(model),
                        TypeId = componentType.Id,
                        ParentComponentId = folder.Id
                    });

                    if (!createComponentResponse.Success)
                        throw new UserFriendlyException("Ошибка создания компонента для проверки: " + createComponentResponse.ErrorMessage);

                    component = createComponentResponse.Data.Component;
                }
                else
                {
                    component = dispatcher.GetComponentById(CurrentUser.AccountId, model.ComponentId.Value).Data;
                }

                // Создадим проверку
                var unitTestId = Guid.NewGuid();

                var createUnitTestData = new GetOrCreateUnitTestRequestData()
                {
                    ActualTimeSecs = null,
                    ComponentId = component.Id,
                    DisplayName = unitTestDisplayName,
                    ErrorColor = UnitTestResult.Alarm,
                    NewId = unitTestId,
                    SimpleMode = true,
                    NoSignalColor = ObjectColor.Gray,
                    PeriodSeconds = TimeSpanHelper.GetSeconds(model.Period),
                    SystemName = UnitTestHelper.GetDynamicSystemName(unitTestId),
                    UnitTestTypeId = GetUnitTestTypeId(),
                    AttempMax = 2
                };
                dispatcher.GetOrCreateUnitTest(CurrentUser.AccountId, createUnitTestData).Check();
                var unitTest = GetUnitTestById(unitTestId);
                SetUnitTestParams(unitTest, model);

                CurrentAccountDbContext.SaveChanges();

                if (!Request.IsSmartBlocksRequest())
                {
                    this.SetTempMessage(TempMessageType.Success, string.Format("Добавлена проверка <a href='{1}' class='alert-link'>{0}</a>", unitTest.DisplayName, Url.Action("Edit", "Checks", new { id = unitTest.Id })));
                }
                return unitTest;
            }
        }

        /// <summary>
        /// Для unit-тестов
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="userId"></param>
        protected SimpleCheckBaseController(Guid accountId, Guid userId) : base(accountId, userId) { }

        protected SimpleCheckBaseController() { }
    }
}