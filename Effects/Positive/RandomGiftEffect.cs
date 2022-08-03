using RimWorld;
using Verse;
using HugsLib.Settings;
using System.Collections.Generic;
using System.Linq;
using System;

namespace RimWorldTV {

    public class RandomGiftEffect : Effect {
        public override string Code => EffectCode.RandomGift;

        private SettingHandle<int> MinCount;
        private SettingHandle<int> MaxCount;
        public override void RegisterSettings(ModSettingsPack Settings) {
            this.RegisterBaseSetting((Settings));
            MinCount = Settings.GetHandle<int>(
                settingName: $"Settings.{Code}.MinCount", title: "Settings.MinCount.Title".Translate(), description: "Settings.MinCount.Description".Translate(),
                defaultValue: 3);
            MaxCount = Settings.GetHandle<int>(
                settingName: $"Settings.{Code}.MaxCount", title: "Settings.MaxCount.Title".Translate(), description: "Settings.MaxCount.Description".Translate(),
                defaultValue: 5);
        }

        public override EffectStatus Execute(EffectCommand command) {
            Map currentMap;
            bool hasMap = ModService.Instance.TryGetColonyMap(out currentMap);
            if (hasMap == false)
                return EffectStatus.Failure;

            var donationItems = DefDatabase<ThingDef>.AllDefs?.Where(IsDonationItem);
            int spawnCount = ModService.Instance.Random.Next(MinCount, MaxCount);
            List<Thing> spawnItems = new List<Thing>(spawnCount);

            foreach (var i in Enumerable.Range(0, spawnCount)) {
                ThingDef itemDef = donationItems.RandomElement();
                spawnItems.Add(CreateItem(itemDef));
            }

            IntVec3 location;
            RCellFinder.TryFindRandomSpotJustOutsideColony(currentMap.areaManager.Home.ActiveCells.RandomElement(), currentMap, out location);
            DropPodUtility.DropThingsNear(location, currentMap, spawnItems);

            SendCardNotification(currentMap, location, LetterDefOf.PositiveEvent, command.viewerName);
            return EffectStatus.Success;
        }

        private Thing CreateItem(ThingDef itemDef) {
            ThingDef stuff = (itemDef.MadeFromStuff) ? GenStuff.DefaultStuffFor(itemDef) : null;
            Thing item = (Thing)Activator.CreateInstance(itemDef.thingClass);
            item.def = itemDef;
            item.SetStuffDirect(stuff);
            item.PostMake();
            item.PostPostMake();
            return item;
        }

        private static bool IsDonationItem(ThingDef thingDef) {
            return (thingDef.BaseMarketValue > 0 &&
                (thingDef.category == ThingCategory.Item || thingDef.category == ThingCategory.Building) &&
                (thingDef.category != ThingCategory.Pawn) &&
                (thingDef.IsApparel || thingDef.IsWeapon || thingDef.IsDrug || thingDef.IsIngestible || thingDef.IsMetal || thingDef.IsMedicine || thingDef.IsArt) &&
                (thingDef.IsCorpse == false));
        }
    }
}