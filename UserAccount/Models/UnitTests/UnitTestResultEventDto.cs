using System;
using Zidium.Api.Dto;

namespace Zidium.UserAccount.Models.UnitTests
{
    public class UnitTestResultEventDto
    {
        public Guid Id { get; set; }

        public Guid UnitTestId { get; set; }

        public DateTime Date { get; set; }

        public string Message { get; set; }

        public EventImportance Importance { get; set; }
    }
}