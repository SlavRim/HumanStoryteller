using System;
using HumanStoryteller.Model;
using HumanStoryteller.Util;
using RimWorld;
using UnityEngine;
using Verse;
using Random = System.Random;

namespace HumanStoryteller.Incidents {
    class HumanIncidentWorker_Aurora : HumanIncidentWorker {
        public const String Name = "Aurora";

        public override IncidentResult Execute(HumanIncidentParms parms) {
            IncidentResult ir = new IncidentResult();

            if (!(parms is HumanIncidentParams_Aurora)) {
                Tell.Err("Tried to execute " + GetType() + " but param type was " + parms.GetType());
                return ir;
            }

            HumanIncidentParams_Aurora
                allParams = Tell.AssertNotNull((HumanIncidentParams_Aurora) parms, nameof(parms), GetType().Name);
            Tell.Log($"Executing event {Name} with:{allParams}");

            Map map = (Map) allParams.GetTarget();
            var def = IncidentDef.Named("Aurora");
            int duration = Mathf.RoundToInt(allParams.Duration != -1
                ? allParams.Duration * 60000f
                : def.durationDays.RandomInRange * 60000f);
            GameCondition_Aurora gameCondition_aurora =
                (GameCondition_Aurora) GameConditionMaker.MakeCondition(GameConditionDefOf.Aurora, duration);
            map.gameConditionManager.RegisterCondition(gameCondition_aurora);
            SendLetter(allParams, def.letterLabel, def.letterText, def.letterDef, null);
            return ir;
        }
    }

    public class HumanIncidentParams_Aurora : HumanIncidentParms {
        public float Duration;

        public HumanIncidentParams_Aurora() {
        }

        public HumanIncidentParams_Aurora(String target, HumanLetter letter, float duration = -1) : base(target,
            letter) {
            Duration = duration;
        }

        public override string ToString() {
            return $"{base.ToString()}, Duration: {Duration}";
        }

        public override void ExposeData() {
            base.ExposeData();
            Scribe_Values.Look(ref Duration, "duration");
        }
    }
}