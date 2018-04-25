namespace Zidium.Core.Api.Limits
{
    public interface ILimitCounter
    {
        void Check(long value);

        void Add(long value);

        long Value { get; }

        void Refresh();
    }
}
