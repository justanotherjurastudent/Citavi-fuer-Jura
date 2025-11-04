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


			//citation must be part of multiple citation
			PlaceholderCitation placeholderCitation = citation as PlaceholderCitation;
			if (placeholderCitation == null) return false;
			if (!placeholderCitation.IsPartOfMultipleCitation) return false;

			//citation must not be the first citation in a multiple citation and must have a predecessor
			if (placeholderCitation.IsFirstInMultipleCitation) return false;
			PlaceholderCitation previousPlaceholderCitation = placeholderCitation.PreviousPlaceholderCitation;
			if (previousPlaceholderCitation == null) return false;

			
			Reference thisReference = placeholderCitation.Reference;
			Reference previousReference = previousPlaceholderCitation.Reference;
			if (previousReference == null) return false;

			//Referenz muss dasselbe Aktenzeichen haben
			string TitleSupplement = thisReference.TitleSupplement;
			string CustomField1 = previousReference.CustomField1;
			if (!TitleSupplement.Equals(CustomField1)) return false;
			
			return true;
		}
	}
}