using System;

namespace Bit0.Plugins
{
    public interface IPlugin
    {
        String FullId { get; }
        Type Implementing { get; }
    }
}
