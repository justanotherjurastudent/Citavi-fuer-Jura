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
		//Feld "Begründer [Autor]" des Gesetzeskommentars oder - bei Beitrag in Gesetzeskommentar - des übergeordneten Titels ist leer
		public bool IsTemplateForReference(ConditionalTemplate template, Citation citation)
		{
			if (citation == null) return false;
			if (citation.Reference == null) return false;
			
			if (citation.Reference.ReferenceType == ReferenceType.LegalCommentary)
			{
				return (citation.Reference.Authors == null || !citation.Reference.Authors.Any());
			}
			
			if (citation.Reference.ReferenceType == ReferenceType.ContributionInLegalCommentary)
			{
				Reference parentReference = citation.Reference.ParentReference;
				if (parentReference == null) return false;
				
				return (parentReference.Authors == null || !parentReference.Authors.Any());
			}

			return false;
		}
	}
}