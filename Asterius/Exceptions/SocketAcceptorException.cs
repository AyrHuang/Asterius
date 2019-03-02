using System;
using System.Runtime.Serialization;

namespace Asterius.Exceptions
{
    public class SocketAcceptorException : Exception
    {
        public SocketAcceptorException() : base()
        {
        }

        public SocketAcceptorException(string message) : base(message)
        {
        }


        public SocketAcceptorException(
            string message,
            Exception innerException
        ) : base(
                message,
                innerException
            )
        {
        }
        
        protected SocketAcceptorException(
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
