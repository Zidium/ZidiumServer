﻿using System;

namespace Zidium.Storage
{
    public class GetGuiDefectsInfo
    {
        public Guid Id;

        public Guid? EventTypeId;

        public LastChangeInfo LastChange;

        public Guid? ResponsibleUserId;

        public int Number;

        public string Title;

        public ResponsibleUserInfo ResponsibleUser;

        public EventTypeInfo EventType;

        public class LastChangeInfo
        {
            public DateTime Date;

            public DefectStatus Status;

            public string Comment;
        }

        public class ResponsibleUserInfo
        {
            public string NameOrLogin;
        }

        public class EventTypeInfo
        {
            public string OldVersion;

            public string Code;
        }
    }
}
