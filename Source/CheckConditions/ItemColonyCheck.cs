using System;
using HumanStoryteller.Incidents;
using HumanStoryteller.Util;
using Verse;

namespace HumanStoryteller.CheckConditions {
    public class ItemColonyCheck : CheckCondition {
        public const String Name = "ItemColony";

        private ThingDef _item;
        private DataBank.CompareType _compareType;
        private float _constant;

        public ItemColonyCheck() {
        }

        public ItemColonyCheck(ThingDef item, DataBank.CompareType compareType, float constant) {
            _item = Tell.AssertNotNull(item, nameof(item), GetType().Name);
            _compareType = Tell.AssertNotNull(compareType, nameof(compareType), GetType().Name);
            _constant = Tell.AssertNotNull(constant, nameof(constant), GetType().Name);
        }

        public override bool Check(IncidentResult result, int checkPosition) {
            Map map = Find.Maps.FindAll(x => x.ParentFaction.IsPlayer).RandomElement();

            int count = map.resourceCounter.GetCount(_item);
            return DataBank.CompareValueWithConst(count, _compareType, _constant);
        }

        public override string ToString() {
            return $"Faction: {_item}, CompareType: {_compareType}, Constant: {_constant}";
        }

        public override void ExposeData() {
            base.ExposeData();
            Scribe_Defs.Look(ref _item, "item");
            Scribe_Values.Look(ref _compareType, "compareType");
            Scribe_Values.Look(ref _constant, "constant");
        }
    }
}