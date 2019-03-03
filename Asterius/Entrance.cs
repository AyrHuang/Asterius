using Asterius.Base;
using Asterius.Exceptions;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Asterius
{
    public class Entrance : IDisposable
    {
        private Socket _socketOfAcceptor = null;
        private readonly SocketAsyncEventArgs _socketAsyncEventArgs = new SocketAsyncEventArgs();
        private Action<Traveler> _actionOfAcceptorCallback = null;

        /// <summary>
        /// Implement of service which will wait accept socket from Host:Port.
        /// </summary>
        /// <param name="hostOfIPv4">Host of address, supported IPv4 Only</param>
        /// <param name="portOfIPv4">Port of address, supported IPv4 Only</param>
        public Entrance(string hostOfIPv4, int portOfIPv4, Action<Traveler> actionOfAcceptorCallback, int maxPendingCount = 10)
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
            _socketOfAcceptor = new Socket(
                socketType:    SocketType.Stream,
                addressFamily: AddressFamily.InterNetwork,
                protocolType:  ProtocolType.Tcp
            );
            // Set SocketOptionLevel.Socket as following active
            //   - SocketOptionName.ReuseAddress: true
            _socketOfAcceptor.SetSocketOption(
                SocketOptionLevel.Socket,
                SocketOptionName.ReuseAddress,
                true
            );

            // Bind to IPAddress with constructor's parameters
            _socketOfAcceptor.Bind(
                ipEndPoint
            );
            _socketOfAcceptor.Listen(
                maxPendingCount
            );

            // Add a processor callback to [SocketAsyncEventArgs.Completed] which will callback on accept a Traveler enter
            _socketAsyncEventArgs.Completed += OnCompleted;
            // Add a processor callback to Action<Traveler> from outside which will invoked on [SocketAsyncEventArgs.Completed] logic.
            _actionOfAcceptorCallback += actionOfAcceptorCallback;
            OnAcceptAsync();
        }

        /// <summary>
        /// Inovke accept active
        /// </summary>
        private void OnAcceptAsync()
        {
            // Reset Socket [SocketAsyncEventArgs] for duplicate use it.
            _socketAsyncEventArgs.AcceptSocket = null;
            bool isAcceptAsyncSuccess = _socketOfAcceptor.AcceptAsync(
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
            if (null == _socketOfAcceptor)
            {
                return;
            }

            SocketAsyncEventArgs socketAsyncEventArgs = o as SocketAsyncEventArgs;
            switch (socketAsyncEventArgs.SocketError)
            {
                case SocketError.Success:
                    {
                        Traveler traveler = new Traveler(
                            socketAsyncEventArgs.AcceptSocket
                        );

                        // Record [Traveler] for manager

                        try
                        {
                            _actionOfAcceptorCallback(
                                traveler
                            );
                        }
                        catch (Exception exception)
                        {
                            Clew.Error(
                                exception
                            );
                        }

                        // If [Socket] wa sset as null, it means that the service was Disposed
                        if (null == _socketOfAcceptor)
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
                        ThreadPool.QueueUserWorkItem(
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
            _socketOfAcceptor?.Close();
            _socketOfAcceptor = null;

            _socketAsyncEventArgs.Dispose();
        }
    }
}
