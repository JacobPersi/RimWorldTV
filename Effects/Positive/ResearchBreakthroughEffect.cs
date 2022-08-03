using RimWorld;
using Verse;
using System.Linq;

namespace RimWorldTV {

    public class ResearchBreakthroughEffect : Effect {
        public override string Code => EffectCode.ResearchBreakthrough;

        public override EffectStatus Execute(EffectCommand command) {
            Map currentMap;
            bool hasMap = ModService.Instance.TryGetColonyMap(out currentMap);
            if (hasMap == false)
                return EffectStatus.Failure;

            ResearchProjectDef researchDef = DefDatabase<ResearchProjectDef>.AllDefs?.Where(def => {
                return (def?.prerequisites == null) ? true : def.prerequisites.All(prereq => {
                    return prereq.IsFinished;
                });
            }).RandomElement();

            if (researchDef != null) {
                ModService.Instance.Game.researchManager.FinishProject(researchDef);
                string customLabel = string.Format($"{Code}.Notification".Translate(), researchDef.LabelCap);
                SendCardNotification(label: customLabel, notificationType: LetterDefOf.PositiveEvent, triggeredBy: command.viewerName);
                return EffectStatus.Success;
            }
            return EffectStatus.Failure;
        }
    }
}