using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.ServiceModel;
using System.ServiceProcess;
using System.Configuration;
using System.Configuration.Install;

namespace Microsoft.ServiceModel.Samples
{

    // Define a service contract.
    [ServiceContract(Namespace = "http://Microsoft.ServiceModel.Samples")]
    public interface ICertificate
    {

        [OperationContract]
        List<X509Certificate2> ListCertificatesInStore(string storeName, StoreLocation storeLocation);

        [OperationContract]
        List<X509Certificate2> ListCertificatesInRemoteStore(string storeName, StoreLocation storeLocation,
            string serverName);

        [OperationContract]
        List<X509Certificate2> ListExpiringCertificatesInStore(string storeName, StoreLocation storeLocation, int days);

        [OperationContract]
        List<X509Certificate2> ListExpiringCertificatesInRemoteStore(string storeName, StoreLocation storeLocation,
            int days, string serverName);

        [OperationContract]
        void PrintCertificateInfo(X509Certificate2 certificate);

        [OperationContract]
        void EnumCertificatesByStoreName(StoreName name, StoreLocation location);

        [OperationContract]
        void EnumCertificates(string name, StoreLocation location);

        [OperationContract]
        bool InstallCertificateLocal(string storeName, StoreLocation storeLocation, X509Certificate2 certificate);

        [OperationContract]
        bool InstallCertificateRemote(string storeName, StoreLocation storeLocation, X509Certificate2 certificate, string serverName);

        [OperationContract]
        bool DeleteCertificate(string certificateName, string storeName, StoreLocation location);

        [OperationContract]
        bool DeleteCertificateRemote(string certificateName, string storeName, StoreLocation location, string serverName);

        [OperationContract]
        bool DeleteCertificateByThumbprint(string certificateName, string thumbprint, string storeName,
            StoreLocation location);

        [OperationContract]
        bool DeleteCertificateByThumbprintRemote(string thumbprint, string storeName, StoreLocation location,
            string serverName);

        [OperationContract]
        bool RemoveCertificateLocal(string storeName, StoreLocation storeLocation, X509Certificate2 certificate);

        [OperationContract]
        X509Certificate2 FindCertificateRemote(string certificateName, string storeName, StoreLocation location,
            string serverName);

        [OperationContract]
        X509Certificate2 FindCertificateByThumbprintRemote(string thumbprint, string storeName, StoreLocation location,
            string serverName);

        [OperationContract]
        List<X509Certificate2> CompareCertificatesInStore(string storeName, StoreLocation storeLocation,
            string serverA, string serverB);

        [OperationContract]
        string GetData(int value);
        /*
                [OperationContract]
                CompositeType GetDataUsingDataContract(CompositeType composite);
                */
        // TODO: Add your service operations here
    }

    /*
        // Use a data contract as illustrated in the sample below to add composite types to service operations.
        [DataContract]
        public class CompositeType
        {
            bool boolValue = true;
            string stringValue = "Hello ";

            [DataMember]
            public bool BoolValue
            {
                get { return boolValue; }
                set { boolValue = value; }
            }

            [DataMember]
            public string StringValue
            {
                get { return stringValue; }
                set { stringValue = value; }
            }
        }
      */
}


