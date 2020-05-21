using AutoMapper;
using Bit0.Plugins;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace DevPlugin2
{
    [ExcludeFromCodeCoverage]
    [Plugin(Id = "dev-plugin2", Name = "Dev Plugin 2", Version = "1.0.1", Implementing = typeof(IMapper))]
    public class DevPlugin2 : PluginBase
    {
        public override IServiceCollection Register(IServiceCollection services)
        {
            services.AddSingleton(new MapperPlugin());
            services.AddSingleton<IList<Char>>(new List<Char> { 'a', 'b', 'c' });

            return services;
        }
    }

    [ExcludeFromCodeCoverage]
    public class MapperPlugin
    {
        public MapperPlugin()
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<Order, OrderDto>());
            Mapper = config.CreateMapper();
        }

        public IMapper Mapper { get; }
    }

    [ExcludeFromCodeCoverage]
    public class Order
    {
        public String Id { get; set; }
    }

    [ExcludeFromCodeCoverage]
    public class OrderDto
    {
        public String Id { get; set; }
    }
}
