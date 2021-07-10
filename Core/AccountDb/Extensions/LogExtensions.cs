using Zidium.Api.Dto;

namespace Zidium.Core.AccountsDb
{
    public static class LogExtensions
    {
        public static long GetSize(this SendLogRequestDataDto data)
        {
            long size = 0;
            if (data.Message != null)
            {
                size += data.Message.Length * sizeof(char);
            }
            if (data.Context != null)
            {
                size += data.Context.Length * sizeof(char);
            }
            if (data.Properties != null)
            {
                foreach (var property in data.Properties)
                {
                    if (property != null)
                    {
                        size += property.GetSize();
                    }
                }
            }
            return size;
        }
    }
}
