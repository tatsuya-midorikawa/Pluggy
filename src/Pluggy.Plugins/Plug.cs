using Pluggy.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pluggy.Plugins
{
    [Plugin]
    public class DefaultPlug : IPlugin
    {
        private readonly string name;

        public DefaultPlug(string name)
        {
            this.name = name;
        }

        public void Print()
            => Console.WriteLine($"{GetType().FullName}: {name}");
    }

    [Plugin]
    public class CustomPlug : IPlugin
    {
        private readonly string name;

        public CustomPlug(string name)
        {
            this.name = name;
        }

        public void Print()
            => Console.WriteLine($"{GetType().FullName}: {name}");
    }
}
