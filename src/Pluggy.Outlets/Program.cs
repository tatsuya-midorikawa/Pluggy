using Pluggy.Interfaces;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Pluggy.Outlets
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var outlet = await Outlet.ConnectAsync("./");
            var plugins = await outlet.GetPluginsAsync();
            foreach (var (plugin, i) in plugins.Select((p, i) => (p, i)))
            {
                plugin.Activate<IPlugin>(i.ToString()).Print();
            }
        }
    }
}
