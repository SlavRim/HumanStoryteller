using System;
using HumanStoryteller.Incidents;
using HumanStoryteller.Util;
using Verse;

namespace HumanStoryteller.CheckConditions {
    public class ColoniesCheck : CheckCondition {
        public const String Name = "Colonies";

        private DataBank.CompareType _compareType;
        private Number _constant;

        public ColoniesCheck() {
        }

        public ColoniesCheck(DataBank.CompareType compareType, Number constant) {
            _compareType = Tell.AssertNotNull(compareType, nameof(compareType), GetType().Name);
            _constant = Tell.AssertNotNull(constant, nameof(constant), GetType().Name);
        }

        public override bool Check(IncidentResult result, int checkPosition) {           
            return DataBank.CompareValueWithConst(Find.Maps.FindAll(x => x.ParentFaction.IsPlayer).Count, _compareType, _constant.GetValue());
        }

        public override string ToString() {
            return $"CompareType: {_compareType}, Constant: {_constant}";
        }

        public override void ExposeData() {
            base.ExposeData();
            Scribe_Values.Look(ref _compareType, "compareType");
            Scribe_Deep.Look(ref _constant, "constant");
        }
    }
}