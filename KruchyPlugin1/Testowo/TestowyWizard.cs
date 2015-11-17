using EnvDTE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KruchyCompany.KruchyPlugin1.Testowo
{
    public class TestowyWizard : IDTWizard
    {
        public void Execute(object Application, int hwndOwner, ref object[] ContextParams, ref object[] CustomParams, ref wizardResult retval)
        {
            MessageBox.Show("Mój pierwszy wizard");
        }
    }
}
