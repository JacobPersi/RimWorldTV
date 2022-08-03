using RimWorld;
using Verse;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;

namespace RimWorldTV {

    public class InfestationEffect : Effect {
        public override string Code => EffectCode.Infestation;

        public override EffectStatus Execute(EffectCommand command) {
            Map currentMap;
            bool hasMap = ModService.Instance.TryGetColonyMap(out currentMap);
            if (hasMap == false)
                return EffectStatus.Failure;

            InfestationWorker infestation = new InfestationWorker();
            infestation.def = IncidentDefOf.Infestation;

            IncidentParms parms = StorytellerUtility.DefaultParmsNow(IncidentCategoryDefOf.ThreatBig, currentMap);
         
            IntVec3 location;
            bool validLocation = InfestationCellFinder.TryFindCell(out location, currentMap);
            if (validLocation == false) {

                IntVec3 nucleationSite;
                RCellFinder.TryFindRandomCellOutsideColonyNearTheCenterOfTheMap(CellFinder.RandomCell(currentMap), currentMap, 45, out nucleationSite);

                validLocation = RCellFinder.TryFindRandomCellNearWith(nucleationSite, (IntVec3 x) => {
                    bool isFogged = x.Fogged(currentMap);
                    bool isStandable = x.Standable(currentMap);
                    bool isRoofed = x.Roofed(currentMap);

                    return isFogged && isStandable == false && isRoofed;
                }, currentMap, out location);

            }
            
            if (validLocation) { 
                parms.spawnCenter = location;
                if (infestation.TryExecute(parms)) {
                    SendCardNotification(notificationType: LetterDefOf.ThreatBig, triggeredBy: command.viewerName);
                    return EffectStatus.Success;
                }
            }
            return EffectStatus.Failure;
        }
    }

    public class InfestationWorker : IncidentWorker {
        public const float HivePoints = 220f;

        protected override bool CanFireNowSub(IncidentParms parms) {
            Map map = (Map)parms.target;
            return base.CanFireNowSub(parms) && Faction.OfInsects != null && HiveUtility.TotalSpawnedHivesCount(map) < 30;
        }

        protected override bool TryExecuteWorker(IncidentParms parms) {
            Map map = (Map)parms.target;
            int hiveCount = Mathf.Max(GenMath.RoundRandom(parms.points / 220f), 1);

            List<IntVec3> spawnCells = GenRadial.RadialCellsAround(parms.spawnCenter, 3, true).ToList();
            spawnCells.ForEach(cell => {
                map.thingGrid.ThingAt(cell, ThingCategory.Building)?.Destroy(DestroyMode.Vanish);
            });

            Thing t = InfestationUtility.SpawnTunnels(hiveCount, map, true, overrideLoc: parms.spawnCenter);
            Find.TickManager.slower.SignalForceNormalSpeedShort();
            return true;
        }
    }
}