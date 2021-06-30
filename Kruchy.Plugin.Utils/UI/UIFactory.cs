using System;
using System.Collections.Generic;

namespace Kruchy.Plugin.Utils.UI
{
    public class UIFactory : IUIFactory
    {
        public static Func<Type, object> factoryFunction;

        public static Dictionary<Type, Type> implementationDictionary =
            new Dictionary<Type, Type>();

        public T Get<T>()
        {
            Type implementationType = typeof(T);

            if (implementationDictionary.ContainsKey(typeof(T)))
            {
                implementationType = implementationDictionary[typeof(T)];
            }

            return (T) factoryFunction(typeof(T));
        }
    }
}
