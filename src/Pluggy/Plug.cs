using Reffy.Expressions;
using System;
using System.Linq;

namespace Pluggy
{
    public sealed class Plug
    {
        private readonly Type type;

        public Plug(Type type)
        {
            this.type = type;
        }

        public T Activate<T>(params object[] args)
        {
            var t = typeof(T);
            if (!t.IsInterface)
                throw new Exception($"{t.FullName} must be an interface type.");

            if (!type.GetInterfaces().Any(type => type == t))
                throw new Exception($"{type.FullName} must implement {t.FullName}.");

            return (T)type.Constructor(args);
        }
    }

}
