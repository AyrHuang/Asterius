using System;
using System.Runtime.Serialization;

namespace Asterius.Exceptions
{
    public class SocketCompletedException : Exception
    {
        public SocketCompletedException() : base()
        {
        }

        public SocketCompletedException(string message) : base(message)
        {
        }


        public SocketCompletedException(
            string message,
            Exception innerException
        ) : base(
                message,
                innerException
            )
        {
        }
        
        protected SocketCompletedException(
            SerializationInfo info,
            StreamingContext context
        ) : base(
                info,
                context
            )
        {
        }
    }
}
