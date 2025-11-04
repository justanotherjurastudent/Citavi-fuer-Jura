// #C5_43233_Abwandlumg
//Version 1.1

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
			
			return citation.Reference.Periodical.Name.StartsWith("JuristischeArbeitsblätter") || citation.Reference.Periodical.Name.StartsWith("Juristische Arbeitsblätter") || citation.Reference.Periodical.Name.StartsWith("JA") || citation.Reference.Periodical.StandardAbbreviation.StartsWith("JA") || 
			citation.Reference.Periodical.Name.StartsWith("JuristischeSchulung") || citation.Reference.Periodical.Name.StartsWith("Juristische Schulung") || citation.Reference.Periodical.Name.StartsWith("JuS") || citation.Reference.Periodical.StandardAbbreviation.StartsWith("JuS") ||
			citation.Reference.Periodical.Name.StartsWith("JuristischeAusbildung") || citation.Reference.Periodical.Name.StartsWith("Juristische Ausbildung") || citation.Reference.Periodical.Name.StartsWith("Jura") || citation.Reference.Periodical.Name.StartsWith("JURA") || citation.Reference.Periodical.StandardAbbreviation.StartsWith("JURA") || citation.Reference.Periodical.StandardAbbreviation.StartsWith("Jura") || 
			citation.Reference.Periodical.Name.StartsWith("JuristenZeitung") || citation.Reference.Periodical.Name.StartsWith("Juristenzeitung") || citation.Reference.Periodical.Name.StartsWith("JZ") || citation.Reference.Periodical.StandardAbbreviation.StartsWith("JZ") ||
			citation.Reference.Periodical.Name.StartsWith("Juristische Rundschau") || citation.Reference.Periodical.Name.StartsWith("JuristischeRundschau") || citation.Reference.Periodical.Name.StartsWith("JR") || citation.Reference.Periodical.StandardAbbreviation.StartsWith("JR") ||
			citation.Reference.Periodical.Name.StartsWith("Medizinrecht") || citation.Reference.Periodical.Name.StartsWith("MedR") || citation.Reference.Periodical.StandardAbbreviation.StartsWith("MedR") ||
			citation.Reference.Periodical.Name.StartsWith("Monatsschrift für Deutsches Recht") || citation.Reference.Periodical.Name.StartsWith("MDR") || citation.Reference.Periodical.StandardAbbreviation.StartsWith("MDR") ||
			citation.Reference.Periodical.Name.StartsWith("Neue Juristische Wochenschrift") || citation.Reference.Periodical.Name.StartsWith("NeueJuristischeWochenschrift") || citation.Reference.Periodical.Name.StartsWith("NJW") || citation.Reference.Periodical.StandardAbbreviation.StartsWith("NJW") ||
			citation.Reference.Periodical.Name.StartsWith("Neue Zeitschrift für Strafrecht") || citation.Reference.Periodical.Name.StartsWith("NStZ") || citation.Reference.Periodical.StandardAbbreviation.StartsWith("NStZ") ||
			citation.Reference.Periodical.Name.StartsWith("Neue Zeitschrift für Strafrecht Rechtsprechungs-Report") || citation.Reference.Periodical.Name.StartsWith("NStZ-RR") || citation.Reference.Periodical.StandardAbbreviation.StartsWith("NStZ-RR") ||
			citation.Reference.Periodical.Name.StartsWith("Neue Zeitschrift für Verwaltungsrecht") || citation.Reference.Periodical.Name.StartsWith("NVwZ") || citation.Reference.Periodical.StandardAbbreviation.StartsWith("NVwZ") ||
			citation.Reference.Periodical.Name.StartsWith("Neue Juristische Online Zeitschrift") || citation.Reference.Periodical.Name.StartsWith("NJOZ") || citation.Reference.Periodical.StandardAbbreviation.StartsWith("NJOZ") ||
			citation.Reference.Periodical.Name.StartsWith("Kommunaljurist") || citation.Reference.Periodical.Name.StartsWith("KommJur") || citation.Reference.Periodical.StandardAbbreviation.StartsWith("KommJur") ||
			citation.Reference.Periodical.Name.StartsWith("Die Polizei") || 
			citation.Reference.Periodical.Name.StartsWith("Rechtsmedizin") ||
			citation.Reference.Periodical.Name.StartsWith("Strafverteidiger") || citation.Reference.Periodical.Name.StartsWith("StV") || citation.Reference.Periodical.StandardAbbreviation.StartsWith("StV") ||
			citation.Reference.Periodical.Name.StartsWith("Zeitschrift für das Juristische Studium") || citation.Reference.Periodical.Name.StartsWith("ZJS") || citation.Reference.Periodical.StandardAbbreviation.StartsWith("ZJS") ||
			citation.Reference.Periodical.Name.StartsWith("Kritische Justiz") || citation.Reference.Periodical.Name.StartsWith("KJ") || citation.Reference.Periodical.StandardAbbreviation.StartsWith("KJ") ||
			citation.Reference.Periodical.Name.StartsWith("Wohnungswirtschaft und Mietrecht") || citation.Reference.Periodical.Name.StartsWith("WuM") || citation.Reference.Periodical.StandardAbbreviation.StartsWith("WuM");
		}
	}
}