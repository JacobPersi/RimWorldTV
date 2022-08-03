using RimWorld;
using Verse;
using HugsLib.Settings;
using System.Linq;
using System.Collections.Generic;
using Verse.AI.Group;
using Verse.AI;

namespace RimWorldTV {

    public class AnimalStampedeEffect : Effect {
        public override string Code => EffectCode.AnimalStampede;

        private SettingHandle<int> MinCount;
        private SettingHandle<int> MaxCount;
        public override void RegisterSettings(ModSettingsPack Settings) {
            this.RegisterBaseSetting(Settings);
            MinCount = Settings.GetHandle<int>(
                settingName: $"Settings.{Code}.MinCount", title: "Settings.MinCount.Title".Translate(), description: "Settings.MinCount.Description".Translate(),
                defaultValue: 7);
            MaxCount = Settings.GetHandle<int>(
                settingName: $"Settings.{Code}.MaxCount", title: "Settings.MaxCount.Title".Translate(), description: "Settings.MaxCount.Description".Translate(),
                defaultValue: 13);
        }
        
        public override EffectStatus Execute(EffectCommand command) {
            Map currentMap;
            bool hasMap = ModService.Instance.TryGetColonyMap(out currentMap);
            if (hasMap == false)
                return EffectStatus.Failure;

            List<PawnKindDef> animalDefs = DefDatabase<PawnKindDef>.AllDefs?.Where(def => 
                def.RaceProps.Animal && def.RaceProps.animalType != AnimalType.Dryad)?.ToList();
            IncidentParms parms = new IncidentParms();
            parms.target = currentMap;
            parms.pawnCount = ModService.Instance.Random.Next(MinCount, MaxCount);
            parms.pawnKind = animalDefs.RandomElement();
            ModService.Instance.TryFindRandomEntryCell(currentMap, out parms.spawnCenter);

            AnimalStampedeWorker stampedeWorker = new AnimalStampedeWorker();
            stampedeWorker.def = IncidentDefOf.RaidFriendly;
            if (stampedeWorker.TryExecute(parms)) {
                SendCardNotification(map: currentMap, location: parms.spawnCenter, notificationType: LetterDefOf.NeutralEvent, triggeredBy: command.viewerName);
                return EffectStatus.Success;
            }
            return EffectStatus.Failure;
        }
    }

    public class AnimalStampedeWorker : IncidentWorker {
        private Map currentMap;
        public IntVec3 spawnLocation;
        public IntVec3 exitLocation;

        protected override bool CanFireNowSub(IncidentParms parms) {
            ModService modService = ModService.Instance;
            bool hasMap = modService.TryGetColonyMap(out currentMap);
            spawnLocation = parms.spawnCenter;
            exitLocation = new IntVec3(currentMap.Size.x - spawnLocation.x, 0, currentMap.Size.z - spawnLocation.z);
            return hasMap;
        }

        protected override bool TryExecuteWorker(IncidentParms parms) {
            if (CanFireNowSub(parms) == false)
                return false;
            List<Pawn> pawnList = new List<Pawn>(parms.pawnCount);
            foreach (var i in Enumerable.Range(0, parms.pawnCount)) {
                Pawn pawn = PawnGenerator.GeneratePawn(parms.pawnKind);
                pawnList.Add(pawn);

                IntVec3 pawnSpawnLocation = CellFinder.RandomClosewalkCellNear(spawnLocation, currentMap, 6, null);
                GenSpawn.Spawn(pawn, pawnSpawnLocation, currentMap, WipeMode.Vanish);
            }
            LordJob exitJob = new LordJob_ExitMapNear(exitLocation, LocomotionUrgency.Jog, 1f, false, false);
            LordMaker.MakeNewLord(null, exitJob, currentMap, pawnList);
            return true;
        }
    }
}