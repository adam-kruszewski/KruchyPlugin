﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Kruchy.Plugin.Utils.Menu
{
    public abstract class AbstractPozycjaMenuDynamicznieRozwijane
        : IPozycjaMenuDynamicznieRozwijane
    {
        public AbstractPozycjaMenuDynamicznieRozwijane()
        {
            pozycjeRozwijane = new List<IPodpozycjaMenuDynamicznego>();
        }

        public uint OstanieCommandID => MenuCommandID +  (uint)(DajDostepnePozycje().Count() - 1);

        abstract public uint MenuCommandID { get; }

        abstract public IEnumerable<WymaganieDostepnosci> Wymagania { get; }

        protected List<IPodpozycjaMenuDynamicznego> pozycjeRozwijane;

        protected abstract IEnumerable<IPodpozycjaMenuDynamicznego> DajDostepnePozycje();

        public IEnumerable<PozycjaMenuRozwijanego> DajPozycje()
        {
            pozycjeRozwijane.Clear();

            pozycjeRozwijane.AddRange(DajDostepnePozycje());

            return pozycjeRozwijane
                .Select(o => new PozycjaMenuRozwijanego
                {
                    PozycjaMenu = o,
                    Tekst = o.DajOpis()
                });
        }

        public bool DostepnaPodakcja(int commandID)
        {
            return true;
        }

        public void Execute(object sender, EventArgs args)
        { }

        public void WykonajPodakcje(int commandID)
        {
            pozycjeRozwijane
                .SingleOrDefault(o => commandID == o.MenuCommandID)
                    ?.Execute(null, null);
        }
    }
}
