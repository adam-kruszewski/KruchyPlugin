using System;
using System.ComponentModel.Design;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
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

            if (SpecyficznaDlaPincasso())
            {
                this.MenuItem.Visible = false;
                this.MenuItem.Enabled = false;
            }
        }

        private bool SpecyficznaDlaPincasso()
        {
            var customAttributes =
                pozycjaMenu
                    .GetType()
                        .GetCustomAttributes();

            if (customAttributes
                .Where(o => o.GetType().Name == "SpecyficzneDlaPincassoAttribute")
                    .Any())
                return true;
            return false;
        }

        private bool SpelnioneWymagania()
        {
            return pozycjaMenu.SpelnioneWymaganie(solution);
        }

        public void Execute(object sender, EventArgs args)
        {
            try
            {
                pozycjaMenu.Execute(sender, args);
            }catch (Exception ex)
            {
                MessageBox.Show(ex.StackTrace);
            }
        }
    }
}