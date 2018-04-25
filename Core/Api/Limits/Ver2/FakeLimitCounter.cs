namespace Zidium.Core.Api.Limits.Ver2
{
    public class FakeLimitCounter : ILimitCounter
    {
        public void Check(long value)
        {
        }

        public void Add(long value)
        {
        }

        public long Value
        {
            get { return 0; }
        }

        public void Refresh()
        {
        }
    }
}
