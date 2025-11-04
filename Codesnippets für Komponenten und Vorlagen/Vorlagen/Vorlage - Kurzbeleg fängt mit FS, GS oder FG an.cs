// #C5_43236
//Version 1.0

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
			
			string CitationKey = citation.Reference.CitationKey.ToUpperInvariant();
			
			return (CitationKey.StartsWith("FS") || CitationKey.StartsWith("GS")  || CitationKey.StartsWith("FG"));
		}
	}
}