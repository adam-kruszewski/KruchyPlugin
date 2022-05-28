using System;
using System.ComponentModel.DataAnnotations;

namespace KruchyParserKoduTests.Samples
{
    [Serializable]
    internal enum Enum1
    {
        [Display(Name = "aa")]
        Pierwsza = 1,
        Druga = 2
    }
}
