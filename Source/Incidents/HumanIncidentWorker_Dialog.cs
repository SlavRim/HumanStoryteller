using System;
using System.Collections.Generic;
using HumanStoryteller.CheckConditions;
using HumanStoryteller.Model;
using HumanStoryteller.Util;
using RimWorld;
using Verse;

namespace HumanStoryteller.Incidents {
    class HumanIncidentWorker_Dialog : HumanIncidentWorker {
        public const String Name = "Dialog";

        public override IncidentResult Execute(HumanIncidentParms parms) {
            if (!(parms is HumanIncidentParams_Dialog)) {
                Tell.Err("Tried to execute " + GetType() + " but param type was " + parms.GetType());
                return null;
            }

            HumanIncidentParams_Dialog
                allParams = Tell.AssertNotNull((HumanIncidentParams_Dialog) parms, nameof(parms), GetType().Name);
            Tell.Log($"Executing event {Name} with:{allParams}");

            Map map = (Map) allParams.GetTarget();
            string title = "todo (more info inside)";
            string message = "This event should be customized in the mail tab of the event";
            LetterDef type = LetterDefOf.NeutralEvent;

            if (parms.Letter?.Type != null) {
                title = parms.Letter.Title;
                message = parms.Letter.Message;
                type = parms.Letter.Type;
            }

            ChoiceLetter_Dialog choiceLetter_Dialog = (ChoiceLetter_Dialog) LetterMaker.MakeLetter(title, message, type);
            choiceLetter_Dialog.title = title;
            choiceLetter_Dialog.radioMode = true;
            choiceLetter_Dialog.map = map;
            choiceLetter_Dialog.fee = allParams.Silver;
            choiceLetter_Dialog.report = new IncidentResult_Dialog(choiceLetter_Dialog);
            if (allParams.Duration > 0) {
                choiceLetter_Dialog.StartTimeout((int) (60000 * allParams.Duration));
            }

            Find.LetterStack.ReceiveLetter(choiceLetter_Dialog);

            return choiceLetter_Dialog.report;
        }
    }

    public class HumanIncidentParams_Dialog : HumanIncidentParms {
        public int Silver;
        public float Duration;

        public HumanIncidentParams_Dialog() {
        }

        public HumanIncidentParams_Dialog(String target, HumanLetter letter, int silver = 0, float duration = 1) : base(target, letter) {
            Silver = silver;
            Duration = duration;
        }

        public override string ToString() {
            return $"{base.ToString()}, Silver: {Silver}";
        }

        public override void ExposeData() {
            base.ExposeData();
            Scribe_Values.Look(ref Silver, "silver");
        }
    }

    public class IncidentResult_Dialog : IncidentResult {
        public ChoiceLetter_Dialog Letter;
        public DialogResponse LetterAnswer;
        
        public IncidentResult_Dialog() {
        }

        public IncidentResult_Dialog(ChoiceLetter_Dialog letter, DialogResponse letterAnswer = DialogResponse.Pending) {
            Letter = letter;
            LetterAnswer = letterAnswer;
        }

        public override void ExposeData() {
            base.ExposeData();
            Scribe_References.Look(ref Letter, "letter");
        }
    }

    public class ChoiceLetter_Dialog : ChoiceLetter {
        public Map map;
        public int fee;
        public IncidentResult_Dialog report;

        public override IEnumerable<DiaOption> Choices {
            get {
                if (!ArchivedOnly) {
                    DiaOption accept =
                        new DiaOption("RansomDemand_Accept".Translate() + (fee == 0 ? "" : " (" + fee + " " + "Silver.label".Translate() + ")")) {
                            action = delegate {
                                report.LetterAnswer = DialogResponse.Accepted;
                                TradeUtility.LaunchSilver(map, fee);
                                Find.LetterStack.RemoveLetter(this);
                            },
                            resolveTree = true
                        };
                    if (!TradeUtility.ColonyHasEnoughSilver(map, fee)) {
                        accept.Disable("NeedSilverLaunchable".Translate(fee.ToString()));
                    }

                    yield return accept;
                }

                yield return new DiaOption("Close".Translate()) {
                    action = () => {
                        report.LetterAnswer = DialogResponse.Denied;
                        Find.LetterStack.RemoveLetter(this);
                    },
                    resolveTree = true
                };
            }
        }

        public override bool CanShowInLetterStack => base.CanShowInLetterStack && Find.Maps.Contains(map);

        public override void ExposeData() {
            base.ExposeData();
            Scribe_References.Look(ref map, "map");
            Scribe_References.Look(ref report, "report");
            Scribe_Values.Look(ref fee, "fee");
        }
    }
}