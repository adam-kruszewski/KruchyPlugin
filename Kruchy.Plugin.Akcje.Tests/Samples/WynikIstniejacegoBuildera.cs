using KomponentyStandardowe.Data;
using Piatka.Infrastructure.Utils;
using Piatka.Log.Attributes;
using Pincasso.Core.Attributes;
using Pincasso.Core.Base;
using Pincasso.Core.Tests.Builders;
using Pincasso.Lokalizacje.Core.Domain;

namespace a1.testsBuilders
{
    public class DomainObjectBuilder : Builder<IDomainService, DomainObject>
    {
        protected override void Init()
        {
            this.Object = new DomainObject();
        }

        public DomainObjectBuilder ZTyp(TypKontaRozliczeniowego typ)
        {
            Object.Typ = typ;
            return this;
        }

        public DomainObjectBuilder ZAdres(Adres adres)
        {
            SetReferencedObject(o => o.Adres, adres);
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