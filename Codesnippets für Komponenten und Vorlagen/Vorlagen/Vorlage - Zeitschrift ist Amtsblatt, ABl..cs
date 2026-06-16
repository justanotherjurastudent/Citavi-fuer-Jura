// #C5_43233
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
			return MatchesName(p, "Amtsblatt") || MatchesName(p, "ABl.");
		}

		private static bool MatchesName(Periodical p, string prefix)
		{
			return p.Name != null && p.Name.StartsWith(prefix);
		}
	}
}