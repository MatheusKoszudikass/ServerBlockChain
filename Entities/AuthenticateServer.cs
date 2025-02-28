using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Threading.Tasks;

namespace ServerBlockChain.Entities
{
    public class AuthenticateServer
    {
        public static async Task<SslStream> AuthenticateServerAsync(Socket socket, Certificate certificate)
        {
            // if(_sslStream != null && _sslStream.IsAuthenticated)return _sslStream;
            try
            {
                if (certificate == null || certificate.X509Certificate == null)
                    throw new ArgumentNullException(nameof(certificate), "Certificate or X509Certificate cannot be null.");

                var networkStream = new NetworkStream(socket);
                var _sslStream = new SslStream(networkStream, false);

                await _sslStream.AuthenticateAsServerAsync(
                    certificate.X509Certificate, false, SslProtocols.Tls12, false);

                return _sslStream;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error when authenticating the server: {ex.Message}");
            }
        }
    }
}