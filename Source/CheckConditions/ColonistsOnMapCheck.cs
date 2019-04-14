using System;
using System.Collections.Generic;
using System.Linq;
using HumanStoryteller.Incidents;
using HumanStoryteller.Util;
using RimWorld;
using Verse;

namespace HumanStoryteller.CheckConditions {
    public class ColonistsOnMapCheck : CheckCondition {
        public const String Name = "ColonistsOnMap";

        private string _mapName;
        private DataBank.CompareType _compareType;
        private float _constant;

        public ColonistsOnMapCheck() {
        }

        public ColonistsOnMapCheck(string mapName, DataBank.CompareType compareType, float constant) {
            _mapName = Tell.AssertNotNull(mapName, nameof(mapName), GetType().Name);
            _compareType = Tell.AssertNotNull(compareType, nameof(compareType), GetType().Name);
            _constant = Tell.AssertNotNull(constant, nameof(constant), GetType().Name);
        }

        public override bool Check(IncidentResult result, int checkPosition) {
            return DataBank.CompareValueWithConst(MapUtil.GetMapByName(_mapName, false)?.mapPawns.ColonistCount ?? 0, _compareType, _constant);
        }

        public override string ToString() {
            return $"MapName: {_mapName}, CompareType: {_compareType}, Constant: {_constant}";
        }

        public override void ExposeData() {
            base.ExposeData();
            Scribe_Values.Look(ref _mapName, "mapName");
            Scribe_Values.Look(ref _compareType, "compareType");
            Scribe_Values.Look(ref _constant, "constant");
        }
    }
}