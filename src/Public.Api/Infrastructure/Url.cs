namespace Public.Api.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using Be.Vlaanderen.Basisregisters.AggregateSource;

    public class Url : ValueObject<Url>
    {
        public string Value { get; }

        public Url(string value)
        {
            Value = value;
        }

        protected override IEnumerable<object> Reflect()
        {
            yield return Value;
        }

        public override string ToString() => Value;
    }
}
