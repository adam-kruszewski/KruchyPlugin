using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.ComponentModel.Design;
using Microsoft.Win32;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using EnvDTE80;
using EnvDTE;
using System.Windows;
using KruchyCompany.KruchyPlugin1.Akcje;
using KruchyCompany.KruchyPlugin1.Extensions;
using KruchyCompany.KruchyPlugin1.Utils;
using KruchyCompany.KruchyPlugin1.Interfejs;
using System.Text;
using KruchyCompany.KruchyPlugin1.KonfiguracjaPlugina;
using KruchyCompany.KruchyPlugin1.Menu;

namespace KruchyCompany.KruchyPlugin1
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    ///
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the 
    /// IVsPackage interface and uses the registration attributes defined in the framework to 
    /// register itself and its components with the shell.
    /// </summary>
    // This attribute tells the PkgDef creation utility (CreatePkgDef.exe) that this class is
    // a package.
    [PackageRegistration(UseManagedResourcesOnly = true)]
    // This attribute is used to register the information needed to show this package
    // in the Help/About dialog of Visual Studio.
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    // This attribute is needed to let the shell know that this package exposes some menus.
    [ProvideMenuResource("Menus.ctmenu", 1)]
    // This attribute registers a tool window exposed by this package.
    [ProvideToolWindow(typeof(MyToolWindow))]
    [Guid(GuidList.guidKruchyPlugin1PkgString)]
    [ProvideLoadKey("Standard", "1.0", "KruchyPlugin1", "Adam Kruszewski", 104)]
    //ręcznie dodane atrybuty
    [ProvideAutoLoad("{f1536ef8-92ec-443c-9ed7-fdadf150da82}")]
    public sealed class KruchyPlugin1Package : Package
    {
        /// <summary>
        /// Default constructor of the package.
        /// Inside this method you can place any initialization code that does not require 
        /// any Visual Studio service because at this point the package object is created but 
        /// not sited yet inside Visual Studio environment. The place to do all the other 
        /// initialization is the Initialize method.
        /// </summary>
        public KruchyPlugin1Package()
        {
            Debug.WriteLine("BBBBB ");
            Debug.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering constructor for: {0}", this.ToString()));
        }

        /// <summary>
        /// This function is called when the user clicks the menu item that shows the 
        /// tool window. See the Initialize method to see how the menu item is associated to 
        /// this function using the OleMenuCommandService service and the MenuCommand class.
        /// </summary>
        private void ShowToolWindow(object sender, EventArgs e)
        {
            // Get the instance number 0 of this tool window. This window is single instance so this instance
            // is actually the only one.
            // The last flag is set to true so that if the tool window does not exists it will be created.
            ToolWindowPane window = this.FindToolWindow(typeof(MyToolWindow), 0, true);
            if ((null == window) || (null == window.Frame))
            {
                throw new NotSupportedException(Resources.CanNotCreateWindow);
            }
            IVsWindowFrame windowFrame = (IVsWindowFrame)window.Frame;
            Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(windowFrame.Show());
        }


        /////////////////////////////////////////////////////////////////////////////
        // Overridden Package Implementation
        #region Package Members

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            Debug.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering Initialize() of: {0}", this.ToString()));
            base.Initialize();

            // Add our command handlers for menu (commands must exist in the .vsct file)
            OleMenuCommandService mcs = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;

            if (null != mcs)
            {
                // Create the command for the menu item.
                CommandID menuCommandID =
                    new CommandID(
                        GuidList.guidKruchyPlugin1CmdSet,
                        (int)PkgCmdIDList.cmdidTestowaCommand);
                MenuCommand menuItem = new MenuCommand(MenuItemCallback, menuCommandID);
                mcs.AddCommand(menuItem);

                new PozycjaUzupelnianieReferencedObject(
                    DajSolution(), (DTE2)GetService(typeof(SDTE)))
                        .Podlacz(mcs);

                new PozycjaUzupelnianieTagowDefiniujacychTabele(
                    DajSolution())
                        .Podlacz(mcs);

                new PozycjaGenerowanieKlasyTestowej(DajSolution())
                    .Podlacz(mcs);

                new PozycjaGenerowanieKlasService(DajSolution())
                    .Podlacz(mcs);

                new PozycjaGenerowanieKlasyWalidatora(DajSolution())
                    .Podlacz(mcs);

                new PozycjaDodawanieNowegoTestu(DajSolution())
                    .Podlacz(mcs);

                new PozycjaDodawanieUsingow(DajSolution())
                    .Podlacz(mcs);

                new PozycjaZmienNaPublic(DajSolution()).Podlacz(mcs);
                new PozycjaZmienNaPrivate(DajSolution()).Podlacz(mcs);

                new PozycjaDodawanieUprawnienDomyslnych(DajSolution()).Podlacz(mcs);

                new PozycjaUzupelnianieKontruktora(DajSolution()).Podlacz(mcs);

                new PozycjaDodawanieNowejMetodyWBuilderze(DajSolution())
                    .Podlacz(mcs);

                new PozycjaUzupelnianieMetodWImplementacji(DajSolution())
                    .Podlacz(mcs);

                PodlaczDoMenu(
                    mcs,
                    PkgCmdIDList.cmdidIdzDoImplementacji,
                    MenuItemIdzDoImplementacjiLubInterfejsu);
                PodlaczDoMenu(
                    mcs,
                    PkgCmdIDList.cmdidIdzDoKlasyTestowej,
                    MenuItemIdzDoKlasyTestowej);

                PodlaczDoMenu(
                    mcs,
                    PkgCmdIDList.cmdidIdzDoKataloguControllera,
                    MenuItemIdzDoKataloguControllera);
                PodlaczDoMenu(
                    mcs,
                    PkgCmdIDList.cmidPrzejdzDoGridRowActions,
                    MenuItemIdzDoGridRowActions);
                PodlaczDoMenu(
                    mcs,
                    PkgCmdIDList.cmidPrzejdzDoGridToolbar,
                    MenuItemIdzDoGridToolbar);
                PodlaczDoMenu(
                    mcs,
                    PkgCmdIDList.cmdidIdzDoWidoku,
                    MenuItemIdzDoWidoku);
                PodlaczDoMenu(
                    mcs,
                    PkgCmdIDList.cmdidGenerujWidok,
                    MenuItemGenerujWidok);
                PodlaczDoMenu(
                    mcs,
                    PkgCmdIDList.cmdidWstawDoSchowkaNazweControllera,
                    MenuItemWstawDoSchowkaNazweControllera);

                // Create the command for the tool window
                CommandID toolwndCommandID = new CommandID(GuidList.guidKruchyPlugin1CmdSet, (int)PkgCmdIDList.cmdidMyTool);
                MenuCommand menuToolWin = new MenuCommand(ShowToolWindow, toolwndCommandID);
                mcs.AddCommand(menuToolWin);
            }
        }

        private void PodlaczDoMenu(
            OleMenuCommandService mcs,
            uint commandIDZPkgCmdIDList,
            EventHandler handler)
        {
            var commandID =
                new CommandID(
                    GuidList.guidKruchyPlugin1CmdSet,
                    (int)commandIDZPkgCmdIDList);
            var menuCommand =
                new MenuCommand(
                    handler,
                    commandID);
            mcs.AddCommand(menuCommand);
        }
        #endregion

        /// <summary>
        /// This function is the callback used to execute a command when the a menu item is clicked.
        /// See the Initialize method to see how the menu item is associated to this function using
        /// the OleMenuCommandService service and the MenuCommand class.
        /// </summary>
        private void MenuItemCallback(object sender, EventArgs e)
        {
            // Show a Message Box to prove we were here
            IVsUIShell uiShell = (IVsUIShell)GetService(typeof(SVsUIShell));
            IntPtr dialogHwnd;
            uiShell.GetDialogOwnerHwnd(out dialogHwnd);
            Guid clsid = Guid.Empty;
            //int result;
            //wizardResult res = wizardResult.wizardResultSuccess;
            //var p1 = new object[0];
            //var p2 = new object[0];
            //new KruchyCompany.KruchyPlugin1.Testowo.TestowyWizard().Execute(
            //    null, (int)dialogHwnd, ref p1, ref p2, ref res);

            var dte = (DTE2)GetService(typeof(SDTE));
            var sol = (_Solution)dte.Solution;
            var proj = sol.Projects.Item(1);
            proj.ProjectItems.AddFromFile(
                "c:\\adam\\projekty\\testy\\Kruchy.Test\\a1.cs");
            //Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(uiShell.ShowMessageBox(
            //           0,
            //           ref clsid,
            //           "KruchyPlugin1",
            //           string.Format(CultureInfo.CurrentCulture, "Inside {0}.MenuItemCallback()", this.ToString()),
            //           string.Empty,
            //           0,
            //           OLEMSGBUTTON.OLEMSGBUTTON_OK,
            //           OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST,
            //           OLEMSGICON.OLEMSGICON_INFO,
            //           0,        // false
            //           out result));
        }

        private void MenuItemIdzDoImplementacjiLubInterfejsu(
            object sender, EventArgs args)
        {
            new IdzMiedzyInterfejsemAImplementacja(DajSolution()).Przejdz();
        }

        private void MenuItemIdzDoKlasyTestowej(object sender, EventArgs e)
        {
            new IdzDoKlasyTestowej(DajSolution()).Przejdz();
        }

        public void MenuItemIdzDoKataloguControllera(object sender, EventArgs args)
        {
            new IdzDoKataloguControllera(DajSolution()).Przejdz();
        }

        public void MenuItemIdzDoGridRowActions(object sender, EventArgs args)
        {
            new IdzDoPlikuWidoku(DajSolution())
                .PrzejdzLubStworz("GridRowActions.cshtml");
        }

        private void MenuItemIdzDoGridToolbar(object sender, EventArgs e)
        {
            new IdzDoPlikuWidoku(DajSolution())
                .PrzejdzLubStworz("GridToolbar.cshtml");
        }

        private void MenuItemIdzDoWidoku(object sender, EventArgs e)
        {
            new IdzDoPlikuWidoku(DajSolution()).PrzejdzDoWidokuDlaAktualnejMetody();
        }

        private void MenuItemGenerujWidok(object sender, EventArgs e)
        {
            var dialog = new NazwaKlasyWindow(false);
            dialog.EtykietaNazwyPliku = "Nazwa widoku";
            dialog.InicjalnaWartosc = DajSolution().NazwaAktualnejMetody();
            dialog.ShowDialog();

            if (!string.IsNullOrEmpty(dialog.NazwaPliku))
            {
                new GenerowanieWidoku(DajSolution()).Generuj(dialog.NazwaPliku);
            }
        }

        private void MenuItemWstawDoSchowkaNazweControllera(
            object sender,
            EventArgs e)
        {
            Clipboard.SetText("Przykładowy tekst");
            new WstawianieNazwyControlleraDoSchowka(DajSolution()).Wstaw();
        }

        void slnExplUIHierarchyExample(DTE2 dte)
        {
            UIHierarchy UIH = dte.ToolWindows.SolutionExplorer;
            // Requires a reference to System.Text.
            // Set a reference to the first level nodes in Solution Explorer. 
            // Automation collections are one-based.
            UIHierarchyItem UIHItem = UIH.UIHierarchyItems.Item(1);
            StringBuilder sb = new StringBuilder();

            // Iterate through first level nodes.
            foreach (UIHierarchyItem fid in UIHItem.UIHierarchyItems)
            {
                sb.AppendLine(fid.Name);
                // Iterate through second level nodes (if they exist).
                foreach (UIHierarchyItem subitem in fid.UIHierarchyItems)
                {
                    sb.AppendLine("   " + subitem.Name);
                    // Iterate through third level nodes (if they exist).
                    foreach (UIHierarchyItem subSubItem in
                      subitem.UIHierarchyItems)
                    {
                        sb.AppendLine("        " + subSubItem.Name);
                    }
                }
            }
            var zaw = sb.ToString();
        }

        #region [POMOCNICZE]
        private IntPtr ParentHwnd()
        {
            IVsUIShell uiShell = (IVsUIShell)GetService(typeof(SVsUIShell));
            IntPtr dialogHwnd;
            uiShell.GetDialogOwnerHwnd(out dialogHwnd);
            return dialogHwnd;
        }

        private SolutionWrapper DajSolution()
        {
            var dte = (DTE2)GetService(typeof(SDTE));
            return new SolutionWrapper(dte);
        }

        private DokumentWrapper DajAktualnyDokument()
        {
            var dte = (DTE2)GetService(typeof(SDTE));
            var textDoc = (TextDocument)dte.ActiveDocument.Object("TextDocument");
            return new DokumentWrapper(textDoc);
        }
        #endregion
    }
}
