using RimWorld;
using Verse;

namespace RimWorldTV {

    public class RandomQuestEffect : Effect {
        public override string Code => EffectCode.RandomQuest;

        public override EffectStatus Execute(EffectCommand command) {
            Map currentMap;
            bool hasMap = ModService.Instance.TryGetColonyMap(out currentMap);
            if (hasMap == false)
                return EffectStatus.Failure;

            IncidentParms parms = StorytellerUtility.DefaultParmsNow(IncidentCategoryDefOf.GiveQuest, currentMap);
            IncidentWorker_GiveQuest worker = new IncidentWorker_GiveQuest();
            worker.def = IncidentDefOf.GiveQuest_Random;
            if (worker.TryExecute(parms) == false) {
                return EffectStatus.Failure;
            }

            SendCardNotification(triggeredBy: command.viewerName);
            return EffectStatus.Success;
        }
    }
}