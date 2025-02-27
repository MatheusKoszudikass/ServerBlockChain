using System;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;

namespace ServerBlockChain.Entities
{
    public class Certificate
    {
        public X509Certificate2? X509Certificate;
        public string Issuer => X509Certificate?.Issuer ?? string.Empty;
        public string Subject => X509Certificate?.Subject ?? string.Empty;
        public DateTime ValidFrom => X509Certificate?.NotBefore ?? DateTime.MinValue;
        public DateTime ValidTo => X509Certificate?.NotAfter ?? DateTime.MinValue;
        public string Thumbprint => X509Certificate?.Thumbprint ?? string.Empty;
        public string SerialNumber => X509Certificate?.SerialNumber ?? string.Empty;

        public event EventHandler<X509Certificate2>? CertificateChanged;

        public void LoadCertificateFromEnvironment()
        {
            // if(this.X509Certificate != null) return;
            
            var path = Environment.GetEnvironmentVariable("CERTIFICATE_PATH");
            path = @"/home/koszudikas/certificado/certificado.pfx";
            var password = Environment.GetEnvironmentVariable("CERTIFICATE_PASSWORD");
            password = "88199299";

            if (string.IsNullOrEmpty(path) || string.IsNullOrEmpty(password)) 
                throw new InvalidOperationException("Certificate path or password is not set in environment variables.");
            
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