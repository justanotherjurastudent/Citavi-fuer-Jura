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
			
			string parentParallelTitle = citation.Reference.ParentReference.ParallelTitle.ToUpperInvariant();
			
			return (parentParallelTitle.StartsWith("FS") || parentParallelTitle.StartsWith("GS")  || parentParallelTitle.StartsWith("FG"));
		}
	}
}