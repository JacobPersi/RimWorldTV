using RimWorld;
using Verse;
using HugsLib.Settings;
using System.Linq;
using System.Collections.Generic;

namespace RimWorldTV {

    public class MentalBreakEffect : Effect {
        public override string Code => EffectCode.MentalBreak;

        private SettingHandle<int> MinCount;
        private SettingHandle<int> MaxCount;
        private SettingHandle<float> Factor;
        public override void RegisterSettings(ModSettingsPack Settings) {
            this.RegisterBaseSetting((Settings));
            MinCount = Settings.GetHandle<int>(
                settingName: $"Settings.{Code}.MinCount", title: "Settings.MinCount.Title".Translate(), description: "Settings.MinCount.Description".Translate(),
                defaultValue: 1);
            MaxCount = Settings.GetHandle<int>(
                settingName: $"Settings.{Code}.MaxCount", title: "Settings.MaxCount.Title".Translate(), description: "Settings.MaxCount.Description".Translate(),
                defaultValue: 2);
            Factor = Settings.GetHandle<float>(
                 settingName: $"Settings.{Code}.Size", title: "Settings.Factor.Title".Translate(), description: "Settings.Factor.Description".Translate(),
                defaultValue: 0.65f);
        }

        public override EffectStatus Execute(EffectCommand command) {
            Map currentMap;
            bool hasMap = ModService.Instance.TryGetColonyMap(out currentMap);
            if (hasMap == false)
                return EffectStatus.Failure;

            int pawnCount = ModService.Instance.Random.Next(MinCount, MaxCount);
            List<Pawn> colonists = Find.ColonistBar?.GetColonistsInOrder()?.OrderBy(pawn =>
                pawn.needs.mood.CurLevel)?.ToList();

            foreach (Pawn colonist in colonists) {
                colonist.needs.mood.CurLevel -= Factor;
                pawnCount--;
                if (pawnCount <= 0) {
                    break;
                }
            }
            SendCardNotification(notificationType: LetterDefOf.ThreatSmall, triggeredBy: command.viewerName);
            return EffectStatus.Success;
        }
    }
}