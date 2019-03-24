using System;

namespace Networks.Attributes
{
    /// <summary>
    /// This attribute will be use to define the protocol code of message.
    /// There should not exists any duplicate code when use this attribute.
    /// </summary>
    public class ProtocolCodeAttribute : Attribute
    {
        public ushort Code { get; private set; }

        public ProtocolCodeAttribute(ushort code)
        {
            Code = code;
        }
    }
}
