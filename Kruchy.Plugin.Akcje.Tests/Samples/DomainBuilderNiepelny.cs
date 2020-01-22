using Pincasso.Core.Tests.Builders;

namespace a1.testsBuilders
{
    public class DomainObjectBuilder : Builder<IDomainService, DomainObject>
    {
        protected override void Init()
        {
            this.Object = new DomainObject();
        }

        public DomainObjectBuilder ZLiczbaDniTerminuPlatnosci(int? liczbaDniTerminuPlatnosci)
        {
            Object.LiczbaDniTerminuPlatnosci = liczbaDniTerminuPlatnosci;
            return this;
        }

        public DomainObjectBuilder ZNazwaKorespondencyjna(string nazwaKorespondencyjna)
        {
            Object.NazwaKorespondencyjna = nazwaKorespondencyjna;
            return this;
        }

        public DomainObjectBuilder ZBlokadaOdsetek(bool blokadaOdsetek)
        {
            Object.BlokadaOdsetek = blokadaOdsetek;
            return this;
        }

        public DomainObjectBuilder ZTyp(TypKontaRozliczeniowego typ)
        {
            Object.Typ = typ;
            return this;
        }

        public DomainObjectBuilder ZNumer(long numer)
        {
            Object.Numer = numer;
            return this;
        }

        public DomainObjectBuilder ZKontrahent(Kontrahent kontrahent)
        {
            SetReferencedObject(o => o.Kontrahent, kontrahent);
            return this;
        }

        protected override void Save(IValidationResultListener validationListener)
        {
        }

    }
}