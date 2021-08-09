using System;

namespace Kruchy.Plugin.Akcje.Tests.Unit
{
    class UzupelnianieKonstruktoraPrzyStalej
    {
        private const int DefaultTimeout = 10000;

        private readonly Class1 appSettingsService;

        public int TimeoutInMilliseconds()
        {
            return 1;
        }
    }
}