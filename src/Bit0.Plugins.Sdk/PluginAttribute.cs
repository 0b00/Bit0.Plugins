using System;

namespace Bit0.Plugins
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class PluginAttribute : Attribute
    {
        public String Id { get; set; }
        public String Name { get; set; }
        public String Version { get; set; }
        public Type Implementing { get; set; }

        public String FullId => $"{Id}@{Version}";
        public override String ToString() => $"'{Name}' ({FullId})";
    }
}