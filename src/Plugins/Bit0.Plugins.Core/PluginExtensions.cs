using System.Reflection;

namespace Bit0.Plugins.Core
{
    public static class PluginExtensions
    {
        public static PluginAttribute GetInfo(this IPlugin plugin)
        {
            return plugin.GetType().GetCustomAttribute<PluginAttribute>();
        }
    }
}
