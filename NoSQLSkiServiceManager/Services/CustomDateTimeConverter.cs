using System.Text.Json;

namespace NoSQLSkiServiceManager.Services
{
    /// <summary>
    /// Custom JSON converter for <see cref="DateTime"/> objects.
    /// </summary>
    /// <remarks>
    /// This converter ensures that <see cref="DateTime"/> objects are serialized and deserialized
    /// using a specific format ("yyyy-MM-dd").
    /// </remarks>
    public class CustomDateTimeConverter : System.Text.Json.Serialization.JsonConverter<DateTime>
    {
        private const string DateFormat = "yyyy-MM-dd";

        /// <summary>
        /// Reads and converts the JSON to <see cref="DateTime"/>.
        /// </summary>
        /// <param name="reader">The reader to read the JSON from.</param>
        /// <param name="typeToConvert">The type of object to convert.</param>
        /// <param name="options">Options for the converter.</param>
        /// <returns>A <see cref="DateTime"/> object.</returns>
        /// <remarks>
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return DateTime.Parse(reader.GetString() ?? string.Empty);
        }

        /// <summary>
        /// Writes a <see cref="DateTime"/> object to JSON.
        /// </summary>
        /// <param name="writer">The writer to write the JSON to.</param>
        /// <param name="value">The <see cref="DateTime"/> value to write.</param>
        /// <param name="options">Options for the converter.</param>
        /// <remarks>
        /// This method converts a <see cref="DateTime"/> object into a JSON string using the specified format.
        /// </remarks>
        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString(DateFormat));
        }
    }
}
