using System;
using System.Collections.Generic;
using System.Linq;
using HumanStoryteller.Model;
using HumanStoryteller.Util;
using RimWorld;
using Verse;

namespace HumanStoryteller.Incidents {
    class HumanIncidentWorker_KillPawn : HumanIncidentWorker {
        public const String Name = "KillPawn";

        protected override IncidentResult Execute(HumanIncidentParms parms) {
            IncidentResult ir = new IncidentResult();

            if (!(parms is HumanIncidentParams_KillPawn)) {
                Tell.Err("Tried to execute " + GetType() + " but param type was " + parms.GetType());
                return ir;
            }

            HumanIncidentParams_KillPawn allParams =
                Tell.AssertNotNull((HumanIncidentParams_KillPawn) parms, nameof(parms), GetType().Name);
            Tell.Log($"Executing event {Name} with:{allParams}");

            foreach (var name in allParams.Names) {
                var pawn = PawnUtil.GetPawnByName(name);
                if (pawn == null) {
                    continue;
                }
                if (allParams.Destroy) {
                    pawn.Destroy();
                } else {
                    pawn.Kill(new DamageInfo(DamageDefOf.ExecutionCut, 5000f));
                }
            }
            
            SendLetter(parms);
            
            return ir;
        }
    }

    public class HumanIncidentParams_KillPawn : HumanIncidentParms {
        public List<String> Names = new List<string>();
        public bool Destroy;

        public HumanIncidentParams_KillPawn() {
        }

        public HumanIncidentParams_KillPawn(string target, HumanLetter letter) : base(target, letter) {
        }

        public override void ExposeData() {
            base.ExposeData();
            Scribe_Collections.Look(ref Names, "names", LookMode.Value);
            Scribe_Values.Look(ref Destroy, "destroy");
        }
    }
}