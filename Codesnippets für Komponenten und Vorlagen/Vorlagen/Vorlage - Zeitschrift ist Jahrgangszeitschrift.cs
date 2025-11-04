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
			
			return citation.Reference.Periodical.Name.StartsWith("Archiv für die civilistische Praxis") || citation.Reference.Periodical.Name.StartsWith("AcP") || citation.Reference.Periodical.StandardAbbreviation.StartsWith("AcP") || 
			citation.Reference.Periodical.Name.StartsWith("Archiv für Kriminologie") || citation.Reference.Periodical.Name.StartsWith("ArchKrim") || citation.Reference.Periodical.Name.StartsWith("Arch. Krim") || citation.Reference.Periodical.Name.StartsWith("Arch Kriminol") || citation.Reference.Periodical.Name.StartsWith("Arch. Kriminol") || citation.Reference.Periodical.StandardAbbreviation.StartsWith("Arch. Krim") || citation.Reference.Periodical.StandardAbbreviation.StartsWith("ArchKrim") ||
			citation.Reference.Periodical.Name.StartsWith("Archiv des öffentlichen Rechts") || citation.Reference.Periodical.Name.StartsWith("AöR") || citation.Reference.Periodical.StandardAbbreviation.StartsWith("AöR") || 
			citation.Reference.Periodical.Name.StartsWith("Archiv für Rechts- und Sozialphilosophie") || citation.Reference.Periodical.Name.StartsWith("ARSP") || citation.Reference.Periodical.StandardAbbreviation.StartsWith("ARSP") ||
			citation.Reference.Periodical.Name.StartsWith("Archiv für Urheber- und Medienrecht") || citation.Reference.Periodical.Name.StartsWith("UFITA") || citation.Reference.Periodical.StandardAbbreviation.StartsWith("JR") ||
			citation.Reference.Periodical.Name.StartsWith("Medizinrecht") || citation.Reference.Periodical.Name.StartsWith("MedR") || citation.Reference.Periodical.StandardAbbreviation.StartsWith("UFITA") ||
			citation.Reference.Periodical.Name.StartsWith("Blutalkohol") ||
			citation.Reference.Periodical.Name.StartsWith("Monatsschrift für Kriminologie und Strafrechtsreform") || citation.Reference.Periodical.Name.StartsWith("MschrKrim") || citation.Reference.Periodical.StandardAbbreviation.StartsWith("MschrKrim") ||
			citation.Reference.Periodical.Name.StartsWith("Rabels Zeitschrift für ausländisches und internationales Privatrecht") || citation.Reference.Periodical.Name.StartsWith("RabelsZ") || citation.Reference.Periodical.StandardAbbreviation.StartsWith("RabelsZ") ||
			citation.Reference.Periodical.Name.StartsWith("Verwaltungsarchiv") || citation.Reference.Periodical.Name.StartsWith("VerwArch") || citation.Reference.Periodical.StandardAbbreviation.StartsWith("VerwArch") ||
			citation.Reference.Periodical.Name.StartsWith("Zeitschrift für ausländisches öffentliches Recht und Völkerrecht") || citation.Reference.Periodical.Name.StartsWith("ZaöRV") || citation.Reference.Periodical.StandardAbbreviation.StartsWith("ZaöRV") ||
			citation.Reference.Periodical.Name.StartsWith("Zeitschrift für das gesamte Handels- und Wirtschaftsrecht") || citation.Reference.Periodical.Name.StartsWith("ZHR") || citation.Reference.Periodical.StandardAbbreviation.StartsWith("ZHR") ||
			citation.Reference.Periodical.Name.StartsWith("Zeitschrift für die gesamte Strafrechtswissenschaft") || citation.Reference.Periodical.Name.StartsWith("ZStW") || citation.Reference.Periodical.StandardAbbreviation.StartsWith("ZStW") ||
			citation.Reference.Periodical.Name.StartsWith("Zeitschrift für Zivilproze") || citation.Reference.Periodical.Name.StartsWith("ZZP") || citation.Reference.Periodical.StandardAbbreviation.StartsWith("ZZP");
			
		}
	}
}