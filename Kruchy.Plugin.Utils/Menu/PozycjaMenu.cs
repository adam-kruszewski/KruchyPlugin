using System;
using System.Collections.Generic;
using Kruchy.Plugin.Utils.Wrappers;
using Microsoft.VisualStudio.Shell;

namespace Kruchy.Plugin.Utils.Menu
{
    public abstract class PozycjaMenu
    {
        public static Guid guidKruchyPluginCmdSetStatic;

        protected readonly ISolutionWrapper solution;
        abstract public uint MenuCommandID { get; }
        public virtual IEnumerable<WymaganieDostepnosci> Wymagania
        {
            get
            {
                return new List<WymaganieDostepnosci>();
            }
        }

        OleMenuCommand MenuItem { get; set; }

        public PozycjaMenu(ISolutionWrapper solution)
        {
            this.solution = solution;
        }

        abstract public void Execute(object sender, EventArgs args);
    }
}
