using System;
using System.Collections.Generic;
using System.Text.Json;
using Modix.JsonConverters;
using NUnit.Framework;
using Shouldly;

namespace Modix.Test.JsonConverters
{
    public class EnumDictionaryConverterTests
    {
        public struct SimpleClass1
        {
            public string Item1 { get; set; }
        }

        public struct SimpleClass2
        {
            public string Item1 { get; set; }

            public int Item2 { get; set; }

            public Dictionary<DayOfWeek, string> Item3 { get; set; }

            public bool Item4 { get; set; }
        }

        public struct SimpleClass3
        {
            public string Item1 { get; set; }

            public int Item2 { get; set; }

            public Dictionary<DayOfWeek, SimpleClass1> Item3 { get; set; }

            public bool Item4 { get; set; }
        }

        [Test]
        public void Converter_GivenSimpleTopLevelDictionary_SuccessfullySerializes()
        {
            var objectToSerialize = new Dictionary<DayOfWeek, string>
            {
                [DayOfWeek.Sunday] = "It is Sunday.",
                [DayOfWeek.Monday] = "It is Monday.",
                [DayOfWeek.Tuesday] = "It is Tuesday.",
                [DayOfWeek.Wednesday] = "It is Wednesday.",
                [DayOfWeek.Thursday] = "It is Thursday.",
                [DayOfWeek.Friday] = "It is Friday.",
                [DayOfWeek.Saturday] = "It is Saturday.",
            };

            var options = new JsonSerializerOptions { WriteIndented = true };
            options.Converters.Add(new EnumDictionaryConverterFactory());

            var actual = JsonSerializer.Serialize(objectToSerialize, options);

            actual.ShouldBe(
@"{
  ""Sunday"": ""It is Sunday."",
  ""Monday"": ""It is Monday."",
  ""Tuesday"": ""It is Tuesday."",
  ""Wednesday"": ""It is Wednesday."",
  ""Thursday"": ""It is Thursday."",
  ""Friday"": ""It is Friday."",
  ""Saturday"": ""It is Saturday.""
}");
        }

        [Test]
        public void Converter_GivenSimpleNestedDictionary_SuccessfullySerializes()
        {
            var objectToSerialize = new SimpleClass2
            {
                Item1 = "SomeProperty",
                Item2 = 42,
                Item3 = new Dictionary<DayOfWeek, string>
                {
                    [DayOfWeek.Sunday] = "It is Sunday.",
                    [DayOfWeek.Monday] = "It is Monday.",
                    [DayOfWeek.Tuesday] = "It is Tuesday.",
                    [DayOfWeek.Wednesday] = "It is Wednesday.",
                    [DayOfWeek.Thursday] = "It is Thursday.",
                    [DayOfWeek.Friday] = "It is Friday.",
                    [DayOfWeek.Saturday] = "It is Saturday.",
                },
                Item4 = false
            };

            var options = new JsonSerializerOptions { WriteIndented = true };
            options.Converters.Add(new EnumDictionaryConverterFactory());

            var actual = JsonSerializer.Serialize(objectToSerialize, options);

            actual.ShouldBe(
@"{
  ""Item1"": ""SomeProperty"",
  ""Item2"": 42,
  ""Item3"": {
    ""Sunday"": ""It is Sunday."",
    ""Monday"": ""It is Monday."",
    ""Tuesday"": ""It is Tuesday."",
    ""Wednesday"": ""It is Wednesday."",
    ""Thursday"": ""It is Thursday."",
    ""Friday"": ""It is Friday."",
    ""Saturday"": ""It is Saturday.""
  },
  ""Item4"": false
}");
        }

        [Test]
        public void Converter_GivenComplexTopLevelDictionary_SuccessfullySerializes()
        {
            var objectToSerialize = new Dictionary<DayOfWeek, SimpleClass1>
            {
                [DayOfWeek.Sunday] = new SimpleClass1 { Item1 = "It is Sunday." },
                [DayOfWeek.Monday] = new SimpleClass1 { Item1 = "It is Monday." },
                [DayOfWeek.Tuesday] = new SimpleClass1 { Item1 = "It is Tuesday." },
                [DayOfWeek.Wednesday] = new SimpleClass1 { Item1 = "It is Wednesday." },
                [DayOfWeek.Thursday] = new SimpleClass1 { Item1 = "It is Thursday." },
                [DayOfWeek.Friday] = new SimpleClass1 { Item1 = "It is Friday." },
                [DayOfWeek.Saturday] = new SimpleClass1 { Item1 = "It is Saturday." },
            };

            var options = new JsonSerializerOptions { WriteIndented = true };
            options.Converters.Add(new EnumDictionaryConverterFactory());

            var actual = JsonSerializer.Serialize(objectToSerialize, options);

            actual.ShouldBe(
@"{
  ""Sunday"": {
    ""Item1"": ""It is Sunday.""
  },
  ""Monday"": {
    ""Item1"": ""It is Monday.""
  },
  ""Tuesday"": {
    ""Item1"": ""It is Tuesday.""
  },
  ""Wednesday"": {
    ""Item1"": ""It is Wednesday.""
  },
  ""Thursday"": {
    ""Item1"": ""It is Thursday.""
  },
  ""Friday"": {
    ""Item1"": ""It is Friday.""
  },
  ""Saturday"": {
    ""Item1"": ""It is Saturday.""
  }
}");
        }

        [Test]
        public void Converter_GivenComplexNestedDictionary_SuccessfullySerializes()
        {
            var objectToSerialize = new SimpleClass3
            {
                Item1 = "SomeProperty",
                Item2 = 42,
                Item3 = new Dictionary<DayOfWeek, SimpleClass1>
                {
                    [DayOfWeek.Sunday] = new SimpleClass1 { Item1 = "It is Sunday." },
                    [DayOfWeek.Monday] = new SimpleClass1 { Item1 = "It is Monday." },
                    [DayOfWeek.Tuesday] = new SimpleClass1 { Item1 = "It is Tuesday." },
                    [DayOfWeek.Wednesday] = new SimpleClass1 { Item1 = "It is Wednesday." },
                    [DayOfWeek.Thursday] = new SimpleClass1 { Item1 = "It is Thursday." },
                    [DayOfWeek.Friday] = new SimpleClass1 { Item1 = "It is Friday." },
                    [DayOfWeek.Saturday] = new SimpleClass1 { Item1 = "It is Saturday." },
                },
                Item4 = false
            };

            var options = new JsonSerializerOptions { WriteIndented = true };
            options.Converters.Add(new EnumDictionaryConverterFactory());

            var actual = JsonSerializer.Serialize(objectToSerialize, options);

            actual.ShouldBe(
@"{
  ""Item1"": ""SomeProperty"",
  ""Item2"": 42,
  ""Item3"": {
    ""Sunday"": {
      ""Item1"": ""It is Sunday.""
    },
    ""Monday"": {
      ""Item1"": ""It is Monday.""
    },
    ""Tuesday"": {
      ""Item1"": ""It is Tuesday.""
    },
    ""Wednesday"": {
      ""Item1"": ""It is Wednesday.""
    },
    ""Thursday"": {
      ""Item1"": ""It is Thursday.""
    },
    ""Friday"": {
      ""Item1"": ""It is Friday.""
    },
    ""Saturday"": {
      ""Item1"": ""It is Saturday.""
    }
  },
  ""Item4"": false
}");
        }

        [Test]
        public void Converter_GivenSimpleTopLevelDictionary_SuccessfullyDeserializes()
        {
            var json =
@"{
  ""Sunday"": ""It is Sunday."",
  ""Monday"": ""It is Monday."",
  ""Tuesday"": ""It is Tuesday."",
  ""Wednesday"": ""It is Wednesday."",
  ""Thursday"": ""It is Thursday."",
  ""Friday"": ""It is Friday."",
  ""Saturday"": ""It is Saturday.""
}";

            var expected = new Dictionary<DayOfWeek, string>
            {
                [DayOfWeek.Sunday] = "It is Sunday.",
                [DayOfWeek.Monday] = "It is Monday.",
                [DayOfWeek.Tuesday] = "It is Tuesday.",
                [DayOfWeek.Wednesday] = "It is Wednesday.",
                [DayOfWeek.Thursday] = "It is Thursday.",
                [DayOfWeek.Friday] = "It is Friday.",
                [DayOfWeek.Saturday] = "It is Saturday.",
            };

            var options = new JsonSerializerOptions { WriteIndented = true };
            options.Converters.Add(new EnumDictionaryConverterFactory());

            var actual = JsonSerializer.Deserialize<Dictionary<DayOfWeek, string>>(json, options);

            actual.ShouldBe(expected);
        }

        [Test]
        public void Converter_GivenSimpleNestedDictionary_SuccessfullyDeserializes()
        {
            var json =
@"{
  ""Item1"": ""SomeProperty"",
  ""Item2"": 42,
  ""Item3"": {
    ""Sunday"": ""It is Sunday."",
    ""Monday"": ""It is Monday."",
    ""Tuesday"": ""It is Tuesday."",
    ""Wednesday"": ""It is Wednesday."",
    ""Thursday"": ""It is Thursday."",
    ""Friday"": ""It is Friday."",
    ""Saturday"": ""It is Saturday.""
  },
  ""Item4"": false
}";

            var expected = new SimpleClass2
            {
                Item1 = "SomeProperty",
                Item2 = 42,
                Item3 = new Dictionary<DayOfWeek, string>
                {
                    [DayOfWeek.Sunday] = "It is Sunday.",
                    [DayOfWeek.Monday] = "It is Monday.",
                    [DayOfWeek.Tuesday] = "It is Tuesday.",
                    [DayOfWeek.Wednesday] = "It is Wednesday.",
                    [DayOfWeek.Thursday] = "It is Thursday.",
                    [DayOfWeek.Friday] = "It is Friday.",
                    [DayOfWeek.Saturday] = "It is Saturday.",
                },
                Item4 = false
            };

            var options = new JsonSerializerOptions { WriteIndented = true };
            options.Converters.Add(new EnumDictionaryConverterFactory());

            var actual = JsonSerializer.Deserialize<SimpleClass2>(json, options);

            actual.Item1.ShouldBe(expected.Item1);
            actual.Item2.ShouldBe(expected.Item2);
            actual.Item3.ShouldBe(expected.Item3);
            actual.Item4.ShouldBe(expected.Item4);
        }

        [Test]
        public void Converter_GivenComplexTopLevelDictionary_SuccessfullyDeserializes()
        {
            var json =
@"{
  ""Sunday"": {
    ""Item1"": ""It is Sunday.""
  },
  ""Monday"": {
    ""Item1"": ""It is Monday.""
  },
  ""Tuesday"": {
    ""Item1"": ""It is Tuesday.""
  },
  ""Wednesday"": {
    ""Item1"": ""It is Wednesday.""
  },
  ""Thursday"": {
    ""Item1"": ""It is Thursday.""
  },
  ""Friday"": {
    ""Item1"": ""It is Friday.""
  },
  ""Saturday"": {
    ""Item1"": ""It is Saturday.""
  }
}";

            var expected = new Dictionary<DayOfWeek, SimpleClass1>
            {
                [DayOfWeek.Sunday] = new SimpleClass1 { Item1 = "It is Sunday." },
                [DayOfWeek.Monday] = new SimpleClass1 { Item1 = "It is Monday." },
                [DayOfWeek.Tuesday] = new SimpleClass1 { Item1 = "It is Tuesday." },
                [DayOfWeek.Wednesday] = new SimpleClass1 { Item1 = "It is Wednesday." },
                [DayOfWeek.Thursday] = new SimpleClass1 { Item1 = "It is Thursday." },
                [DayOfWeek.Friday] = new SimpleClass1 { Item1 = "It is Friday." },
                [DayOfWeek.Saturday] = new SimpleClass1 { Item1 = "It is Saturday." },
            };

            var options = new JsonSerializerOptions { WriteIndented = true };
            options.Converters.Add(new EnumDictionaryConverterFactory());

            var actual = JsonSerializer.Deserialize<Dictionary<DayOfWeek, SimpleClass1>>(json, options);

            actual.ShouldBe(expected);
        }

        [Test]
        public void Converter_GivenComplexNestedDictionary_SuccessfullyDeserializes()
        {
            var json =
@"{
  ""Item1"": ""SomeProperty"",
  ""Item2"": 42,
  ""Item3"": {
    ""Sunday"": {
      ""Item1"": ""It is Sunday.""
    },
    ""Monday"": {
      ""Item1"": ""It is Monday.""
    },
    ""Tuesday"": {
      ""Item1"": ""It is Tuesday.""
    },
    ""Wednesday"": {
      ""Item1"": ""It is Wednesday.""
    },
    ""Thursday"": {
      ""Item1"": ""It is Thursday.""
    },
    ""Friday"": {
      ""Item1"": ""It is Friday.""
    },
    ""Saturday"": {
      ""Item1"": ""It is Saturday.""
    }
  },
  ""Item4"": false
}";

            var expected = new SimpleClass3
            {
                Item1 = "SomeProperty",
                Item2 = 42,
                Item3 = new Dictionary<DayOfWeek, SimpleClass1>
                {
                    [DayOfWeek.Sunday] = new SimpleClass1 { Item1 = "It is Sunday." },
                    [DayOfWeek.Monday] = new SimpleClass1 { Item1 = "It is Monday." },
                    [DayOfWeek.Tuesday] = new SimpleClass1 { Item1 = "It is Tuesday." },
                    [DayOfWeek.Wednesday] = new SimpleClass1 { Item1 = "It is Wednesday." },
                    [DayOfWeek.Thursday] = new SimpleClass1 { Item1 = "It is Thursday." },
                    [DayOfWeek.Friday] = new SimpleClass1 { Item1 = "It is Friday." },
                    [DayOfWeek.Saturday] = new SimpleClass1 { Item1 = "It is Saturday." },
                },
                Item4 = false
            };

            var options = new JsonSerializerOptions { WriteIndented = true };
            options.Converters.Add(new EnumDictionaryConverterFactory());

            var actual = JsonSerializer.Deserialize<SimpleClass3>(json, options);

            actual.Item1.ShouldBe(expected.Item1);
            actual.Item2.ShouldBe(expected.Item2);
            actual.Item3.ShouldBe(expected.Item3);
            actual.Item4.ShouldBe(expected.Item4);
        }
    }
}
