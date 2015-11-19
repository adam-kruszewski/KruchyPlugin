using EnvDTE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KruchyCompany.KruchyPlugin1.Utils
{
    public class PlikWrapper
    {
        private Document document;

        public PlikWrapper(EnvDTE.Document document)
        {
            this.document = document;
        }
        public string Nazwa
        {
            get
            {
                return document.Name;
            }
        }

        public string NazwaBezRozszerzenia
        {
            get
            {
                var index = Nazwa.LastIndexOf('.');
                if (index < 0)
                    return Nazwa;
                else
                    return Nazwa.Substring(0, index);
            }
        }

        public string SciezkaPelna
        {
            get { return document.FullName; }
        }

        public string SciezkaWzgledna { get; set; }

        public ProjektWrapper Projekt
        {
            get
            {
                return new ProjektWrapper(document.ProjectItem.ContainingProject);
            }
        }
    }
}
