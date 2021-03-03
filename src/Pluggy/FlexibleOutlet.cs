using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Pluggy
{
    public sealed class FlexibleOutlet
    {
        private readonly List<Type> types;
        private readonly List<FlexiblePlug> caches = new List<FlexiblePlug>();

        private FlexibleOutlet(List<Type> types)
        {
            this.types = types;
        }

        private static FlexibleOutlet Connect(string path)
        {
            var acc = new List<Type>();
            var fullpath = Path.GetFullPath(path);
            foreach (var dll in Directory.EnumerateFiles(fullpath, "*.dll"))
            {
                var types = Assembly.LoadFrom(dll)
                    .GetTypes()
                    .Where(type => type.GetCustomAttribute<PluginAttribute>() != null && !type.IsAbstract && (type.IsClass || type.IsValueType))
                    .ToArray();
                if (types.Any())
                    acc.AddRange(types);
            }
            return new FlexibleOutlet(acc);
        }

        public static async Task<FlexibleOutlet> ConnectAsync(string path)
            => await Task.Factory.StartNew(() => Connect(path));

        public static async Task<FlexibleOutlet> ConnectAsync(string path, CancellationToken cancellationToken)
            => await Task.Factory.StartNew(() => Connect(path), cancellationToken);

        public static async Task<FlexibleOutlet> ConnectAsync(string path, TaskCreationOptions creationOptions)
            => await Task.Factory.StartNew(() => Connect(path), creationOptions);

        public static async Task<FlexibleOutlet> ConnectAsync(string path, CancellationToken cancellationToken, TaskCreationOptions creationOptions, TaskScheduler scheduler)
            => await Task.Factory.StartNew(() => Connect(path), cancellationToken, creationOptions, scheduler);

        private IEnumerable<FlexiblePlug> GetPlugins()
        {
            if (!types.Any() || caches.Any())
                return caches;

            foreach (var type in types)
                caches.Add(new FlexiblePlug(type));

            return caches;
        }

        public async Task<IEnumerable<FlexiblePlug>> GetPluginsAsync()
            => await Task.Factory.StartNew(GetPlugins);

        public async Task<IEnumerable<FlexiblePlug>> GetPluginsAsync(CancellationToken cancellationToken)
            => await Task.Factory.StartNew(GetPlugins, cancellationToken);

        public async Task<IEnumerable<FlexiblePlug>> GetPluginsAsync(TaskCreationOptions creationOption)
            => await Task.Factory.StartNew(GetPlugins, creationOption);

        public async Task<IEnumerable<FlexiblePlug>> GetPluginsAsync(CancellationToken cancellationToken, TaskCreationOptions creationOptions, TaskScheduler scheduler)
            => await Task.Factory.StartNew(GetPlugins, cancellationToken, creationOptions, scheduler);
    }
}
