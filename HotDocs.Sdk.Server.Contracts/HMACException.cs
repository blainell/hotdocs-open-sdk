using System;

namespace HotDocs.Sdk.Server.Contracts
{
    /// <summary>
    ///     This is the exception that is raised when HMAC values do not match between the client and server.
    /// </summary>
    public class HMACException : Exception
    {
        /// <summary>
        ///     This exception is thrown when the HMAC value included in the request does not match the value calculated on the
        ///     server.
        ///     Usually this happens when some value, such as the subscriber ID or signing key, was incorrect when creating the
        ///     original
        ///     HMAC value.
        /// </summary>
        /// <param name="hmac">This is a hash key used to authenticate the request made to HotDocs Core Services. </param>
        /// <param name="calculatedHMAC">The HMAC key calculated by the service.</param>
        /// <param name="paramList">An array of parameters that were included in the request.</param>
        public HMACException(string hmac, string calculatedHMAC, params object[] paramList) :
            base("Error: Invalid request signature.")
        {
        }
    }
}