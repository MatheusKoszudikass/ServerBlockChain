using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;

namespace ServerBlockChain.Entities
{
    public static class AuthenticateServer
    {
        public static async Task<SslStream> AuthenticateServerAsync(Socket socket, Certificate certificate)
        {
            try
            {
                if (certificate?.X509Certificate == null)
                    throw new ArgumentNullException(nameof(certificate), "Certificate or X509Certificate cannot be null.");

                var networkStream = new NetworkStream(socket);
                var sslStream = new SslStream(networkStream, false);
                
                await sslStream.AuthenticateAsServerAsync(
                    certificate.X509Certificate, false, SslProtocols.Tls12, true);

                return ConfigureSslStream(sslStream);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error when authenticating the server: {ex.Message}");
            }
        }

        private static SslStream ConfigureSslStream(SslStream sslStream)
        {
            sslStream.ReadTimeout = 60000;
            sslStream.WriteTimeout = 60000;
            return sslStream;
        }
    }
}