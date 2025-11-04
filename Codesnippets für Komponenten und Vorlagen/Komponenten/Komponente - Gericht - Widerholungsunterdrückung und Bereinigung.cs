//C6#CPS011
//C5#43122
//Description: Clean court name (remove numbers, dots, "Kammer", "Senat") on first mention; suppress entire component on repetition
//Version: 3.8  
//Bitte an eine Komponente mit Feld "Gericht" beim Dokumententyp "Gerichtsentscheidung" anhängen.
//Bei erster Nennung: Gerichtsname wird bereinigt (Zahlen, Punkte, "Kammer", "Senat" entfernt).
//Bei Wiederholung: Ganze Komponente wird unterdrückt (Vergleich erfolgt mit bereinigten Namen!).

using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using SwissAcademic.Citavi;
using SwissAcademic.Citavi.Metadata;
using SwissAcademic.Collections;
using SwissAcademic.Drawing;

namespace SwissAcademic.Citavi.Citations
{
	public class ComponentPartFilter
	   :
	   IComponentPartFilter
	{
		public IEnumerable<ITextUnit> GetTextUnits(ComponentPart componentPart, Template template, Citation citation, out bool handled)
		{

			handled = false;

			if (citation == null) return null;
			if (citation.Reference == null) return null;

			//We limit this code to ReferenceType "CourtDecision/Gerichtsentscheidung" only!
			if (citation.Reference.ReferenceType != ReferenceType.CourtDecision) return null;

			//If /option1 is set, the code is deactivated
			var placeholderCitation = citation as PlaceholderCitation;
			if (placeholderCitation == null) return null;
			if (placeholderCitation.FormatOption1) return null;

			if (componentPart == null || componentPart.Elements == null || componentPart.Elements.Count == 0) return null;

			var courtFieldElements = componentPart.Elements.OfType<PersonFieldElement>().Where(element => element.PropertyId == ReferencePropertyId.Organizations);
			if (courtFieldElements == null || courtFieldElements.Count() == 0) return null;


			var thisFootnoteCitation = citation as FootnoteCitation;
			var thisInTextCitation = citation as InTextCitation;

			if (thisFootnoteCitation == null && thisInTextCitation == null) return null;


			var thisOrganizations = citation.Reference.Organizations;
			if (thisOrganizations == null || thisOrganizations.Count == 0) return null;

			// Geändert: Erstelle bereinigte Vergleichstring aus Organisationsnamen
			var thisCleanedComparison = GetCleanedComparisonString(thisOrganizations);


			if (thisFootnoteCitation != null)
			{
				var previousFootnoteCitation = GetPreviousVisibleFootnoteCitation(thisFootnoteCitation, true); //considers the fact, that it MUST be inside the same footnote
				if (previousFootnoteCitation == null) 
				{
					// Erste Nennung: Bereinigung durchführen
					var textUnits = componentPart.GetTextUnitsUnfiltered(citation, template);
					if (textUnits != null && textUnits.Any())
					{
						var cleanedUnits = CreateCleanedTextUnits(textUnits);
						handled = true;
						return cleanedUnits;
					}
					return null;
				}

				if (previousFootnoteCitation.Reference == null) return null;

				var previousOrganizations = previousFootnoteCitation.Reference.Organizations;
				if (previousOrganizations == null || previousOrganizations.Count == 0) return null;

				// Geändert: Vergleich mit bereinigten Organisationsnamen
				var previousCleanedComparison = GetCleanedComparisonString(previousOrganizations);

				if (!thisCleanedComparison.Equals(previousCleanedComparison))
				{
					// Unterschiedliches Gericht (auch nach Bereinigung): Bereinigung durchführen
					var textUnits = componentPart.GetTextUnitsUnfiltered(citation, template);
					if (textUnits != null && textUnits.Any())
					{
						var cleanedUnits = CreateCleanedTextUnits(textUnits);
						handled = true;
						return cleanedUnits;
					}
					return null;
				}

				// Gleiche Gericht erkannt (nach Bereinigung): Unterdrückung
				handled = true;
				return null;
			}

			else if (thisInTextCitation != null)
			{
				var previousInTextCitation = GetPreviousVisibleInTextCitation(thisInTextCitation);
				if (previousInTextCitation == null)
				{
					// Erste Nennung: Bereinigung durchführen
					var textUnits = componentPart.GetTextUnitsUnfiltered(citation, template);
					if (textUnits != null && textUnits.Any())
					{
						var cleanedUnits = CreateCleanedTextUnits(textUnits);
						handled = true;
						return cleanedUnits;
					}
					return null;
				}

				if (previousInTextCitation.Reference == null) return null;

				var previousOrganizations = previousInTextCitation.Reference.Organizations;
				if (previousOrganizations == null || previousOrganizations.Count == 0) return null;

				// Geändert: Vergleich mit bereinigten Organisationsnamen
				var previousCleanedComparison = GetCleanedComparisonString(previousOrganizations);

				if (!thisCleanedComparison.Equals(previousCleanedComparison))
				{
					// Unterschiedliches Gericht (auch nach Bereinigung): Bereinigung durchführen
					var textUnits = componentPart.GetTextUnitsUnfiltered(citation, template);
					if (textUnits != null && textUnits.Any())
					{
						var cleanedUnits = CreateCleanedTextUnits(textUnits);
						handled = true;
						return cleanedUnits;
					}
					return null;
				}

				// Gleiche Gericht erkannt (nach Bereinigung): Unterdrückung
				handled = true;
				return null;
			}

			return null;
		}

		// Hilfsmethode: Extrahiere bereinigte Vergleichstrings aus Organizations
		private string GetCleanedComparisonString(IEnumerable<Person> organizations)
		{
			var combinedText = "";
			foreach (var org in organizations)
			{
				if (org != null)
				{
					// Nutze LastName und Abbreviation (wie im offiziellen Skript)
					if (!string.IsNullOrEmpty(org.LastName))
					{
						combinedText += org.LastName + " ";
					}
					if (!string.IsNullOrEmpty(org.Abbreviation))
					{
						combinedText += org.Abbreviation + " ";
					}
				}
			}
			return CleanText(combinedText);
		}

		// Hilfsmethode: Erstelle neue bereinigte TextUnits mit TextUnitCollection
		// Geändert: Direkte Modifizierung der Text Property, ohne zu casten
		private IEnumerable<ITextUnit> CreateCleanedTextUnits(IEnumerable<ITextUnit> originalUnits)
		{
			var output = new TextUnitCollection();

			foreach (var unit in originalUnits)
			{
				if (unit == null) continue;

				// Versuche direkt die Text Property zu modifizieren
				if (!string.IsNullOrEmpty(unit.Text))
				{
					var cleanedText = CleanText(unit.Text);
					unit.Text = cleanedText;
				}

				output.Add(unit);
			}

			return output;
		}

		// Hilfsmethode: Text-Bereinigung (zentral, für beide Zwecke)
		private string CleanText(string text)
		{
			if (string.IsNullOrEmpty(text)) return text;

			// Entfernung aller Zahlen
			text = Regex.Replace(text, @"\d", "");
			
			// Entfernung aller Punkte
			text = text.Replace(".", "");
			
			// Entfernung aller Klammern
			text = text.Replace("(", "");
			text = text.Replace(")", "");
			text = text.Replace("[", "");
			text = text.Replace("]", "");
			
			// Entfernung der Wörter "Kammer" und "Senat" (mit Wortgrenzen)
			text = Regex.Replace(text, @"\bKammer\b", "", RegexOptions.IgnoreCase);
			text = Regex.Replace(text, @"\bSenat\b", "", RegexOptions.IgnoreCase);
			
			// Entfernung überflüssiger Leerzeichen
			text = Regex.Replace(text, @"\s{2,}", " ");
			
			// Entfernung von Leerzeichen am Anfang und Ende
			text = text.Trim();

			return text;
		}

		#region GetPreviousVisibleCitation

		private static FootnoteCitation GetPreviousVisibleFootnoteCitation(FootnoteCitation thisFootnoteCitation, bool sameFootnote)
		{

			if (thisFootnoteCitation == null) return null;

			FootnoteCitation previousFootnoteCitation = thisFootnoteCitation;

			//consider bibonly
			do
			{
				previousFootnoteCitation = previousFootnoteCitation.PreviousFootnoteCitation;
				if (previousFootnoteCitation == null) return null;

			} while (previousFootnoteCitation.BibOnly == true);

			//still here? found one!

			if (sameFootnote && previousFootnoteCitation.FootnoteIndex != thisFootnoteCitation.FootnoteIndex) return null;
			return previousFootnoteCitation;
		}

		private static InTextCitation GetPreviousVisibleInTextCitation(InTextCitation thisInTextCitation)
		{
			if (thisInTextCitation == null) return null;

			InTextCitation previousInTextCitation = thisInTextCitation;

			//consider bibonly
			do
			{
				previousInTextCitation = previousInTextCitation.PreviousInTextCitation;
				if (previousInTextCitation == null) return null;

			} while (previousInTextCitation.BibOnly == true);

			//still here? found one!
			return previousInTextCitation;
		}

		#endregion GetPreviousVisibleCitation
	}
}
