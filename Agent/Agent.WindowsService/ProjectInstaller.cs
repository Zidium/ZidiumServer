using System;
using System.Collections;
using System.Configuration.Install;
using System.ServiceProcess;

namespace Zidium.Agent
{
    public class ProjectInstaller : IDisposable
    {
        private readonly Installer _installer;

        private readonly string _filename;

        protected string ServiceName;

        public ProjectInstaller(string serviceName, string description, string fileName)
        {
            ServiceName = serviceName;
            _installer = new Installer();
            _installer.AfterInstall += InstallerAfterInstall;
            _filename = fileName;

            CreateSubInstallers(serviceName, description);
            SetInstallerContext();
        }

        public void Install()
        {
            var state = new Hashtable();

            try
            {
                _installer.Install(state);
                _installer.Commit(state);
            }
            catch
            {
                try
                {
                    _installer.Rollback(state);
                }
                catch
                {
                }

                throw;
            }
        }

        public void Uninstall()
        {
            _installer.Uninstall(null);
        }

        public void Dispose()
        {
            _installer.Dispose();
        }

        private void CreateSubInstallers(string serviceName, string description)
        {
            _installer.Installers.Add(new ServiceProcessInstaller { Account = ServiceAccount.LocalSystem });

            _installer.Installers.Add(
                new ServiceInstaller
                {
                    ServiceName = serviceName,
                    DisplayName = serviceName,
                    Description = description,
                    StartType = ServiceStartMode.Automatic
                });
        }

        private void SetInstallerContext()
        {
            _installer.Context = new InstallContext();
            _installer.Context.Parameters["assemblypath"] = _filename;
        }

        private void InstallerAfterInstall(object sender, InstallEventArgs e)
        {
            var sc = new ServiceController(ServiceName);
            sc.Start();
        }
    }
}