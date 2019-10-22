using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kruchy.Plugin.Utils.Wrappers
{
    public interface ISolutionExplorerWrapper
    {
        void OtworzPlik(string sciezka);

        void OtworzPlik(IPlikWrapper plik);

        void UstawSieNaMiejscu(string sciezka);
    }
}
