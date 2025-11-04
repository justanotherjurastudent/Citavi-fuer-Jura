using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
            if (citation.Reference.ParentReference == null) return false;
            if (string.IsNullOrEmpty(citation.Reference.Title)) return false;
            
            string field = citation.Reference.ParentReference.Title;
            string field2 = citation.Reference.ParentReference.SpecificField1;

            var wordList1 = new string[] {
		    "Palandt[\\w.-]*",
		    "Grüneberg[\\w.-]*",
		    "Münchener Kommentar[\\w.-]*",
		    "Leipziger Kommentar[\\w.-]*",
		    "Bonner Kommentar[\\w.-]*",
		    "Berliner Kommentar[\\w.-]*",
		    "Karlsruher Kommentar[\\w.-]*",
		    "Frankfurter Kommentar[\\w.-]*",
		    "Erfurter Kommentar[\\w.-]*",
			"Tübinger Kommentar[\\w.-]*",
		    "Nomos Kommentar[\\w.-]*",
		    "Systematischer Kommentar[\\w.-]*",
		    "J. von Staudinger[\\w.-]*",
		    "Staudinger[\\w.-]*",
		    "Großkomm[\\w.-]* HGB",
		    "Großkomm[\\w.-]* AktG"
			};

			var regEx = new Regex(@"\b(" + string.Join("|", wordList1) + @")\b", RegexOptions.IgnoreCase);


            // Überprüfung auf ein Match in einem der beiden Felder
            return regEx.IsMatch(field) || regEx.IsMatch(field2);
        }
    }
}
