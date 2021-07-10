namespace Zidium.Api.Dto
{
    public class GetOrCreateUnitTestTypeRequestDataDto
    {
        public string SystemName { get; set; }

        public string DisplayName { get; set; }

        public ObjectColor? NoSignalColor;

        public int? ActualTimeSecs;
    }
}
