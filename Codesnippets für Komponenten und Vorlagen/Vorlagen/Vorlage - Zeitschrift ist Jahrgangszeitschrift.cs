// #C5_43233_Abwandlumg
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
				Matches(p, "Archiv für die civilistische Praxis", "AcP") ||
				Matches(p, "Archiv für Kriminologie", "ArchKrim") ||
				MatchesName(p, "Arch. Krim") || MatchesName(p, "Arch Kriminol") || MatchesName(p, "Arch. Kriminol") ||
				MatchesAbbreviation(p, "Arch. Krim") || MatchesAbbreviation(p, "ArchKrim") ||
				Matches(p, "Archiv des öffentlichen Rechts", "AöR") ||
				Matches(p, "Archiv für Rechts- und Sozialphilosophie", "ARSP") ||
				Matches(p, "Archiv für Urheber- und Medienrecht", "UFITA") ||
				MatchesAbbreviation(p, "JR") ||
				Matches(p, "Medizinrecht", "MedR") ||
				MatchesAbbreviation(p, "UFITA") ||
				MatchesName(p, "Blutalkohol") ||
				Matches(p, "Monatsschrift für Kriminologie und Strafrechtsreform", "MschrKrim") ||
				Matches(p, "Rabels Zeitschrift für ausländisches und internationales Privatrecht", "RabelsZ") ||
				Matches(p, "Verwaltungsarchiv", "VerwArch") ||
				Matches(p, "Zeitschrift für ausländisches öffentliches Recht und Völkerrecht", "ZaöRV") ||
				Matches(p, "Zeitschrift für das gesamte Handels- und Wirtschaftsrecht", "ZHR") ||
				Matches(p, "Zeitschrift für die gesamte Strafrechtswissenschaft", "ZStW") ||
				Matches(p, "Zeitschrift für Zivilproze", "ZZP");
		}

		// Prueft, ob Name oder Abkuerzung mit dem jeweiligen Prefix beginnt (null-sicher)
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