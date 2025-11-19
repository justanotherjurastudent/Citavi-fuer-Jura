//C6#COT_Court_Decision_Type_Filter
//Description: Filtert "Art der Entscheidung" und ersetzt "Urteil" mit "Urt." und "Beschluss" mit "Beschl."
//Version 1.2: C# 5-kompatibel, ohne Element-Prüfung
//Version 1.1: Versuchte PropertyId-Prüfung (nicht unterstützt)
//Version 1.0: Initiale Version
//Author: Angepasst nach Lumivero-Beispielen

using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using SwissAcademic.Citavi;
using SwissAcademic.Citavi.Citations;

namespace SwissAcademic.Citavi.Citations
{
    public class ComponentPartFilter
        :
        IComponentPartFilter
    {
        // Hauptmethode: Filtert die Textausgabe der Komponente
        public IEnumerable<ITextUnit> GetTextUnits(ComponentPart componentPart, Template template, Citation citation, out bool handled)
        {
            handled = false;

            // Validierung: Prüfe ob alle notwendigen Objekte vorhanden sind
            if (citation == null) return null;
            if (citation.Reference == null) return null;
            if (componentPart == null || componentPart.Elements == null || !componentPart.Elements.Any()) return null;
            if (template == null) return null;

            // Hole die ungefilterten Texteinheiten der Komponente
            var textUnits = componentPart.GetTextUnitsUnfiltered(citation, template);
            if (textUnits == null || !textUnits.Any()) return null;

            // Durchlaufe alle Texteinheiten und wende die Filterung an
            foreach (var textUnit in textUnits)
            {
                if (string.IsNullOrEmpty(textUnit.Text)) continue;

                string modifiedText = textUnit.Text;

                // Ersetze "Urteil" durch "Urt." (exakte Wortgrenzen, Groß-/Kleinschreibung beachten)
                modifiedText = Regex.Replace(modifiedText, @"\bUrteil\b", "Urt.", RegexOptions.None);
                
                // Ersetze "Beschluss" durch "Beschl." (exakte Wortgrenzen, Groß-/Kleinschreibung beachten)
                modifiedText = Regex.Replace(modifiedText, @"\bBeschluss\b", "Beschl.", RegexOptions.None);

                // Optional: Behandle auch kleingeschriebene Varianten (falls im Feld vorhanden)
                modifiedText = Regex.Replace(modifiedText, @"\burteil\b", "urt.", RegexOptions.None);
                modifiedText = Regex.Replace(modifiedText, @"\bbeschluss\b", "beschl.", RegexOptions.None);

                // Aktualisiere die Texteinheit mit dem gefilterten Text
                textUnit.Text = modifiedText;
            }

            handled = true;
            return textUnits;
        }
    }
}
