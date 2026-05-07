namespace SSMLEditor.Serialization.Converters;

using System;
using System.Globalization;
using System.Text.Json;

internal class CultureInfoJsonConverter : System.Text.Json.Serialization.JsonConverter<System.Globalization.CultureInfo>
{
    public override CultureInfo Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }

        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException($"Expected string for CultureInfo but got {reader.TokenType}");
        }

        var cultureName = reader.GetString();
        if (string.IsNullOrWhiteSpace(cultureName))
        {
            return null;
        }

        try
        {
            return new CultureInfo(cultureName);
        }
        catch (CultureNotFoundException ex)
        {
            throw new JsonException($"Invalid culture name '{cultureName}'", ex);
        }
    }

    public override void Write(Utf8JsonWriter writer, CultureInfo value, JsonSerializerOptions options)
    {
        if (value is null)
        {
            writer.WriteNullValue();
            return;
        }

        writer.WriteStringValue(value.Name);
    }
}
