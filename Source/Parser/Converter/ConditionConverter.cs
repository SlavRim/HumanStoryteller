using System;
using System.Collections.Generic;
using HumanStoryteller.CheckConditions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HumanStoryteller.Parser.Converter {
    public class ConditionConverter : JsonConverter {
        public override bool CanWrite => false;
        public override bool CanRead => true;

        public override bool CanConvert(Type objectType) {
            return objectType == typeof(List<CheckCondition>);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
            throw new InvalidOperationException("Use default serialization.");
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
            List<CheckCondition> conditions = new List<CheckCondition>();
            if (reader.TokenType == JsonToken.StartObject) {
                conditions.Add(GetCondition(JObject.Load(reader)));
            } else {
                JArray array = JArray.Load(reader);
                foreach (var jToken in array) {
                    var item = (JObject) jToken;
                    conditions.Add(GetCondition(item));
                }
            }

            return conditions;
        }

        private CheckCondition GetCondition(JObject obj) {
            String type = obj["type"].Value<string>();
            if (type == null) {
                Parser.LogParseError("condition", type);
                return null;
            }

            switch (type) {
                case PawnHealthCheck.Name:
                    return new PawnHealthCheck(obj["pawnName"].Value<string>(),
                        GetHealthCondition(obj["healthCondition"].Value<string>()));
                case DialogCheck.Name:
                    return new DialogCheck(GetDialogResponse(obj["response"].Value<string>()));
                default:
                    Parser.LogParseError("condition", type);
                    return null;
            }
        }
        
        private HealthCondition GetHealthCondition(String type) {
            if (type == null) {
                Parser.LogParseError("health condition", type);
                return HealthCondition.Alive;
            }

            try {
                return PawnHealthCheck.dict[type];
            } catch (KeyNotFoundException) {
                Parser.LogParseError("health condition", type);
                return HealthCondition.Alive;
            }
        }
        
        private DialogResponse GetDialogResponse(String type) {
            if (type == null) {
                Parser.LogParseError("dialog response condition", type);
                return DialogResponse.Accepted;
            }

            try {
                return DialogCheck.dict[type];
            } catch (KeyNotFoundException) {
                Parser.LogParseError("dialog response condition", type);
                return DialogResponse.Accepted;
            }
        }
    }
}