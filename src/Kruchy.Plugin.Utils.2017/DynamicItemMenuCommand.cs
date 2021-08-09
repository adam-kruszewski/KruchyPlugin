using System;
using System.ComponentModel.Design;
using System.Linq;
using System.Windows.Forms;
using EnvDTE80;
using Kruchy.Plugin.Utils.Menu;
using Microsoft.VisualStudio.Shell;

namespace Kruchy.Plugin.Utils._2017
{
    public class DynamicItemMenuCommand : OleMenuCommand
    {
        private Predicate<int> matches;
        private CommandID rootID;
        private IPozycjaMenuDynamicznieRozwijane pozycjaMenu;

        internal IPozycjaMenuDynamicznieRozwijane Pozycja
        {
            get { return pozycjaMenu; }
        }

        public DynamicItemMenuCommand(
            CommandID rootId,
            Predicate<int> matches,
            EventHandler invokeHandler,
            EventHandler beforeQueryStatusHandler)
            : base(invokeHandler, null /*changeHandler*/, beforeQueryStatusHandler, rootId)
        {
            if (matches == null)
            {
                throw new ArgumentNullException("matches");
            }

            this.matches = matches;
        }

        public DynamicItemMenuCommand(
            CommandID rootID,
            IPozycjaMenuDynamicznieRozwijane pozycjaMenu)
            : base(
                  OnInvokedDynamicItem,
                  null /*changeHandler*/,
                  OnBeforeQueryStatusDynamicItem,
                  rootID)
        {
            this.rootID = rootID;
            this.pozycjaMenu = pozycjaMenu;
        }

        private bool IsValidDynamicItem(int commandId)
        {
            return (commandId >= rootID.ID) && (commandId <= pozycjaMenu.OstanieCommandID);
        }

        public override bool DynamicItemMatch(int commandID)
        {
            // Call the supplied predicate to test whether the given cmdId is a match.
            // If it is, store the command id in MatchedCommandid
            // for use by any BeforeQueryStatus handlers, and then return that it is a match.
            // Otherwise clear any previously stored matched cmdId and return that it is not a match.
            if (IsValidDynamicItem(commandID))
            {
                this.MatchedCommandId = commandID;
                return true;
            }

            this.MatchedCommandId = 0;
            return false;
        }

        private static void OnInvokedDynamicItem(object sender, EventArgs args)
        {
            DynamicItemMenuCommand invokedCommand = (DynamicItemMenuCommand)sender;
            var matchedCommandID = invokedCommand.MatchedCommandId;

            invokedCommand.Pozycja.WykonajPodakcje(matchedCommandID);
        }

        private static void OnBeforeQueryStatusDynamicItem(object sender, EventArgs args)
        {
            DynamicItemMenuCommand matchedCommand = (DynamicItemMenuCommand)sender;
            matchedCommand.Enabled = true;
            matchedCommand.Visible = true;

            // Find out whether the command ID is 0, which is the ID of the root item.
            // If it is the root item, it matches the constructed DynamicItemMenuCommand,
            // and IsValidDynamicItem won't be called.
            bool isRootItem = (matchedCommand.MatchedCommandId == 0);

            // The index is set to 1 rather than 0 because the Solution.Projects collection is 1-based.
            int indexForDisplay = (isRootItem ? 0 : (matchedCommand.MatchedCommandId -
                (int)matchedCommand.Pozycja.MenuCommandID));

            var pozycje = matchedCommand.Pozycja.DajPozycje();

            if (pozycje.Any())
            {
                var pozycjaDlaIndeksu = pozycje.ToArray()[indexForDisplay];

                matchedCommand.Text = pozycjaDlaIndeksu.Tekst;
            }

            if (isRootItem && !pozycje.Any())
                matchedCommand.Enabled = false;

            matchedCommand.Enabled =
                matchedCommand.Pozycja.DostepnaPodakcja(matchedCommand.MatchedCommandId);
            //matchedCommand.Text =
            //    miasta[indexForDisplay - 1];
            ////dte2.Solution.Projects.Item(indexForDisplay).Name;

            //// Clear the ID because we are done with this item.
            matchedCommand.MatchedCommandId = 0;
        }

    }
}