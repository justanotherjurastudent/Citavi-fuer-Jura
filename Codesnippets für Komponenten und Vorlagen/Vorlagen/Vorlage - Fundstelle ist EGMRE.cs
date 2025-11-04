// #C5_43233
//Version 1.1

using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
			if (citation.Reference.ReferenceType != ReferenceType.CourtDecision) return false;
			
			var periodical = citation.Reference.Periodical;
			if (periodical == null) return false;
			if (string.IsNullOrEmpty(periodical.FullName)) return false;
			
			//note: if you do not specify the whole word, but rather its first characters, do NOT use * as a wildcard, but
			//\\w*, which means "zero or more word characters"
			var wordList = new string[] {
				"EGMRE",
				"EGMR-E"
				};
			var regEx = new Regex(@"\b(" + string.Join("|", wordList) + @")\b", RegexOptions.IgnoreCase);
			return 
				regEx.IsMatch(periodical.Name) || 
				regEx.IsMatch(periodical.StandardAbbreviation) ||
				regEx.IsMatch(periodical.UserAbbreviation1) ||
				regEx.IsMatch(periodical.UserAbbreviation2);
	
		}
	}
}