using System;

namespace HotDocs.Sdk.Server.OnPremise
{
    [Serializable]
    internal class ExceptionResponse : Exception
    {
        public string ExceptionType { get; set; }
        public string ExceptionMessage { get; set; }
    }
}