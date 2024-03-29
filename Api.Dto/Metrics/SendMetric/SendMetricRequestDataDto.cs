﻿using System;

namespace Zidium.Api.Dto
{
    public class SendMetricRequestDataDto
    {
        public Guid? ComponentId { get; set; }

        public string Name { get; set; }

        public double? Value { get; set; }

        public double? ActualIntervalSecs { get; set; }
    }
}
