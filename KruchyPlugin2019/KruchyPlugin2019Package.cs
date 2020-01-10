using System;
using System.ComponentModel.Design;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using EnvDTE80;
using Kruchy.Plugin.Utils._2017;
using Kruchy.Plugin.Utils._2017.Wrappers;
using Kruchy.Plugin.Utils.Menu;
using KruchyCompany.KruchyPlugin1;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Task = System.Threading.Tasks.Task;
using Kruchy.Plugin.Utils._2017;
using EnvDTE;

namespace KruchyPlugin2019
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the
    /// IVsPackage interface and uses the registration attributes defined in the framework to
    /// register itself and its components with the shell. These attributes tell the pkgdef creation
    /// utility what data to put into .pkgdef file.
    /// </para>
    /// <para>
    /// To get loaded into VS, the package must be referred by &lt;Asset Type="Microsoft.VisualStudio.VsPackage" ...&gt; in .vsixmanifest file.
    /// </para>
    /// </remarks>
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [Guid(KruchyPlugin2019Package.PackageGuidString)]
    [ProvideMenuResource("Menus.ctmenu", 1)]

    //dodane atrybuty ręcznie
    [ProvideAutoLoad(VSConstants.UICONTEXT.NoSolution_string, PackageAutoLoadFlags.BackgroundLoad)]
    [ProvideAutoLoad(VSConstants.UICONTEXT.SolutionExists_string, PackageAutoLoadFlags.BackgroundLoad)]
    public sealed class KruchyPlugin2019Package : AsyncPackage
    {
        /// <summary>
        /// KruchyPlugin2019Package GUID string.
        /// </summary>
        public const string PackageGuidString = "5ffc0f84-e8b7-4742-b523-902d1f2b31e5";

        #region Package Members

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token to monitor for initialization cancellation, which can occur when VS is shutting down.</param>
        /// <param name="progress">A provider for progress updates.</param>
        /// <returns>A task representing the async work of package initialization, or an already completed task if there is none. Do not return null from this method.</returns>
        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            // When initialized asynchronously, the current thread may be a background thread at this point.
            // Do any initialization that requires the UI thread after switching to the UI thread.
            await this.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

            PozycjaMenuAdapter.guidKruchyPluginCmdSetStatic = new Guid("090c66f0-5900-4ef9-a243-d42476371281");
            var dte = (DTE2)await GetServiceAsync(typeof(SDTE));
            var sw = new SolutionWrapper(dte);
            IMenuCommandService mcs = await GetServiceAsync(typeof(IMenuCommandService)) as IMenuCommandService;
            var mcs2 = await GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;

            var wszystkieKlasy = GetType().Assembly.GetTypes();
            var klasyPozycji =
                wszystkieKlasy
                    .Where(o => typeof(IPozycjaMenu).IsAssignableFrom(o))
                        .ToList();

            foreach (var klasa in klasyPozycji)
            {
                var konstruktor = klasa.GetConstructors().Single();

                object[] parametry = new[] { sw };
                if (konstruktor.GetParameters().Length == 2)
                    parametry = new[] { sw, (object)SolutionExplorerWrapper.DajDlaSolution(sw, dte) };

                var pozycjaMenu = Activator.CreateInstance(klasa, parametry) as IPozycjaMenu;
                new PozycjaMenuAdapter(pozycjaMenu, sw).Podlacz(mcs2);
            }

            CommandID dynamicItemRootId = new CommandID(
                PozycjaMenuAdapter.guidKruchyPluginCmdSetStatic, 
                (int)PkgCmdIDList.cmdidMyDynamicStartCommand);

            var dynamicMenuCommand =
                new DynamicItemMenuCommand(dynamicItemRootId,
                IsValidDynamicItem,
                OnInvokedDynamicItem,
                OnBeforeQueryStatusDynamicItem);
            mcs2.AddCommand(dynamicMenuCommand);

            await Command1.InitializeAsync(this);
        }

        private bool IsValidDynamicItem(int commandId)
        {
            var dte2 = (DTE2)GetService(typeof(SDTE));
            // The match is valid if the command ID is >= the id of our root dynamic start item
            // and the command ID minus the ID of our root dynamic start item
            // is less than or equal to the number of projects in the solution.
            return (commandId >= (int)PkgCmdIDList.cmdidMyDynamicStartCommand) &&
                ((commandId - (int)PkgCmdIDList.cmdidMyDynamicStartCommand) <
                    miasta.Count());
                    //dte2.Solution.Projects.Count);
        }

        private void OnInvokedDynamicItem(object sender, EventArgs args)
        {
            var dte2 = (DTE2)GetService(typeof(SDTE));
            DynamicItemMenuCommand invokedCommand = (DynamicItemMenuCommand)sender;
            // If the command is already checked, we don't need to do anything
            if (invokedCommand.Checked)
                return;

            // Find the project that corresponds to the command text and set it as the startup project
            var projects = dte2.Solution.Projects;
            foreach (Project proj in projects)
            {
                if (invokedCommand.Text.Equals(proj.Name))
                {
                    dte2.Solution.SolutionBuild.StartupProjects = proj.FullName;
                    return;
                }
            }
        }

        private static string[] miasta = { "Warszawa", "Kraków", "Gdańsk", "Wrocław" };

        private void OnBeforeQueryStatusDynamicItem(object sender, EventArgs args)
        {
            var dte2 = (DTE2)GetService(typeof(SDTE));

            DynamicItemMenuCommand matchedCommand = (DynamicItemMenuCommand)sender;
            matchedCommand.Enabled = true;
            matchedCommand.Visible = true;

            // Find out whether the command ID is 0, which is the ID of the root item.
            // If it is the root item, it matches the constructed DynamicItemMenuCommand,
            // and IsValidDynamicItem won't be called.
            bool isRootItem = (matchedCommand.MatchedCommandId == 0);

            // The index is set to 1 rather than 0 because the Solution.Projects collection is 1-based.
            int indexForDisplay = (isRootItem ? 1 : (matchedCommand.MatchedCommandId - 
                (int)PkgCmdIDList.cmdidMyDynamicStartCommand) + 1);

            matchedCommand.Text =
                miasta[indexForDisplay - 1];
            //dte2.Solution.Projects.Item(indexForDisplay).Name;

            Array startupProjects = (Array)dte2.Solution.SolutionBuild.StartupProjects;
            string startupProject = System.IO.Path.GetFileNameWithoutExtension((string)startupProjects.GetValue(0));

            // Check the command if it isn't checked already selected
            matchedCommand.Checked =
            //(matchedCommand.Text == startupProject);
            matchedCommand.Checked = (matchedCommand.Text == "Warszawa");

            // Clear the ID because we are done with this item.
            matchedCommand.MatchedCommandId = 0;
        }
        #endregion
    }
}
