﻿using System;
using System.IO;
using System.Text;
using Kruchy.Plugin.Akcje.Tests.Utils;
using Kruchy.Plugin.Utils.Wrappers;

namespace Kruchy.Plugin.Akcje.Tests.WrappersMocks
{
    class PlikWrapper : IPlikWrapper
    {
        private string sciezkaPelna;

        public PlikWrapper(
            string nazwa,
            string katalog,
            IProjectWrapper projekt,
            string zawartosc = null)
        {
            Nazwa = nazwa;
            Katalog = Path.Combine(projekt.DirectoryPath, katalog, nazwa);
            SciezkaWzgledna = Path.Combine(katalog, nazwa);
            Projekt = projekt;
            (Projekt as ProjektWrapper).DodajPlik(this);
            sciezkaPelna = Path.Combine(Katalog, Nazwa);
        }

        public PlikWrapper(string sciezkaPelna)
        {
            var fi = new FileInfo(sciezkaPelna);
            Nazwa = fi.Name;
            Katalog = fi.DirectoryName;
            this.sciezkaPelna = fi.FullName;
        }

        public PlikWrapper(string nazwaZasobu, IProjectWrapper projekt)
        {
            Nazwa = nazwaZasobu;
            Katalog = projekt.DirectoryPath;
            SciezkaWzgledna = nazwaZasobu;
            Projekt = projekt;
            (Projekt as ProjektWrapper).DodajPlik(this);
            sciezkaPelna = Path.Combine(Katalog, Nazwa);

            File.WriteAllText(
                sciezkaPelna,
                new WczytywaczZawartosciPrzykladow().DajZawartoscPrzykladu(nazwaZasobu),
                Encoding.UTF8);
        }

        public string Nazwa { get; set; }

        public string NazwaBezRozszerzenia
        {
            get
            {
                var indekskropki = Nazwa.LastIndexOf(".");
                if (indekskropki > 0)
                    return Nazwa.Substring(0, indekskropki);
                else
                    return Nazwa;
            }
        }

        public string SciezkaPelna { get { return sciezkaPelna; } }

        public string Katalog { get; set; }

        public string SciezkaWzgledna { get; set; }

        public IProjectWrapper Projekt { get; set; }

        public IDocumentWrapper Dokument => throw new NotImplementedException();

        public override string ToString()
        {
            return string.Format("{0} [{1}]", Nazwa, Katalog);
        }
    }
}
