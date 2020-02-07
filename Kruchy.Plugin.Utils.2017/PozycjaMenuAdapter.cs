using System;
using System.ComponentModel.Design;
using Kruchy.Plugin.Utils.Menu;
using Kruchy.Plugin.Utils.Wrappers;
using Microsoft.VisualStudio.Shell;

namespace Kruchy.Plugin.Utils._2017
{
    public class PozycjaMenuAdapter
    {
        public static Guid guidKruchyPluginCmdSetStatic;

        private readonly IPozycjaMenu pozycjaMenu;
        private readonly ISolutionWrapper solution;

        OleMenuCommand MenuItem { get; set; }

        public PozycjaMenuAdapter(IPozycjaMenu pozycjaMenu, ISolutionWrapper solution)
        {
            this.pozycjaMenu = pozycjaMenu;
            this.solution = solution;
        }

        public void Podlacz(IMenuCommandService service)
        {
            Podlacz(service, guidKruchyPluginCmdSetStatic);
        }

        public void Podlacz(IMenuCommandService service, Guid guidKruchyPluginCmdSet)
        {
            var menuCommandID =
                new CommandID(guidKruchyPluginCmdSet, (int)pozycjaMenu.MenuCommandID);
            if (!(pozycjaMenu is IPozycjaMenuDynamicznieRozwijane))
                MenuItem = new OleMenuCommand(Execute, menuCommandID);
            else
                MenuItem = new DynamicItemMenuCommand(
                    menuCommandID, (IPozycjaMenuDynamicznieRozwijane)pozycjaMenu);
            MenuItem.BeforeQueryStatus += BeforeQueryStatus;
            service.AddCommand(MenuItem);
        }

        void BeforeQueryStatus(object sender, EventArgs e)
        {
            if (!SpelnioneWymagania())
            {
                this.MenuItem.Enabled = false;
            }
            else
            {
                this.MenuItem.Enabled = true;
            }
        }

        private bool SpelnioneWymagania()
        {
            return pozycjaMenu.SpelnioneWymaganie(solution);
        }

        public void Execute(object sender, EventArgs args)
        {
            pozycjaMenu.Execute(sender, args);
        }
    }
}