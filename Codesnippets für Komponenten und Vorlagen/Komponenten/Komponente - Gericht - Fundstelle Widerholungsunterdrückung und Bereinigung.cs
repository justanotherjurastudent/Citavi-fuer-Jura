//C6#COT016
//C5#431525
//Description: Suppress reporter if repeated AND court is identical
//Version: 1.2
//Ausgabe der Fundstelle unterdruecken bei WDH, aber nur wenn das Gericht identisch ist [/opt2 zeigt Fundstelle an]
//An eine Komponente mit Feld "Fundstelle" beim Dokumententyp "Gerichtsentscheidung" anhängen
//Bei WDH (gleiches Gericht + gleiche Fundstelle) wird die gesamte Komponente unterdrückt.

using System.Linq;
using System.Collections.Generic;
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

         //If /option2 is set, the code is deactivated
         var placeholderCitation = citation as PlaceholderCitation;
         if (placeholderCitation == null) return null;
         if (placeholderCitation.FormatOption2) return null;

         if (componentPart == null || componentPart.Elements == null || componentPart.Elements.Count == 0) return null;

         var reporterFieldElements = componentPart.Elements.OfType<PeriodicalFieldElement>();   //Reporter/Fundstelle == Periodical
         if (reporterFieldElements == null || reporterFieldElements.Count() == 0) return null;


         var thisFootnoteCitation = citation as FootnoteCitation;
         var thisInTextCitation = citation as InTextCitation;

         if (thisFootnoteCitation == null && thisInTextCitation == null) return null;


         var thisReporter = citation.Reference.Periodical;
         if (thisReporter == null) return null;

         // Geändert: Gerichtsname (Organizations) dieser Entscheidung ermitteln
         var thisCourtKey = GetCourtKey(citation.Reference);
         if (string.IsNullOrEmpty(thisCourtKey)) return null; //ohne Gerichtsangabe keine Unterdrückung


         if (thisFootnoteCitation != null)
         {
            var previousFootnoteCitation = GetPreviousVisibleFootnoteCitation(thisFootnoteCitation, true); //considers the fact, that it MUST be inside the same footnote
            if (previousFootnoteCitation == null) return null;
            if (previousFootnoteCitation.Reference == null) return null;

            var previousReporter = previousFootnoteCitation.Reference.Periodical;
            if (previousReporter == null) return null;

            if (thisReporter != previousReporter) return null;

            // Geändert: Gerichtsvergleich - nur unterdrücken, wenn Gerichte übereinstimmen
            var previousCourtKey = GetCourtKey(previousFootnoteCitation.Reference);
            if (string.IsNullOrEmpty(previousCourtKey)) return null;
            if (!string.Equals(thisCourtKey, previousCourtKey, System.StringComparison.Ordinal)) return null;
         }

         else if (thisInTextCitation != null)
         {
            var previousInTextCitation = GetPreviousVisibleInTextCitation(thisInTextCitation);
            if (previousInTextCitation == null) return null;
            if (previousInTextCitation.Reference == null) return null;

            var previousReporter = previousInTextCitation.Reference.Periodical;
            if (previousReporter == null) return null;

            if (thisReporter != previousReporter) return null;

            // Geändert: Gerichtsvergleich - nur unterdrücken, wenn Gerichte übereinstimmen
            var previousCourtKey = GetCourtKey(previousInTextCitation.Reference);
            if (string.IsNullOrEmpty(previousCourtKey)) return null;
            if (!string.Equals(thisCourtKey, previousCourtKey, System.StringComparison.Ordinal)) return null;
         }

         //still here? we suppress the output of the component
         handled = true;
         return null;
      }


		#region GetCourtKey
		// Geändert: Erzeugt aus allen Gerichts-Einträgen einen vergleichbaren Schlüssel
		private static string GetCourtKey(Reference reference)
		{
		   if (reference == null) return null;
		   if (reference.Organizations == null || reference.Organizations.Count == 0) return null;

		   var names = reference.Organizations
		      .Cast<Person>()
		      .Select(o => o != null ? o.LastName : null)
		      .Where(n => !string.IsNullOrWhiteSpace(n))
		      .Select(n => n.Trim())
		      .OrderBy(n => n, System.StringComparer.Ordinal);

		   var key = string.Join("|", names);
		   return string.IsNullOrEmpty(key) ? null : key;
		}
		#endregion GetCourtKey


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