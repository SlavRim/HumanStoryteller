using System.Collections.Generic;
using HumanStoryteller.Model;
using HumanStoryteller.Util.Overlay;
using RimWorld.Planet;
using Verse;

namespace HumanStoryteller.Util {
    public class StoryComponent : GameComponent {
        public bool Initialised;
        public Story Story;
        public long StoryId;
        public bool ForcedUpdate;
        public List<StoryEventNode> CurrentNodes = new List<StoryEventNode>();
        public List<StoryNode> AllNodes = new List<StoryNode>();
        public Dictionary<string, float> VariableBank = new Dictionary<string, float>();
        public Dictionary<string, Pawn> PawnBank = new Dictionary<string, Pawn>();
        public Dictionary<string, MapParent> MapBank = new Dictionary<string, MapParent>();
        public StorytellerComp_HumanThreatCycle ThreatCycle;
        public StoryQueue StoryQueue = new StoryQueue();
        public StoryStatus StoryStatus = new StoryStatus();
        public StoryOverlay StoryOverlay = new StoryOverlay();

        private Map _firstMapOfPlayer;

        public Map FirstMapOfPlayer {
            get => _firstMapOfPlayer ?? Find.Maps.FindAll(x => x.ParentFaction.IsPlayer).RandomElement();
            set => _firstMapOfPlayer = value;
        }

        private Map _lastColonizedMap;

        public Map LastColonizedMap {
            get => _lastColonizedMap ?? FirstMapOfPlayer;
            set => _lastColonizedMap = value;
        }

        private Map _sameAsLastEvent;

        public Map SameAsLastEvent {
            get => _sameAsLastEvent ?? FirstMapOfPlayer;
            set => _sameAsLastEvent = value;
        }

        private List<string> reservedPawnKeysWorkingList;
        private List<Pawn> reservedPawnValuesWorkingList;
        private List<string> reservedMapKeysWorkingList;
        private List<MapParent> reservedMapValuesWorkingList;

        public StoryComponent(Game game) {
            Tell.Log("StoryComponent init:" + game.DebugString());
        }

        public void Reset() {
            Initialised = false;
            Story = null;
            ForcedUpdate = false;
            CurrentNodes = new List<StoryEventNode>();
            AllNodes = new List<StoryNode>();
            VariableBank = new Dictionary<string, float>();
            PawnBank = new Dictionary<string, Pawn>();
            MapBank = new Dictionary<string, MapParent>();
            ThreatCycle = null;
            _firstMapOfPlayer = null;
            _sameAsLastEvent = null;
            _lastColonizedMap = null;
            StoryQueue = new StoryQueue();
            StoryOverlay = new StoryOverlay();
            StoryStatus = new StoryStatus();
        }

        public override void GameComponentUpdate() {
            if (Story == null || !Initialised) {
                return;
            }

            FollowThing();
            ChangeWorldView();
        }

        private void ChangeWorldView() {
            if (!StoryStatus.DisableCameraControls) {
                return;
            }

            Find.World.renderer.wantedMode = StoryStatus.ShowWorld ? WorldRenderMode.Planet : WorldRenderMode.None;
        }

        private void FollowThing() {
            if (StoryStatus.FollowThing == GlobalTargetInfo.Invalid) return;
            var target = CameraJumper.GetAdjustedTarget(StoryStatus.FollowThing);
            if (!target.IsValid) {
                Tell.Log("Invalid thing to follow, resetting...");
                StoryStatus.FollowThing = GlobalTargetInfo.Invalid;
                return;
            }

            if (target.HasThing) {
                if (Current.ProgramState != ProgramState.Playing)
                    return;
                Map mapHeld = target.Thing.MapHeld;
                if (mapHeld == null || !Find.Maps.Contains(mapHeld) || !target.Thing.DrawPos.InBounds(mapHeld))
                    return;
                if (Find.CurrentMap != mapHeld) {
                    Current.Game.CurrentMap = mapHeld;
                }

                StoryStatus.JumpException = true;
                Find.CameraDriver.JumpToCurrentMapLoc(target.Thing.DrawPos);
                StoryStatus.JumpException = false;
            } else {
                StoryStatus.JumpException = true;
                CameraJumper.TryJump(target);
                StoryStatus.JumpException = false;

            }
        }

        public override void ExposeData() {
            base.ExposeData();
            Scribe_Values.Look(ref Initialised, "initialised");
            Scribe_Deep.Look(ref Story, "story");
            Scribe_Deep.Look(ref StoryOverlay, "storyOverlay");
            Scribe_Deep.Look(ref StoryStatus, "storyStatus");
            Scribe_Values.Look(ref StoryId, "storyId");
            Scribe_Collections.Look(ref CurrentNodes, "currentNode", LookMode.Deep);
            Scribe_Collections.Look(ref AllNodes, "allNodes", LookMode.Deep);
            Scribe_Collections.Look(ref VariableBank, "variableBank", LookMode.Value, LookMode.Value);
            Scribe_Collections.Look(ref PawnBank, "pawnBank", LookMode.Value, LookMode.Reference, ref reservedPawnKeysWorkingList,
                ref reservedPawnValuesWorkingList);
            Scribe_Collections.Look(ref MapBank, "mapBank", LookMode.Value, LookMode.Reference, ref reservedMapKeysWorkingList,
                ref reservedMapValuesWorkingList);

            Scribe_References.Look(ref _firstMapOfPlayer, "firstMapOfPlayer");
            Scribe_References.Look(ref _sameAsLastEvent, "sameAsLastEvent");
            Scribe_References.Look(ref _lastColonizedMap, "lastColonizedMap");
            if (Scribe.mode == LoadSaveMode.LoadingVars && HumanStoryteller.CreatorTools) {
                Tell.Log("StoryComponent Loaded:" + ToString());
            }
        }

        public override string ToString() {
            return
                $"Initialised: {Initialised}, Story: {Story}, StoryId: {StoryId}, CurrentNodes: {CurrentNodes}, AllNodes: {AllNodes}, VariableBank: {VariableBank}, PawnBank: {PawnBank}, MapBank: {MapBank}, ThreatCycle: {ThreatCycle}, FirstMapOfPlayer: {_firstMapOfPlayer}, SameAsLastEvent: {_sameAsLastEvent}, ReservedPawnKeysWorkingList: {reservedPawnKeysWorkingList}, ReservedPawnValuesWorkingList: {reservedPawnValuesWorkingList}, ReservedMapKeysWorkingList: {reservedMapKeysWorkingList}, ReservedMapValuesWorkingList: {reservedMapValuesWorkingList}, QueueSize: {StoryQueue.Size()}";
        }
    }
}