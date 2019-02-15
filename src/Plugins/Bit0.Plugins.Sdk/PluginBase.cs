using System;

namespace Bit0.Plugins
{
    public abstract class PluginBase : IPlugin
    {
        private String _fullId;

        public String FullId
        {
            get
            {
                if (String.IsNullOrWhiteSpace(_fullId))
                {
                    _fullId = this.GetInfo().FullId;
                }

                return _fullId;
            }
        }

        private Type _implementing;

        public Type Implementing
        {
            get
            {
                if (_implementing == null)
                {
                    _implementing = this.GetInfo().Implementing;
                }

                return _implementing;
            }
        }
    }
}
