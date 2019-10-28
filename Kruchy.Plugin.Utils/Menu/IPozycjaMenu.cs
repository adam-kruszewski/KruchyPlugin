using System;
using System.Collections.Generic;

namespace Kruchy.Plugin.Utils.Menu
{
    public interface IPozycjaMenu
    {
        uint MenuCommandID { get; }

        IEnumerable<WymaganieDostepnosci> Wymagania { get; }

        void Execute(object sender, EventArgs args);
    }
}