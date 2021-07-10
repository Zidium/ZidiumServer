namespace Zidium.Agent
{
    public class WindowsServiceOptions
    {
        public InstallOptions Install { get; set; }

        public class InstallOptions
        {
            public string ServiceName { get; set; }

            public string ServiceDescription { get; set; }
        }

    }
}
