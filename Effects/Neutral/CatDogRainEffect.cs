using RimWorld;
using Verse;
using HugsLib.Settings;
using System.Linq;
using System.Collections.Generic;

namespace RimWorldTV {

    public class CatDogRainEffect : Effect {
        public override string Code => EffectCode.CatDogRain;
    
        private static readonly string[] CatDogLabels = { "cat", "yorkshire terrier", "labrador retriever", "husky" };
        
        private SettingHandle<int> MinCount;
        private SettingHandle<int> MaxCount;
        public override void RegisterSettings(ModSettingsPack Settings) {
            this.RegisterBaseSetting((Settings));
            MinCount = Settings.GetHandle<int>(
                settingName: $"Settings.{Code}.MinCount", title: "Settings.MinCount.Title".Translate(), description: "Settings.MinCount.Description".Translate(),
                defaultValue: 10);
            MaxCount = Settings.GetHandle<int>(
                settingName: $"Settings.{Code}.MaxCount", title: "Settings.MaxCount.Title".Translate(), description: "Settings.MaxCount.Description".Translate(),
                defaultValue: 15);
        }

        public override EffectStatus Execute(EffectCommand command) {
            Map currentMap;
            bool hasMap = ModService.Instance.TryGetColonyMap(out currentMap);
            if (hasMap == false)
                return EffectStatus.Failure;

            CellRect mapCells = CellRect.WholeMap(currentMap).ContractedBy(5);
            List<PawnKindDef> animalDefs = DefDatabase<PawnKindDef>.AllDefs.Where(def => CatDogLabels.Contains(def.label)).ToList();
            int spawnCount = ModService.Instance.Random.Next(MinCount, MaxCount);

            foreach (var i in Enumerable.Range(0, spawnCount)) {
                IntVec3 spawnLocation = IntVec3.Invalid;
                if (CellFinder.TryFindRandomCellInsideWith(mapCells, cell => cell.Roofed(currentMap) == false, out spawnLocation)) {
                    Pawn pawn = PawnGenerator.GeneratePawn(animalDefs.RandomElement());
                    //TradeUtility.SpawnDropPod(spawnLocation, currentMap, pawn);
                    SkyfallerMaker.SpawnSkyfaller(ThingDefOf.MeteoriteIncoming, pawn, spawnLocation, currentMap);
                    HealthUtility.DamageUntilDead(pawn);

                }
            }
            SendCardNotification(triggeredBy: command.viewerName);
            return EffectStatus.Success;
        }
    }
}
