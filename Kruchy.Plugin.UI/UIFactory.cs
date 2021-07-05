using Kruchy.Plugin.Utils.UI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kruchy.Plugin.UI
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

            var implementations =
                GetType().Assembly.GetTypes().Where(o => typeof(T).IsAssignableFrom(o));

            if (implementations.Count() == 1)
                return (T)(implementations.Single().GetConstructors().Single().Invoke(new object[0]));

            return (T)factoryFunction(implementationType);
        }
    }
}
