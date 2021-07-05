using RimWorld;
using System;
using UnityEngine;
using Verse;

namespace RGWF_Wasteland
{
	public class IncidentWorker_DeathclawsPasses : IncidentWorker
	{
		protected override bool CanFireNowSub(IncidentParms parms)
		{
			Map map = (Map)parms.target;
			if (map.gameConditionManager.ConditionIsActive(GameConditionDefOf.ToxicFallout))
			{
				return true;
			}
			if (!map.mapTemperature.SeasonAndOutdoorTemperatureAcceptableFor(ThingDefOf.Thrumbo))
			{
				return false;
			}
			IntVec3 cell;
			return TryFindEntryCell(map, out cell);
		}

		protected override bool TryExecuteWorker(IncidentParms parms)
		{
			Map map = (Map)parms.target;
			if (map.Biome.defName != "RG-W_Wasteland")
			{
				return false;
			}
			if (!TryFindEntryCell(map, out IntVec3 cell))
			{
				return false;
			}
			PawnKindDef deathClaws = PawnKindDef.Named("RG-WF_WastelandDeathclaw");
			int value = GenMath.RoundRandom(StorytellerUtility.DefaultThreatPointsNow(map) / deathClaws.combatPower);
			int max = Rand.RangeInclusive(3, 6);
			value = Mathf.Clamp(value, 2, max);
			int num = Rand.RangeInclusive(90000, 150000);
			IntVec3 result = IntVec3.Invalid;
			if (!RCellFinder.TryFindRandomCellOutsideColonyNearTheCenterOfTheMap(cell, map, 10f, out result))
			{
				result = IntVec3.Invalid;
			}
			Pawn pawn = null;
			for (int i = 0; i < value; i++)
			{
				IntVec3 loc = CellFinder.RandomClosewalkCellNear(cell, map, 10);
				pawn = PawnGenerator.GeneratePawn(deathClaws);
				GenSpawn.Spawn(pawn, loc, map, Rot4.Random);
				pawn.mindState.exitMapAfterTick = Find.TickManager.TicksGame + num;
				if (result.IsValid)
				{
					pawn.mindState.forcedGotoPosition = CellFinder.RandomClosewalkCellNear(result, map, 10);
				}
			}
			SendStandardLetter("LetterLabelDeathclawsPasses".Translate(deathClaws.label).CapitalizeFirst(), "LetterDeathclawsPasses".Translate(deathClaws.label), LetterDefOf.PositiveEvent, parms, pawn);
			return true;
		}

		private bool TryFindEntryCell(Map map, out IntVec3 cell)
		{
			return RCellFinder.TryFindRandomPawnEntryCell(out cell, map, CellFinder.EdgeRoadChance_Animal + 0.2f);
		}
	}
}
