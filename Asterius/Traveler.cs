using System;
using System.Net.Sockets;

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

        /// <summary>
        /// Create a Traveler with accept active, this constructor should be only invoke with Entrance.
        /// </summary>
        /// <param name="socket"></param>
        public Traveler(Socket socket)
        {
            _socket = socket;

            _travelerStates |= TravelerStates.Connected;
        }

        public void Dispose()
        {
            _socket.Dispose();
            _socket = null;
        }
    }
}
