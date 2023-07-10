﻿using Kruchy.Plugin.Utils.Wrappers;

namespace Kruchy.Plugin.Utils.Extensions
{
    public static class AkcjaExtension
    {
        public static bool CzyPlikControllera(this ISolutionWrapper solution)
        {
            var aktualny = solution.CurrentFile;
            if (aktualny == null)
                return false;

            if (!aktualny.Name.ToLower().EndsWith("controller.cs"))
                return false;

            return true;
        }

        public static string DajNazweControllera(this string nazwaKlasyControllera)
        {
            var dl = "Controller".Length;
            return nazwaKlasyControllera.Substring(
                0,
                nazwaKlasyControllera.Length - dl);
        }
    }
}
