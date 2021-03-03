using Reffy.Expressions;
using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Pluggy
{
    public sealed class FlexiblePlug
    {
        private readonly Type type;

        public FlexiblePlug(Type type)
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

        public dynamic Activate(params object[] args)
        {
            if (args == null)
                return type.Constructor();

            var types = args.Select(arg => arg.GetType()).ToArray();
            return type.GetConstructors().Any(ctor => Same(types, ctor))
                ? type.Constructor(args)
                : type.Constructor();
        }

        public async Task<dynamic> ActivateAsync(params object[] args)
            => await Task.Factory.StartNew(() => Activate(args));

        public async Task<dynamic> ActivateAsync(CancellationToken cancellationToken, params object[] args)
            => await Task.Factory.StartNew(() => Activate(args), cancellationToken);

        public async Task<dynamic> ActivateAsync(TaskCreationOptions creationOptions, params object[] args)
            => await Task.Factory.StartNew(() => Activate(args), creationOptions);

        public async Task<dynamic> ActivateAsync(CancellationToken cancellationToken, TaskCreationOptions creationOptions, TaskScheduler scheduler, params object[] args)
            => await Task.Factory.StartNew(() => Activate(args), cancellationToken, creationOptions, scheduler);
    }
}
