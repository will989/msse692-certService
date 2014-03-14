using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]
namespace Microsoft.ServiceModel.Samples
{
    public partial class CertificateWindowsService : ServiceBase
    {
        public ServiceHost serviceHost = null;

        public CertificateWindowsService()
        {
            // Name the Windows Service
            ServiceName = "WCFCertificateService";
        }

        public static void Main()
        {
            ServiceBase.Run(new CertificateWindowsService());
        }

        // Start the Windows service.
        protected override void OnStart(string[] args)
        {
            if (serviceHost != null)
            {
                serviceHost.Close();
            }

            // Create a ServiceHost for the CertificateService type and 
            // provide the base address.
            serviceHost = new ServiceHost(typeof (CertificateService));

            // Open the ServiceHostBase to create listeners and start 
            // listening for messages.
            serviceHost.Open();
        }

        protected override void OnStop()
        {
            if (serviceHost != null)
            {
                serviceHost.Close();
                serviceHost = null;
            }
        }

    }
}