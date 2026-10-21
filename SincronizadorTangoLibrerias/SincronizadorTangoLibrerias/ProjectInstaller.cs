using System.Collections;
using System.ComponentModel;

namespace Sincronizador
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();
        }

        public override void Install(IDictionary stateSaver)
        {
            RetrieveServiceName();
            base.Install(stateSaver);
        }

        public override void Uninstall(IDictionary savedState)
        {
            RetrieveServiceName();
            base.Uninstall(savedState); 
        }

        private void RetrieveServiceName() {
            var nombre = Context.Parameters["servicename"];
            if (!string.IsNullOrEmpty(nombre))
            {
                this.serviceInstaller1.ServiceName = nombre;
                this.serviceInstaller1.DisplayName = nombre;
            }

        }
    }


}
