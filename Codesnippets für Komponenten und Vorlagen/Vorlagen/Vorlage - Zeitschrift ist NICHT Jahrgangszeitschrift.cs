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
				Matches(p, "Juristische Arbeitsblätter", "JA") || MatchesName(p, "JuristischeArbeitsblätter") ||
				Matches(p, "Juristische Schulung", "JuS") || MatchesName(p, "JuristischeSchulung") ||
				Matches(p, "Juristische Ausbildung", "JURA") || MatchesName(p, "JuristischeAusbildung") ||
				MatchesName(p, "Jura") || MatchesAbbreviation(p, "Jura") ||
				Matches(p, "Juristenzeitung", "JZ") || MatchesName(p, "JuristenZeitung") ||
				Matches(p, "Juristische Rundschau", "JR") || MatchesName(p, "JuristischeRundschau") ||
				Matches(p, "Medizinrecht", "MedR") ||
				Matches(p, "Monatsschrift für Deutsches Recht", "MDR") ||
				Matches(p, "Neue Juristische Wochenschrift", "NJW") || MatchesName(p, "NeueJuristischeWochenschrift") ||
				Matches(p, "Neue Zeitschrift für Strafrecht", "NStZ") ||
				Matches(p, "Neue Zeitschrift für Strafrecht Rechtsprechungs-Report", "NStZ-RR") ||
				Matches(p, "Neue Zeitschrift für Verwaltungsrecht", "NVwZ") ||
				Matches(p, "Neue Juristische Online Zeitschrift", "NJOZ") ||
				Matches(p, "Kommunaljurist", "KommJur") ||
				MatchesName(p, "Die Polizei") ||
				MatchesName(p, "Rechtsmedizin") ||
				Matches(p, "Strafverteidiger", "StV") ||
				Matches(p, "Zeitschrift für das Juristische Studium", "ZJS") ||
				Matches(p, "Kritische Justiz", "KJ") ||
				Matches(p, "Wohnungswirtschaft und Mietrecht", "WuM");
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