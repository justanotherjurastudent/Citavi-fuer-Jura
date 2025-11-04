using System.Linq;
using System.Collections.Generic;
using SwissAcademic.Citavi;
using SwissAcademic.Citavi.Metadata;
using SwissAcademic.Collections;
using SwissAcademic.Drawing;

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
			
			var placeholderCitation = citation as PlaceholderCitation;
			if (placeholderCitation == null) return false;

			return placeholderCitation.FormatOption1 == false;
		}
	}
}