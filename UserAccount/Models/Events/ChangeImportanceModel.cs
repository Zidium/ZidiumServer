﻿using System;
using Zidium.Api.Dto;

namespace Zidium.UserAccount.Models.Events
{
    public class ChangeImportanceModel
    {
        public Guid EventId { get; set; }

        public Guid EventTypeId { get; set; }

        public EventImportance? Importance { get; set; }

        public string Version { get; set; }
    }
}