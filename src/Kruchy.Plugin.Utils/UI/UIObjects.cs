using System;

namespace Kruchy.Plugin.Utils.UI
{
    public class UIObjects
    {
        public static IUIFactory FactoryInstance;

        public static Action<object> ShowWindow;

        public static Action<object> ShowWindowModal;

        public static Action<string, string> ShowMessageBox;
    }
}