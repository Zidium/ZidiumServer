namespace Zidium.Storage
{
    public class TimeZoneForRead
    {
        public TimeZoneForRead(int offsetMinutes, string name)
        {
            OffsetMinutes = offsetMinutes;
            Name = name;
        }

        public int OffsetMinutes { get; }

        public string Name { get; }
    }
}
