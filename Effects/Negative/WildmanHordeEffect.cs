using RimWorld;
using Verse;
using HugsLib.Settings;
using System.Linq;
using System.Collections.Generic;
using Verse.AI.Group;

namespace RimWorldTV {

    public class WildmanHordeEffect : Effect {
        public override string Code => EffectCode.WildmanHorde;

        private SettingHandle<int> MinCount;
        private SettingHandle<int> MaxCount;
        public override void RegisterSettings(ModSettingsPack Settings) {
            this.RegisterBaseSetting((Settings));
            MinCount = Settings.GetHandle<int>(
                settingName: $"Settings.{Code}.MinCount", title: "Settings.MinCount.Title".Translate(), description: "Settings.MinCount.Description".Translate(),
                defaultValue: 7);
            MaxCount = Settings.GetHandle<int>(
                settingName: $"Settings.{Code}.MaxCount", title: "Settings.MaxCount.Title".Translate(), description: "Settings.MaxCount.Description".Translate(),
                defaultValue: 15);
        }

        public override EffectStatus Execute(EffectCommand command) {
            Map currentMap;
            bool hasMap = ModService.Instance.TryGetColonyMap(out currentMap);
            if (hasMap == false)
                return EffectStatus.Failure;

            IncidentParms parms = StorytellerUtility.DefaultParmsNow(IncidentCategoryDefOf.ThreatBig, currentMap);
            parms.pawnCount = ModService.Instance.Random.Next(MinCount, MaxCount);

            WildmanHordeWorker wildmanHorde = new WildmanHordeWorker();
            wildmanHorde.def = IncidentDefOf.RaidEnemy;

            if (wildmanHorde.TryExecute(parms) == false)
                return EffectStatus.Failure;

            SendCardNotification(map: currentMap, location: wildmanHorde.spawnLocation, notificationType: LetterDefOf.ThreatBig, triggeredBy: command.viewerName);
            return EffectStatus.Success;
        }
    }

    public class WildmanHordeWorker : IncidentWorker {
        public IntVec3 spawnLocation;

        private Map currentMap;
        private Faction wildmenFaction;

        protected override bool CanFireNowSub(IncidentParms parms) {
            ModService modService = ModService.Instance;
            bool hasMap = modService.TryGetColonyMap(out currentMap);
            bool hasNonPlayerHumanFaction = modService.TryFindNonPlayerEnemyHumanFaction(out wildmenFaction);
            bool hasSpawnLocation = modService.TryFindRandomEntryCell(currentMap, out spawnLocation);
            return hasMap && hasSpawnLocation && hasNonPlayerHumanFaction && base.CanFireNowSub(parms);
        }

        protected override bool TryExecuteWorker(IncidentParms parms) {
            if (CanFireNowSub(parms) == false)
                return false;

            List<Pawn> pawnList = new List<Pawn>(parms.pawnCount);
            foreach (var i in Enumerable.Range(0, parms.pawnCount)) {
                IntVec3 pawnSpawnLocation = CellFinder.RandomClosewalkCellNear(spawnLocation, currentMap, 12, null);
                Pawn pawn = PawnGenerator.GeneratePawn(PawnKindDefOf.WildMan, wildmenFaction);
                pawnList.Add(pawn);
                GenSpawn.Spawn(pawn, pawnSpawnLocation, currentMap, WipeMode.Vanish);
            }
            LordJob assaultJob = new LordJob_AssaultColony(wildmenFaction, canTimeoutOrFlee: true, canKidnap: true, canSteal: true, canPickUpOpportunisticWeapons: true);
            LordMaker.MakeNewLord(wildmenFaction, assaultJob, currentMap, pawnList);
            return true;
        }
    }
}
