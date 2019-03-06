using Asterius.Exceptions;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Asterius
{
    public class Traveler : IDisposable
    {
        // Create a private enum to check states which actives as flags
        [Flags]
        private enum TravelerStates : byte
        {
            None      = 0x0,
            Sending   = 0x1,
            Recving   = 0x2,
            Connected = 0x4,
        }

        private Socket _socket = null;
        private TravelerStates _travelerStates = TravelerStates.None;

        private readonly SocketAsyncEventArgs _socketAsyncEventArgsOfOutter = new SocketAsyncEventArgs();

        private IPEndPoint _ipEndPoint = null;

        /// <summary>
        /// Create a Traveler with accept active which will be active like a server, this constructor should be only invoke with Entrance.
        /// </summary>
        /// <param name="socket"></param>
        public Traveler(Socket socket)
        {
            _ipEndPoint = socket.RemoteEndPoint as IPEndPoint;

            _socket = socket;
            _socket.NoDelay = true;

            _travelerStates |= TravelerStates.Connected;

            _socketAsyncEventArgsOfOutter.Completed += OnCompleted;
            OnDispatch();
        }

        /// <summary>
        /// Create a Traveler with Host:Port, this constructor should be only invoke with Custom.
        /// </summary>
        /// <param name="hostOfIPv4"></param>
        /// <param name="portOfIPv4"></param>
        public Traveler(string hostOfIPv4, int portOfIPv4)
        {
            IPAddress ipAddress = IPAddress.Parse(
                hostOfIPv4
            );
            _ipEndPoint = new IPEndPoint(
                ipAddress,
                portOfIPv4
            );

            // Just like setting of [Entrance] processor
            _socket = new Socket(
                AddressFamily.InterNetwork,
                SocketType.Stream,
                ProtocolType.Tcp
            );
            _socket.NoDelay = true;

            _socketAsyncEventArgsOfOutter.Completed += OnCompleted;
            OnDispatch();
        }

        private void OnConnectAsync()
        {
            // Set Socket [RemoteEndPoint] for connect to.
            _socketAsyncEventArgsOfOutter.RemoteEndPoint = _ipEndPoint;
            bool isAcceptAsyncSuccess = _socket.ConnectAsync(
                _socketAsyncEventArgsOfOutter
            );

            if (isAcceptAsyncSuccess)
            {
                return;
            }

            OnConnectComplete(
                _socketAsyncEventArgsOfOutter
            );
        }

        private void OnConnectComplete(object o)
        {
            if (null == _socket)
            {
                return;
            }

            SocketAsyncEventArgs socketAsyncEventArgs = o as SocketAsyncEventArgs;
            switch (socketAsyncEventArgs.SocketError)
            {
                case SocketError.Success:
                    {
                        socketAsyncEventArgs.RemoteEndPoint = null;
                        _travelerStates |= TravelerStates.Connected;
                        OnDispatch();
                    }
                    break;
                default:
                    throw new SocketCompletedException(
                        $"Error: {socketAsyncEventArgs.SocketError} on connect"
                    );
            }
        }

        private void OnCompleted(object sender, SocketAsyncEventArgs socketAsyncEventArgs)
        {
            switch (socketAsyncEventArgs.LastOperation)
            {
                case SocketAsyncOperation.Connect:
                    {
                        Task.Factory.StartNew(
                            OnConnectComplete,
                            socketAsyncEventArgs
                        );
                    }
                    break;
                case SocketAsyncOperation.Disconnect:
                    break;
                default:
                    throw new SocketCompletedException(
                        $"Error: {socketAsyncEventArgs.LastOperation} received on completed of acceptor"
                    );
            }
        }

        private void OnDispatch()
        {
            if (TravelerStates.Connected != (TravelerStates.Connected & _travelerStates))
            {
                OnConnectAsync();
            }
        }

        public void Dispose()
        {
            _socket.Dispose();
            _socket = null;

            _socketAsyncEventArgsOfOutter.Dispose();
        }
    }
}
