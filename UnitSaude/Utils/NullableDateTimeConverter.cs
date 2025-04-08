using System.Text.Json.Serialization;
using System.Text.Json;

namespace UnitSaude.Utils
{
    public class NullableDateTimeConverter : JsonConverter<DateTime?>
    {
        private const string Format = "dd/MM/yyyy";

        public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            => string.IsNullOrWhiteSpace(reader.GetString())
               ? null
               : DateTime.ParseExact(reader.GetString(), Format, null);

        public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
            => writer.WriteStringValue(value?.ToString(Format));
    }
}
