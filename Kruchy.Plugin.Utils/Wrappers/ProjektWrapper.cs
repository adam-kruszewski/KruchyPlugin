using System.Collections.Generic;
using System.IO;
using EnvDTE;

namespace Kruchy.Plugin.Utils.Wrappers
{
    public class ProjektWrapper : IProjektWrapper
    {
        private readonly Project project;

        public ProjektWrapper(Project project)
        {
            this.project = project;
        }

        public string Nazwa { get { return project.Name; } }

        public string Sciezka { get { return project.FullName; } }

        public string SciezkaDoKatalogu
        {
            get
            {
                FileInfo f = new FileInfo(Sciezka);
                return f.DirectoryName;
            }
        }

        public IPlikWrapper[] Pliki
        {
            get
            {
                var listaPlikow = new List<IPlikWrapper>();
                foreach (ProjectItem pi in project.ProjectItems)
                {
                    SzukajPlikow(listaPlikow, pi);
                }
                return listaPlikow.ToArray();
            }
        }

        private void SzukajPlikow(
            List<IPlikWrapper> listaPlikow,
            ProjectItem pi)
        {
            if (JestPlikiemWProjekcie(pi))
                listaPlikow.Add(new PlikWrapper(pi));
            else
            {
                foreach (ProjectItem piDzieci in pi.ProjectItems)
                    SzukajPlikow(listaPlikow, piDzieci);
            }
        }

        private bool JestPlikiemWProjekcie(ProjectItem pi)
        {
            if (pi.FileCount != 1)
                return false;
            var fi = new FileInfo(pi.FileNames[0]);
            if (Directory.Exists(fi.FullName))
                return false;
            return true;
        }

        public IPlikWrapper DodajPlik(string sciezka)
        {
            return new PlikWrapper(
                project.ProjectItems.AddFromFile(sciezka));
        }

        public bool NamespaceNalezyDoProjektu(string nazwaNamespace)
        {
            return nazwaNamespace.ToLower().StartsWith(Nazwa.ToLower());
        }

        public IEnumerable<string> DajPlikiZNamespace(string nazwaNamespace)
        {
            var wzglednyNamespace =
                nazwaNamespace.Substring(Nazwa.Length + 1);
            string sciezkaPolozeniaPlikow = BudujSciezke(wzglednyNamespace);
            foreach (var plik in Pliki)
            {
                if (plik.SciezkaPelna.ToLower()
                    .StartsWith(sciezkaPolozeniaPlikow.ToLower()))
                    yield return plik.SciezkaPelna;
            }
        }

        private string BudujSciezke(string wzglednyNamespace)
        {
            var wynik = SciezkaDoKatalogu;
            var czesci = wzglednyNamespace.Split('.');
            foreach (var c in czesci)
            {
                if (Directory.Exists(wynik))
                    wynik = Path.Combine(wynik, c);
                else
                    wynik += "." + c;
            }

            return wynik;
        }
    }
}
