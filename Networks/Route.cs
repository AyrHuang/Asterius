using Networks.Exceptions;
using Microsoft.IO;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Networks
{
    public class Route : IDisposable
    {
        private string _TagOfMemory = "Message";

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

        private readonly SocketAsyncEventArgs _socketAsyncEventArgsOfSend = new SocketAsyncEventArgs();
        private readonly SocketAsyncEventArgs _socketAsyncEventArgsOfRecv = new SocketAsyncEventArgs();

        private readonly IPEndPoint _ipEndPoint = null;
        private readonly int _sizeOfMemory = 0;
        private readonly MemoryStream _memoryStreamOfSend = null;
        private readonly MemoryStream _memoryStreamOfRecv = null;

        /// <summary>
        /// Create a Route with accept active which will be active like a server, this constructor should be only invoke with Entrance.
        /// </summary>
        /// <param name="socket"></param>
        public Route(Socket socket, RecyclableMemoryStreamManager recyclableMemoryStreamManager, int sizeOfMemory = ushort.MaxValue)
        {
            _ipEndPoint = socket.RemoteEndPoint as IPEndPoint;

            _socket = socket;
            _socket.NoDelay = true;

            _travelerStates |= TravelerStates.Connected;

            _sizeOfMemory = sizeOfMemory;
            _memoryStreamOfSend = recyclableMemoryStreamManager.GetStream(
                _TagOfMemory,
                _sizeOfMemory
            );
            _memoryStreamOfRecv = recyclableMemoryStreamManager.GetStream(
                _TagOfMemory,
                _sizeOfMemory
            );

            _socketAsyncEventArgsOfSend.Completed += OnCompleted;
            OnDispatch();
        }

        /// <summary>
        /// Create a Route with IPEndPoint, this constructor should be only invoke with Entrance.
        /// </summary>
        /// <param name="hostOfIPv4"></param>
        /// <param name="portOfIPv4"></param>
        public Route(IPEndPoint ipEndPoint, RecyclableMemoryStreamManager recyclableMemoryStreamManager, int sizeOfMemory = ushort.MaxValue)
        {
            _ipEndPoint = ipEndPoint; 

            // Just like setting of [Entrance] processor
            _socket = new Socket(
                addressFamily: AddressFamily.InterNetwork,
                socketType: SocketType.Stream,
                protocolType: ProtocolType.Tcp
            );
            _socket.NoDelay = true;

            _sizeOfMemory = sizeOfMemory;
            _memoryStreamOfSend = recyclableMemoryStreamManager.GetStream(
                _TagOfMemory,
                _sizeOfMemory
            );
            _memoryStreamOfRecv = recyclableMemoryStreamManager.GetStream(
                _TagOfMemory,
                _sizeOfMemory
            );

            _socketAsyncEventArgsOfSend.Completed += OnCompleted;
            OnDispatch();
        }

        private void OnConnectAsync()
        {
            // Set Socket [RemoteEndPoint] for connect to.
            _socketAsyncEventArgsOfSend.RemoteEndPoint = _ipEndPoint;
            bool isAcceptAsyncSuccess = _socket.ConnectAsync(
                _socketAsyncEventArgsOfSend
            );

            if (isAcceptAsyncSuccess)
            {
                return;
            }

            OnConnectCompleted(
                _socketAsyncEventArgsOfSend
            );
        }

        private void OnConnectCompleted(object o)
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
                            OnConnectCompleted,
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

        private void OnReceivedAsync()
        {
            _travelerStates |= TravelerStates.Recving;
            _socketAsyncEventArgsOfRecv.SetBuffer(
                _memoryStreamOfRecv.GetBuffer(),
                0,
                _memoryStreamOfRecv.Capacity
            );

            bool isReceiveAsyncSuccess = _socket.ReceiveAsync(
                _socketAsyncEventArgsOfRecv
            );
            if (!isReceiveAsyncSuccess)
            {
                return;
            }
            OnReceivedCompleted(
                _socketAsyncEventArgsOfRecv
            );
        }

        private void OnReceivedCompleted(object o)
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
                        if (0 == socketAsyncEventArgs.BytesTransferred)
                        {
                            throw new SocketCompletedException(
                                $"Receive byte: {socketAsyncEventArgs.BytesTransferred} was zero"
                            );
                        }

                        // We will required to unpack here
                    }
                    break;
                default:
                    throw new SocketCompletedException(
                        $"Error: {socketAsyncEventArgs.SocketError} on receive"
                    );
            }
        }

        private void OnDispatch()
        {
            if (TravelerStates.Connected != (TravelerStates.Connected & _travelerStates))
            {
                OnConnectAsync();
            }
            else
            {
                if (TravelerStates.Recving != (TravelerStates.Recving & _travelerStates))
                {
                    OnReceivedAsync();
                }
            }
        }

        public void Dispose()
        {
            _socket.Dispose();
            _socket = null;

            _socketAsyncEventArgsOfSend.Dispose();
            _socketAsyncEventArgsOfRecv.Dispose();
            _memoryStreamOfSend.Dispose();
            _memoryStreamOfRecv.Dispose();
        }
    }
}
