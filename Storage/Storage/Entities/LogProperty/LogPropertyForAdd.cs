﻿using System;
using Zidium.Api.Dto;

namespace Zidium.Storage
{
    public class LogPropertyForAdd
    {
        public Guid Id;

        public Guid LogId;

        public string Name;

        public DataType DataType;

        public string Value;
    }
}
