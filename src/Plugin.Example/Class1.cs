using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugin.Example
{
    class Class1
    {
        public Class1()
        {
            CSharpSyntaxTree.ParseText("a");
        }
    }
}
