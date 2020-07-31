using System;
using Zidium.Storage;

namespace Zidium.Core.AccountsDb
{
    public interface IDefectService
    {
        Guid CreateDefect(Guid accountId, string title, UserForRead createUser, UserForRead responsibleUser, string notes);

        Guid CreateAndCloseDefectForEventType(Guid accountId, EventTypeForRead eventType, UserForRead user);

        DefectForRead GetOrCreateDefectForEventType(Guid accountId,
            EventTypeForRead eventType,
            UserForRead createUser,
            UserForRead responsibleUser,
            string comment);

        Guid ChangeStatus(Guid accountId,
            DefectForRead defect,
            DefectStatus status,
            UserForRead user, string comment);

        Guid AutoReopen(Guid accountId, DefectForRead defect);

        DefectStatus GetStatus(Guid defectId);

    }
}
