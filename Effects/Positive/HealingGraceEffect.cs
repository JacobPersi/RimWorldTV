using RimWorld;
using Verse;
using System.Linq;
using System.Collections.Generic;

namespace RimWorldTV {

    public class HealingGraceEffect : Effect {
        public override string Code => EffectCode.HealingGrace;

        public override EffectStatus Execute(EffectCommand command) {
            Map currentMap;

            bool hasMap = ModService.Instance.TryGetColonyMap(out currentMap);
            if (hasMap == false)
                return EffectStatus.Failure;

            List<HediffDef> diseaseDefs = DefDatabase<HediffDef>.AllDefs?.Where(def => (
                def.isBad
            )).ToList();

            List<Pawn> colonists = Find.ColonistBar.GetColonistsInOrder();
            List<Thing> curedColonists = new List<Thing>(colonists.Count);
            colonists?.ForEach(colonist => colonist.health?.hediffSet?.hediffs.FindAll(hediff =>
                diseaseDefs.Contains(hediff.def)).ForEach(hediff => {
                    colonist.health.RemoveHediff(hediff);
                    curedColonists.Add(colonist);
                })
            );

            if (curedColonists.Count > 0) {
                SendCardNotification(curedColonists, LetterDefOf.PositiveEvent, command.viewerName);
                return EffectStatus.Success;
            }
            return EffectStatus.Failure;

        }
    }
}