// #C5_43233
//Version 1.2

using System.Linq;
using System.Collections.Generic;
using SwissAcademic.Citavi;
using SwissAcademic.Citavi.Metadata;
using SwissAcademic.Collections;

namespace SwissAcademic.Citavi.Citations
{
	public class CustomTemplateCondition
		:
		ITemplateConditionMacro
	{
		public bool IsTemplateForReference(ConditionalTemplate template, Citation citation)
		{
			if (citation == null) return false;
			if (citation.Reference == null) return false;
			if (citation.Reference.Periodical == null) return false;

			var p = citation.Reference.Periodical;

			return
				MatchesName(p, "Bundestagsdrucksache") ||
				MatchesName(p, "Bundestagplenar") ||
				MatchesName(p, "Bundesratsdrucksache") ||
				MatchesName(p, "Bundesratplenar") ||
				MatchesName(p, "Deutscher Bundesrat Drucksache") ||
				MatchesName(p, "Deutscher Bundestag Drucksache") ||
				Matches(p, "BT-Dr") ||
				Matches(p, "BT-Plenar") ||
				Matches(p, "BR-Dr") ||
				Matches(p, "BR-Plenar") ||
				Contains(p, "StenBer") ||
				Contains(p, "PP") ||
				MatchesNameContains(p, "Stenographischer Bericht") ||
				MatchesNameEndsWith(p, "protokoll") ||
				MatchesNameEndsWith(p, "Drucksache") ||
				MatchesNameEndsWith(p, "Drs.") ||
				MatchesNameEndsWith(p, "Drucks.");
		}

		// Prueft Name und Standardabkuerzung null-sicher auf ein Praefix
		private static bool Matches(Periodical p, string prefix)
		{
			return MatchesName(p, prefix) || MatchesAbbreviation(p, prefix);
		}

		// Prueft Name und Standardabkuerzung null-sicher auf einen enthaltenen Wert
		private static bool Contains(Periodical p, string value)
		{
			return MatchesNameContains(p, value) || MatchesAbbreviationContains(p, value);
		}

		private static bool MatchesName(Periodical p, string prefix)
		{
			return p.Name != null && p.Name.StartsWith(prefix);
		}

		private static bool MatchesAbbreviation(Periodical p, string prefix)
		{
			return p.StandardAbbreviation != null && p.StandardAbbreviation.StartsWith(prefix);
		}

		private static bool MatchesNameContains(Periodical p, string value)
		{
			return p.Name != null && p.Name.Contains(value);
		}

		private static bool MatchesAbbreviationContains(Periodical p, string value)
		{
			return p.StandardAbbreviation != null && p.StandardAbbreviation.Contains(value);
		}

		private static bool MatchesNameEndsWith(Periodical p, string suffix)
		{
			return p.Name != null && p.Name.EndsWith(suffix);
		}
	}
}