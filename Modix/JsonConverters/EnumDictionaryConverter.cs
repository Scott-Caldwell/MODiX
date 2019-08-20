using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Humanizer;

namespace Modix.JsonConverters
{
    public class EnumDictionaryConverterFactory : JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert.IsGenericType
                && typeToConvert.GetGenericTypeDefinition() == _dictionaryTypeDefinition
                && typeToConvert.GenericTypeArguments[0].IsEnum;
        }

        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            var converterType = _converterTypeDefinition.MakeGenericType(
                typeToConvert.GenericTypeArguments[0],
                typeToConvert.GenericTypeArguments[1]);

            var converter = (JsonConverter)Activator.CreateInstance(
                converterType,
                BindingFlags.Public | BindingFlags.Instance,
                binder: null,
                Array.Empty<object>(),
                culture: null);

            return converter;
        }

        private static readonly Type _dictionaryTypeDefinition = typeof(Dictionary<,>);
        private static readonly Type _converterTypeDefinition = typeof(EnumDictionaryConverter<,>);
    }

    public class EnumDictionaryConverter<TEnum, TValue>
        : JsonConverter<Dictionary<TEnum, TValue>> where TEnum : struct, Enum
    {
        public override Dictionary<TEnum, TValue> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException($"Cannot convert a {reader.TokenType} token to {typeToConvert.Name}.");
            }

            var dictionary = new Dictionary<TEnum, TValue>();

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    return dictionary;
                }

                if (reader.TokenType != JsonTokenType.PropertyName)
                {
                    throw new JsonException($"Unexpected {reader.TokenType} token.");
                }

                var keyString = reader.GetString();

                if (!Enum.TryParse<TEnum>(keyString, out var key))
                {
                    throw new JsonException($"Cannot convert {keyString} to {typeof(TEnum).Name}.");
                }

                if (!reader.Read())
                {
                    throw new JsonException("Unexpected end of JSON.");
                }

                TValue value;

                if (options?.GetConverter(_valueType) is JsonConverter<TValue> valueConverter)
                {
                    value = valueConverter.Read(ref reader, _valueType, options);
                }
                else
                {
                    value = JsonSerializer.Deserialize<TValue>(ref reader, options);
                }

                dictionary[key] = value;
            }

            throw new JsonException("Unexpected end of JSON.");
        }

        public override void Write(Utf8JsonWriter writer, Dictionary<TEnum, TValue> dictionary, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            foreach (var (key, value) in dictionary)
            {
                writer.WritePropertyName(key.ToString());

                if (options?.GetConverter(_valueType) is JsonConverter<TValue> valueConverter)
                {
                    valueConverter.Write(writer, value, options);
                }
                else
                {
                    JsonSerializer.Serialize(writer, value, options);
                }
            }

            writer.WriteEndObject();
        }

        private static readonly Type _valueType = typeof(TValue);
    }
}
