using System;
using Microsoft.AspNetCore.Mvc;
using Zidium.Api.Dto;

namespace Zidium.UserAccount.Helpers
{
    /// <summary>
    /// Строит Url для цветных кружков событий, проверок, метрик и детей.
    /// Если в кружке 1 элемент = сразу показывает страницу элемента.
    /// Иначе показывает страницу со списком элементов.
    /// </summary>
    public static class ColorLinkHelper
    {
        public static string GetUnitTestsUrl(IUrlHelper urlHelper, Guid componentId, ObjectColor color, Guid[] unittestIds)
        {
            if (unittestIds.Length == 1)
            {
                return LinkHelper.GenerateUrl(urlHelper, "ResultDetails", "UnitTests", new { id = unittestIds[0] });
            }
            return LinkHelper.GenerateUrl(urlHelper, "Index", "UnitTests", new { ComponentId = componentId, Color = color });
        }

        public static string GetEventsUrl(
            IUrlHelper urlHelper,
            Guid componentId,
            DateTime fromTime,
            ObjectColor color,
            Guid[] eventIds)
        {
            if (eventIds.Length == 1)
            {
                return LinkHelper.GenerateUrl(urlHelper, "Show", "Events", new { id = eventIds[0] });
            }
            var time = GuiHelper.GetUrlDateTimeString(fromTime);
            return LinkHelper.GenerateUrl(urlHelper, "Index", "Events", new { ComponentId = componentId, Color = color, FromDate = time });
        }

        public static string GetMetricsUrl(IUrlHelper urlHelper, Guid componentId, ObjectColor color, Guid[] metricIds)
        {
            if (metricIds.Length == 1)
            {
                return LinkHelper.GenerateUrl(urlHelper, "Show", "Metrics", new { id = metricIds[0] });
            }
            return LinkHelper.GenerateUrl(urlHelper, "Values", "Metrics", new { componentId = componentId, color = color });
        }

        public static string GetChildsUrl(IUrlHelper urlHelper, Guid componentId, ObjectColor color, Guid[] childIds)
        {
            if (childIds.Length == 1)
            {
                return LinkHelper.GenerateUrl(urlHelper, "Show", "Components", new { id = childIds[0] });
            }
            return LinkHelper.GenerateUrl(urlHelper, "List", "Components", new { parentComponentId = componentId, Color = color });
        }
    }
}