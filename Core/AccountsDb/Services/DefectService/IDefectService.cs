using System;
using Zidium.Core.AccountsDb.Classes;

namespace Zidium.Core.AccountsDb
{
    public interface IDefectService
    {
        Defect CreateDefect(Guid accountId, string title, User createUser, User responsibleUser, string notes);

        Defect CreateAndCloseDefectForEventType(Guid accountId,  EventType eventType, User user);

        Defect GetOrCreateDefectForEventType(
            Guid accountId,
            EventType eventType, 
            User createUser, 
            User responsibleUser, 
            string comment);

        DefectChange ChangeStatus(
            Guid accountId,
            Defect defect, 
            DefectStatus status, 
            User user, string 
            comment);

        DefectChange AutoReopen(Guid accountId, Defect defect);
    }
}
