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

        public const uint cmdidDodajUsingMapowan = 0x0142;
        public const uint cmdidDodajUsingFluentAssertion = 0x0143;
        public const uint cmdidDodajUsingBuilderow = 0x0144;
        public const uint cmdidDodajUsingLinq = 0x0145;
        public const uint cmdidZmienNaPublic = 0x0150;
        public const uint cmdidZmienNaPrivate = 0x0151;
        public const uint cmdidDodajKlaseWalidatora = 0x152;
        public const uint cmdidDodajNaczesciejUzywaneUsingi = 0x153;

        //dla poruszania się po klasach ui
        public const uint cmdidIdzDoImplementacji = 0x0170;
        public const uint cmdidIdzDoKataloguControllera = 0x0171;
        public const uint cmdidIdzDoWidoku = 0x0172;
        public const uint cmdidGenerujWidok = 0x0173;

        public const uint cmidPrzejdzDoGridRowActions = 0x0174;
        public const uint cmidPrzejdzDoGridToolbar = 0x0175;
    };
}