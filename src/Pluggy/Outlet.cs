using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Pluggy
{
    public sealed class Outlet<T>
    {
        private readonly List<Type> types;
        private readonly List<Plug<T>> caches = new List<Plug<T>>();

        private Outlet(List<Type> types)
        {
            this.types = types;
        }

        private static Outlet<T> Connect(string path)
        {
            var inf = typeof(T);
            if (!inf.IsInterface)
                throw new Exception($"{inf.FullName} must be an interface type.");

            var acc = new List<Type>();
            var fullpath = Path.GetFullPath(path);
            foreach (var dll in Directory.EnumerateFiles(fullpath, "*.dll"))
            {
                var types = Assembly.LoadFrom(dll)
                    .GetTypes()
                    .Where(type => type.GetCustomAttribute<PluginAttribute>() != null && !type.IsAbstract && (type.IsClass || type.IsValueType))
                    .Where(type => type.GetInterfaces().Any(t => t == inf))
                    .ToArray();
                if (types.Any())
                    acc.AddRange(types);
            }
            return new Outlet<T>(acc);
        }

        public static async Task<Outlet<T>> ConnectAsync(string path)
            => await Task.Factory.StartNew(() => Connect(path));

        public static async Task<Outlet<T>> ConnectAsync(string path, CancellationToken cancellationToken)
            => await Task.Factory.StartNew(() => Connect(path), cancellationToken);

        public static async Task<Outlet<T>> ConnectAsync(string path, TaskCreationOptions creationOptions)
            => await Task.Factory.StartNew(() => Connect(path), creationOptions);

        public static async Task<Outlet<T>> ConnectAsync(string path, CancellationToken cancellationToken, TaskCreationOptions creationOptions, TaskScheduler scheduler)
            => await Task.Factory.StartNew(() => Connect(path), cancellationToken, creationOptions, scheduler);

        private IEnumerable<Plug<T>> GetPlugins()
        {
            if (!types.Any() || caches.Any())
                return caches;

            foreach (var type in types)
                caches.Add(new Plug<T>(type));

            return caches;
        }

        public async Task<IEnumerable<Plug<T>>> GetPluginsAsync()
            => await Task.Factory.StartNew(GetPlugins);

        public async Task<IEnumerable<Plug<T>>> GetPluginsAsync(CancellationToken cancellationToken)
            => await Task.Factory.StartNew(GetPlugins, cancellationToken);

        public async Task<IEnumerable<Plug<T>>> GetPluginsAsync(TaskCreationOptions creationOption)
            => await Task.Factory.StartNew(GetPlugins, creationOption);

        public async Task<IEnumerable<Plug<T>>> GetPluginsAsync(CancellationToken cancellationToken, TaskCreationOptions creationOptions, TaskScheduler scheduler)
            => await Task.Factory.StartNew(GetPlugins, cancellationToken, creationOptions, scheduler);
    }
}
