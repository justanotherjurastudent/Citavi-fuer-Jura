// #C5_43233
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
			
			return 	citation.Reference.Periodical.Name.StartsWith("Bundestagsdrucksache") || 
					citation.Reference.Periodical.Name.StartsWith("Bundestagplenar") ||
					citation.Reference.Periodical.Name.StartsWith("Bundesratsdrucksache") || 
					citation.Reference.Periodical.Name.StartsWith("Bundesratplenar") ||
					citation.Reference.Periodical.Name.StartsWith("Deutscher Bundesrat Drucksache") || 
					citation.Reference.Periodical.Name.StartsWith("Deutscher Bundestag Drucksache") || 
					citation.Reference.Periodical.Name.StartsWith("BT-Dr") ||
					citation.Reference.Periodical.Name.StartsWith("BT-Plenar") ||
					citation.Reference.Periodical.Name.StartsWith("BR-Dr") ||
					citation.Reference.Periodical.Name.StartsWith("BR-Plenar") ||
					citation.Reference.Periodical.StandardAbbreviation.StartsWith("BT-Dr")  ||
					citation.Reference.Periodical.StandardAbbreviation.StartsWith("BT-Plenar")  ||
					citation.Reference.Periodical.StandardAbbreviation.StartsWith("BR-Dr") ||
					citation.Reference.Periodical.StandardAbbreviation.StartsWith("BR-Plenar")  ||
					citation.Reference.Periodical.StandardAbbreviation.Contains("StenBer")  ||
					citation.Reference.Periodical.StandardAbbreviation.Contains("PP")  ||
					citation.Reference.Periodical.Name.Contains("StenBer") ||
					citation.Reference.Periodical.Name.Contains("Stenographischer Bericht") ||
					citation.Reference.Periodical.Name.Contains("PP") ||
					citation.Reference.Periodical.Name.EndsWith("protokoll") ||
					//ab hier kommen die Landtagsdrucksachen
					citation.Reference.Periodical.Name.EndsWith("Drucksache") ||	
					citation.Reference.Periodical.Name.EndsWith("Drs.") || 
					citation.Reference.Periodical.Name.EndsWith("Drucks.");
					
		}
	}
}