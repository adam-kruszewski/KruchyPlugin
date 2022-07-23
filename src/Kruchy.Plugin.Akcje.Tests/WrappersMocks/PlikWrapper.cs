using System;
using System.IO;
using System.Text;
using Kruchy.Plugin.Akcje.Tests.Utils;
using Kruchy.Plugin.Utils.Wrappers;

namespace Kruchy.Plugin.Akcje.Tests.WrappersMocks
{
    class PlikWrapper : IFileWrapper
    {
        private string sciezkaPelna;

        public PlikWrapper(
            string nazwa,
            string katalog,
            IProjectWrapper projekt,
            string zawartosc = null)
        {
            Name = nazwa;
            Directory = Path.Combine(projekt.DirectoryPath, katalog, nazwa);
            RelativePath = Path.Combine(katalog, nazwa);
            Project = projekt;
            (Project as ProjektWrapper).DodajPlik(this);
            sciezkaPelna = Path.Combine(Directory, Name);
        }

        public PlikWrapper(string sciezkaPelna)
        {
            var fi = new FileInfo(sciezkaPelna);
            Name = fi.Name;
            Directory = fi.DirectoryName;
            this.sciezkaPelna = fi.FullName;
        }

        public PlikWrapper(string nazwaZasobu, IProjectWrapper projekt)
        {
            Name = nazwaZasobu;
            Directory = projekt.DirectoryPath;
            RelativePath = nazwaZasobu;
            Project = projekt;
            (Project as ProjektWrapper).DodajPlik(this);
            sciezkaPelna = Path.Combine(Directory, Name);

            File.WriteAllText(
                sciezkaPelna,
                new WczytywaczZawartosciPrzykladow().DajZawartoscPrzykladu(nazwaZasobu),
                Encoding.UTF8);
        }

        public string Name { get; set; }

        public string NameWithoutExtension
        {
            get
            {
                var indekskropki = Name.LastIndexOf(".");
                if (indekskropki > 0)
                    return Name.Substring(0, indekskropki);
                else
                    return Name;
            }
        }

        public string FullPath { get { return sciezkaPelna; } }

        public string Directory { get; set; }

        public string RelativePath { get; set; }

        public IProjectWrapper Project { get; set; }

        public IDocumentWrapper Document => throw new NotImplementedException();

        public override string ToString()
        {
            return string.Format("{0} [{1}]", Name, Directory);
        }
    }
}
