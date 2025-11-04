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
			//Name: Check if court name or abbreviation contains EuGH, EuG
			//Note: (except "Europäischer Gerichtshof für Menschenrechte" or "EGMR", which is treated separately)

			if (citation == null) return false;
			if (citation.Reference == null) return false;
			if (citation.Reference.ReferenceType != ReferenceType.CourtDecision) return false;

			var courts = citation.Reference.Organizations;
			if (courts == null || courts.Count == 0) return false;

			//"Europäischer Gerichtshof für Menschenrechte" needs to be excluded
			var wordList1 = new string[] {
				"Europäischer Gerichtshof für Menschenrechte", 
				"EGMR", 
				"European Court of Human Rights", 
				"ECHR" 
			};

			var regEx1 = new Regex(@"\b(" + string.Join("|", wordList1) + @")\b", RegexOptions.IgnoreCase);
			if (courts.Any<Person>(court => regEx1.IsMatch(court.LastName) || regEx1.IsMatch(court.Abbreviation))) return false;

			//note: if you do not specify the whole word, but rather its first characters, do NOT use * as a wildcard, but
			//\\w*, which means "zero or more word characters"
			var wordList2 = new string[] {
				"EuGH", 
				"EuG"
			};
			var regEx2 = new Regex(@"\b(" + string.Join("|", wordList2) + @")\b", RegexOptions.IgnoreCase);
			return courts.Any<Person>(court => regEx2.IsMatch(court.LastName) || regEx2.IsMatch(court.Abbreviation));

		}
	}
}