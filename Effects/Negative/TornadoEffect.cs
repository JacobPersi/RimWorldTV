using RimWorld;
using Verse;

namespace RimWorldTV {

    public class TornadoEffect : Effect {
        public override string Code => EffectCode.Tornado;

        public override EffectStatus Execute(EffectCommand command) {
            Map currentMap;
            bool hasMap = ModService.Instance.TryGetColonyMap(out currentMap);
            if (hasMap == false)
                return EffectStatus.Failure;

            IntVec3 spawnLocation = CellFinder.RandomNotEdgeCell(30, currentMap);
            GenSpawn.Spawn(ThingDefOf.Tornado, spawnLocation, currentMap);

            SendCardNotification(map: currentMap, location: spawnLocation,notificationType: LetterDefOf.ThreatBig, triggeredBy: command.viewerName);
            return EffectStatus.Success;
        }
    }
}
