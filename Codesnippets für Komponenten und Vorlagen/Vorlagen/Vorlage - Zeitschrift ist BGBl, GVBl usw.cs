// #C5_43233
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
			if (citation.Reference.Periodical == null) return false;
			
			return citation.Reference.Periodical.Name.StartsWith("Bundesgesetzblatt") || citation.Reference.Periodical.Name.StartsWith("BGBl") || citation.Reference.Periodical.StandardAbbreviation.StartsWith("BGBl") ||
				citation.Reference.Periodical.Name.StartsWith("Gesetz- und Verordnungsblatt") || citation.Reference.Periodical.Name.StartsWith("GVBl") ||
				citation.Reference.Periodical.Name.StartsWith("Bundessteuerblatt") || citation.Reference.Periodical.Name.StartsWith("BStBl") || citation.Reference.Periodical.StandardAbbreviation.StartsWith("BStBl") ||
				citation.Reference.Periodical.Name.StartsWith("Reichsgesetzblatt") || citation.Reference.Periodical.Name.StartsWith("RGBl");
		}
	}
}