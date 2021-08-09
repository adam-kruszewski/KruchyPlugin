using KomponentyStandardowe.Data;
using Piatka.Infrastructure.Utils;
using Piatka.Log.Attributes;
using Pincasso.Core.Attributes;
using Pincasso.Core.Base;
using Pincasso.Lokalizacje.Core.Domain;

namespace Pincasso.Kontrahenci.Core.Domain
{
    [TableDescription("v_konta", "kon_id")]
    [NazwaLogowana("Konto rozliczeniowe")]
    public class DomainObject : PincassoDomainObject
    {
        [ColumnName("kon_numer")]
        public long Numer { get; set; }

        [ForeignKey(typeof(Kontrahent))]
        [ColumnName("kon_fk_kth_id")]
        public int? KontrahentID { get; set; }

        [ReferencedObject("KontrahentID")]
        public Kontrahent Kontrahent { get; set; }

        [ForeignKey(typeof(Adres))]
        [ColumnName("kon_fk_adr_id")]
        public int? AdresID { get; set; }

        [ReferencedObject("AdresID")]
        public Adres Adres { get; set; }

        [ColumnName("kon_fk_wwl_typ")]
        public TypKontaRozliczeniowego Typ { get; set; }

        public DomainObject()
        {
            FakturowanieZbiorcze = true;
            Typ = TypKontaRozliczeniowego.Kontrahent;
        }

        public override string ToString()
        {
            return Typ == TypKontaRozliczeniowego.Kontrahent
                ? (Numer + (Kontrahent == null ? string.Empty : (" " + Kontrahent)))
                : Typ.ConvertToString();
        }
    }
}