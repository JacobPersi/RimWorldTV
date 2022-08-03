using RimWorld;
using Verse;
using HugsLib.Settings;
using System.Collections.Generic;

namespace RimWorldTV {

    public class WildfireEffect : Effect {
        public override string Code => EffectCode.Wildfire;

        private SettingHandle<int> MinCount;
        private SettingHandle<int> MaxCount;
        private SettingHandle<float> Size;
        public override void RegisterSettings(ModSettingsPack Settings) {
            this.RegisterBaseSetting(Settings);
            MinCount = Settings.GetHandle<int>(
                settingName: $"Settings.{Code}.MinCount", title: "Settings.MinCount.Title".Translate(), description: "Settings.MinCount.Description".Translate(),
                defaultValue: 3);
            MaxCount = Settings.GetHandle<int>(
                settingName: $"Settings.{Code}.MaxCount", title: "Settings.MaxCount.Title".Translate(), description: "Settings.MaxCount.Description".Translate(),
                defaultValue: 6);
            Size = Settings.GetHandle<float>(
                settingName: $"Settings.{Code}.Size", title: "Settings.Size.Title".Translate(), description: "Settings.Size.Description".Translate(),
                defaultValue: 10);
        }

        public override EffectStatus Execute(EffectCommand command) {
            Map currentMap;
            bool hasMap = ModService.Instance.TryGetColonyMap(out currentMap);
            if (hasMap == false)
                return EffectStatus.Failure;

            int fireCount = ModService.Instance.Random.Next(MinCount, MaxCount);
            List<TargetInfo> fireLocations = new List<TargetInfo>(fireCount);

            while (fireCount > 0) {
                IntVec3 targetLocation = CellFinder.RandomCell(currentMap);
                targetLocation = CellFinder.StandableCellNear(targetLocation, currentMap, 30.0f);
                bool wasSuccessful = FireUtility.TryStartFireIn(targetLocation, currentMap, Size);
                if (wasSuccessful) {
                    fireLocations.Add(new TargetInfo(targetLocation, currentMap));
                    fireCount--;
                }
            }

            SendCardNotification(lookAtThings: fireLocations, notificationType: LetterDefOf.ThreatSmall, triggeredBy: command.viewerName); 
            return EffectStatus.Success;
        }
    }
}