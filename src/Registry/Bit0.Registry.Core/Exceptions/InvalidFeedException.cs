using Microsoft.Extensions.Logging;
using System;
using System.Runtime.Serialization;

namespace Bit0.Registry.Core.Exceptions
{
    public class InvalidFeedException : Exception
    {
        public EventId EventId => 3001;

        public InvalidFeedException()
        {
        }

        public InvalidFeedException(Uri url) : this(url, null)
        {
        }

        public InvalidFeedException(Uri url, Exception innerException) : base(url.ToString(), innerException)
        {
        }

        public InvalidFeedException(String message) : base(message)
        {
        }

        public InvalidFeedException(String message, Exception innerException) : base(message, innerException)
        {
        }

        protected InvalidFeedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
