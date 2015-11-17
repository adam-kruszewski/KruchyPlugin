// PkgCmdID.cs
// MUST match PkgCmdID.h
using System;

namespace KruchyCompany.KruchyPlugin1
{
    static class PkgCmdIDList
    {
        public const uint cmdidTestowaCommand = 0x100;
        public const uint cmdidMyTool = 0x101;
        public const uint cmdidUzupelnijAtrybutKluczaObcego = 0x0102;
        public const uint cmdidUzupelnijTagiDefiniujaceTabele = 0x0103;
        public const uint cmdidZrobKlaseTestowa = 0x0105;
        public const uint cmdidZrobKlaseService = 0x0106;
        public const uint cmdidZrobKlaseDao = 0x0107;

        //dla poruszania się po klasach ui
        public const uint cmidPrzejdzDoGridRowActions = 0x0201;
        public const uint cmidPrzejdzDoGridToolbar = 0x0202;
        public const uint cmidGenerujKatalogViewDlaControllera = 0x0203;
    };
}