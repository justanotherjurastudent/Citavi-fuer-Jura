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
			if (string.IsNullOrEmpty(citation.Reference.SpecificField2)) return false;
			
			var field = citation.Reference.SpecificField2;
						
			//note: if you do not specify the whole word, but rather its first characters, do NOT use * as a wildcard, but
			//\\w*, which means "zero or more word characters"
			var wordList = new string[] {
				"No."					
			};
			var regEx = new Regex(@"\b(" + string.Join("|", wordList) + @")\b", RegexOptions.IgnoreCase);
			return regEx.IsMatch(field);

		}
	}
}