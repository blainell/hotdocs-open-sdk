/* Copyright (c) 2013, HotDocs Limited
   Use, modification and redistribution of this source is subject
   to the New BSD License as set out in LICENSE.TXT. */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace HotDocs.Sdk.Server.Contracts
{
    /// <summary>
    ///     This class is used for security verification when sending requests to HotDocs Core Services.
    ///     HMAC stands for Hash-based Message Authentication Code.
    /// </summary>
    public static class HMAC
    {
        /// <summary>
        ///     Canonicalizes an array of parameters into a string.
        /// </summary>
        /// <remarks>
        ///     The canonicalization algorithm is as follows:
        ///     - Strings are included as-is, even if they contain '\n'.
        ///     - Integral types are converted to strings using base 10 and no leading zeros.
        ///     - Enums and bools are converted to strings.
        ///     - DateTime types are converted to UTC and formatted as "yyyy-MM-ddTHH:mm:ssZ".
        ///     - Dictionaries (must have string key and string value) are sorted alphabetically by key,
        ///     and projected to individual strings as "key=value", e.g. keyA=valueA\nkeyB=valueB\n etc.
        ///     - Nulls and all other types are converted to empty strings.
        ///     - All parameters are separated by '\n'.
        ///     - The resulting string is encoded in UTF-8.
        ///     This canonicalization is in keeping with REST services provided by Amazon, Microsoft, SpatialCloud, etc.
        /// </remarks>
        /// <param name="paramList">The parameters to be canonicalized</param>
        /// <returns>A canonicalized string</returns>
        public static string Canonicalize(params object[] paramList)
        {
            if (paramList == null)
                throw new ArgumentNullException();

            var strings = paramList.Select(param =>
            {
                if (param is string || param is int || param is Enum || param is bool)
                {
                    return param.ToString();
                }
                if (param is DateTime)
                {
                    var utcTime = ((DateTime) param).ToUniversalTime();
                    return utcTime.ToString("yyyy-MM-ddTHH:mm:ssZ");
                }
                if (param is Dictionary<string, string>)
                {
                    var sorted = ((Dictionary<string, string>) param).OrderBy(kv => kv.Key);
                    var stringified = sorted.Select(kv => kv.Key + "=" + kv.Value).ToArray();
                    return string.Join("\n", stringified);
                }
                return "";
            });
            return string.Join("\n", strings.ToArray());
        }

        /// <summary>
        ///     Creates an HMAC-SHA1 hash from an array of parameters and a security token.
        /// </summary>
        /// <param name="paramList">The parameters to be hashed</param>
        /// <param name="signingKey">The Subscriber's unique signing key.</param>
        /// <returns>The BASE64-encoded HMAC</returns>
        public static string CalculateHMAC(string signingKey, params object[] paramList)
        {
            var key = Encoding.UTF8.GetBytes(signingKey);
            var stringToSign = Canonicalize(paramList);
            var bytesToSign = Encoding.UTF8.GetBytes(stringToSign);
            byte[] signature;

            using (var hmac = new HMACSHA1(key))
            {
                signature = hmac.ComputeHash(bytesToSign);
            }

            return Convert.ToBase64String(signature);
        }

        /// <summary>
        ///     Validates the provided HMAC against the provided security token and parameter list.
        /// </summary>
        /// <param name="hmac">This is a hash key used to authenticate the request made to HotDocs Core Services. </param>
        /// <param name="signingKey">The Subscriber's unique signing key.</param>
        /// <param name="paramList">The parameters used to create the HMAC</param>
        public static void ValidateHMAC(string hmac, string signingKey, params object[] paramList)
        {
            var calculatedHMAC = CalculateHMAC(signingKey, paramList);

            if (hmac != calculatedHMAC)
            {
                throw new HMACException(hmac, calculatedHMAC, paramList);
            }
        }
    }
}