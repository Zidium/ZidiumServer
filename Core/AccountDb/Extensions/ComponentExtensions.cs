using System;
using Zidium.Storage;

namespace Zidium.Core.AccountsDb
{
    public static class ComponentExtensions
    {
        /// <summary>
        /// Признак корневого компонента
        /// </summary>
        public static bool IsRoot(this ComponentForRead component)
        {
            return component.ComponentTypeId == SystemComponentType.Root.Id;
        }

        /// <summary>
        /// Признак папки
        /// </summary>
        public static bool IsFolder(this ComponentForRead component)
        {
            return component.ComponentTypeId == SystemComponentType.Folder.Id;
        }

        /// <summary>
        /// Компонент и его родитель включены
        /// </summary>
        public static bool CanProcess(this ComponentForRead component)
        {
            return component.Enable && component.ParentEnable;
        }

        public static Guid[] GetAllStatuses(this ComponentForRead component)
        {
            return new []
            {
                component.InternalStatusId,
                component.ExternalStatusId,
                component.UnitTestsStatusId,
                component.EventsStatusId,
                component.MetricsStatusId,
                component.ChildComponentsStatusId
            };
        }

    }
}
