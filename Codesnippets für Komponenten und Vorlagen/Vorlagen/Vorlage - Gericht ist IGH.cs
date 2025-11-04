// #C5_43238
//Version 1.0

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

			var courts = citation.Reference.Organizations;
			if (courts == null || courts.Count == 0) return false;

				var wordList2 = new string[] {
				"Internationaler Gerichtshof",  
				"International Court of Justice", 
				"IGH",
				"ICJ"
			};
			var regEx2 = new Regex(@"\b(" + string.Join("|", wordList2) + @")\b", RegexOptions.IgnoreCase);
			return courts.Any<Person>(court => regEx2.IsMatch(court.LastName) || regEx2.IsMatch(court.Abbreviation));

		}
	}
}