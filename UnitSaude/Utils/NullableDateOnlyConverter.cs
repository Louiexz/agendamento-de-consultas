using System.Text.Json;
using System.Text.Json.Serialization;

namespace UnitSaude.Utils
{
    public class NullableDateOnlyConverter : JsonConverter<DateOnly?>
    {
        private const string Format = "dd/MM/yyyy";

        public override DateOnly? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var str = reader.GetString();
            return string.IsNullOrWhiteSpace(str)
                ? null
                : DateOnly.ParseExact(str, Format);
        }

        public override void Write(Utf8JsonWriter writer, DateOnly? value, JsonSerializerOptions options)
        {
            if (value.HasValue)
                writer.WriteStringValue(value.Value.ToString(Format));
            else
                writer.WriteNullValue();
        }
    }
}
