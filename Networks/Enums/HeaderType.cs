namespace Networks.Enums
{
    /// <summary>
    /// This enum was use to define what the protocol type was.
    /// This enum will required 4 bit (half of byte/char) when send/recv protocol at header.
    /// </summary>
    public enum HeaderType
    {
        /// <summary>
        /// The default not useful.
        /// </summary>
        None   = 0,
        /// <summary>
        /// The protocol should be active like RPC(Remote Procedure Call). The value should be 1.
        /// </summary>
        AsRPC  = 1,
        /// <summary>
        /// The protocol should be active like Message. The value should be 2.
        /// </summary>
        AsCall = 2,
    }
}
