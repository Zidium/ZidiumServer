namespace Zidium.Storage
{
    public interface ITimeZoneRepository
    {
        void Add(TimeZoneForAdd entity);

        TimeZoneForRead[] GetAll();

        TimeZoneForRead GetOneByOffsetMinutes(int offsetMinutes);
    }
}
