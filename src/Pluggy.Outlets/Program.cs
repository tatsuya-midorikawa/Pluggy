﻿using Pluggy.Interfaces;
using System.Linq;
using System.Threading.Tasks;

namespace Pluggy.Outlets
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var outlet = await Outlet<IPlugin>.ConnectAsync("./");
            var plugins = await outlet.GetPluginsAsync();
            foreach (var (plugin, i) in plugins.Select((p, i) => (p, i)))
            {
                plugin.Activate(i.ToString()).Print();
            }
        }
    }
}
