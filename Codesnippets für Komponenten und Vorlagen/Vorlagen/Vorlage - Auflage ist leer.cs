using System;
using System.Collections.Generic;
      
namespace SwissAcademic.Citavi.Citations
{
	public class CustomTemplateCondition : ITemplateConditionMacro
	{
		public bool IsTemplateForReference(ConditionalTemplate template, Citation envelope)
		{
			return string.IsNullOrEmpty(envelope.Reference.Edition);
		}
	}
}