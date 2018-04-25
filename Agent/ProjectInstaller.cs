namespace Zidium.Agent
{
	using System;
	using System.Collections;
	using System.Configuration.Install;
	using System.Reflection;
	using System.ServiceProcess;

	public class ProjectInstaller : IDisposable
	{
		private readonly Installer installer;

		private readonly Assembly appAssembly;

		public ProjectInstaller(String serviceName, String description, Assembly appAssembly)
		{
			installer = new Installer();
			installer.AfterInstall += InstallerAfterInstall;
			this.appAssembly = appAssembly;

			CreateSubInstallers(serviceName, description);
			SetInstallerContext();
		}

		public void Install()
		{
			var state = new Hashtable();

			try
			{
				installer.Install(state);
				installer.Commit(state);
			}
			catch
			{
				try
				{
					this.installer.Rollback(state);
				}
				catch
				{
				}

				throw;
			}
		}

		public void Uninstall()
		{
			installer.Uninstall(null);
		}

		public void Dispose()
		{
			installer.Dispose();
		}

		private void CreateSubInstallers(String serviceName, String description)
		{
			installer.Installers.Add(new ServiceProcessInstaller { Account = ServiceAccount.LocalSystem });

			installer.Installers.Add(
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
			installer.Context = new InstallContext();
			installer.Context.Parameters["assemblypath"] = this.appAssembly.Location;
		}

		private void InstallerAfterInstall(Object sender, InstallEventArgs e)
		{
			var sc = new ServiceController(ServiceConfiguration.ServiceName);
			sc.Start();
		}
	}
}