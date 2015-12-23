using System;
using System.Diagnostics;
using System.Globalization;
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
using KruchyCompany.KruchyPlugin1.Utils;
using KruchyCompany.KruchyPlugin1.Interfejs;
using System.Text;

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

                var menuCommandUzupReferencje =
                    new CommandID(
                        GuidList.guidKruchyPlugin1CmdSet,
                        (int)PkgCmdIDList.cmdidUzupelnijAtrybutKluczaObcego);
                var mcUzupelnijReferencje =
                    new MenuCommand(
                        MenuItemUzupelnijReferencjeDlaKluczaObcegoCallback,
                        menuCommandUzupReferencje);
                mcs.AddCommand(mcUzupelnijReferencje);

                var menuCommandUzupTagiTabeli =
                    new CommandID(
                        GuidList.guidKruchyPlugin1CmdSet,
                        (int)PkgCmdIDList.cmdidUzupelnijTagiDefiniujaceTabele);
                var mcUzupelnijTagiTabeli =
                    new MenuCommand(
                        MenuItemUzupelnijTagiDefiniujaceTabele,
                        menuCommandUzupTagiTabeli);
                mcs.AddCommand(mcUzupelnijTagiTabeli);

                var menuStworzKlaseTestowa =
                    new CommandID(
                        GuidList.guidKruchyPlugin1CmdSet,
                        (int)PkgCmdIDList.cmdidZrobKlaseTestowa);
                var mcStworzKlaseTestowa =
                    new MenuCommand(
                        MenuItemZrobKlaseTestowa,
                        menuStworzKlaseTestowa);
                mcs.AddCommand(mcStworzKlaseTestowa);

                PodlaczDoMenu(
                    mcs,
                    PkgCmdIDList.cmdidZrobKlaseService,
                    MenuItemZrobKlaseService);

                PodlaczDoMenu(
                    mcs,
                    PkgCmdIDList.cmdidDodajNaczesciejUzywaneUsingi,
                    MenuItemDodajNajczesciejUzywaneUsingi);
                PodlaczDoMenu(
                    mcs,
                    PkgCmdIDList.cmdidDodajNowyTest,
                    MenuItemDodajNowyTest);

                PodlaczDoMenu(
                    mcs,
                    PkgCmdIDList.cmdidZmienNaPublic,
                    MenuItemZmienNaPublic);
                PodlaczDoMenu(
                    mcs,
                    PkgCmdIDList.cmdidZmienNaPrivate,
                    MenuItemZmienNaPrivate);
                PodlaczDoMenu(
                    mcs,
                    PkgCmdIDList.cmdidDodajKlaseWalidatora,
                    MenuItemZrobKlaseWalidatora);
                PodlaczDoMenu(
                    mcs,
                    PkgCmdIDList.cmdidDodajUprawnienieDomyslne,
                    MenuItemDodajUprawnieniaDomyslne);
                PodlaczDoMenu(
                    mcs,
                    PkgCmdIDList.cmdidUzupelnijKontruktor,
                    MenuItemUzupelnijKonstruktor);

                PodlaczDoMenu(
                    mcs,
                    PkgCmdIDList.cmdidIdzDoImplementacji,
                    MenuItemIdzDoImplementacjiLubInterfejsu);
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

        private void MenuItemUzupelnijReferencjeDlaKluczaObcegoCallback(
            object sender, EventArgs args)
        {
            var dte = (DTE2)GetService(typeof(SDTE));
            new UzupelnianieReferencedObject(dte).Uzupelnij();
        }

        private void MenuItemUzupelnijTagiDefiniujaceTabele(
            object sender, EventArgs args)
        {
            var dte = (DTE2)GetService(typeof(SDTE));
            var textDoc = (TextDocument)dte.ActiveDocument.Object("TextDocument");
            var dokumentWrapper = new DokumentWrapper(textDoc);
            new UzupelnianieTagowDefiniujacychTabele(dokumentWrapper).Uzupelnij();

        }

        private void MenuItemZrobKlaseTestowa(
            object sender, EventArgs args)
        {
            var dialog = new NazwaKlasyTestowForm(DajSolution());
            dialog.ShowDialog();

            if (string.IsNullOrEmpty(dialog.NazwaKlasy))
                return;

            new GenerowanieKlasyTestowej(DajSolution())
                .Generuj(
                    dialog.NazwaKlasy,
                    dialog.Rodzaj,
                    dialog.InterfejsTestowany);
        }

        private void MenuItemZrobKlaseService(object sender, EventArgs args)
        {
            var dte = (DTE2)GetService(typeof(SDTE));
            var n = dte.ActiveWindow.ProjectItem;
            var dialog = new NazwaKlasyWindow();
            dialog.EtykietaNazwyPliku = "Nazwa pliku implementacji serwisu";
            dialog.ShowDialog();
            if (string.IsNullOrEmpty(dialog.NazwaPliku))
                return;

            var solution = new SolutionWrapper(dte);
            var g = new GenerowanieKlasService(solution);

            g.Generuj(solution.AktualnyPlik, dialog.NazwaPliku);
        }

        private void MenuItemDodajNajczesciejUzywaneUsingi(
            object sender, EventArgs args)
        {
            new DodawaniaUsinga(DajSolution())
                .Dodaj(
                "Piatka.Infrastructure.Mappings",
                "FluentAssertions",
                "Piatka.Infrastructure.Tests.Builders",
                "System.Linq");
        }

        private void MenuItemDodajNowyTest(object sender, EventArgs e)
        {
            var dialog = new NazwaKlasyWindow();
            dialog.EtykietaNazwyPliku = "Nazwa metody testu";
            dialog.ShowDialog();
            if (string.IsNullOrEmpty(dialog.NazwaPliku))
                return;

            new DodawanieNowegoTestu(DajSolution())
                .DodajNowyTest(dialog.NazwaPliku);
        }

        private void MenuItemZmienNaPublic(object sender, EventArgs args)
        {
            new ZmianaModyfikatoraMetody(DajAktualnyDokument()).ZmienNa("public");
        }

        private void MenuItemZmienNaPrivate(object sender, EventArgs args)
        {
            new ZmianaModyfikatoraMetody(DajAktualnyDokument()).ZmienNa("private");
        }

        private void MenuItemZrobKlaseWalidatora(object sender, EventArgs args)
        {
            var nazwaPlikuDoWalidacji =
                DajSolution().AktualnyPlik.NazwaBezRozszerzenia;
            var dialog = new NazwaKlasyWindow();
            dialog.EtykietaNazwyPliku = "Nazwa klasy implementacji walidatora";
            dialog.InicjalnaWartosc = nazwaPlikuDoWalidacji + "Validator";
            dialog.ShowDialog();
            if (!string.IsNullOrEmpty(dialog.NazwaPliku))
                new GenerowanieKlasyWalidatora(DajSolution())
                    .Generuj(dialog.NazwaPliku, nazwaPlikuDoWalidacji);
        }

        private void MenuItemDodajUprawnieniaDomyslne(object sender, EventArgs e)
        {
            new DodawanieUprawnienDomyslnych(DajSolution()).Dodaj();
        }

        private void MenuItemIdzDoImplementacjiLubInterfejsu(
            object sender, EventArgs args)
        {
            new IdzMiedzyInterfejsemAImplementacja(DajSolution()).Przejdz();
        }

        public void MenuItemIdzDoKataloguControllera(object sender, EventArgs args)
        {
            new IdzDoKataloguControllera(DajSolution()).Przejdz();
        }

        public void MenuItemIdzDoGridRowActions(object sender, EventArgs args)
        {
            new IdzDoPlikuWidoku("GridRowActions.cshtml", DajSolution())
                .PrzejdzLubStworz();
        }

        private void MenuItemIdzDoGridToolbar(object sender, EventArgs e)
        {
            new IdzDoPlikuWidoku("GridToolbar.cshtml", DajSolution())
                .PrzejdzLubStworz();
        }

        private void MenuItemUzupelnijKonstruktor(object sender, EventArgs e)
        {
            MessageBox.Show("Jeszcze nie zaimplementowane");
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
