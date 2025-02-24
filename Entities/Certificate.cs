using System;
using System.Security.Cryptography.X509Certificates;

namespace ServerBlockChain.Entities
{
    public class Certificate
    {
        public X509Certificate2? X509Certificate { get; private set; }
        public string Issuer => X509Certificate?.Issuer ?? string.Empty;
        public string Subject => X509Certificate?.Subject ?? string.Empty;
        public DateTime ValidFrom => X509Certificate?.NotBefore ?? DateTime.MinValue;
        public DateTime ValidTo => X509Certificate?.NotAfter ?? DateTime.MinValue;
        public string Thumbprint => X509Certificate?.Thumbprint ?? string.Empty;
        public string SerialNumber => X509Certificate?.SerialNumber ?? string.Empty;

        public event EventHandler<X509Certificate2>? CertificateChanged;

        public Certificate()
        {
            LoadCertificateFromEnvironment();
        }

        public void LoadCertificateFromEnvironment()
        {
            string path = Environment.GetEnvironmentVariable("CERTIFICATE_PATH") ?? string.Empty;
            string password = Environment.GetEnvironmentVariable("CERTIFICATE_PASSWORD") ?? string.Empty;

            if (string.IsNullOrEmpty(path) || string.IsNullOrEmpty(password))
            {
                throw new InvalidOperationException("Certificate path or password is not set in environment variables.");
            }

            try
            {
                X509Certificate = new X509Certificate2(path, password);
                OnCertificateChanged(X509Certificate);

                Console.WriteLine($"Certificate loaded from {path} with subject {X509Certificate.Subject}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error loading certificate: {ex.Message}");
            }
        }

        public bool IsValid()
        {
            return DateTime.UtcNow >= X509Certificate?.NotBefore && DateTime.UtcNow <= X509Certificate?.NotAfter;
        }

        protected virtual void OnCertificateChanged(X509Certificate2 certificate)
        {
            CertificateChanged?.Invoke(this, certificate);
        }
    }
}