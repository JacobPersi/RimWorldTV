using RimWorld;
using Verse;
using System.Collections.Generic;

namespace RimWorldTV {

    public class InspireColonyEffect : Effect {
        public override string Code => EffectCode.InspireColony;

        public override EffectStatus Execute(EffectCommand command) {
            Map currentMap;
            bool hasMap = ModService.Instance.TryGetColonyMap(out currentMap);
            if (hasMap == false)
                return EffectStatus.Failure;

            List<Pawn> colonists = Find.ColonistBar.GetColonistsInOrder();
            List<Thing> inspiredColonists = new List<Thing>(colonists.Count);

            foreach (Pawn colonist in colonists) {
                InspirationDef inspirationDef = colonist.mindState.inspirationHandler.GetRandomAvailableInspirationDef();
                bool inspired = colonist.mindState.inspirationHandler.TryStartInspiration(inspirationDef);
                if (inspired)
                    inspiredColonists.Add(colonist);
            }

            if (inspiredColonists.Count > 0) {
                SendCardNotification(inspiredColonists, LetterDefOf.PositiveEvent, command.viewerName);
                return EffectStatus.Success;
            }
            return EffectStatus.Failure;
        }
    }
}