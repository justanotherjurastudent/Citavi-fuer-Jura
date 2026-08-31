using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

			// Die zu prüfenden Felder: Speichermedium und Titelzusatz
			string storageMedium = citation.Reference.StorageMedium;
			string titleSupplement = citation.Reference.TitleSupplement;

			// Gesuchte Wörter: KI oder LLM (case-insensitive, als eigenständiges Wort)
			var wordList = new string[] {
				"KI",
				"LLM"
			};

			var regEx = new Regex(@"\b(" + string.Join("|", wordList) + @")\b", RegexOptions.ExplicitCapture);

			// ODER-Verknüpfung: Bedingung wahr, wenn in EINEM der beiden Felder
			// das Wort "KI" oder "LLM" vorkommt
			bool storageMatch = !string.IsNullOrEmpty(storageMedium) && regEx.IsMatch(storageMedium);
			bool titleMatch  = !string.IsNullOrEmpty(titleSupplement) && regEx.IsMatch(titleSupplement);

			return storageMatch || titleMatch;
		}
	}
}