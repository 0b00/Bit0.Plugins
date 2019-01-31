using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Bit0.Registry.Core.Exceptions
{
    [Serializable]
    [ExcludeFromCodeCoverage]
    public class InvalidPackFileException : Exception
    {
        public EventId EventId => 3002;

        public InvalidPackFileException()
        {
        }

        public InvalidPackFileException(String message) : base(message)
        {
        }

        public InvalidPackFileException(String message, Exception innerException) : base(message, innerException)
        {
        }

        protected InvalidPackFileException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}