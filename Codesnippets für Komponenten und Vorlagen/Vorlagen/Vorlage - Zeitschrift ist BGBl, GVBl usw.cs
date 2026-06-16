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
				Matches(p, "Bundesgesetzblatt", "BGBl") ||
				MatchesName(p, "Gesetz- und Verordnungsblatt") || MatchesName(p, "GVBl") ||
				Matches(p, "Bundessteuerblatt", "BStBl") ||
				MatchesName(p, "Reichsgesetzblatt") || MatchesName(p, "RGBl");
		}

		// Prueft Name und Standardabkuerzung null-sicher auf ein Praefix
		private static bool Matches(Periodical p, string namePrefix, string abbreviationPrefix)
		{
			return MatchesName(p, namePrefix) || MatchesName(p, abbreviationPrefix) || MatchesAbbreviation(p, abbreviationPrefix);
		}

		private static bool MatchesName(Periodical p, string prefix)
		{
			return p.Name != null && p.Name.StartsWith(prefix);
		}

		private static bool MatchesAbbreviation(Periodical p, string prefix)
		{
			return p.StandardAbbreviation != null && p.StandardAbbreviation.StartsWith(prefix);
		}
	}
}