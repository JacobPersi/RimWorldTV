using HugsLib.Settings;
using HugsLib.Utils;
using System.Collections.Generic;
using Verse;
using System.Linq;
using RimWorld;
using UnityEngine;

namespace RimWorldTV {
    public class ModService : ISettingProvider {

        #region "Singleton" 
        private static readonly ModService instance = new ModService();
        public static ModService Instance { get { return instance; } }
        static ModService() { }
        private ModService() { }
        #endregion

        public System.Random Random = new System.Random();

        public ModLogger Logger { get; internal set; }
        public EffectManager EffectManager { get; internal set; }
        public Game Game { get; internal set; }

        public SettingHandle<string> Status;
        public SettingHandle<string> Hostname;
        public SettingHandle<uint> Port;
        public SettingHandle<bool> ShouldDisplayViewerName;
        public SettingHandle<bool> ShowAdvanced;

        public Dictionary<string, Effect> EffectList = new Dictionary<string, Effect>() {

            {EffectCode.AnimalSelfTame, new AnimalSelfTameEffect()},
            {EffectCode.InspireColony, new InspireColonyEffect()},
            {EffectCode.HarvestBounty, new HarvestBountyEffect()},
            {EffectCode.HealingGrace, new HealingGraceEffect()},
            {EffectCode.NewRecruit, new NewRecruitEffect()},
            {EffectCode.RandomGift, new RandomGiftEffect()},
            {EffectCode.ResearchBreakthrough, new ResearchBreakthroughEffect()},
            {EffectCode.ResurrectColonist, new ResurrectColonistEffect()},

            {EffectCode.AnimalStampede, new AnimalStampedeEffect()},
            {EffectCode.MeteoriteLanding, new MeteoriteLandingEffect()},
            {EffectCode.CatDogRain, new CatDogRainEffect()},
            {EffectCode.RandomQuest, new RandomQuestEffect()},
            {EffectCode.TradeCaravan, new TradeCaravanEffect()},

            {EffectCode.DestroyHats, new DestroyHatsEffect()},
            {EffectCode.Infestation, new InfestationEffect()},
            {EffectCode.MentalBreak, new MentalBreakEffect()},
            {EffectCode.OrbitalBarrage, new OrbitalBarrageEffect()},
            {EffectCode.Outbreak, new OutbreakEffect()},
            {EffectCode.Tornado, new TornadoEffect()},
            {EffectCode.Wildfire, new WildfireEffect()},
            {EffectCode.WildmanHorde, new WildmanHordeEffect()},

        };

        public void RegisterSettings(ModSettingsPack Settings) {
            Status = Settings.GetHandle<string>(
                settingName: "Settings.Status", title: "Settings.Status.Title".Translate(), description: "Settings.Status.Description".Translate(),
                defaultValue: "");
            Status.VisibilityPredicate = () => EffectManager != null;
            Status.CustomDrawer = rect => {
                string labelText = EffectManager.GetConnectionStatusCode();
                Widgets.Label(new Rect(rect.x, rect.y, rect.width / 2, rect.height), label: labelText.Translate());
                bool retry = Widgets.ButtonText(new Rect(rect.x + (rect.width / 2), rect.y, rect.width / 2, rect.height), label: "Settings.Status.Retry".Translate());
                if (retry) {
                    EffectManager.LoadedGame();
                }
                return false;
            };

            Hostname = Settings.GetHandle<string>(
                settingName: "Settings.Hostname", title: "Settings.Hostname.Title".Translate(), description: "Settings.Hostname.Description".Translate(),
                defaultValue: "127.0.0.1");

            Port = Settings.GetHandle<uint>(
                settingName: "Settings.Port", title: "Settings.Port.Title".Translate(), description: "Settings.Port.Description".Translate(),
                defaultValue: 43384);

            ShouldDisplayViewerName = Settings.GetHandle<bool>(
                settingName: "Settings.DisplayViewer", title: "Settings.DisplayViewer.Title".Translate(), description: "Settings.DisplayViewer.Description".Translate(),
                defaultValue: false);

            ShowAdvanced = Settings.GetHandle<bool>(
                settingName: "Settings.ShowAdvanced", title: "Settings.ShowAdvanced.Title".Translate(), description: "Settings.ShowAdvanced.Description".Translate(),
                defaultValue: false);
        }

        public bool TryGetColonyMap(out Map outMap) {
            var colonyMaps = (from x in Find.Maps where x.IsPlayerHome select x);
            outMap = colonyMaps.RandomElement();
            return colonyMaps.Any();
        }

        public bool TryFindNonPlayerEnemyHumanFaction(out Faction faction) {
            faction = Find.FactionManager.RandomEnemyFaction(allowNonHumanlike: false);
            return faction != null;
        }

        public bool TryGetTradableFaction(out Faction faction) {
            faction = Find.FactionManager.RandomNonHostileFaction(allowDefeated: false, allowNonHumanlike: false, minTechLevel: TechLevel.Industrial);
            return faction != null;
        }

        public bool TryFindRandomEntryCell(Map map, out IntVec3 cell) {
            return CellFinder.TryFindRandomEdgeCellWith((IntVec3 c) => map.reachability.CanReachColony(c), map, CellFinder.EdgeRoadChance_Ignore, out cell);
        }

        public bool TryFindRandomRoadEntryCell(Map map, out IntVec3 cell) {
            return CellFinder.TryFindRandomEdgeCellWith((IntVec3 c) => map.reachability.CanReachColony(c), map, CellFinder.EdgeRoadChance_Friendly, out cell);
        }

        public bool TryFindFactionTraderKind(IncidentParms parms, Map currentMap) {
            return parms.faction.def.caravanTraderKinds.TryRandomElementByWeight((TraderKindDef traderKindDef) => HasTraderCommonality(traderKindDef, currentMap, parms.faction), out parms.traderKind);
        }

        private float HasTraderCommonality(TraderKindDef traderKind, Map map, Faction faction) {
            if (traderKind.faction != null && faction.def != traderKind.faction) {
                return 0f;
            }


            /*
            if (ModsConfig.IdeologyActive && faction.ideos != null && traderKind.category == "Slaver") {
                using (IEnumerator<Ideo> enumerator = faction.ideos.AllIdeos.GetEnumerator()) {
                    while (enumerator.MoveNext()) {
                        if (!enumerator.Current.IdeoApprovesOfSlavery()) {
                            return 0f;
                        }
                    }
                }
            }
            if (traderKind.permitRequiredForTrading != null && !map.mapPawns.FreeColonists.Any((Pawn p) => p.royalty != null && p.royalty.HasPermit(traderKind.permitRequiredForTrading, faction))) {
                return 0f;
            }
            */

            return traderKind.CalculatedCommonality;
        }

        public void Alert(string messageCode) {
            string label = "Notification.Label".Translate().RawText;
            string translatedText = messageCode.Translate().RawText;
            Message msg = new Message($"{label} {translatedText}",
                MessageTypeDefOf.NeutralEvent);
            Messages.Message(msg);
        }
    }
}
