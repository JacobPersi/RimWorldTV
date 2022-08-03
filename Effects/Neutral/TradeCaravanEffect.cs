using RimWorld;
using Verse;
using System.Linq;
using System.Collections.Generic;
using Verse.AI;
using Verse.AI.Group;

namespace RimWorldTV {

    public class TradeCaravanEffect : Effect {
        public override string Code => EffectCode.TradeCaravan;

        public override EffectStatus Execute(EffectCommand command) {
            Map currentMap;
            bool hasMap = ModService.Instance.TryGetColonyMap(out currentMap);
            if (hasMap == false)
                return EffectStatus.Failure;

            IncidentParms parms = StorytellerUtility.DefaultParmsNow(IncidentCategoryDefOf.FactionArrival, currentMap);

            IntVec3 loc;
            ModService.Instance.TryFindRandomRoadEntryCell(currentMap, out loc);
            parms.spawnCenter = loc;

            TraderCaravanWorker tradeWorker = new TraderCaravanWorker();
            tradeWorker.def = IncidentDefOf.TraderCaravanArrival;

            if (tradeWorker.TryExecute(parms) == false) {
                return EffectStatus.Failure;
            }

            SendCardNotification(map: currentMap, location: parms.spawnCenter, label: null, description: null, triggeredBy: command.viewerName);
            return EffectStatus.Success;
        }
    }

    public class TraderCaravanWorker : IncidentWorker {
        private Map currentMap;

        protected override bool CanFireNowSub(IncidentParms parms) {
            ModService modService = ModService.Instance;
            bool hasMap = modService.TryGetColonyMap(out currentMap);
            bool hasTradableFaction = modService.TryGetTradableFaction(out parms.faction);
            bool hasTraderKind = modService.TryFindFactionTraderKind(parms, currentMap);
            bool hasSpawnLoc = modService.TryFindRandomEntryCell(currentMap, out parms.spawnCenter);
            return hasMap && hasTradableFaction && hasSpawnLoc && hasTraderKind;
        }

        protected override bool TryExecuteWorker(IncidentParms parms) {
            if (CanFireNowSub(parms) == false)
                return false;

            List<Pawn> pawns = SpawnTraderPawns(parms);
            if (pawns.Count == 0)
                return false;

            foreach (Pawn pawn in pawns) {
                pawn.needs.food.CurLevel = pawn.needs.food.MaxLevel;
            }

            IntVec3 chillSpot;
            RCellFinder.TryFindRandomSpotJustOutsideColony(pawns[0].Position, pawns[0].MapHeld, pawns[0], out chillSpot, delegate (IntVec3 c) {
                for (int k = 0; k < pawns.Count; k++) {
                    if (!pawns[k].CanReach(c, PathEndMode.OnCell, Danger.Deadly, false, false, TraverseMode.ByPawn)) {
                        return false;
                    }
                }
                return true;
            });

            LordJob_TradeWithColony lordJob = new LordJob_TradeWithColony(parms.faction, chillSpot);
            LordMaker.MakeNewLord(parms.faction, lordJob, currentMap, pawns);
            return true;
        }

        private List<Pawn> SpawnTraderPawns(IncidentParms parms) {
            List<Pawn> list = PawnGroupMakerUtility.GeneratePawns(IncidentParmsUtility.GetDefaultPawnGroupMakerParms(PawnGroupKindDefOf.Trader, parms, true), false).ToList<Pawn>();
            foreach (Pawn pawn in list) {
                GenSpawn.Spawn(pawn, parms.spawnCenter, currentMap, WipeMode.Vanish);
                List<Pawn> storeGeneratedNeutralPawns = parms.storeGeneratedNeutralPawns;
                if (storeGeneratedNeutralPawns != null) {
                    storeGeneratedNeutralPawns.Add(pawn);
                }
            }
            return list;
        }
    }
}