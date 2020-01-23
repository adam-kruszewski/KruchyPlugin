﻿using Pincasso.Core.Tests.Builders;

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

        protected override void Save(IValidationResultListener validationListener)
        {
        }

    }
}