//#43214
//Version 1.1 C5

using System;
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
			
			if (citation.Reference.HasCoreField(ReferenceTypeCoreFieldId.Number))
			{
				return !string.IsNullOrWhiteSpace(citation.Reference.Number);
			}
			return false;
			
		}
	}
}
