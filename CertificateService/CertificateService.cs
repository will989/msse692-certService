using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceModel.Samples;

namespace Microsoft.ServiceModel.Samples
{
    // Implement the ICertificate service contract in a service class.
    public class CertificateService : ICertificate
    {
        //Return a list of certificates

        public List<X509Certificate2> ListCertificatesInStore(string storeName, StoreLocation storeLocation)
        {
            X509Store store = new X509Store(storeName, storeLocation);
            int certCount = 0;
            List<X509Certificate2> certList = new List<X509Certificate2>();

            //OpenExistingOnly so no exception is thrown for missing AddressBook for example
            store.Open(OpenFlags.OpenExistingOnly);

            X509Certificate2Collection collection = (X509Certificate2Collection)store.Certificates;

            System.Diagnostics.Debug.WriteLine("The collection's size is {0}", collection.Count);

            foreach (X509Certificate2 x509 in collection)
            {
                try
                {
                    certList.Add(x509);
                }
                catch (CryptographicException)
                {
                    Console.WriteLine("CryptographicException caught.");
                }
                finally
                {
                    certCount = certList.Count;
                    System.Diagnostics.Debug.WriteLine("The list's size is {0}", certList.Count);
                    if (certCount != collection.Count)
                    {
                        System.Diagnostics.Debug.WriteLine("THE COUNTS WERE DIFFERENT {0}, {1}", certCount,
                            collection.Count);
                    }
                }
            }
            store.Close();
            return certList;
        }

        //Return a list of certificates from remote machine

        public List<X509Certificate2> ListCertificatesInRemoteStore(string storeName, StoreLocation storeLocation,
            string serverName)
        {
            log.Debug("In ListCertificatesInRemoteStore");
            string newStoreName = null;
            //make sure we aren't connecting to localhost, and got a good servername
            if (!serverName.ToUpper().Equals("LOCALHOST") && serverName.Length > 3)
            {
                log.Debug("in !serverName.ToUpper().Equals(localhost");
                // trying to concatenate the server name and store name for remote connection:
                newStoreName = string.Format(@"\\{0}\{1}", serverName, storeName);
            }
            else
            {
                log.Debug("In the else for some reason - we didn't get a good server name");
                //we didn't get a good server name - use local host for now
                newStoreName = string.Format("{0}", storeName);
            }
            var store = new X509Store(newStoreName, storeLocation);


            int certCount = 0;

            var certList = new List<X509Certificate2>();
            try
            {
                //OpenExistingOnly so no exception is thrown for missing AddressBook for example
                store.Open(OpenFlags.OpenExistingOnly);
            }
            catch (Exception ex)
            {
                //log.Error("Caught exception in ListCertificatesInRemoteStore: ", ex);
                throw new Exception(string.Format("Cannot connect to remote machine: {0}", serverName));

            }

            var collection = (X509Certificate2Collection)store.Certificates;

            System.Diagnostics.Debug.WriteLine("The collection's size is {0}", collection.Count);

            foreach (X509Certificate2 x509 in collection)
            {
                try
                {
                    certList.Add(x509);
                }
                catch (CryptographicException)
                {
                    Console.WriteLine("CryptographicException caught.");
                }
                finally
                {
                    certCount = certList.Count;
                    System.Diagnostics.Debug.WriteLine("The list's size is {0}", certList.Count);
                    if (certCount != collection.Count)
                    {
                        System.Diagnostics.Debug.WriteLine("THE COUNTS WERE DIFFERENT {0}, {1}", certCount,
                            collection.Count);
                    }
                }
            }
            store.Close();
            return certList;
        }



        public List<X509Certificate2> ListExpiringCertificatesInStore(string storeName, StoreLocation storeLocation,
            int days)
        {
            log.Debug("In List ExpiringCertificatesInStore");
            //X509Store store = new X509Store(storeName, storeLocation);
            int certCount = 0;
            List<X509Certificate2> expiringCertList = new List<X509Certificate2>();
            var today = DateTime.Now;

            //OpenExistingOnly so no exception is thrown for missing AddressBook for example
            // store.Open(OpenFlags.OpenExistingOnly);

            List<X509Certificate2> testStoreList = ListCertificatesInStore(storeName, storeLocation);
            //X509Certificate2Collection collection = (X509Certificate2Collection)store.Certificates;
            //System.Diagnostics.Debug.WriteLine("The collection's size is {0}", collection.Count);
            log.Debug("After testStoreList = ListCertificatesInStore");
            foreach (X509Certificate2 x509 in testStoreList)
            {
                log.Debug("In foreach loop checking for expired certificates");
                //we want to add the number of days from today so we know whether the certificate will still be good then
                if (x509.NotAfter <= today.AddDays(days))
                {
                    try
                    {
                        expiringCertList.Add(x509);
                    }
                    catch (CryptographicException ce)
                    {
                        Console.WriteLine("CryptographicException caught.");
                        log.Error("Caught CryptographicException: Unable to list expiring certificats: {0}", ce);
                    }
                    catch (Exception ex)
                    {
                        log.Error("Caught Exception: {0}", ex);
                    }
                    finally
                    {
                        certCount = expiringCertList.Count;
                        System.Diagnostics.Debug.WriteLine("The list's size is {0}", expiringCertList.Count);
                    }
                }
            }
            //store.Close();
            return expiringCertList;
        }


        public List<X509Certificate2> ListExpiringCertificatesInRemoteStore(string storeName,
            StoreLocation storeLocation,
            int days, string serverName)
        {
            log.Debug("In List ExpiringCertificatesInRemoteStore");
            //X509Store store = new X509Store(storeName, storeLocation);
            int certCount = 0;
            List<X509Certificate2> expiringCertList = new List<X509Certificate2>();
            var today = DateTime.Now;

            string newStoreName = null;
            //make sure we aren't connecting to localhost, and got a good servername
            if (!serverName.ToUpper().Equals("LOCALHOST") && serverName.Length > 3)
            {
                // trying to concatenate the server name and store name for remote connection:
                newStoreName = string.Format(@"\\{0}\{1}", serverName, storeName);
            }
            else
            {
                //we didn't get a good server name - use local host for now
                newStoreName = string.Format("{0}", storeName);
            }
            var store = new X509Store(newStoreName, storeLocation);

            List<X509Certificate2> testStoreList = ListCertificatesInStore(storeName, storeLocation);
            //log.Debug("Got testStoreList");
            foreach (X509Certificate2 x509 in testStoreList)
            {
                log.Debug("In foreach loop checking for expired certificates");
                //we want to add the number of days from today so we know whether the certificate will still be good then
                if (x509.NotAfter <= today.AddDays(days))
                {
                    try
                    {
                        expiringCertList.Add(x509);
                    }
                    catch (CryptographicException ce)
                    {
                        Console.WriteLine("CryptographicException caught.");
                        log.Error("Caught CryptographicException: Unable to list expiring certificats: {0}", ce);
                    }
                    catch (Exception ex)
                    {
                        // log.Error("Caught Exception: {0}", ex);
                    }
                    finally
                    {
                        certCount = expiringCertList.Count;
                        System.Diagnostics.Debug.WriteLine("The list's size is {0}", expiringCertList.Count);
                    }
                }
            }
            //store.Close();
            return expiringCertList;
        }


        public void PrintCertificateInfo(X509Certificate2 certificate)
        {
            System.Diagnostics.Debug.WriteLine("Name: {0}", certificate.FriendlyName);
            System.Diagnostics.Debug.WriteLine("Issuer: {0}", certificate.IssuerName.Name);
            System.Diagnostics.Debug.WriteLine("Subject: {0}", certificate.SubjectName.Name);
            System.Diagnostics.Debug.WriteLine("Version: {0}", certificate.Version);
            System.Diagnostics.Debug.WriteLine("Valid from: {0}", certificate.NotBefore);
            System.Diagnostics.Debug.WriteLine("Valid until: {0}", certificate.NotAfter);
            System.Diagnostics.Debug.WriteLine("Serial number: {0}", certificate.SerialNumber);
            System.Diagnostics.Debug.WriteLine("Signature Algorithm: {0}", certificate.SignatureAlgorithm.FriendlyName);
            System.Diagnostics.Debug.WriteLine("Thumbprint: {0}", certificate.Thumbprint);
            System.Diagnostics.Debug.WriteLine("");
        }


        public void EnumCertificatesByStoreName(StoreName name, StoreLocation location)
        {
            X509Store store = new X509Store(name, location);
            try
            {
                store.Open(OpenFlags.ReadOnly);
                foreach (X509Certificate2 certificate in store.Certificates)
                {
                    PrintCertificateInfo(certificate);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                store.Close();
            }
        }


        public void EnumCertificates(string name, StoreLocation location)
        {
            X509Store store = new X509Store(name, location);
            try
            {
                store.Open(OpenFlags.ReadOnly);
                foreach (X509Certificate2 certificate in store.Certificates)
                {
                    System.Diagnostics.Debug.WriteLine("Name: {0}", certificate.FriendlyName);
                    System.Diagnostics.Debug.WriteLine("Issuer: {0}", certificate.IssuerName.Name);
                    System.Diagnostics.Debug.WriteLine("Subject: {0}", certificate.SubjectName.Name);
                    System.Diagnostics.Debug.WriteLine("Version: {0}", certificate.Version);
                    System.Diagnostics.Debug.WriteLine("Valid from: {0}", certificate.NotBefore);
                    System.Diagnostics.Debug.WriteLine("Valid until: {0}", certificate.NotAfter);
                    System.Diagnostics.Debug.WriteLine("Serial number: {0}", certificate.SerialNumber);
                    System.Diagnostics.Debug.WriteLine("Signature Algorithm: {0}",
                        certificate.SignatureAlgorithm.FriendlyName);
                    System.Diagnostics.Debug.WriteLine("Thumbprint: {0}", certificate.Thumbprint);
                    System.Diagnostics.Debug.WriteLine("");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                store.Close();
            }
        }


        public bool InstallCertificateLocal(string storeName, StoreLocation storeLocation, X509Certificate2 certificate)
        {
            System.Diagnostics.Debug.WriteLine("StoreName = {0}", storeName);
            System.Diagnostics.Debug.WriteLine("StoreLocation = {0}", storeLocation.ToString());
            bool added = false;

            if (storeName != null)
            {
                var store = new X509Store(storeName, storeLocation);

                System.Diagnostics.Debug.WriteLine(" now StoreName = {0}", store.Name.ToString());
                System.Diagnostics.Debug.WriteLine("now StoreLocation = {0}", store.Location.ToString());

                try
                {
                    store.Open(OpenFlags.ReadWrite);
                    store.Add(certificate);
                    added = true;
                }
                catch (Exception ex)
                {
                    added = false;
                    System.Diagnostics.Debug.WriteLine(ex);
                    log.Error("Caught exception in InstallCertificateLocal: ", ex);
                }

                finally
                {
                    store.Close();
                }
                return added;
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("StoreName cannot be Null");
                return added;
            }
        }


        public bool InstallCertificateRemote(string storeName, StoreLocation storeLocation, X509Certificate2 certificate,
            string serverName)
        {
            string newStoreName = null;

            //make sure we aren't connecting to localhost, and got a good servername
            if (!serverName.ToUpper().Equals("LOCALHOST") && serverName.Length > 3)
            {
                // trying to concatenate the server name and store name for remote connection:
                newStoreName = string.Format(@"\\{0}\{1}", serverName, storeName);
            }
            else
            {
                //we didn't get a good server name - use local host for now
                newStoreName = string.Format("{0}", storeName);
            }
            var store = new X509Store(newStoreName, storeLocation);

            System.Diagnostics.Debug.WriteLine("newStoreName = {0}", newStoreName);
            System.Diagnostics.Debug.WriteLine("StoreLocation = {0}", storeLocation.ToString());


            bool added = false;
            try
            {
                store.Open(OpenFlags.ReadWrite);
                store.Add(certificate);
                added = true;
            }
            catch (Exception ex)
            {
                added = false;
                System.Diagnostics.Debug.WriteLine(ex);
                log.Error("Unable to add certificate: {0}", ex);
            }

            finally
            {
                store.Close();
            }
            return added;
        }



        public bool DeleteCertificate(string certificateName, string storeName, StoreLocation location)
        {
            bool success = false;

            X509Store store = new X509Store(storeName, location);
            try
            {
                store.Open(OpenFlags.ReadWrite);

                X509Certificate2Collection certificates =
                    store.Certificates.Find(X509FindType.FindBySubjectName, certificateName, true);

                if (certificates != null && certificates.Count > 0)
                {
                    store.RemoveRange(certificates);
                    success = true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                log.Error("Caught exception in DeleteCertificate: ", ex);
            }
            finally
            {
                store.Close();
            }

            return success;
        }


        public bool DeleteCertificateRemote(string certificateName, string storeName, StoreLocation location,
            string serverName)
        {
            bool success = false;
            log.Debug("In DeleteCertificateRemote");
            string newStoreName = null;
            //make sure we aren't connecting to localhost, and got a good servername
            if (!serverName.ToUpper().Equals("LOCALHOST") && serverName.Length > 3)
            {
                // trying to concatenate the server name and store name for remote connection:
                newStoreName = string.Format(@"\\{0}\{1}", serverName, storeName);
            }
            else
            {
                //we didn't get a good server name - use local host for now
                newStoreName = string.Format("{0}", storeName);
            }

            X509Store store = new X509Store(newStoreName, location);
            try
            {
                store.Open(OpenFlags.ReadWrite);

                log.Debug("Getting certificate store contents");
                X509Certificate2Collection certificates =
                    store.Certificates;
                log.Debug("Got certificate store contents");
                {
                    foreach (X509Certificate2 certificate in certificates)
                    {
                        if (certificate.SubjectName.Name != null)
                        {
                            int result = String.Compare(certificate.SubjectName.Name, certificateName, new CultureInfo("en-US"),
                                CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreWidth | CompareOptions.IgnoreCase);

                            System.Diagnostics.Debug.WriteLine("result = {0}", result);

                            if (result != 0)
                            {
                                {
                                    log.Debug("In if (result !=0) so we did not find a match");
                                    System.Diagnostics.Debug.WriteLine("certificate.SubjectName.ToString: {0}", certificate.SubjectName.ToString());
                                    System.Diagnostics.Debug.WriteLine("does not equal");
                                    System.Diagnostics.Debug.WriteLine("This cert subject name..: {0}", certificateName);
                                }
                            }
                            else
                            {
                                store.Remove(certificate);
                                log.Debug("In the else, after store.Remove(certificate)!");
                                success = true;
                                log.Debug("success = true, certificate removed!");
                                break;
                            }
                        }
                        // else
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                store.Close();
            }
            return success;
        }


        public bool DeleteCertificateByThumbprint(string certificateName, string thumbprint, string storeName,
            StoreLocation location)
        {
            bool success = false;
            log.Debug("In DeleteCertificateByThumbprint");
            X509Store store = new X509Store(storeName, location);

            try
            {
                store.Open(OpenFlags.ReadWrite);

                System.Diagnostics.Debug.WriteLine("Calling EnumCertificates...", thumbprint);
                //EnumCertificates(storeName, location);

                log.Debug("Getting certificate store contents");
                X509Certificate2Collection certificates =
                    store.Certificates;
                log.Debug("Got certificate store contents");

                //store.Certificates.Find(X509FindType.FindByThumbprint, thumbprint, true);

                //if (certificates != null && certificates.Count > 0)
                {
                    foreach (X509Certificate2 certificate in certificates)
                    {
                        int result = String.Compare(certificate.Thumbprint, thumbprint, new CultureInfo("en-US"),
                            CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreWidth | CompareOptions.IgnoreCase);

                        System.Diagnostics.Debug.WriteLine("result = {0}", result);

                        if (result != 0)
                        {
                            {
                                log.Debug("In if (result !=0) so we did not find a match that time");
                                System.Diagnostics.Debug.WriteLine("certificate.Thumbprint: {0}", certificate.Thumbprint);
                                System.Diagnostics.Debug.WriteLine("does not equal");
                                System.Diagnostics.Debug.WriteLine("This cert thumbprint..: {0}", thumbprint);
                            }
                        }
                        else
                        {
                            store.Remove(certificate);
                            log.Debug("In the else, after store.Remove(certificate)!");
                            success = true;
                            log.Debug("success = true, certificate removed!");
                            break;
                        }
                        // else
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                store.Close();
            }

            return success;
        }


        public bool DeleteCertificateByThumbprintRemote(string thumbprint, string storeName, StoreLocation location,
            string serverName)
        {
            bool success = false;
            log.Debug("In DeleteCertificateByThumbprintRemote");
            string newStoreName = null;
            //make sure we aren't connecting to localhost, and got a good servername
            if (!serverName.ToUpper().Equals("LOCALHOST") && serverName.Length > 3)
            {
                // trying to concatenate the server name and store name for remote connection:
                newStoreName = string.Format(@"\\{0}\{1}", serverName, storeName);
            }
            else
            {
                //we didn't get a good server name - use local host for now
                newStoreName = string.Format("{0}", storeName);
            }

            X509Store store = new X509Store(newStoreName, location);
            try
            {
                store.Open(OpenFlags.ReadWrite);

                System.Diagnostics.Debug.WriteLine("Calling EnumCertificates...", thumbprint);
                //EnumCertificates(storeName, location);

                log.Debug("Getting certificate store contents");
                X509Certificate2Collection certificates =
                    store.Certificates;
                log.Debug("Got certificate store contents");

                //store.Certificates.Find(X509FindType.FindByThumbprint, thumbprint, true);

                //if (certificates != null && certificates.Count > 0)
                {
                    foreach (X509Certificate2 certificate in certificates)
                    {
                        int result = String.Compare(certificate.Thumbprint, thumbprint, new CultureInfo("en-US"),
                            CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreWidth | CompareOptions.IgnoreCase);

                        System.Diagnostics.Debug.WriteLine("result = {0}", result);

                        if (result != 0)
                        {
                            {
                                log.Debug("In if (result !=0) so we did not find a match that time");
                                System.Diagnostics.Debug.WriteLine("certificate.Thumbprint: {0}", certificate.Thumbprint);
                                System.Diagnostics.Debug.WriteLine("does not equal");
                                System.Diagnostics.Debug.WriteLine("This cert thumbprint..: {0}", thumbprint);
                            }
                        }
                        else
                        {
                            store.Remove(certificate);
                            log.Debug("In the else, after store.Remove(certificate)!");
                            success = true;
                            log.Debug("success = true, certificate removed!");
                            break;
                        }
                        // else
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                store.Close();
            }
            return success;
        }

        public bool RemoveCertificateLocal(string storeName, StoreLocation storeLocation, X509Certificate2 certificate)
        {
            X509Store store = new X509Store(storeName, storeLocation);
            bool removed = false;
            try
            {
                store.Open(OpenFlags.ReadWrite);
                store.Remove(certificate);
                removed = true;
            }
            catch (Exception ex)
            {
                removed = false;
                System.Diagnostics.Debug.WriteLine(ex);
            }

            finally
            {
                store.Close();
            }

            return removed;
        }

        public string GetData(int value)
        {
            return string.Format("You entered: {0}", value);
        }
        /*
                public CompositeType GetDataUsingDataContract(CompositeType composite)
                {
                    if (composite == null)
                    {
                        throw new ArgumentNullException("composite");
                    }
                    if (composite.BoolValue)
                    {
                        composite.StringValue += "Suffix";
                    }
                    return composite;
                }
        */
        public List<X509Certificate2> CompareCertificatesInStore(string storeName, StoreLocation storeLocation,
            string serverA, string serverB)
        {
            int certCount = 0;

            try
            {
                //get certificates in remote stores for both servers
                log.Debug("right before list certificates in serverA");
                var storeListA = ListCertificatesInRemoteStore(storeName, storeLocation, serverA);
                log.Debug("after serverA check, before serverB");
                var storeListB = ListCertificatesInRemoteStore(storeName, storeLocation, serverB);
                log.Debug("after serverB check");

                //compare the contents of serverA and serverB, storing the differences
                List<X509Certificate2> noMatchingCertListA = storeListA.Except(storeListB).ToList();
                log.Debug("between noMatchingCertListA = storeListA.Except...");
                List<X509Certificate2> noMatchingCertListB = storeListB.Except(storeListA).ToList();
                log.Debug("after noMatchingCertListB = storeListB.Except...");
                //this should be the list of certificates that are on one server but not the other
                var differencesList = noMatchingCertListA.Union(noMatchingCertListB).ToList();
                log.Debug("after populating differencesList");

                return differencesList;
            }
            catch (Exception ex)
            {
                log.Error("Caught exception in CompareCertificatesInStore: {0}", ex);
                throw;
            }
        }

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger
    (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    }
}