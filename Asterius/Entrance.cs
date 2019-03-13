using Asterius.Base;
using Asterius.Exceptions;
using Microsoft.IO;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Asterius
{
    public class Entrance : IDisposable
    {
        private Socket _socket = null;
        private readonly SocketAsyncEventArgs _socketAsyncEventArgs = new SocketAsyncEventArgs();
        private Action<Route> _actionOfAcceptorCallback = null;
        private RecyclableMemoryStreamManager _recyclableMemoryStreamManager = new RecyclableMemoryStreamManager();

        /// <summary>
        /// Implement of service which will wait accept socket from Host:Port.
        /// </summary>
        /// <param name="hostOfIPv4">Host of address, supported IPv4 Only</param>
        /// <param name="portOfIPv4">Port of address, supported IPv4 Only</param>
        public Entrance(string hostOfIPv4, int portOfIPv4, Action<Route> actionOfAcceptorCallback, int maxPendingCount = 10)
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
            _socket = new Socket(
                socketType: SocketType.Stream,
                addressFamily: AddressFamily.InterNetwork,
                protocolType: ProtocolType.Tcp
            );
            // Set SocketOptionLevel.Socket as following active
            //   - SocketOptionName.ReuseAddress: true
            _socket.SetSocketOption(
                SocketOptionLevel.Socket,
                SocketOptionName.ReuseAddress,
                true
            );

            // Bind to IPAddress with constructor's parameters
            _socket.Bind(
                ipEndPoint
            );
            _socket.Listen(
                maxPendingCount
            );

            // Add a processor callback to [SocketAsyncEventArgs.Completed] which will callback on accept a Traveler enter
            _socketAsyncEventArgs.Completed += OnCompleted;
            // Add a processor callback to Action<Traveler> from outside which will invoked on [SocketAsyncEventArgs.Completed] logic.
            _actionOfAcceptorCallback += actionOfAcceptorCallback;
            OnAcceptAsync();
        }

        public Traveler Create(string hostOfIPv4, int portOfIPv4)
        {
            IPAddress ipAddress = IPAddress.Parse(
                hostOfIPv4
            );
            IPEndPoint ipEndPoint =new IPEndPoint(
                ipAddress,
                portOfIPv4
            );

            Route route = new Route(
                ipEndPoint,
                _recyclableMemoryStreamManager
            );

            // Record [Route] for manager
            // Create [Traveler] for manager
            return null;
        }

        /// <summary>
        /// Inovke accept active
        /// </summary>
        private void OnAcceptAsync()
        {
            // Reset Socket [SocketAsyncEventArgs] for duplicate use it.
            _socketAsyncEventArgs.AcceptSocket = null;
            bool isAcceptAsyncSuccess = _socket.AcceptAsync(
                _socketAsyncEventArgs
            );
            // If [Socket.AcceptAsync] invoke with parameter [SocketAsyncEventArgs], it will send back a [Boolean] to feedback result of active
            // True : Means that there not exist any Accept active to invoke
            // False: Means that there exsit some accept active need to process right now
            if (isAcceptAsyncSuccess)
            {
                return;
            }

            OnAcceptComplete(
                _socketAsyncEventArgs
            );
        }

        /// <summary>
        /// Inovke accept active when need complete process
        /// </summary>
        /// <param name="o">Object that should be [SocketAsyncEventArgs]</param>
        private void OnAcceptComplete(object o)
        {
            // If [Socket] was null, it means that the service was Disposed
            if (null == _socket)
            {
                return;
            }

            SocketAsyncEventArgs socketAsyncEventArgs = o as SocketAsyncEventArgs;
            switch (socketAsyncEventArgs.SocketError)
            {
                case SocketError.Success:
                    {
                        Route route = new Route(
                            socketAsyncEventArgs.AcceptSocket,
                            _recyclableMemoryStreamManager
                        );

                        // Record [Traveler] for manager

                        try
                        {
                            _actionOfAcceptorCallback.Invoke(
                                route
                            );
                        }
                        catch (Exception exception)
                        {
                            Clew.Error(
                                exception
                            );
                        }

                        // If [Socket] wa sset as null, it means that the service was Disposed
                        if (null == _socket)
                        {
                            return;
                        }

                        OnAcceptAsync();
                    }
                    break;
                default:
                    {
                        Clew.Error(
                            $"Accept error: {socketAsyncEventArgs.SocketError} happened."
                        );
                    }
                    break;
            }
        }

        private void OnCompleted(object sender, SocketAsyncEventArgs socketAsyncEventArgs)
        {
            switch (socketAsyncEventArgs.LastOperation)
            {
                case SocketAsyncOperation.Accept:
                    {
                        Task.Factory.StartNew(
                            OnAcceptComplete,
                            socketAsyncEventArgs
                        );
                    }
                    break;
                default:
                    throw new SocketCompletedException(
                        $"Error: {socketAsyncEventArgs.LastOperation} received on completed of acceptor"
                    );
            }
        }

        public void Dispose()
        {
            _socket?.Close();
            _socket = null;

            _socketAsyncEventArgs.Dispose();
        }
    }
}
