// #C5_43236
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
			if (citation.Reference.ParentReference == null) return false;
			if (citation.Reference.CitationKey == null) return false;

			string citationKey = citation.Reference.CitationKey.ToUpperInvariant();

			return citationKey.StartsWith("FS") || citationKey.StartsWith("GS") || citationKey.StartsWith("FG");
		}
	}
}