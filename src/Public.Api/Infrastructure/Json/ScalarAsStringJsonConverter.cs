namespace Public.Api.Infrastructure.Json
{
    using System;
    using System.Globalization;
    using Newtonsoft.Json;

    /// <summary>
    ///     Reads a JSON scalar into a string property whatever its token type is, so a value of the wrong type reaches
    ///     the registry behind this api and is answered by its validation.
    /// </summary>
    // A string converter is registered for every string this api binds, and for a token that is not a string it
    // yields null. A required parameter then holds null, which nothing refuses on the way in - until the body is
    // serialized for the registry behind this api, where Newtonsoft does enforce it: 'Cannot write a null value for
    // property ... Property requires a value.' Nothing catches that, so a caller that sent a number where a name was
    // expected got a 500 with an empty detail and the request never left this api (GAWR-7870).
    //
    // A number or a boolean is a value, though - an invalid one for a parameter that takes a name, which is for the
    // registry behind this api to say, in its own words, as it already does for any other invalid value. So the token
    // is read as the text it was sent as, and the request goes on its way.
    //
    // Only tokens that are not strings are this converter's business. A string is handed to the converter this one
    // was put in front of, so whatever that one does with it - trimming, as it happens - keeps happening.
    //
    // An object or an array is a different matter. There is no text to read them as, so they stay what they were: a
    // body that does not match the contract, which the serializer reports and this api answers as invalid JSON.
    public sealed class ScalarAsStringJsonConverter : JsonConverter
    {
        private readonly JsonConverter? _stringConverter;

        public ScalarAsStringJsonConverter(JsonConverter? stringConverter)
        {
            _stringConverter = stringConverter;
        }

        public override bool CanWrite => false;

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(string);
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            throw new NotSupportedException($"{nameof(ScalarAsStringJsonConverter)} only reads.");
        }

        public override object? ReadJson(
            JsonReader reader,
            Type objectType,
            object? existingValue,
            JsonSerializer serializer)
        {
            switch (reader.TokenType)
            {
                case JsonToken.String:
                    return _stringConverter is not null
                        ? _stringConverter.ReadJson(reader, objectType, existingValue, serializer)
                        : reader.Value;

                case JsonToken.Null:
                case JsonToken.Undefined:
                    return null;

                case JsonToken.StartObject:
                case JsonToken.StartArray:
                    throw new JsonSerializationException(
                        $"Unexpected token {reader.TokenType} while reading a value of type {objectType.Name}.");

                default:
                    return reader.Value switch
                    {
                        null => null,
                        // Written the way JSON writes it, so 'true' reads back as the text that was sent.
                        bool boolean => boolean ? "true" : "false",
                        IFormattable formattable => formattable.ToString(null, CultureInfo.InvariantCulture),
                        var value => value.ToString()
                    };
            }
        }
    }
}
