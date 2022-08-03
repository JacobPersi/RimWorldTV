using RimWorld;
using Verse;
using System.Linq;

namespace RimWorldTV {

    public class AnimalSelfTameEffect : Effect {
        public override string Code => EffectCode.AnimalSelfTame;

        public override EffectStatus Execute(EffectCommand command) {
			Map currentMap;
			bool hasMap = ModService.Instance.TryGetColonyMap(out currentMap);
            if (hasMap == false)
                return EffectStatus.Failure;

            Pawn animalToTame = currentMap.mapPawns.AllPawns?.Where(pawn => TameUtility.CanTame(pawn)).RandomElement();
			if (animalToTame != null) {
				if (animalToTame.guest != null) {
					animalToTame.guest.SetGuestStatus(null, GuestStatus.Guest);
				}
				animalToTame.SetFaction(Faction.OfPlayer, null);
				SendCardNotification(animalToTame, LetterDefOf.PositiveEvent, command.viewerName);
				return EffectStatus.Success;
			}
			return EffectStatus.Failure;
		}
    }
}
