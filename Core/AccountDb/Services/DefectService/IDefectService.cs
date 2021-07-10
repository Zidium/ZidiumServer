using System;
using Zidium.Storage;

namespace Zidium.Core.AccountsDb
{
    public interface IDefectService
    {
        Guid CreateDefect(string title, UserForRead createUser, UserForRead responsibleUser, string notes);

        Guid CreateAndCloseDefectForEventType(EventTypeForRead eventType, UserForRead user);

        DefectForRead GetOrCreateDefectForEventType(EventTypeForRead eventType,
            UserForRead createUser,
            UserForRead responsibleUser,
            string comment);

        Guid ChangeStatus(DefectForRead defect,
            DefectStatus status,
            UserForRead user,
            string comment);

        Guid AutoReopen(DefectForRead defect);

        DefectStatus GetStatus(Guid defectId);

    }
}
