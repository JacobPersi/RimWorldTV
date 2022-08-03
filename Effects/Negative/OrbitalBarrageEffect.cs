using RimWorld;
using Verse;

namespace RimWorldTV {

    public class OrbitalBarrageEffect : Effect {
        public override string Code => EffectCode.OrbitalBarrage;

        public override EffectStatus Execute(EffectCommand command) {
            Map currentMap;
            bool hasMap = ModService.Instance.TryGetColonyMap(out currentMap);
            if (hasMap == false)
                return EffectStatus.Failure;

            IntVec3 spawnLocation = CellFinder.RandomNotEdgeCell(5, currentMap);
            GenSpawn.Spawn(ThingDefOf.Bombardment, spawnLocation, currentMap);

            SendCardNotification(map: currentMap, location: spawnLocation, notificationType: LetterDefOf.ThreatBig, triggeredBy: command.viewerName);
            return EffectStatus.Success;
        }
    }
}
