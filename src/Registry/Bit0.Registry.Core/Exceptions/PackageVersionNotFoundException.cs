using Microsoft.Extensions.Logging;
using System;
using System.Runtime.Serialization;

namespace Bit0.Registry.Core.Exceptions
{
    public class PackageVersionNotFoundException : Exception
    {
        public EventId EventId => 3004;

        public StreamingContext Context { get; }

        public PackageVersionNotFoundException()
        {
        }

        public PackageVersionNotFoundException(Package package) : this(package, null)
        {
        }

        public PackageVersionNotFoundException(Package package, Exception innerException) : base(package.ToString(), innerException)
        {
        }

        public PackageVersionNotFoundException(String message) : base(message)
        {
        }

        public PackageVersionNotFoundException(String message, Exception innerException) : base(message, innerException)
        {
        }

        protected PackageVersionNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            Context = context;
        }
    }
}
