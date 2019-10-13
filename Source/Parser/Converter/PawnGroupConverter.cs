using System;
using HumanStoryteller.Incidents;
using HumanStoryteller.Model.PawnGroup;
using HumanStoryteller.Model.PawnGroup.Filter;
using HumanStoryteller.Model.StoryPart;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RimWorld;

namespace HumanStoryteller.Parser.Converter {
    public class PawnGroupConverter : JsonConverter {
        public override bool CanWrite => false;
        public override bool CanRead => true;

        public override bool CanConvert(Type objectType) {
            return objectType == typeof(PawnGroupFilter);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
            throw new InvalidOperationException("Use default serialization.");
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
            var jsonObject = JObject.Load(reader);
            PawnGroupFilter pawnGroupFilter;
            String type = jsonObject["type"].Value<string>();
            if (type == null) {
                Parser.LogParseError("pawn group", type);
                return null;
            }

            switch (type) {
                case AwakeFilter.Name:
                    pawnGroupFilter = new AwakeFilter();
                    break;
                case DownedFilter.Name:
                    pawnGroupFilter = new DownedFilter();
                    break;
                case DraftedFilter.Name:
                    pawnGroupFilter = new DraftedFilter();
                    break;
                case FactionFilter.Name:
                    pawnGroupFilter = new FactionFilter();
                    break;
                case FightingFilter.Name:
                    pawnGroupFilter = new FightingFilter();
                    break;
                case IndoorsFilter.Name:
                    pawnGroupFilter = new IndoorsFilter();
                    break;
                case KindFilter.Name:
                    pawnGroupFilter = new KindFilter();
                    break;
                case MaleFilter.Name:
                    pawnGroupFilter = new MaleFilter();
                    break;
                case PrisonerFilter.Name:
                    pawnGroupFilter = new PrisonerFilter();
                    break;
                case RelationFilter.Name:
                    pawnGroupFilter = new RelationFilter();
                    break;
                case StarvingFilter.Name:
                    pawnGroupFilter = new StarvingFilter();
                    break;
                case CanReachFilter.Name:
                    pawnGroupFilter = new CanReachFilter();
                    break;
                case InBedFilter.Name:
                    pawnGroupFilter = new InBedFilter();
                    break;
                case IsKidnappedFilter.Name:
                    pawnGroupFilter = new IsKidnappedFilter();
                    break;
                case OnFireFilter.Name:
                    pawnGroupFilter = new OnFireFilter();
                    break;
                case UnderRoofFilter.Name:
                    pawnGroupFilter = new UnderRoofFilter();
                    break;
                case NaturalAgeUnderFilter.Name:
                    pawnGroupFilter = new NaturalAgeUnderFilter();
                    break;
                case PrisonerInCellFilter.Name:
                    pawnGroupFilter = new PrisonerInCellFilter();
                    break;
                case CanSeeOneOfFilter.Name:
                    pawnGroupFilter = new CanSeeOneOfFilter();
                    break;
                default:
                    Parser.LogParseError("pawn group", type);
                    return null;
            }

            serializer.Populate(jsonObject.CreateReader(), pawnGroupFilter);
            return pawnGroupFilter;
        }
    }
}