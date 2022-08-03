using System;
using System.Collections.Generic;
using CrowdControl.Common;
using CrowdControl.Games.Packs;
using ConnectorType = CrowdControl.Common.ConnectorType;

public class RimWorld : SimpleTCPPack {

    public override string Host { get; } = "0.0.0.0";
    public override ushort Port { get; } = 43384;

    public RimWorld(IPlayer player, Func<CrowdControlBlock, bool> responseHandler, Action<object> statusUpdateHandler) : base(player, responseHandler, statusUpdateHandler) { }

    public override Game Game { get; } = new Game(90, "RimWorldTV", "RimWorld", "PC", ConnectorType.SimpleTCPConnector);

    public override List<Effect> Effects => new List<Effect>
    {
        new Effect("Positive Effects","positive", ItemKind.Folder),
        new Effect("Animal Self Tame", "animalselftame","positive"),
        new Effect("Colony Inspiration", "inspirecolony","positive"),
        new Effect("Harvest Bountry", "harvestbounty","positive"),
        new Effect("Healing Grace", "healinggrace","positive"),
        new Effect("New Recruit", "newrecruit","positive"),
        new Effect("Random Gift", "randomgift","positive"),
        new Effect("Research Breakthrough", "researchbreakthrough","positive"),
        new Effect("Resurrect Colonist", "resurrectcolonist","positive"),

        new Effect("Neutral Effects","neutral", ItemKind.Folder),
        new Effect("Animal Stampede", "animalstampede","neutral"),
        new Effect("Meteorite Crash Landing", "meteoritelanding","neutral"),
        new Effect("Raining Cats and Dogs", "catdograin","neutral"),
        new Effect("Random Quest", "randomquest","neutral"),
        new Effect("Trade Caravan", "tradecaravan","neutral"),

        new Effect("Negative Effects","negative", ItemKind.Folder),
        new Effect("Destroy Hats", "destroyhats","negative"),
        new Effect("Infestation", "infestation","negative"),
        new Effect("Mental Break", "mentalbreak","negative"),
        new Effect("Orbital Barrage", "orbitalbarrage","negative"),
        new Effect("Outbreak", "outbreak","negative"),
        new Effect("Tornado", "tornado","negative"),
        new Effect("Wildfire", "wildfire","negative"),
        new Effect("Wildman Horde", "wildmanhorde","negative"),

    };
}
