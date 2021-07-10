using System;
using Zidium.Api.Dto;

namespace Zidium.Storage
{
    public interface IGuiRepository
    {
        GetGuiChecksResultsInfo[] GetChecksResults();

        GetGuiComponentHistoryInfo[] GetComponentHistory();

        GetGuiComponentListInfo[] GetComponentList(
            Guid? componentTypeId,
            Guid[] excludeComponentTypeIds,
            Guid? parentComponentId,
            MonitoringStatus[] statuses,
            string search, 
            int maxCount);


        GetGuiComponentShowInfo GetComponentShow(Guid id, DateTime now);

        GetGuiComponentMiniListInfo[] GetComponentMiniList();

        GetGuiComponentStatesInfo[] GetComponentStates(
            Guid? componentTypeId,
            Guid? componentId,
            Guid? parentId,
            Guid[] excludeTypes,
            MonitoringStatus[] statuses,
            string searchString, 
            DateTime now);

        GetGuiTimelinesUnitTestsInfo[] GetTimelinesUnitTests(
            Guid componentId,
            EventImportance[] importances,
            DateTime fromDate, 
            DateTime toDate
            );

        GetGuiTimelinesMetricsInfo[] GetTimelinesMetrics(
            Guid componentId,
            EventImportance[] importances,
            DateTime fromDate,
            DateTime toDate
        );

        GetGuiTimelinesChildsInfo[] GetTimelinesChilds(
            Guid componentId,
            EventImportance[] importances,
            DateTime fromDate,
            DateTime toDate
        );

        GetGuiTimelinesEventsInfo[] GetTimelinesEvents(
            Guid componentId,
            EventImportance[] importances,
            DateTime fromDate,
            DateTime toDate
        );

        GetGuiComponentMiniTreeInfo[] GetComponentMiniTree();

        GetGuiSimplifiedComponentListInfo[] GetSimplifiedComponentList();

        GetGuiDefectsInfo[] GetDefects(string title);

    }
}
