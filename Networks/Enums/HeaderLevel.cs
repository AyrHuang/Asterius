namespace Networks.Enums
{
    /// <summary>
    /// This enum was use to define what the protocol level was.
    /// This enum will required 4 bit (half of byte/char) when send/recv protocol at header.
    /// </summary>
    public enum HeaderLevel : byte
    {
        /// <summary>
        /// The default not useful.
        /// </summary>
        None      = 0,
        /// <summary>
        /// The protocol should be on accept. The value should be 1.
        /// </summary>
        OnAccept  = 1,
        /// <summary>
        /// The protocol should be on connect. The value should be 2.
        /// </summary>
        OnConnect = 2,
    }
}
