using System;

namespace RimWorldTV {
    public class EffectCode {
        private EffectCode(string value) { Value = value; }

        public string Value { get; private set; }

        public static EffectCode AnimalSelfTame { get { return new EffectCode("animalselftame"); } }
        public static EffectCode InspireColony { get { return new EffectCode("inspirecolony"); } }
        public static EffectCode HarvestBounty { get { return new EffectCode("harvestbounty"); } }
        public static EffectCode HealingGrace { get { return new EffectCode("healinggrace"); } }
        public static EffectCode NewRecruit { get { return new EffectCode("newrecruit"); } }
        public static EffectCode RandomGift { get { return new EffectCode("randomgift"); } }
        public static EffectCode ResearchBreakthrough { get { return new EffectCode("researchbreakthrough"); } }
        public static EffectCode ResurrectColonist { get { return new EffectCode("resurrectcolonist"); } }

        public static EffectCode AnimalStampede { get { return new EffectCode("animalstampede"); } }
        public static EffectCode MeteoriteLanding { get { return new EffectCode("meteoritelanding"); } }
        public static EffectCode CatDogRain { get { return new EffectCode("catdograin"); } }
        public static EffectCode RandomQuest { get { return new EffectCode("randomquest"); } }
        public static EffectCode TradeCaravan { get { return new EffectCode("tradecaravan"); } }

        public static EffectCode DestroyHats { get { return new EffectCode("destroyhats"); } }
        public static EffectCode Infestation { get { return new EffectCode("infestation"); } }
        public static EffectCode MentalBreak { get { return new EffectCode("mentalbreak"); } }
        public static EffectCode OrbitalBarrage { get { return new EffectCode("orbitalbarrage"); } }
        public static EffectCode Outbreak { get { return new EffectCode("outbreak"); } }
        public static EffectCode Tornado { get { return new EffectCode("tornado"); } }
        public static EffectCode Wildfire { get { return new EffectCode("wildfire"); } }
        public static EffectCode WildmanHorde { get { return new EffectCode("wildmanhorde"); } }

        public static EffectCode SolarFlare { get { return new EffectCode("worldeffect_emp"); } }
        public static EffectCode HeatWave { get { return new EffectCode("worldeffect_heatwave"); } }
        public static EffectCode ColdSnap { get { return new EffectCode("worldeffect_cold"); } }
        public static EffectCode SolarEclipse { get { return new EffectCode("worldeffect_eclipse"); } }
        public static EffectCode ToxicFallout { get { return new EffectCode("worldeffect_fallout"); } }
        public static EffectCode VolcanicWinter { get { return new EffectCode("worldeffect_winter"); } }

        public override string ToString() {
            return Value;
        }

        public static implicit operator String(EffectCode category) { 
            return category.Value; 
        }

    }
}
