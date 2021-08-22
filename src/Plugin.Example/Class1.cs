using Microsoft.CodeAnalysis.CSharp;

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
