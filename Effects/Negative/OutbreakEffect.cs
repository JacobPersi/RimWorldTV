using RimWorld;
using Verse;
using HugsLib.Settings;
using System.Linq;
using System;
using System.Collections.Generic;

namespace RimWorldTV {

    public class OutbreakEffect : Effect {
        public override string Code => EffectCode.Outbreak;

        private SettingHandle<int> MinCount;
        private SettingHandle<int> MaxCount;
        public override void RegisterSettings(ModSettingsPack Settings) {
            this.RegisterBaseSetting((Settings));
            MinCount = Settings.GetHandle<int>(
                settingName: $"Settings.{Code}.MinCount", title: "Settings.MinCount.Title".Translate(), description: "Settings.MinCount.Description".Translate(),
                defaultValue: 2);
            MaxCount = Settings.GetHandle<int>(
                settingName: $"Settings.{Code}.MaxCount", title: "Settings.MaxCount.Title".Translate(), description: "Settings.MaxCount.Description".Translate(),
                defaultValue: 5);
        }

        public override EffectStatus Execute(EffectCommand command) {
            Map currentMap;
            bool hasMap = ModService.Instance.TryGetColonyMap(out currentMap);
            if (hasMap == false)
                return EffectStatus.Failure;

            // This is pretty hacky! There is no known way to filter for infectous diseases.
            // This list will excluding non-native defs. 
            List<HediffDef> diseaseDefs = DefDatabase<HediffDef>.AllDefs.Where(def => (
                def.fileName == "Hediffs_Local_Infections.xml"
            )).ToList();

            HediffDef ilness = diseaseDefs.RandomElement();
            List<Pawn> colonists = Find.ColonistBar.GetColonistsInOrder();

            int infectionCount = ModService.Instance.Random.Next(MinCount, MaxCount);
            while (infectionCount > 0) {
                if (colonists.Count > 0) {
                    Pawn colonist = colonists.RandomElement();
                    colonists.Remove(colonist);
                    Hediff hediff = HediffMaker.MakeHediff(ilness, colonist);
                    colonist.health.AddHediff(hediff, null, null, null);

                    string label = String.Format("Notification.Indicator".Translate(), hediff.LabelCap, colonist.LabelShortCap);
                    string description = String.Format("outbreak.Letter".Translate(), colonist.LabelShortCap, hediff.LabelCap);

                    SendCardNotification(label: label, description: description, map: currentMap, location: colonist.Position, notificationType: LetterDefOf.ThreatSmall);
                }
                infectionCount--;
            }
            SendCardNotification(notificationType: LetterDefOf.ThreatBig, triggeredBy: command.viewerName);
            return EffectStatus.Success;
        }
    }
}
