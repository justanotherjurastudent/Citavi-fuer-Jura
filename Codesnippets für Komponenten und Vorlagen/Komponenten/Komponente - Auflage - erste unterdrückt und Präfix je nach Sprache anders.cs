using System;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
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
			//Version 1.1 Filter can handle both Edition and EditionNumberResolved, can be part of a multi-element component part
			//Version 1.0 Filter handles Edition only (no EditionNumberResolved)
			
			handled = false;
			
			if (componentPart == null) return null;
			if (componentPart.Elements == null) return null;
			if (citation == null || citation.Reference == null) return null;
			if (componentPart.Scope == ComponentPartScope.ParentReference && citation.Reference.ParentReference == null) return null;
			
			
			var editionFieldElement = componentPart.GetFieldElements().FirstOrDefault<FieldElement>(item => item.PropertyId == ReferencePropertyId.Edition || item.PropertyId == ReferencePropertyId.EditionNumberResolved);
			if (editionFieldElement == null) return null;

		
			string editionNumberResolved;
			if (componentPart.Scope == ComponentPartScope.ParentReference)
			{
				editionNumberResolved = citation.Reference.ParentReference.EditionNumberResolved;
			}
			else
			{
				editionNumberResolved = citation.Reference.EditionNumberResolved;
			}
			
			
			//now suppress the output ONLY if edition number is 1
			if (editionNumberResolved == "1")
			{
				componentPart.Elements.Remove(editionFieldElement);
			}
			
			
			if (citation == null) return null;
			if (citation.Reference == null) return null;
			if (componentPart == null) return null;

			#region Find numeric field elements

			//we treat only numeric field elemements that output the Edition field
			var numericFieldElements = componentPart.Elements.OfType<NumericFieldElement>().Where(element => element.PropertyId == ReferencePropertyId.EditionNumberResolved).ToList();
			if (numericFieldElements == null || !numericFieldElements.Any()) return null;

			#endregion Find numeric field elements

			#region Determine reference to look at

			Reference reference;
			if (componentPart.Scope == ComponentPartScope.ParentReference)
			{
				if (citation.Reference.ParentReference == null) return null;
				reference = citation.Reference.ParentReference as Reference;
			}
			else
			{
				reference = citation.Reference as Reference;
			}
			if (reference == null) return null;

			#endregion Determine reference to look at

			#region Determine reference language

			Language language;
			if (String.Equals(reference.LanguageCode, CultureInfo.GetCultureInfo("en").TwoLetterISOLanguageName, StringComparison.OrdinalIgnoreCase))
			{
				language = Language.English;
			}
			else if (String.Equals(reference.LanguageCode, CultureInfo.GetCultureInfo("de").TwoLetterISOLanguageName, StringComparison.OrdinalIgnoreCase) || String.Equals(reference.LanguageCode, CultureInfo.GetCultureInfo("ger").ThreeLetterISOLanguageName, StringComparison.OrdinalIgnoreCase))
			{
				language = Language.German;
			}
			else if (String.Equals(reference.LanguageCode, CultureInfo.GetCultureInfo("it").TwoLetterISOLanguageName, StringComparison.OrdinalIgnoreCase))
			{
				language = Language.Italian;
			}
			else if (String.Equals(reference.LanguageCode, CultureInfo.GetCultureInfo("fr").TwoLetterISOLanguageName, StringComparison.OrdinalIgnoreCase))
			{
				language = Language.French;
			}
			else if (String.Equals(reference.LanguageCode, CultureInfo.GetCultureInfo("spa").ThreeLetterISOLanguageName, StringComparison.OrdinalIgnoreCase) || String.Equals(reference.LanguageCode, CultureInfo.GetCultureInfo("es").TwoLetterISOLanguageName, StringComparison.OrdinalIgnoreCase))
			{
				language = Language.Spain;
			}
			else
			{
				language = Language.Other;
			}

			#endregion Determine reference language

			foreach (NumericFieldElement element in numericFieldElements)
			{
				var property = element.PropertyId;
				var value = (string)reference.GetValue(property);

				if (string.IsNullOrEmpty(value)) continue;

				int editionNumber;
				bool isNumeric = int.TryParse(editionNumberResolved, out editionNumber);

				#region Edition field contains just a number
				

				if (isNumeric)
				{
					switch (language)
					{
						case Language.English:
							{							
								element.UseNumericAbbreviations = true;
								element.DefaultNumericAbbreviation.Text = "th";
								element.DefaultNumericAbbreviation.FontStyle = FontStyle.Superscript;
								
								element.SpecialNumericAbbreviations.Text = "1|st|2|nd|3|rd";
								element.SpecialNumericAbbreviations.FontStyle = FontStyle.Superscript;
								
								//to avoid the Superscript propagating any further, we set the font style to neutral
								element.SingularSuffix.Text = "\u00A0ed.";
								element.SingularSuffix.FontStyle = FontStyle.Neutral;
								
								element.PluralSuffix.Text = "\u00A0ed.";
								element.PluralSuffix.FontStyle = FontStyle.Neutral;
							}
							break;

						
						case Language.French:
							{
								element.UseNumericAbbreviations = true;
								element.DefaultNumericAbbreviation.Text = "e";
								element.DefaultNumericAbbreviation.FontStyle = FontStyle.Superscript;
								element.SpecialNumericAbbreviations.Text = "1|e|2|e";
								element.SpecialNumericAbbreviations.FontStyle = FontStyle.Superscript;
								element.SingularSuffix.Text = "\u00A0éd.";
								element.PluralSuffix.Text = "\u00A0éd.";
							}
							break;
							
						case Language.Italian:
							{
								element.UseNumericAbbreviations = true;
								element.DefaultNumericAbbreviation.Text = "a";
								element.DefaultNumericAbbreviation.FontStyle = FontStyle.Superscript;
								element.SpecialNumericAbbreviations.Text = "1|a|2|a";
								element.SpecialNumericAbbreviations.FontStyle = FontStyle.Superscript;
								element.SingularSuffix.Text = "\u00A0ed.";
								element.PluralSuffix.Text = "\u00A0ed.";
							}
							break;
							
						case Language.Spain:
						{
							element.UseNumericAbbreviations = true;
							element.DefaultNumericAbbreviation.Text = "a";
							element.DefaultNumericAbbreviation.FontStyle = FontStyle.Superscript;
							element.SpecialNumericAbbreviations.Text = "1|a|2|a";
							element.SpecialNumericAbbreviations.FontStyle = FontStyle.Superscript;
							element.SingularSuffix.Text = "\u00A0ed.";
							element.PluralSuffix.Text = "\u00A0ed.";
						}	
							break;
							
						case Language.Other:
							{
								element.SingularSuffix.Text = ".\u00A0Aufl.";
								element.PluralSuffix.Text = ".\u00A0Aufl.";
							}
							break;
						
						default:
						case Language.German:
							{
								element.SingularSuffix.Text = ".\u00A0Aufl.";
								element.PluralSuffix.Text = ".\u00A0Aufl.";
							}
							break;
					}
				}

				#endregion Edition field contains just a number

				#region Edition field contains text

				else
				{
					switch (language)
					{
						case Language.English:
							{
								element.SingularSuffix.Text = "";
								element.PluralSuffix.Text = "";
							}
							break;

						default:
						case Language.German:
							{
								element.SingularSuffix.Text = "";
								element.PluralSuffix.Text = "";
							}
							break;
						case Language.French:
							{
								element.SingularSuffix.Text = "";
								element.PluralSuffix.Text = "";
							}
							break;
							
						case Language.Italian:
							{
								element.SingularSuffix.Text = "";
								element.PluralSuffix.Text = "";
							}
							break;
						
						case Language.Spain:
							{
								element.SingularSuffix.Text = "";
								element.PluralSuffix.Text = "";
							}
							break;
							
						case Language.Other:
							{
								element.SingularSuffix.Text = "";
								element.PluralSuffix.Text = "";
							}
							break;
					}
				}

				#endregion Edition field contains text
			}

			return null;
		}

		private enum Language
		{
			English,
			German,
			French,
			Italian,
			Spain,
			Other
		}
	}
}