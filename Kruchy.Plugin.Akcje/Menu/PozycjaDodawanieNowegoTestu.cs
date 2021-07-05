using Kruchy.Plugin.Akcje.Akcje;
using Kruchy.Plugin.Akcje.Interfejs;
using Kruchy.Plugin.Utils.Menu;
using Kruchy.Plugin.Utils.UI;
using Kruchy.Plugin.Utils.Wrappers;
using System;
using System.Collections.Generic;

namespace Kruchy.Plugin.Akcje.Menu
{
    class PozycjaDodawanieNowegoTestu : IPozycjaMenu
    {
        private readonly ISolutionWrapper solution;

        public PozycjaDodawanieNowegoTestu(ISolutionWrapper solution)
        {
            this.solution = solution;
        }

        public uint MenuCommandID
        {
            get { return PkgCmdIDList.cmdidDodajNowyTest; }
        }

        public IEnumerable<WymaganieDostepnosci> Wymagania
        {
            get
            {
                yield return WymaganieDostepnosci.KlasaTestowa;
            }
        }

        public void Execute(object sender, EventArgs args)
        {
            INewTestWindow dialog = UIObjects.FactoryInstance.Get<INewTestWindow>();
            UIObjects.ShowWindowModal(dialog);

            if (string.IsNullOrEmpty(dialog.Name))
                return;

            new DodawanieNowegoTestu(solution)
                .DodajNowyTest(dialog.Name, dialog.Async);
        }
    }
}