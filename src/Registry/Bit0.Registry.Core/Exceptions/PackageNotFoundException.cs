using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Bit0.Registry.Core.Exceptions
{
    [Serializable]
    [ExcludeFromCodeCoverage]
    public class PackageNotFoundException : Exception
    {
        public EventId EventId => 3003;

        public StreamingContext Context { get; }

        public PackageNotFoundException()
        {
        }

        public PackageNotFoundException(String name, String semVer) : this(name, semVer, null)
        {
        }

        public PackageNotFoundException(String name, String semVer, Exception innerException) : base($"{name} {semVer}", innerException)
        {
        }

        public PackageNotFoundException(String message) : base(message)
        {
        }

        public PackageNotFoundException(String message, Exception innerException) : base(message, innerException)
        {
        }

        protected PackageNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            Context = context;
        }
    }
}
