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
			if (string.IsNullOrEmpty(citation.Reference.Title)) return false;
			
			string field = citation.Reference.Title;

			//note: if you do not specify the whole word, but rather its first characters, do NOT use * as a wildcard, but
			//\\w*, which means "zero or more word characters"
			var wordList1 = new string[] {
				"Leipziger Kommentar StGB Online"
			};

			var regEx = new Regex(@"\b(" + string.Join("|", wordList1) + @")\b", RegexOptions.IgnoreCase);
			return regEx.IsMatch(field);
			
		}
	}
}