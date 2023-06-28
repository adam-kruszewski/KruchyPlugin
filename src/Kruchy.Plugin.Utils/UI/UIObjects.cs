using System;

namespace Kruchy.Plugin.Utils.UI
{
    public class UIObjects
    {
        public static IUIFactory FactoryInstance;

        public static Action<object> ShowWindow = o => {};

        public static Action<object> ShowWindowModal = o => { };

        public static Action<string, string> ShowMessageBox = (o1, o2) => { };
    }
}