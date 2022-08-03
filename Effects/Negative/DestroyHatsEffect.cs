using RimWorld;
using Verse;

namespace RimWorldTV {

    public class DestroyHatsEffect : Effect {
        public override string Code => EffectCode.DestroyHats;

        public override EffectStatus Execute(EffectCommand command) {
            Map currentMap;
            bool hasMap = ModService.Instance.TryGetColonyMap(out currentMap);
            if (hasMap == false)
                return EffectStatus.Failure;

            foreach (Pawn pawn in PawnsFinder.AllMaps) {
                if (pawn.RaceProps.Humanlike) {
                    for (int i = pawn.apparel.WornApparel.Count - 1; i >= 0; i--) {
                        Apparel apparel = pawn.apparel.WornApparel[i];
                        if (apparel.def.apparel.bodyPartGroups.Contains(BodyPartGroupDefOf.FullHead) || apparel.def.apparel.bodyPartGroups.Contains(BodyPartGroupDefOf.UpperHead)) {
                            apparel.Destroy(DestroyMode.Vanish);
                        }
                    }
                }
            }
            SendCardNotification(notificationType: LetterDefOf.ThreatSmall, triggeredBy: command.viewerName);
            return EffectStatus.Success;
        }
    }
}