using System.Net;
using System.Net.Sockets;

namespace Asterius
{
    public class Service
    {
        private Socket _socketAcceptor = null;
        private readonly SocketAsyncEventArgs _socketAsyncEventArgs = new SocketAsyncEventArgs();

        /// <summary>
        /// Implement of server which will wait accept socket from target address on target port.
        /// </summary>
        /// <param name="hostOfIPv4">Host of address, supported IPv4 Only</param>
        /// <param name="portOfIPv4">Port of address, supported IPv4 Only</param>
        public Service(string hostOfIPv4, int portOfIPv4, int maxPendingCount = 10)
        {
            // Convert string to IPAddress
            IPAddress ipAddress = IPAddress.Parse(
                hostOfIPv4
            );
            // Create an IPEndPoint by IPAddress which convert from hostOfIPv4
            IPEndPoint ipEndPoint = new IPEndPoint(
                ipAddress,
                portOfIPv4
            );

            // Create a Socket which will accept IPv4
            _socketAcceptor = new Socket(
                socketType:    SocketType.Stream,
                addressFamily: AddressFamily.InterNetwork,
                protocolType:  ProtocolType.Tcp
            );
            // Set SocketOptionLevel.Socket as following active
            //   - SocketOptionName.ReuseAddress: true
            _socketAcceptor.SetSocketOption(
                SocketOptionLevel.Socket,
                SocketOptionName.ReuseAddress,
                true
            );

            // Bind to IPAddress with constructor's parameters
            _socketAcceptor.Bind(
                ipEndPoint
            );
            _socketAcceptor.Listen(
                maxPendingCount
            );

            OnAcceptAsync();
        }

        /// <summary>
        /// Inovke accept active
        /// </summary>
        private void OnAcceptAsync()
        {
            bool isAcceptAsyncSuccess = _socketAcceptor.AcceptAsync(
                _socketAsyncEventArgs
            );
            if (isAcceptAsyncSuccess)
            {
                return;
            }

            OnAcceptComplete(
                _socketAsyncEventArgs
            );
        }

        private void OnAcceptComplete(SocketAsyncEventArgs socketAsyncEventArgs)
        {

        }
    }
}
