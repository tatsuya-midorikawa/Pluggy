using Reffy.Expressions;
using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Pluggy
{
    public sealed class Plug
    {
        private readonly Type type;

        public Plug(Type type)
        {
            this.type = type;
        }

        private static bool Same(Type[] types, ConstructorInfo ctor)
        {
            var parameters = ctor.GetParameters();
            if (types.Length != parameters.Length)
                return false;

            for (int i = 0; i < types.Length; i++)
            {
                if (types[i] != parameters[i].ParameterType)
                    return false;
            }

            return true;
        }

        public T Activate<T>(params object[] args)
        {
            var t = typeof(T);
            if (!t.IsInterface)
                throw new Exception($"{t.FullName} must be an interface type.");

            if (!type.GetInterfaces().Any(type => type == t))
                throw new Exception($"{type.FullName} must implement {t.FullName}.");

            if (args == null)
                return (T)type.Constructor();

            var types = args.Select(arg => arg.GetType()).ToArray();
            return type.GetConstructors().Any(ctor => Same(types, ctor))
                ? (T)type.Constructor(args)
                : (T)type.Constructor();
        }

        public async Task<T> ActivateAsync<T>(params object[] args)
            => await Task.Factory.StartNew(() => Activate<T>(args));

        public async Task<T> ActivateAsync<T>(CancellationToken cancellationToken, params object[] args)
            => await Task.Factory.StartNew(() => Activate<T>(args), cancellationToken);

        public async Task<T> ActivateAsync<T>(TaskCreationOptions creationOptions, params object[] args)
            => await Task.Factory.StartNew(() => Activate<T>(args), creationOptions);

        public async Task<T> ActivateAsync<T>(CancellationToken cancellationToken, TaskCreationOptions creationOptions, TaskScheduler scheduler, params object[] args)
            => await Task.Factory.StartNew(() => Activate<T>(args), cancellationToken, creationOptions, scheduler);
    }

}
