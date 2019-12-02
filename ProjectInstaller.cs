using System.Collections;
using System.ComponentModel;

namespace Application1
{
	[RunInstaller(true)]
	public partial class ProjectInstaller : System.Configuration.Install.Installer {
		public ProjectInstaller() {
			InitializeComponent();
		}

		protected override void OnBeforeInstall(IDictionary savedState) {
			SetServiceName();
			base.OnBeforeInstall(savedState);
		}

		protected override void OnBeforeUninstall(IDictionary savedState) {
			SetServiceName();
			base.OnBeforeUninstall(savedState);
		}

		private void SetServiceName() {
			if (Context.Parameters.ContainsKey("ServiceName")) {
				serviceInstaller1.ServiceName = Context.Parameters["ServiceName"];
			}
			if (Context.Parameters.ContainsKey("DisplayName")) {
				serviceInstaller1.DisplayName = Context.Parameters["DisplayName"];
			}
			if (Context.Parameters.ContainsKey("Description")) {
				serviceInstaller1.Description = Context.Parameters["Description"];
			}
		}
	}
}
