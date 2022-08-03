using RimWorld;
using Verse;
using System;

namespace RimWorldTV {

    public class NewRecruitEffect : Effect {
        public override string Code => EffectCode.NewRecruit;

        public override EffectStatus Execute(EffectCommand command) {
            Map currentMap;
            bool hasMap = ModService.Instance.TryGetColonyMap(out currentMap);
            if (hasMap == false)
                return EffectStatus.Failure;

            IntVec3 spawnLocation;
            ModService.Instance.TryFindRandomRoadEntryCell(currentMap, out spawnLocation);
            Pawn newRecruit = PawnGenerator.GeneratePawn(PawnKindDefOf.Colonist, Faction.OfPlayer);

            if (ModService.Instance.ShouldDisplayViewerName && String.IsNullOrEmpty(command.viewerName) == false) {
                NameTriple name = newRecruit.Name as NameTriple;
                newRecruit.Name = new NameTriple(name.First, command.viewerName, name.Last);
            }
            
            GenSpawn.Spawn(newRecruit, spawnLocation, currentMap, WipeMode.Vanish);
            SendCardNotification(newRecruit, LetterDefOf.PositiveEvent, command.viewerName);
            return EffectStatus.Success;
        }
    }
}
