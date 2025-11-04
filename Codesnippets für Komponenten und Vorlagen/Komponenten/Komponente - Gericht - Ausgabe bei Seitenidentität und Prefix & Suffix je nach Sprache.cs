using System.Linq;
using System.Collections.Generic;
using SwissAcademic.Citavi;
using SwissAcademic.Citavi.Metadata;
using SwissAcademic.Collections;
using SwissAcademic.Drawing;

namespace SwissAcademic.Citavi.Citations
{
    public class ComponentPartFilter : IComponentPartFilter
    {
        public IEnumerable<ITextUnit> GetTextUnits(ComponentPart componentPart, Template template, Citation citation, out bool handled)
        {
            handled = false;

            if (componentPart == null) return null;
            if (componentPart.Elements == null || componentPart.Elements.Count == 0) return null;
            if (citation == null) return null;
            if (citation.Reference == null) return null;

            var placeholderCitation = citation as PlaceholderCitation;
            if (placeholderCitation == null) return null;
            if (placeholderCitation.Entry == null) return null;

            PageRangeFieldElement pageRangeFieldElement = null;
            QuotationPageRangeFieldElement quotationPageRangeFieldElement = null;
            if (!TryValidateComponentPartStructure(componentPart, out pageRangeFieldElement, out quotationPageRangeFieldElement)) return null;

            if (!citation.Reference.HasCoreField(ReferenceTypeCoreFieldId.PageRange) || citation.Reference.PageRange == null || string.IsNullOrEmpty(citation.Reference.PageRange.PrettyString))
            {
                TransferSettings(pageRangeFieldElement, quotationPageRangeFieldElement);
                RemoveAllButElement(componentPart, quotationPageRangeFieldElement);
                RemoveParenthesesFromPrefixSuffix(quotationPageRangeFieldElement);
                return null;
            }

            var startPageArticle = citation.Reference.PageRange.StartPage;
            var startPageQuotation = placeholderCitation.Entry.PageRange.StartPage;

            if (startPageQuotation == startPageArticle)
            {
                TransferSettings(pageRangeFieldElement, quotationPageRangeFieldElement);
                RemoveAllButElement(componentPart, quotationPageRangeFieldElement);
                RemoveParenthesesFromPrefixSuffix(quotationPageRangeFieldElement);
                return null;
            }
            else
            {
                string language = string.Empty;
                if (componentPart.Scope == ComponentPartScope.Reference)
                    language = citation.Reference.Language.ToUpperInvariant();
                else if (componentPart.Scope == ComponentPartScope.ParentReference)
                {
                    var parentReference = citation.Reference.ParentReference;
                    if (parentReference == null) return null;
                    language = parentReference.Language.ToUpperInvariant();
                }

                if (string.IsNullOrEmpty(language)) return null;


                SetQuotationPageRangeFormatting(quotationPageRangeFieldElement, language);

                pageRangeFieldElement.PageOneHasSpecialFormat = false;
                pageRangeFieldElement.PageTwoHasSpecialFormat = false;
                pageRangeFieldElement.PageMultiNumberingStyle = NumberingStyle.StartPageOnly;
                pageRangeFieldElement.PageMultiSuffix.Text = string.Empty;

                pageRangeFieldElement.ColumnOneHasSpecialFormat = false;
				pageRangeFieldElement.ColumnTwoHasSpecialFormat = false;
				pageRangeFieldElement.ColumnMultiNumberingStyle = NumberingStyle.StartPageOnly;
				pageRangeFieldElement.ColumnMultiSuffix.Text = string.Empty;
				
				pageRangeFieldElement.ParagraphOneHasSpecialFormat = false;
				pageRangeFieldElement.ParagraphTwoHasSpecialFormat = false;
				pageRangeFieldElement.ParagraphMultiNumberingStyle = NumberingStyle.StartPageOnly;
				pageRangeFieldElement.ParagraphMultiSuffix.Text = string.Empty;
				
				pageRangeFieldElement.MarginOneHasSpecialFormat = false;
				pageRangeFieldElement.MarginTwoHasSpecialFormat = false;
				pageRangeFieldElement.MarginMultiNumberingStyle = NumberingStyle.StartPageOnly;
				pageRangeFieldElement.MarginMultiSuffix.Text = string.Empty;
				
				pageRangeFieldElement.OtherOneHasSpecialFormat = false;
				pageRangeFieldElement.OtherTwoHasSpecialFormat = false;
				pageRangeFieldElement.OtherMultiNumberingStyle = NumberingStyle.StartPageOnly;
				pageRangeFieldElement.OtherMultiSuffix.Text = string.Empty;

                return null;
            }
        }

        private bool TryValidateComponentPartStructure(ComponentPart componentPart, out PageRangeFieldElement pageRangeFieldElement, out QuotationPageRangeFieldElement quotationPageRangeFieldElement)
        {
            /*
				Allowed structures:
				/1/ [article pages] [quotation pages]
				/2/ [article pages] [literal] [quotation pages]
				/3/ [article pages] [literal] [quotation pages] [literal]
			*/
			
			pageRangeFieldElement = null;
			quotationPageRangeFieldElement = null;

			if (componentPart == null) return false;
			if (componentPart.Elements == null || 
				componentPart.Elements.Count == 0 || 
				componentPart.Elements.Count < 2 || 
				componentPart.Elements.Count > 4) return false;

			IElement element01 = componentPart.Elements.ElementAtOrDefault(0);
			IElement element02 = componentPart.Elements.ElementAtOrDefault(1);
			IElement element03 = componentPart.Elements.ElementAtOrDefault(2);
			IElement element04 = componentPart.Elements.ElementAtOrDefault(3);
			
			
			#region /1/ [article pages] [quotation pages]
			if (element04 == null &&
				element03 == null &&
				element02 != null &&
				element02 is QuotationPageRangeFieldElement &&
				element01 != null &&
				element01 is PageRangeFieldElement &&
				((PageRangeFieldElement)element01).PropertyId == ReferencePropertyId.PageRange)
			{
				pageRangeFieldElement = (PageRangeFieldElement)element01;
				quotationPageRangeFieldElement = (QuotationPageRangeFieldElement)element02;
				return true;
			}
			#endregion
			
			#region /2/ [article pages] [literal] [quotation pages]
			if (element04 == null &&
				element03 != null &&
				element03 is QuotationPageRangeFieldElement &&
				element02 != null &&
				element02 is LiteralElement &&
				element01 != null &&
				element01 is PageRangeFieldElement &&
				((PageRangeFieldElement)element01).PropertyId == ReferencePropertyId.PageRange)
			{
				pageRangeFieldElement = (PageRangeFieldElement)element01;
				quotationPageRangeFieldElement = (QuotationPageRangeFieldElement)element03;
				return true;
			}
			#endregion
			
			#region /3/ [article pages] [literal] [quotation pages] [literal]
			if (element04 != null &&
				element04 is LiteralElement &&
				element03 != null &&
				element03 is QuotationPageRangeFieldElement &&
				element02 != null &&
				element02 is LiteralElement &&
				element01 != null && 
				element01 is PageRangeFieldElement &&
				((PageRangeFieldElement)element01).PropertyId == ReferencePropertyId.PageRange)
			{
				pageRangeFieldElement = (PageRangeFieldElement)element01;
				quotationPageRangeFieldElement = (QuotationPageRangeFieldElement)element03;
				return true;
			}
			#endregion
			
			return false;
        }

        private void RemoveAllButElement(ComponentPart componentPart, IElement elementToKeep)
        {  
            if (componentPart == null || componentPart.Elements == null || componentPart.Elements.Count == 0) return;
			var elements = componentPart.Elements.ToList();

			foreach (IElement element in elements)
			{
				if (element == elementToKeep) continue;
				componentPart.Elements.Remove(element);
			}
        }

        private void RemoveParenthesesFromPrefixSuffix(PageRangeFieldElement pageRangeFieldElement)
        {
            if (pageRangeFieldElement == null) return;
			
			LiteralElement[] prefixes = new LiteralElement[] {
				pageRangeFieldElement.PageOnePrefix,
				pageRangeFieldElement.PageTwoPrefix,
				pageRangeFieldElement.PageMultiPrefix,
				
				pageRangeFieldElement.ColumnOnePrefix,
				pageRangeFieldElement.ColumnTwoPrefix,
				pageRangeFieldElement.ColumnMultiPrefix,
				
				pageRangeFieldElement.ParagraphOnePrefix,
				pageRangeFieldElement.ParagraphTwoPrefix,
				pageRangeFieldElement.ParagraphMultiPrefix,
				
				pageRangeFieldElement.MarginOnePrefix,
				pageRangeFieldElement.MarginTwoPrefix,
				pageRangeFieldElement.MarginMultiPrefix,
				
				pageRangeFieldElement.OtherOnePrefix,
				pageRangeFieldElement.OtherTwoPrefix,
				pageRangeFieldElement.OtherMultiPrefix
			};
			
			LiteralElement[] suffixes = new LiteralElement[] { 
				pageRangeFieldElement.PageOneSuffix,
				pageRangeFieldElement.PageTwoSuffix,
				pageRangeFieldElement.PageMultiSuffix,
				
				pageRangeFieldElement.ColumnOneSuffix,
				pageRangeFieldElement.ColumnTwoSuffix,
				pageRangeFieldElement.ColumnMultiSuffix,
				
				pageRangeFieldElement.ParagraphOneSuffix,
				pageRangeFieldElement.ParagraphTwoSuffix,
				pageRangeFieldElement.ParagraphMultiSuffix,
				
				pageRangeFieldElement.MarginOneSuffix,
				pageRangeFieldElement.MarginTwoSuffix,
				pageRangeFieldElement.MarginMultiSuffix,
				
				pageRangeFieldElement.OtherOneSuffix,
				pageRangeFieldElement.OtherTwoSuffix,
				pageRangeFieldElement.OtherMultiSuffix
			};
			
			foreach(LiteralElement element in prefixes)
			{
				TrimStartParentheses(element);
			}
			foreach(LiteralElement element in suffixes)
			{
				TrimEndParentheses(element);
			}
        }
        

            private void TrimStartParentheses(LiteralElement literalElement)
		    {
			if (literalElement == null) return;
			if (string.IsNullOrEmpty(literalElement.Text)) return;
			
			char[] charsToTrim = {' ', '('};
			literalElement.Text = literalElement.Text.TrimStart(charsToTrim);
		    }
		
		   private void TrimEndParentheses(LiteralElement literalElement)
		    {
			if (literalElement == null) return;
			if (string.IsNullOrEmpty(literalElement.Text)) return;
			
			char[] charsToTrim = {' ', ')'};
			literalElement.Text = literalElement.Text.TrimEnd(charsToTrim);
		    }
        
        
        private void TransferSettings(PageRangeFieldElement sourceElement, PageRangeFieldElement targetElement)
        {
            #region Page
			
			targetElement.PageOneHasSpecialFormat = sourceElement.PageOneHasSpecialFormat;
			targetElement.PageOnePrefix.Text = sourceElement.PageOnePrefix.Text;
			targetElement.PageOnePrefix.FontStyle = sourceElement.PageOnePrefix.FontStyle;
			targetElement.PageOneSuffix.Text = sourceElement.PageOneSuffix.Text;
			targetElement.PageOneSuffix.FontStyle = sourceElement.PageOneSuffix.FontStyle;
			
			targetElement.PageTwoHasSpecialFormat = sourceElement.PageTwoHasSpecialFormat;
			targetElement.PageTwoPrefix.Text = sourceElement.PageTwoPrefix.Text;
			targetElement.PageTwoPrefix.FontStyle = sourceElement.PageTwoPrefix.FontStyle;
			targetElement.PageTwoSuffix.Text = sourceElement.PageTwoSuffix.Text;
			targetElement.PageTwoSuffix.FontStyle = sourceElement.PageTwoSuffix.FontStyle;
			
			targetElement.PageMultiPrefix.Text = sourceElement.PageMultiPrefix.Text;
			targetElement.PageMultiPrefix.FontStyle = sourceElement.PageMultiPrefix.FontStyle;
			targetElement.PageMultiSuffix.Text = sourceElement.PageMultiSuffix.Text;
			targetElement.PageMultiSuffix.FontStyle = sourceElement.PageMultiSuffix.FontStyle;
			
			#endregion
			
			#region Column
			
			targetElement.ColumnOneHasSpecialFormat = sourceElement.ColumnOneHasSpecialFormat;
			targetElement.ColumnOnePrefix.Text = sourceElement.ColumnOnePrefix.Text;
			targetElement.ColumnOnePrefix.FontStyle = sourceElement.ColumnOnePrefix.FontStyle;
			targetElement.ColumnOneSuffix.Text = sourceElement.ColumnOneSuffix.Text;
			targetElement.ColumnOneSuffix.FontStyle = sourceElement.ColumnOneSuffix.FontStyle;
			
			targetElement.ColumnTwoHasSpecialFormat = sourceElement.ColumnTwoHasSpecialFormat;
			targetElement.ColumnTwoPrefix.Text = sourceElement.ColumnTwoPrefix.Text;
			targetElement.ColumnTwoPrefix.FontStyle = sourceElement.ColumnTwoPrefix.FontStyle;
			targetElement.ColumnTwoSuffix.Text = sourceElement.ColumnTwoSuffix.Text;
			targetElement.ColumnTwoSuffix.FontStyle = sourceElement.ColumnTwoSuffix.FontStyle;
			
			targetElement.ColumnMultiPrefix.Text = sourceElement.ColumnMultiPrefix.Text;
			targetElement.ColumnMultiPrefix.FontStyle = sourceElement.ColumnMultiPrefix.FontStyle;
			targetElement.ColumnMultiSuffix.Text = sourceElement.ColumnMultiSuffix.Text;
			targetElement.ColumnMultiSuffix.FontStyle = sourceElement.ColumnMultiSuffix.FontStyle;
			
			#endregion
			
			#region Paragraph
			
			targetElement.ParagraphOneHasSpecialFormat = sourceElement.ParagraphOneHasSpecialFormat;
			targetElement.ParagraphOnePrefix.Text = sourceElement.ParagraphOnePrefix.Text;
			targetElement.ParagraphOnePrefix.FontStyle = sourceElement.ParagraphOnePrefix.FontStyle;
			targetElement.ParagraphOneSuffix.Text = sourceElement.ParagraphOneSuffix.Text;
			targetElement.ParagraphOneSuffix.FontStyle = sourceElement.ParagraphOneSuffix.FontStyle;
			
			targetElement.ParagraphTwoHasSpecialFormat = sourceElement.ParagraphTwoHasSpecialFormat;
			targetElement.ParagraphTwoPrefix.Text = sourceElement.ParagraphTwoPrefix.Text;
			targetElement.ParagraphTwoPrefix.FontStyle = sourceElement.ParagraphTwoPrefix.FontStyle;
			targetElement.ParagraphTwoSuffix.Text = sourceElement.ParagraphTwoSuffix.Text;
			targetElement.ParagraphTwoSuffix.FontStyle = sourceElement.ParagraphTwoSuffix.FontStyle;
			
			targetElement.ParagraphMultiPrefix.Text = sourceElement.ParagraphMultiPrefix.Text;
			targetElement.ParagraphMultiPrefix.FontStyle = sourceElement.ParagraphMultiPrefix.FontStyle;
			targetElement.ParagraphMultiSuffix.Text = sourceElement.ParagraphMultiSuffix.Text;
			targetElement.ParagraphMultiSuffix.FontStyle = sourceElement.ParagraphMultiSuffix.FontStyle;
			
			#endregion
			
			#region Margin
			
			targetElement.MarginOneHasSpecialFormat = sourceElement.MarginOneHasSpecialFormat;
			targetElement.MarginOnePrefix.Text = sourceElement.MarginOnePrefix.Text;
			targetElement.MarginOnePrefix.FontStyle = sourceElement.MarginOnePrefix.FontStyle;
			targetElement.MarginOneSuffix.Text = sourceElement.MarginOneSuffix.Text;
			targetElement.MarginOneSuffix.FontStyle = sourceElement.MarginOneSuffix.FontStyle;
			
			targetElement.MarginTwoHasSpecialFormat = sourceElement.MarginTwoHasSpecialFormat;
			targetElement.MarginTwoPrefix.Text = sourceElement.MarginTwoPrefix.Text;
			targetElement.MarginTwoPrefix.FontStyle = sourceElement.MarginTwoPrefix.FontStyle;
			targetElement.MarginTwoSuffix.Text = sourceElement.MarginTwoSuffix.Text;
			targetElement.MarginTwoSuffix.FontStyle = sourceElement.MarginTwoSuffix.FontStyle;
			
			targetElement.MarginMultiPrefix.Text = sourceElement.MarginMultiPrefix.Text;
			targetElement.MarginMultiPrefix.FontStyle = sourceElement.MarginMultiPrefix.FontStyle;
			targetElement.MarginMultiSuffix.Text = sourceElement.MarginMultiSuffix.Text;
			targetElement.MarginMultiSuffix.FontStyle = sourceElement.MarginMultiSuffix.FontStyle;
			
			#endregion
			
			#region Other
			
			targetElement.OtherOneHasSpecialFormat = sourceElement.OtherOneHasSpecialFormat;
			targetElement.OtherOnePrefix.Text = sourceElement.OtherOnePrefix.Text;
			targetElement.OtherOnePrefix.FontStyle = sourceElement.OtherOnePrefix.FontStyle;
			targetElement.OtherOneSuffix.Text = sourceElement.OtherOneSuffix.Text;
			targetElement.OtherOneSuffix.FontStyle = sourceElement.OtherOneSuffix.FontStyle;
			
			targetElement.OtherTwoHasSpecialFormat = sourceElement.OtherTwoHasSpecialFormat;
			targetElement.OtherTwoPrefix.Text = sourceElement.OtherTwoPrefix.Text;
			targetElement.OtherTwoPrefix.FontStyle = sourceElement.OtherTwoPrefix.FontStyle;
			targetElement.OtherTwoSuffix.Text = sourceElement.OtherTwoSuffix.Text;
			targetElement.OtherTwoSuffix.FontStyle = sourceElement.OtherTwoSuffix.FontStyle;
			
			targetElement.OtherMultiPrefix.Text = sourceElement.OtherMultiPrefix.Text;
			targetElement.OtherMultiPrefix.FontStyle = sourceElement.OtherMultiPrefix.FontStyle;
			targetElement.OtherMultiSuffix.Text = sourceElement.OtherMultiSuffix.Text;
			targetElement.OtherMultiSuffix.FontStyle = sourceElement.OtherMultiSuffix.FontStyle;
			
			#endregion
        }

        private void SetQuotationPageRangeFormatting(QuotationPageRangeFieldElement quotationPageRangeFieldElement, string language)
        {
            if (language.IndexOf("DE", System.StringComparison.OrdinalIgnoreCase) >= 0 || language.IndexOf("ger", System.StringComparison.OrdinalIgnoreCase) >= 0)
            {
                #region Page
				
				quotationPageRangeFieldElement.PageOneHasSpecialFormat = true;
				quotationPageRangeFieldElement.PageOneNumberingStyle = NumberingStyle.StartPageOnly;
				quotationPageRangeFieldElement.PageOnePrefix.Text = "";
				quotationPageRangeFieldElement.PageOnePrefix.FontStyle = FontStyle.Neutral;
                quotationPageRangeFieldElement.PageOneSuffix.Text = "";
				quotationPageRangeFieldElement.PageOneSuffix.FontStyle = FontStyle.Neutral;
				
				quotationPageRangeFieldElement.PageTwoHasSpecialFormat = true;
				quotationPageRangeFieldElement.PageTwoNumberingStyle = NumberingStyle.StartPageOnly;
                quotationPageRangeFieldElement.PageTwoPrefix.Text = "";
				quotationPageRangeFieldElement.PageTwoPrefix.FontStyle = FontStyle.Neutral;
                quotationPageRangeFieldElement.PageTwoSuffix.Text = "\u00A0f.";
				quotationPageRangeFieldElement.PageTwoSuffix.FontStyle = FontStyle.Neutral;
				
				quotationPageRangeFieldElement.PageMultiNumberingStyle = NumberingStyle.StartPageOnly;
                quotationPageRangeFieldElement.PageMultiPrefix.Text = "";
				quotationPageRangeFieldElement.PageMultiPrefix.FontStyle = FontStyle.Neutral;
				quotationPageRangeFieldElement.PageMultiSuffix.Text = "\u00A0ff.";
				quotationPageRangeFieldElement.PageMultiSuffix.FontStyle = FontStyle.Neutral;
				
				#endregion
				
				#region Column
				
				quotationPageRangeFieldElement.ColumnOneHasSpecialFormat = true;
				quotationPageRangeFieldElement.ColumnOneNumberingStyle = NumberingStyle.StartPageOnly;
				quotationPageRangeFieldElement.ColumnOnePrefix.Text = "Sp.\u00A0";
				quotationPageRangeFieldElement.ColumnOnePrefix.FontStyle = FontStyle.Neutral;
				quotationPageRangeFieldElement.ColumnOneSuffix.Text = "";
				quotationPageRangeFieldElement.ColumnOneSuffix.FontStyle = FontStyle.Neutral;
				
				quotationPageRangeFieldElement.ColumnTwoHasSpecialFormat = true;
				quotationPageRangeFieldElement.ColumnTwoNumberingStyle = NumberingStyle.StartPageOnly;
				quotationPageRangeFieldElement.ColumnTwoPrefix.Text = "Sp.\u00A0";
				quotationPageRangeFieldElement.ColumnTwoPrefix.FontStyle = FontStyle.Neutral;
				quotationPageRangeFieldElement.ColumnTwoSuffix.Text = "\u00A0f.";
				quotationPageRangeFieldElement.ColumnTwoSuffix.FontStyle = FontStyle.Neutral;
				
				quotationPageRangeFieldElement.ColumnMultiNumberingStyle = NumberingStyle.StartPageOnly;
				quotationPageRangeFieldElement.ColumnMultiPrefix.Text = "Sp.\u00A0";
				quotationPageRangeFieldElement.ColumnMultiPrefix.FontStyle = FontStyle.Neutral;
				quotationPageRangeFieldElement.ColumnMultiSuffix.Text = "\u00A0ff.";
				quotationPageRangeFieldElement.ColumnMultiSuffix.FontStyle = FontStyle.Neutral;				
				
				#endregion
				
				#region Paragraph
				
				quotationPageRangeFieldElement.ParagraphOneHasSpecialFormat = true;
				quotationPageRangeFieldElement.ParagraphOneNumberingStyle = NumberingStyle.StartPageOnly;
				quotationPageRangeFieldElement.ParagraphOnePrefix.Text = "§\u00A0";
				quotationPageRangeFieldElement.ParagraphOnePrefix.FontStyle = FontStyle.Neutral;
				quotationPageRangeFieldElement.ParagraphOneSuffix.Text = "";
				quotationPageRangeFieldElement.ParagraphOneSuffix.FontStyle = FontStyle.Neutral;				
				
				quotationPageRangeFieldElement.ParagraphTwoHasSpecialFormat = false;
				quotationPageRangeFieldElement.ParagraphTwoNumberingStyle = NumberingStyle.FullRange;
				quotationPageRangeFieldElement.ParagraphTwoPrefix.Text = "§§\u00A0";
				quotationPageRangeFieldElement.ParagraphTwoPrefix.FontStyle = FontStyle.Neutral;
				quotationPageRangeFieldElement.ParagraphTwoSuffix.Text = "";
				quotationPageRangeFieldElement.ParagraphTwoSuffix.FontStyle = FontStyle.Neutral;
				
				quotationPageRangeFieldElement.ParagraphMultiNumberingStyle = NumberingStyle.FullRange;
				quotationPageRangeFieldElement.ParagraphMultiPrefix.Text = "§§\u00A0";
				quotationPageRangeFieldElement.ParagraphMultiPrefix.FontStyle = FontStyle.Neutral;
				quotationPageRangeFieldElement.ParagraphMultiSuffix.Text = "";
				quotationPageRangeFieldElement.ParagraphMultiSuffix.FontStyle = FontStyle.Neutral;
				
				#endregion
				
				#region Margin
				
				quotationPageRangeFieldElement.MarginOneHasSpecialFormat = true;
				quotationPageRangeFieldElement.MarginOneNumberingStyle = NumberingStyle.StartPageOnly;
				quotationPageRangeFieldElement.MarginOnePrefix.Text = "Rn.\u00A0";
				quotationPageRangeFieldElement.MarginOnePrefix.FontStyle = FontStyle.Neutral;
				quotationPageRangeFieldElement.MarginOneSuffix.Text = "";
				quotationPageRangeFieldElement.MarginOneSuffix.FontStyle = FontStyle.Neutral;				
				
				quotationPageRangeFieldElement.MarginTwoHasSpecialFormat = true;
				quotationPageRangeFieldElement.MarginTwoNumberingStyle = NumberingStyle.StartPageOnly;
				quotationPageRangeFieldElement.MarginTwoPrefix.Text = "Rn.\u00A0";
				quotationPageRangeFieldElement.MarginTwoPrefix.FontStyle = FontStyle.Neutral;
				quotationPageRangeFieldElement.MarginTwoSuffix.Text = "\u00A0f.";
				quotationPageRangeFieldElement.MarginTwoSuffix.FontStyle = FontStyle.Neutral;				
				
				quotationPageRangeFieldElement.MarginMultiNumberingStyle = NumberingStyle.StartPageOnly;
				quotationPageRangeFieldElement.MarginMultiPrefix.Text = "Rn.\u00A0";
				quotationPageRangeFieldElement.MarginMultiPrefix.FontStyle = FontStyle.Neutral;
				quotationPageRangeFieldElement.MarginMultiSuffix.Text = "\u00A0ff.";
				quotationPageRangeFieldElement.MarginMultiSuffix.FontStyle = FontStyle.Neutral;				
				
				#endregion
				
				#region Other
				
				quotationPageRangeFieldElement.OtherOneHasSpecialFormat = false;
				quotationPageRangeFieldElement.OtherOneNumberingStyle = NumberingStyle.StartPageOnly;
				quotationPageRangeFieldElement.OtherOnePrefix.Text = "";
				quotationPageRangeFieldElement.OtherOnePrefix.FontStyle = FontStyle.Neutral;
				quotationPageRangeFieldElement.OtherOneSuffix.Text = "";
				quotationPageRangeFieldElement.OtherOneSuffix.FontStyle = FontStyle.Neutral;				
				
				quotationPageRangeFieldElement.OtherTwoHasSpecialFormat = false;
				quotationPageRangeFieldElement.OtherTwoNumberingStyle = NumberingStyle.FullRange;
				quotationPageRangeFieldElement.OtherTwoPrefix.Text = "";
				quotationPageRangeFieldElement.OtherTwoPrefix.FontStyle = FontStyle.Neutral;
				quotationPageRangeFieldElement.OtherTwoSuffix.Text = "";
				quotationPageRangeFieldElement.OtherTwoSuffix.FontStyle = FontStyle.Neutral;					
				
				quotationPageRangeFieldElement.OtherMultiNumberingStyle = NumberingStyle.FullRange;
				quotationPageRangeFieldElement.OtherMultiPrefix.Text = "";
				quotationPageRangeFieldElement.OtherMultiPrefix.FontStyle = FontStyle.Neutral;
				quotationPageRangeFieldElement.OtherMultiSuffix.Text = "";
				quotationPageRangeFieldElement.OtherMultiSuffix.FontStyle = FontStyle.Neutral;					
				
				#endregion
				
				
            }
            else if (language.IndexOf("EN", System.StringComparison.OrdinalIgnoreCase) >= 0) 
            {
                #region Page
				
				quotationPageRangeFieldElement.PageOneHasSpecialFormat = true;
				quotationPageRangeFieldElement.PageOneNumberingStyle = NumberingStyle.StartPageOnly;
                quotationPageRangeFieldElement.PageOnePrefix.Text = "";
				quotationPageRangeFieldElement.PageOnePrefix.FontStyle = FontStyle.Neutral;
				quotationPageRangeFieldElement.PageOneSuffix.Text = "";
				quotationPageRangeFieldElement.PageOneSuffix.FontStyle = FontStyle.Neutral;
				
				quotationPageRangeFieldElement.PageTwoHasSpecialFormat = true;
				quotationPageRangeFieldElement.PageTwoNumberingStyle = NumberingStyle.StartPageOnly;
                quotationPageRangeFieldElement.PageTwoPrefix.Text = "";
				quotationPageRangeFieldElement.PageTwoPrefix.FontStyle = FontStyle.Neutral;
				quotationPageRangeFieldElement.PageTwoSuffix.Text = "\u00A0f.";
				quotationPageRangeFieldElement.PageTwoSuffix.FontStyle = FontStyle.Neutral;
				
				quotationPageRangeFieldElement.PageMultiNumberingStyle = NumberingStyle.StartPageOnly;
                quotationPageRangeFieldElement.PageMultiPrefix.Text = "";
				quotationPageRangeFieldElement.PageMultiPrefix.FontStyle = FontStyle.Neutral;
				quotationPageRangeFieldElement.PageMultiSuffix.Text = "\u00A0ff.";
				quotationPageRangeFieldElement.PageMultiSuffix.FontStyle = FontStyle.Neutral;
				
				#endregion
				
				#region Column
				
				quotationPageRangeFieldElement.ColumnOneHasSpecialFormat = true;
				quotationPageRangeFieldElement.ColumnOneNumberingStyle = NumberingStyle.StartPageOnly;
				quotationPageRangeFieldElement.ColumnOnePrefix.Text = "col.\u00A0";
				quotationPageRangeFieldElement.ColumnOnePrefix.FontStyle = FontStyle.Neutral;
				quotationPageRangeFieldElement.ColumnOneSuffix.Text = "";
				quotationPageRangeFieldElement.ColumnOneSuffix.FontStyle = FontStyle.Neutral;
				
				quotationPageRangeFieldElement.ColumnTwoHasSpecialFormat = true;
				quotationPageRangeFieldElement.ColumnTwoNumberingStyle = NumberingStyle.StartPageOnly;
				quotationPageRangeFieldElement.ColumnTwoPrefix.Text = "col.\u00A0";
				quotationPageRangeFieldElement.ColumnTwoPrefix.FontStyle = FontStyle.Neutral;
				quotationPageRangeFieldElement.ColumnTwoSuffix.Text = "\u00A0f.";
				quotationPageRangeFieldElement.ColumnTwoSuffix.FontStyle = FontStyle.Neutral;
				
				quotationPageRangeFieldElement.ColumnMultiNumberingStyle = NumberingStyle.StartPageOnly;
				quotationPageRangeFieldElement.ColumnMultiPrefix.Text = "col.\u00A0";
				quotationPageRangeFieldElement.ColumnMultiPrefix.FontStyle = FontStyle.Neutral;
				quotationPageRangeFieldElement.ColumnMultiSuffix.Text = "\u00A0ff.";
				quotationPageRangeFieldElement.ColumnMultiSuffix.FontStyle = FontStyle.Neutral;				
				
				#endregion
				
				#region Paragraph
				
				quotationPageRangeFieldElement.ParagraphOneHasSpecialFormat = true;
				quotationPageRangeFieldElement.ParagraphOneNumberingStyle = NumberingStyle.StartPageOnly;
				quotationPageRangeFieldElement.ParagraphOnePrefix.Text = "§\u00A0";
				quotationPageRangeFieldElement.ParagraphOnePrefix.FontStyle = FontStyle.Neutral;
				quotationPageRangeFieldElement.ParagraphOneSuffix.Text = "";
				quotationPageRangeFieldElement.ParagraphOneSuffix.FontStyle = FontStyle.Neutral;				
				
				quotationPageRangeFieldElement.ParagraphTwoHasSpecialFormat = false;
				quotationPageRangeFieldElement.ParagraphTwoNumberingStyle = NumberingStyle.FullRange;
				quotationPageRangeFieldElement.ParagraphTwoPrefix.Text = "§§\u00A0";
				quotationPageRangeFieldElement.ParagraphTwoPrefix.FontStyle = FontStyle.Neutral;
				quotationPageRangeFieldElement.ParagraphTwoSuffix.Text = "";
				quotationPageRangeFieldElement.ParagraphTwoSuffix.FontStyle = FontStyle.Neutral;
				
				quotationPageRangeFieldElement.ParagraphMultiNumberingStyle = NumberingStyle.FullRange;
				quotationPageRangeFieldElement.ParagraphMultiPrefix.Text = "§§\u00A0";
				quotationPageRangeFieldElement.ParagraphMultiPrefix.FontStyle = FontStyle.Neutral;
				quotationPageRangeFieldElement.ParagraphMultiSuffix.Text = "";
				quotationPageRangeFieldElement.ParagraphMultiSuffix.FontStyle = FontStyle.Neutral;
				
				#endregion
				
				#region Margin
				
				quotationPageRangeFieldElement.MarginOneHasSpecialFormat = true;
				quotationPageRangeFieldElement.MarginOneNumberingStyle = NumberingStyle.StartPageOnly;
				quotationPageRangeFieldElement.MarginOnePrefix.Text = "no.\u00A0";
				quotationPageRangeFieldElement.MarginOnePrefix.FontStyle = FontStyle.Neutral;
				quotationPageRangeFieldElement.MarginOneSuffix.Text = "";
				quotationPageRangeFieldElement.MarginOneSuffix.FontStyle = FontStyle.Neutral;				
				
				quotationPageRangeFieldElement.MarginTwoHasSpecialFormat = true;
				quotationPageRangeFieldElement.MarginTwoNumberingStyle = NumberingStyle.StartPageOnly;
				quotationPageRangeFieldElement.MarginTwoPrefix.Text = "no.\u00A0";
				quotationPageRangeFieldElement.MarginTwoPrefix.FontStyle = FontStyle.Neutral;
				quotationPageRangeFieldElement.MarginTwoSuffix.Text = "\u00A0f.";
				quotationPageRangeFieldElement.MarginTwoSuffix.FontStyle = FontStyle.Neutral;				
				
				quotationPageRangeFieldElement.MarginMultiNumberingStyle = NumberingStyle.StartPageOnly;
				quotationPageRangeFieldElement.MarginMultiPrefix.Text = "no.\u00A0";
				quotationPageRangeFieldElement.MarginMultiPrefix.FontStyle = FontStyle.Neutral;
				quotationPageRangeFieldElement.MarginMultiSuffix.Text = "\u00A0ff.";
				quotationPageRangeFieldElement.MarginMultiSuffix.FontStyle = FontStyle.Neutral;				
				
				#endregion
				
				#region Other
				
				quotationPageRangeFieldElement.OtherOneHasSpecialFormat = false;
				quotationPageRangeFieldElement.OtherOneNumberingStyle = NumberingStyle.StartPageOnly;
				quotationPageRangeFieldElement.OtherOnePrefix.Text = "";
				quotationPageRangeFieldElement.OtherOnePrefix.FontStyle = FontStyle.Neutral;
				quotationPageRangeFieldElement.OtherOneSuffix.Text = "";
				quotationPageRangeFieldElement.OtherOneSuffix.FontStyle = FontStyle.Neutral;				
				
				quotationPageRangeFieldElement.OtherTwoHasSpecialFormat = false;
				quotationPageRangeFieldElement.OtherTwoNumberingStyle = NumberingStyle.FullRange;
				quotationPageRangeFieldElement.OtherTwoPrefix.Text = "";
				quotationPageRangeFieldElement.OtherTwoPrefix.FontStyle = FontStyle.Neutral;
				quotationPageRangeFieldElement.OtherTwoSuffix.Text = "";
				quotationPageRangeFieldElement.OtherTwoSuffix.FontStyle = FontStyle.Neutral;					
				
				quotationPageRangeFieldElement.OtherMultiNumberingStyle = NumberingStyle.FullRange;
				quotationPageRangeFieldElement.OtherMultiPrefix.Text = "";
				quotationPageRangeFieldElement.OtherMultiPrefix.FontStyle = FontStyle.Neutral;
				quotationPageRangeFieldElement.OtherMultiSuffix.Text = "";
				quotationPageRangeFieldElement.OtherMultiSuffix.FontStyle = FontStyle.Neutral;					
				
				#endregion
				
				
            }
            else if (language.IndexOf("FR", System.StringComparison.OrdinalIgnoreCase) >= 0)
            {
                #region Page
				
				quotationPageRangeFieldElement.PageOneHasSpecialFormat = true;
				quotationPageRangeFieldElement.PageOneNumberingStyle = NumberingStyle.StartPageOnly;
                quotationPageRangeFieldElement.PageOnePrefix.Text = "";
                quotationPageRangeFieldElement.PageOnePrefix.FontStyle = FontStyle.Neutral;
				quotationPageRangeFieldElement.PageOneSuffix.Text = "";
				quotationPageRangeFieldElement.PageOneSuffix.FontStyle = FontStyle.Neutral;
				
				quotationPageRangeFieldElement.PageTwoHasSpecialFormat = true;
				quotationPageRangeFieldElement.PageTwoNumberingStyle = NumberingStyle.StartPageOnly;
                quotationPageRangeFieldElement.PageTwoPrefix.Text = "";
                quotationPageRangeFieldElement.PageTwoPrefix.FontStyle = FontStyle.Neutral;
				quotationPageRangeFieldElement.PageTwoSuffix.Text = "\u00A0sq.";
				quotationPageRangeFieldElement.PageTwoSuffix.FontStyle = FontStyle.Neutral;
				
				quotationPageRangeFieldElement.PageMultiNumberingStyle = NumberingStyle.StartPageOnly;
                quotationPageRangeFieldElement.PageMultiPrefix.Text = "";
                quotationPageRangeFieldElement.PageMultiPrefix.FontStyle = FontStyle.Neutral;
				quotationPageRangeFieldElement.PageMultiSuffix.Text = "\u00A0sqq.";
				quotationPageRangeFieldElement.PageMultiSuffix.FontStyle = FontStyle.Neutral;
				
				#endregion
				
				#region Column
				
				quotationPageRangeFieldElement.ColumnOneHasSpecialFormat = true;
				quotationPageRangeFieldElement.ColumnOneNumberingStyle = NumberingStyle.StartPageOnly;
				quotationPageRangeFieldElement.ColumnOnePrefix.Text = "col.\u00A0";
				quotationPageRangeFieldElement.ColumnOnePrefix.FontStyle = FontStyle.Neutral;
				quotationPageRangeFieldElement.ColumnOneSuffix.Text = "";
				quotationPageRangeFieldElement.ColumnOneSuffix.FontStyle = FontStyle.Neutral;
				
				quotationPageRangeFieldElement.ColumnTwoHasSpecialFormat = true;
				quotationPageRangeFieldElement.ColumnTwoNumberingStyle = NumberingStyle.StartPageOnly;
				quotationPageRangeFieldElement.ColumnTwoPrefix.Text = "col.\u00A0";
				quotationPageRangeFieldElement.ColumnTwoPrefix.FontStyle = FontStyle.Neutral;
				quotationPageRangeFieldElement.ColumnTwoSuffix.Text = "\u00A0sq.";
				quotationPageRangeFieldElement.ColumnTwoSuffix.FontStyle = FontStyle.Neutral;
				
				quotationPageRangeFieldElement.ColumnMultiNumberingStyle = NumberingStyle.StartPageOnly;
				quotationPageRangeFieldElement.ColumnMultiPrefix.Text = "col.\u00A0";
				quotationPageRangeFieldElement.ColumnMultiPrefix.FontStyle = FontStyle.Neutral;
				quotationPageRangeFieldElement.ColumnMultiSuffix.Text = "\u00A0sqq.";
				quotationPageRangeFieldElement.ColumnMultiSuffix.FontStyle = FontStyle.Neutral;				
				
				#endregion
				
				#region Paragraph
				
				quotationPageRangeFieldElement.ParagraphOneHasSpecialFormat = true;
				quotationPageRangeFieldElement.ParagraphOneNumberingStyle = NumberingStyle.StartPageOnly;
				quotationPageRangeFieldElement.ParagraphOnePrefix.Text = "§\u00A0";
				quotationPageRangeFieldElement.ParagraphOnePrefix.FontStyle = FontStyle.Neutral;
				quotationPageRangeFieldElement.ParagraphOneSuffix.Text = "";
				quotationPageRangeFieldElement.ParagraphOneSuffix.FontStyle = FontStyle.Neutral;				
				
				quotationPageRangeFieldElement.ParagraphTwoHasSpecialFormat = false;
				quotationPageRangeFieldElement.ParagraphTwoNumberingStyle = NumberingStyle.FullRange;
				quotationPageRangeFieldElement.ParagraphTwoPrefix.Text = "§§\u00A0";
				quotationPageRangeFieldElement.ParagraphTwoPrefix.FontStyle = FontStyle.Neutral;
				quotationPageRangeFieldElement.ParagraphTwoSuffix.Text = "";
				quotationPageRangeFieldElement.ParagraphTwoSuffix.FontStyle = FontStyle.Neutral;
				
				quotationPageRangeFieldElement.ParagraphMultiNumberingStyle = NumberingStyle.FullRange;
				quotationPageRangeFieldElement.ParagraphMultiPrefix.Text = "§§\u00A0";
				quotationPageRangeFieldElement.ParagraphMultiPrefix.FontStyle = FontStyle.Neutral;
				quotationPageRangeFieldElement.ParagraphMultiSuffix.Text = "";
				quotationPageRangeFieldElement.ParagraphMultiSuffix.FontStyle = FontStyle.Neutral;
				
				#endregion
				
				#region Margin
				
				quotationPageRangeFieldElement.MarginOneHasSpecialFormat = true;
				quotationPageRangeFieldElement.MarginOneNumberingStyle = NumberingStyle.StartPageOnly;
				quotationPageRangeFieldElement.MarginOnePrefix.Text = "n°\u00A0";
				quotationPageRangeFieldElement.MarginOnePrefix.FontStyle = FontStyle.Neutral;
				quotationPageRangeFieldElement.MarginOneSuffix.Text = "";
				quotationPageRangeFieldElement.MarginOneSuffix.FontStyle = FontStyle.Neutral;				
				
				quotationPageRangeFieldElement.MarginTwoHasSpecialFormat = true;
				quotationPageRangeFieldElement.MarginTwoNumberingStyle = NumberingStyle.StartPageOnly;
				quotationPageRangeFieldElement.MarginTwoPrefix.Text = "n°\u00A0";
				quotationPageRangeFieldElement.MarginTwoPrefix.FontStyle = FontStyle.Neutral;
				quotationPageRangeFieldElement.MarginTwoSuffix.Text = "\u00A0sq.";
				quotationPageRangeFieldElement.MarginTwoSuffix.FontStyle = FontStyle.Neutral;				
				
				quotationPageRangeFieldElement.MarginMultiNumberingStyle = NumberingStyle.StartPageOnly;
				quotationPageRangeFieldElement.MarginMultiPrefix.Text = "n°\u00A0";
				quotationPageRangeFieldElement.MarginMultiPrefix.FontStyle = FontStyle.Neutral;
				quotationPageRangeFieldElement.MarginMultiSuffix.Text = "\u00A0sqq.";
				quotationPageRangeFieldElement.MarginMultiSuffix.FontStyle = FontStyle.Neutral;				
				
				#endregion
				
				#region Other
				
				quotationPageRangeFieldElement.OtherOneHasSpecialFormat = false;
				quotationPageRangeFieldElement.OtherOneNumberingStyle = NumberingStyle.StartPageOnly;
				quotationPageRangeFieldElement.OtherOnePrefix.Text = "";
				quotationPageRangeFieldElement.OtherOnePrefix.FontStyle = FontStyle.Neutral;
				quotationPageRangeFieldElement.OtherOneSuffix.Text = "";
				quotationPageRangeFieldElement.OtherOneSuffix.FontStyle = FontStyle.Neutral;				
				
				quotationPageRangeFieldElement.OtherTwoHasSpecialFormat = false;
				quotationPageRangeFieldElement.OtherTwoNumberingStyle = NumberingStyle.FullRange;
				quotationPageRangeFieldElement.OtherTwoPrefix.Text = "";
				quotationPageRangeFieldElement.OtherTwoPrefix.FontStyle = FontStyle.Neutral;
				quotationPageRangeFieldElement.OtherTwoSuffix.Text = "";
				quotationPageRangeFieldElement.OtherTwoSuffix.FontStyle = FontStyle.Neutral;					
				
				quotationPageRangeFieldElement.OtherMultiNumberingStyle = NumberingStyle.FullRange;
				quotationPageRangeFieldElement.OtherMultiPrefix.Text = "";
				quotationPageRangeFieldElement.OtherMultiPrefix.FontStyle = FontStyle.Neutral;
				quotationPageRangeFieldElement.OtherMultiSuffix.Text = "";
				quotationPageRangeFieldElement.OtherMultiSuffix.FontStyle = FontStyle.Neutral;					
				
				#endregion
				
				
            }
            else if (language.IndexOf("IT", System.StringComparison.OrdinalIgnoreCase) >= 0)
            {  
                #region Page
				
				quotationPageRangeFieldElement.PageOneHasSpecialFormat = true;
				quotationPageRangeFieldElement.PageOneNumberingStyle = NumberingStyle.StartPageOnly;
				quotationPageRangeFieldElement.PageOnePrefix.Text = "";
                quotationPageRangeFieldElement.PageOnePrefix.FontStyle = FontStyle.Neutral;
				quotationPageRangeFieldElement.PageOneSuffix.Text = "";
				quotationPageRangeFieldElement.PageOneSuffix.FontStyle = FontStyle.Neutral;
				
				quotationPageRangeFieldElement.PageTwoHasSpecialFormat = true;
				quotationPageRangeFieldElement.PageTwoNumberingStyle = NumberingStyle.StartPageOnly;
				quotationPageRangeFieldElement.PageTwoPrefix.Text = "";
                quotationPageRangeFieldElement.PageTwoPrefix.FontStyle = FontStyle.Neutral;
				quotationPageRangeFieldElement.PageTwoSuffix.Text = "\u00A0e\u00A0seg.";
				quotationPageRangeFieldElement.PageTwoSuffix.FontStyle = FontStyle.Neutral;
				
				quotationPageRangeFieldElement.PageMultiNumberingStyle = NumberingStyle.StartPageOnly;
				quotationPageRangeFieldElement.PageMultiPrefix.Text = "";
                quotationPageRangeFieldElement.PageMultiPrefix.FontStyle = FontStyle.Neutral;
				quotationPageRangeFieldElement.PageMultiSuffix.Text = "\u00A0e\u00A0segg.";
				quotationPageRangeFieldElement.PageMultiSuffix.FontStyle = FontStyle.Neutral;
				
				#endregion
				
				#region Column
				
				quotationPageRangeFieldElement.ColumnOneHasSpecialFormat = true;
				quotationPageRangeFieldElement.ColumnOneNumberingStyle = NumberingStyle.StartPageOnly;
				quotationPageRangeFieldElement.ColumnOnePrefix.Text = "col.\u00A0";
				quotationPageRangeFieldElement.ColumnOnePrefix.FontStyle = FontStyle.Neutral;
				quotationPageRangeFieldElement.ColumnOneSuffix.Text = "";
				quotationPageRangeFieldElement.ColumnOneSuffix.FontStyle = FontStyle.Neutral;
				
				quotationPageRangeFieldElement.ColumnTwoHasSpecialFormat = true;
				quotationPageRangeFieldElement.ColumnTwoNumberingStyle = NumberingStyle.StartPageOnly;
				quotationPageRangeFieldElement.ColumnTwoPrefix.Text = "col.\u00A0";
				quotationPageRangeFieldElement.ColumnTwoPrefix.FontStyle = FontStyle.Neutral;
				quotationPageRangeFieldElement.ColumnTwoSuffix.Text = "\u00A0e\u00A0seg.";
				quotationPageRangeFieldElement.ColumnTwoSuffix.FontStyle = FontStyle.Neutral;
				
				quotationPageRangeFieldElement.ColumnMultiNumberingStyle = NumberingStyle.StartPageOnly;
				quotationPageRangeFieldElement.ColumnMultiPrefix.Text = "col.\u00A0";
				quotationPageRangeFieldElement.ColumnMultiPrefix.FontStyle = FontStyle.Neutral;
				quotationPageRangeFieldElement.ColumnMultiSuffix.Text = "\u00A0e\u00A0segg.";
				quotationPageRangeFieldElement.ColumnMultiSuffix.FontStyle = FontStyle.Neutral;				
				
				#endregion
				
				#region Paragraph
				
				quotationPageRangeFieldElement.ParagraphOneHasSpecialFormat = true;
				quotationPageRangeFieldElement.ParagraphOneNumberingStyle = NumberingStyle.StartPageOnly;
				quotationPageRangeFieldElement.ParagraphOnePrefix.Text = "§\u00A0";
				quotationPageRangeFieldElement.ParagraphOnePrefix.FontStyle = FontStyle.Neutral;
				quotationPageRangeFieldElement.ParagraphOneSuffix.Text = "";
				quotationPageRangeFieldElement.ParagraphOneSuffix.FontStyle = FontStyle.Neutral;				
				
				quotationPageRangeFieldElement.ParagraphTwoHasSpecialFormat = false;
				quotationPageRangeFieldElement.ParagraphTwoNumberingStyle = NumberingStyle.FullRange;
				quotationPageRangeFieldElement.ParagraphTwoPrefix.Text = "§§\u00A0";
				quotationPageRangeFieldElement.ParagraphTwoPrefix.FontStyle = FontStyle.Neutral;
				quotationPageRangeFieldElement.ParagraphTwoSuffix.Text = "";
				quotationPageRangeFieldElement.ParagraphTwoSuffix.FontStyle = FontStyle.Neutral;
				
				quotationPageRangeFieldElement.ParagraphMultiNumberingStyle = NumberingStyle.FullRange;
				quotationPageRangeFieldElement.ParagraphMultiPrefix.Text = "§§\u00A0";
				quotationPageRangeFieldElement.ParagraphMultiPrefix.FontStyle = FontStyle.Neutral;
				quotationPageRangeFieldElement.ParagraphMultiSuffix.Text = "";
				quotationPageRangeFieldElement.ParagraphMultiSuffix.FontStyle = FontStyle.Neutral;
				
				#endregion
				
				#region Margin
				
				quotationPageRangeFieldElement.MarginOneHasSpecialFormat = true;
				quotationPageRangeFieldElement.MarginOneNumberingStyle = NumberingStyle.StartPageOnly;
				quotationPageRangeFieldElement.MarginOnePrefix.Text = "n.\u00A0marg.\u00A0";
				quotationPageRangeFieldElement.MarginOnePrefix.FontStyle = FontStyle.Neutral;
				quotationPageRangeFieldElement.MarginOneSuffix.Text = "";
				quotationPageRangeFieldElement.MarginOneSuffix.FontStyle = FontStyle.Neutral;				
				
				quotationPageRangeFieldElement.MarginTwoHasSpecialFormat = true;
				quotationPageRangeFieldElement.MarginTwoNumberingStyle = NumberingStyle.StartPageOnly;
				quotationPageRangeFieldElement.MarginTwoPrefix.Text = "n.\u00A0marg.\u00A0";
				quotationPageRangeFieldElement.MarginTwoPrefix.FontStyle = FontStyle.Neutral;
				quotationPageRangeFieldElement.MarginTwoSuffix.Text = "\u00A0e\u00A0seg.";
				quotationPageRangeFieldElement.MarginTwoSuffix.FontStyle = FontStyle.Neutral;				
				
				quotationPageRangeFieldElement.MarginMultiNumberingStyle = NumberingStyle.StartPageOnly;
				quotationPageRangeFieldElement.MarginMultiPrefix.Text = "n.\u00A0marg.\u00A0";
				quotationPageRangeFieldElement.MarginMultiPrefix.FontStyle = FontStyle.Neutral;
				quotationPageRangeFieldElement.MarginMultiSuffix.Text = "\u00A0e\u00A0segg.";
				quotationPageRangeFieldElement.MarginMultiSuffix.FontStyle = FontStyle.Neutral;				
				
				#endregion
				
				#region Other
				
				quotationPageRangeFieldElement.OtherOneHasSpecialFormat = false;
				quotationPageRangeFieldElement.OtherOneNumberingStyle = NumberingStyle.StartPageOnly;
				quotationPageRangeFieldElement.OtherOnePrefix.Text = "";
				quotationPageRangeFieldElement.OtherOnePrefix.FontStyle = FontStyle.Neutral;
				quotationPageRangeFieldElement.OtherOneSuffix.Text = "";
				quotationPageRangeFieldElement.OtherOneSuffix.FontStyle = FontStyle.Neutral;				
				
				quotationPageRangeFieldElement.OtherTwoHasSpecialFormat = false;
				quotationPageRangeFieldElement.OtherTwoNumberingStyle = NumberingStyle.FullRange;
				quotationPageRangeFieldElement.OtherTwoPrefix.Text = "";
				quotationPageRangeFieldElement.OtherTwoPrefix.FontStyle = FontStyle.Neutral;
				quotationPageRangeFieldElement.OtherTwoSuffix.Text = "";
				quotationPageRangeFieldElement.OtherTwoSuffix.FontStyle = FontStyle.Neutral;					
				
				quotationPageRangeFieldElement.OtherMultiNumberingStyle = NumberingStyle.FullRange;
				quotationPageRangeFieldElement.OtherMultiPrefix.Text = "";
				quotationPageRangeFieldElement.OtherMultiPrefix.FontStyle = FontStyle.Neutral;
				quotationPageRangeFieldElement.OtherMultiSuffix.Text = "";
				quotationPageRangeFieldElement.OtherMultiSuffix.FontStyle = FontStyle.Neutral;					
				
				#endregion
				
				
            }
            else if (language.IndexOf("ES", System.StringComparison.OrdinalIgnoreCase) >= 0 || language.IndexOf("spa", System.StringComparison.OrdinalIgnoreCase) >= 0)
            {
                #region Page
				
				quotationPageRangeFieldElement.PageOneHasSpecialFormat = true;
				quotationPageRangeFieldElement.PageOneNumberingStyle = NumberingStyle.StartPageOnly;
				quotationPageRangeFieldElement.PageOnePrefix.Text = "";
                quotationPageRangeFieldElement.PageOnePrefix.FontStyle = FontStyle.Neutral;
				quotationPageRangeFieldElement.PageOneSuffix.Text = "";
				quotationPageRangeFieldElement.PageOneSuffix.FontStyle = FontStyle.Neutral;
				
				quotationPageRangeFieldElement.PageTwoHasSpecialFormat = true;
				quotationPageRangeFieldElement.PageTwoNumberingStyle = NumberingStyle.StartPageOnly;
				quotationPageRangeFieldElement.PageTwoPrefix.Text = "";
                quotationPageRangeFieldElement.PageTwoPrefix.FontStyle = FontStyle.Neutral;
				quotationPageRangeFieldElement.PageTwoSuffix.Text = "\u00A0e\u00A0s.";
				quotationPageRangeFieldElement.PageTwoSuffix.FontStyle = FontStyle.Neutral;
				
				quotationPageRangeFieldElement.PageMultiNumberingStyle = NumberingStyle.StartPageOnly;
				quotationPageRangeFieldElement.PageMultiPrefix.Text = "";
                quotationPageRangeFieldElement.PageMultiPrefix.FontStyle = FontStyle.Neutral;
				quotationPageRangeFieldElement.PageMultiSuffix.Text = "\u00A0e\u00A0ss.";
				quotationPageRangeFieldElement.PageMultiSuffix.FontStyle = FontStyle.Neutral;
				
				#endregion
				
				#region Column
				
				quotationPageRangeFieldElement.ColumnOneHasSpecialFormat = true;
				quotationPageRangeFieldElement.ColumnOneNumberingStyle = NumberingStyle.StartPageOnly;
				quotationPageRangeFieldElement.ColumnOnePrefix.Text = "columna\u00A0";
				quotationPageRangeFieldElement.ColumnOnePrefix.FontStyle = FontStyle.Neutral;
				quotationPageRangeFieldElement.ColumnOneSuffix.Text = "";
				quotationPageRangeFieldElement.ColumnOneSuffix.FontStyle = FontStyle.Neutral;
				
				quotationPageRangeFieldElement.ColumnTwoHasSpecialFormat = true;
				quotationPageRangeFieldElement.ColumnTwoNumberingStyle = NumberingStyle.StartPageOnly;
				quotationPageRangeFieldElement.ColumnTwoPrefix.Text = "columna\u00A0";
				quotationPageRangeFieldElement.ColumnTwoPrefix.FontStyle = FontStyle.Neutral;
				quotationPageRangeFieldElement.ColumnTwoSuffix.Text = "\u00A0e\u00A0s.";
				quotationPageRangeFieldElement.ColumnTwoSuffix.FontStyle = FontStyle.Neutral;
				
				quotationPageRangeFieldElement.ColumnMultiNumberingStyle = NumberingStyle.StartPageOnly;
				quotationPageRangeFieldElement.ColumnMultiPrefix.Text = "columna\u00A0";
				quotationPageRangeFieldElement.ColumnMultiPrefix.FontStyle = FontStyle.Neutral;
				quotationPageRangeFieldElement.ColumnMultiSuffix.Text = "\u00A0e\u00A0ss.";
				quotationPageRangeFieldElement.ColumnMultiSuffix.FontStyle = FontStyle.Neutral;				
				
				#endregion
				
				#region Paragraph
				
				quotationPageRangeFieldElement.ParagraphOneHasSpecialFormat = true;
				quotationPageRangeFieldElement.ParagraphOneNumberingStyle = NumberingStyle.StartPageOnly;
				quotationPageRangeFieldElement.ParagraphOnePrefix.Text = "§\u00A0";
				quotationPageRangeFieldElement.ParagraphOnePrefix.FontStyle = FontStyle.Neutral;
				quotationPageRangeFieldElement.ParagraphOneSuffix.Text = "";
				quotationPageRangeFieldElement.ParagraphOneSuffix.FontStyle = FontStyle.Neutral;				
				
				quotationPageRangeFieldElement.ParagraphTwoHasSpecialFormat = false;
				quotationPageRangeFieldElement.ParagraphTwoNumberingStyle = NumberingStyle.FullRange;
				quotationPageRangeFieldElement.ParagraphTwoPrefix.Text = "§§\u00A0";
				quotationPageRangeFieldElement.ParagraphTwoPrefix.FontStyle = FontStyle.Neutral;
				quotationPageRangeFieldElement.ParagraphTwoSuffix.Text = "";
				quotationPageRangeFieldElement.ParagraphTwoSuffix.FontStyle = FontStyle.Neutral;
				
				quotationPageRangeFieldElement.ParagraphMultiNumberingStyle = NumberingStyle.FullRange;
				quotationPageRangeFieldElement.ParagraphMultiPrefix.Text = "§§\u00A0";
				quotationPageRangeFieldElement.ParagraphMultiPrefix.FontStyle = FontStyle.Neutral;
				quotationPageRangeFieldElement.ParagraphMultiSuffix.Text = "";
				quotationPageRangeFieldElement.ParagraphMultiSuffix.FontStyle = FontStyle.Neutral;
				
				#endregion
				
				#region Margin
				
				quotationPageRangeFieldElement.MarginOneHasSpecialFormat = true;
				quotationPageRangeFieldElement.MarginOneNumberingStyle = NumberingStyle.StartPageOnly;
				quotationPageRangeFieldElement.MarginOnePrefix.Text = "n.º\u00A0margin.\u00A0";
				quotationPageRangeFieldElement.MarginOnePrefix.FontStyle = FontStyle.Neutral;
				quotationPageRangeFieldElement.MarginOneSuffix.Text = "";
				quotationPageRangeFieldElement.MarginOneSuffix.FontStyle = FontStyle.Neutral;				
				
				quotationPageRangeFieldElement.MarginTwoHasSpecialFormat = true;
				quotationPageRangeFieldElement.MarginTwoNumberingStyle = NumberingStyle.StartPageOnly;
				quotationPageRangeFieldElement.MarginTwoPrefix.Text = "n.º\u00A0margin.\u00A0";
				quotationPageRangeFieldElement.MarginTwoPrefix.FontStyle = FontStyle.Neutral;
				quotationPageRangeFieldElement.MarginTwoSuffix.Text = "\u00A0e\u00A0s.";
				quotationPageRangeFieldElement.MarginTwoSuffix.FontStyle = FontStyle.Neutral;				
				
				quotationPageRangeFieldElement.MarginMultiNumberingStyle = NumberingStyle.StartPageOnly;
				quotationPageRangeFieldElement.MarginMultiPrefix.Text = "n.º\u00A0margin.\u00A0";
				quotationPageRangeFieldElement.MarginMultiPrefix.FontStyle = FontStyle.Neutral;
				quotationPageRangeFieldElement.MarginMultiSuffix.Text = "\u00A0e\u00A0ss.";
				quotationPageRangeFieldElement.MarginMultiSuffix.FontStyle = FontStyle.Neutral;				
				
				#endregion
				
				#region Other
				
				quotationPageRangeFieldElement.OtherOneHasSpecialFormat = false;
				quotationPageRangeFieldElement.OtherOneNumberingStyle = NumberingStyle.StartPageOnly;
				quotationPageRangeFieldElement.OtherOnePrefix.Text = "";
				quotationPageRangeFieldElement.OtherOnePrefix.FontStyle = FontStyle.Neutral;
				quotationPageRangeFieldElement.OtherOneSuffix.Text = "";
				quotationPageRangeFieldElement.OtherOneSuffix.FontStyle = FontStyle.Neutral;				
				
				quotationPageRangeFieldElement.OtherTwoHasSpecialFormat = false;
				quotationPageRangeFieldElement.OtherTwoNumberingStyle = NumberingStyle.FullRange;
				quotationPageRangeFieldElement.OtherTwoPrefix.Text = "";
				quotationPageRangeFieldElement.OtherTwoPrefix.FontStyle = FontStyle.Neutral;
				quotationPageRangeFieldElement.OtherTwoSuffix.Text = "";
				quotationPageRangeFieldElement.OtherTwoSuffix.FontStyle = FontStyle.Neutral;					
				
				quotationPageRangeFieldElement.OtherMultiNumberingStyle = NumberingStyle.FullRange;
				quotationPageRangeFieldElement.OtherMultiPrefix.Text = "";
				quotationPageRangeFieldElement.OtherMultiPrefix.FontStyle = FontStyle.Neutral;
				quotationPageRangeFieldElement.OtherMultiSuffix.Text = "";
				quotationPageRangeFieldElement.OtherMultiSuffix.FontStyle = FontStyle.Neutral;					
				
				#endregion
				
				
            }
            else
            {
                #region Page
				
				quotationPageRangeFieldElement.PageOneHasSpecialFormat = true;
				quotationPageRangeFieldElement.PageOneNumberingStyle = NumberingStyle.StartPageOnly;
				quotationPageRangeFieldElement.PageOnePrefix.Text = "";
                quotationPageRangeFieldElement.PageOnePrefix.FontStyle = FontStyle.Neutral;
				quotationPageRangeFieldElement.PageOneSuffix.Text = "";
				quotationPageRangeFieldElement.PageOneSuffix.FontStyle = FontStyle.Neutral;
				
				quotationPageRangeFieldElement.PageTwoHasSpecialFormat = true;
				quotationPageRangeFieldElement.PageTwoNumberingStyle = NumberingStyle.StartPageOnly;
				quotationPageRangeFieldElement.PageTwoPrefix.Text = "";
                quotationPageRangeFieldElement.PageTwoPrefix.FontStyle = FontStyle.Neutral;
				quotationPageRangeFieldElement.PageTwoSuffix.Text = "\u00A0f.";
				quotationPageRangeFieldElement.PageTwoSuffix.FontStyle = FontStyle.Neutral;
				
				quotationPageRangeFieldElement.PageMultiNumberingStyle = NumberingStyle.StartPageOnly;
				quotationPageRangeFieldElement.PageMultiPrefix.Text = "";
                quotationPageRangeFieldElement.PageMultiPrefix.FontStyle = FontStyle.Neutral;
				quotationPageRangeFieldElement.PageMultiSuffix.Text = "\u00A0ff.";
				quotationPageRangeFieldElement.PageMultiSuffix.FontStyle = FontStyle.Neutral;
				
				#endregion
				
				#region Column
				
				quotationPageRangeFieldElement.ColumnOneHasSpecialFormat = true;
				quotationPageRangeFieldElement.ColumnOneNumberingStyle = NumberingStyle.StartPageOnly;
				quotationPageRangeFieldElement.ColumnOnePrefix.Text = "col.\u00A0";
				quotationPageRangeFieldElement.ColumnOnePrefix.FontStyle = FontStyle.Neutral;
				quotationPageRangeFieldElement.ColumnOneSuffix.Text = "";
				quotationPageRangeFieldElement.ColumnOneSuffix.FontStyle = FontStyle.Neutral;
				
				quotationPageRangeFieldElement.ColumnTwoHasSpecialFormat = true;
				quotationPageRangeFieldElement.ColumnTwoNumberingStyle = NumberingStyle.StartPageOnly;
				quotationPageRangeFieldElement.ColumnTwoPrefix.Text = "col.\u00A0";
				quotationPageRangeFieldElement.ColumnTwoPrefix.FontStyle = FontStyle.Neutral;
				quotationPageRangeFieldElement.ColumnTwoSuffix.Text = "\u00A0f.";
				quotationPageRangeFieldElement.ColumnTwoSuffix.FontStyle = FontStyle.Neutral;
				
				quotationPageRangeFieldElement.ColumnMultiNumberingStyle = NumberingStyle.StartPageOnly;
				quotationPageRangeFieldElement.ColumnMultiPrefix.Text = "col.\u00A0";
				quotationPageRangeFieldElement.ColumnMultiPrefix.FontStyle = FontStyle.Neutral;
				quotationPageRangeFieldElement.ColumnMultiSuffix.Text = "\u00A0ff.";
				quotationPageRangeFieldElement.ColumnMultiSuffix.FontStyle = FontStyle.Neutral;				
				
				#endregion
				
				#region Paragraph
				
				quotationPageRangeFieldElement.ParagraphOneHasSpecialFormat = true;
				quotationPageRangeFieldElement.ParagraphOneNumberingStyle = NumberingStyle.StartPageOnly;
				quotationPageRangeFieldElement.ParagraphOnePrefix.Text = "§\u00A0";
				quotationPageRangeFieldElement.ParagraphOnePrefix.FontStyle = FontStyle.Neutral;
				quotationPageRangeFieldElement.ParagraphOneSuffix.Text = "";
				quotationPageRangeFieldElement.ParagraphOneSuffix.FontStyle = FontStyle.Neutral;				
				
				quotationPageRangeFieldElement.ParagraphTwoHasSpecialFormat = false;
				quotationPageRangeFieldElement.ParagraphTwoNumberingStyle = NumberingStyle.FullRange;
				quotationPageRangeFieldElement.ParagraphTwoPrefix.Text = "§§\u00A0";
				quotationPageRangeFieldElement.ParagraphTwoPrefix.FontStyle = FontStyle.Neutral;
				quotationPageRangeFieldElement.ParagraphTwoSuffix.Text = "";
				quotationPageRangeFieldElement.ParagraphTwoSuffix.FontStyle = FontStyle.Neutral;
				
				quotationPageRangeFieldElement.ParagraphMultiNumberingStyle = NumberingStyle.FullRange;
				quotationPageRangeFieldElement.ParagraphMultiPrefix.Text = "§§\u00A0";
				quotationPageRangeFieldElement.ParagraphMultiPrefix.FontStyle = FontStyle.Neutral;
				quotationPageRangeFieldElement.ParagraphMultiSuffix.Text = "";
				quotationPageRangeFieldElement.ParagraphMultiSuffix.FontStyle = FontStyle.Neutral;
				
				#endregion
				
				#region Margin
				
				quotationPageRangeFieldElement.MarginOneHasSpecialFormat = true;
				quotationPageRangeFieldElement.MarginOneNumberingStyle = NumberingStyle.StartPageOnly;
				quotationPageRangeFieldElement.MarginOnePrefix.Text = "no.\u00A0";
				quotationPageRangeFieldElement.MarginOnePrefix.FontStyle = FontStyle.Neutral;
				quotationPageRangeFieldElement.MarginOneSuffix.Text = "";
				quotationPageRangeFieldElement.MarginOneSuffix.FontStyle = FontStyle.Neutral;				
				
				quotationPageRangeFieldElement.MarginTwoHasSpecialFormat = true;
				quotationPageRangeFieldElement.MarginTwoNumberingStyle = NumberingStyle.StartPageOnly;
				quotationPageRangeFieldElement.MarginTwoPrefix.Text = "no.\u00A0";
				quotationPageRangeFieldElement.MarginTwoPrefix.FontStyle = FontStyle.Neutral;
				quotationPageRangeFieldElement.MarginTwoSuffix.Text = "\u00A0f.";
				quotationPageRangeFieldElement.MarginTwoSuffix.FontStyle = FontStyle.Neutral;				
				
				quotationPageRangeFieldElement.MarginMultiNumberingStyle = NumberingStyle.StartPageOnly;
				quotationPageRangeFieldElement.MarginMultiPrefix.Text = "no.\u00A0";
				quotationPageRangeFieldElement.MarginMultiPrefix.FontStyle = FontStyle.Neutral;
				quotationPageRangeFieldElement.MarginMultiSuffix.Text = "\u00A0ff.";
				quotationPageRangeFieldElement.MarginMultiSuffix.FontStyle = FontStyle.Neutral;				
				
				#endregion
				
				#region Other
				
				quotationPageRangeFieldElement.OtherOneHasSpecialFormat = false;
				quotationPageRangeFieldElement.OtherOneNumberingStyle = NumberingStyle.StartPageOnly;
				quotationPageRangeFieldElement.OtherOnePrefix.Text = "";
				quotationPageRangeFieldElement.OtherOnePrefix.FontStyle = FontStyle.Neutral;
				quotationPageRangeFieldElement.OtherOneSuffix.Text = "";
				quotationPageRangeFieldElement.OtherOneSuffix.FontStyle = FontStyle.Neutral;				
				
				quotationPageRangeFieldElement.OtherTwoHasSpecialFormat = false;
				quotationPageRangeFieldElement.OtherTwoNumberingStyle = NumberingStyle.FullRange;
				quotationPageRangeFieldElement.OtherTwoPrefix.Text = "";
				quotationPageRangeFieldElement.OtherTwoPrefix.FontStyle = FontStyle.Neutral;
				quotationPageRangeFieldElement.OtherTwoSuffix.Text = "";
				quotationPageRangeFieldElement.OtherTwoSuffix.FontStyle = FontStyle.Neutral;					
				
				quotationPageRangeFieldElement.OtherMultiNumberingStyle = NumberingStyle.FullRange;
				quotationPageRangeFieldElement.OtherMultiPrefix.Text = "";
				quotationPageRangeFieldElement.OtherMultiPrefix.FontStyle = FontStyle.Neutral;
				quotationPageRangeFieldElement.OtherMultiSuffix.Text = "";
				quotationPageRangeFieldElement.OtherMultiSuffix.FontStyle = FontStyle.Neutral;					
				
				#endregion
				
				
            }
            quotationPageRangeFieldElement.PageOnePrefix.Text = "(" + quotationPageRangeFieldElement.PageOnePrefix.Text;
                quotationPageRangeFieldElement.PageTwoPrefix.Text = "(" + quotationPageRangeFieldElement.PageTwoPrefix.Text;
                quotationPageRangeFieldElement.PageMultiPrefix.Text = "(" + quotationPageRangeFieldElement.PageMultiPrefix.Text;
                quotationPageRangeFieldElement.ColumnOnePrefix.Text = "(" + quotationPageRangeFieldElement.ColumnOnePrefix.Text;
                quotationPageRangeFieldElement.ColumnTwoPrefix.Text = "(" + quotationPageRangeFieldElement.ColumnTwoPrefix.Text;
                quotationPageRangeFieldElement.ColumnMultiPrefix.Text = "(" + quotationPageRangeFieldElement.ColumnMultiPrefix.Text;
                quotationPageRangeFieldElement.ParagraphOnePrefix.Text = "(" + quotationPageRangeFieldElement.ParagraphOnePrefix.Text;
                quotationPageRangeFieldElement.ParagraphTwoPrefix.Text = "(" + quotationPageRangeFieldElement.ParagraphTwoPrefix.Text;
                quotationPageRangeFieldElement.ParagraphMultiPrefix.Text = "(" + quotationPageRangeFieldElement.ParagraphMultiPrefix.Text;
                quotationPageRangeFieldElement.MarginOnePrefix.Text = "(" + quotationPageRangeFieldElement.MarginOnePrefix.Text;
                quotationPageRangeFieldElement.MarginTwoPrefix.Text = "(" + quotationPageRangeFieldElement.MarginTwoPrefix.Text;
                quotationPageRangeFieldElement.MarginMultiPrefix.Text = "(" + quotationPageRangeFieldElement.MarginMultiPrefix.Text;
                quotationPageRangeFieldElement.OtherOnePrefix.Text = "(" + quotationPageRangeFieldElement.OtherOnePrefix.Text;
                quotationPageRangeFieldElement.OtherTwoPrefix.Text = "(" + quotationPageRangeFieldElement.OtherTwoPrefix.Text;
                quotationPageRangeFieldElement.OtherMultiPrefix.Text = "(" + quotationPageRangeFieldElement.OtherMultiPrefix.Text;

                quotationPageRangeFieldElement.PageOneSuffix.Text += ")";
                quotationPageRangeFieldElement.PageTwoSuffix.Text += ")";
                quotationPageRangeFieldElement.PageMultiSuffix.Text += ")";
                quotationPageRangeFieldElement.ColumnOneSuffix.Text += ")";
                quotationPageRangeFieldElement.ColumnTwoSuffix.Text += ")";
                quotationPageRangeFieldElement.ColumnMultiSuffix.Text += ")";
                quotationPageRangeFieldElement.ParagraphOneSuffix.Text += ")";
                quotationPageRangeFieldElement.ParagraphTwoSuffix.Text += ")";
                quotationPageRangeFieldElement.ParagraphMultiSuffix.Text += ")";
                quotationPageRangeFieldElement.MarginOneSuffix.Text += ")";
                quotationPageRangeFieldElement.MarginTwoSuffix.Text += ")";
                quotationPageRangeFieldElement.MarginMultiSuffix.Text += ")";
                quotationPageRangeFieldElement.OtherOneSuffix.Text += ")";
                quotationPageRangeFieldElement.OtherTwoSuffix.Text += ")";
                quotationPageRangeFieldElement.OtherMultiSuffix.Text += ")";
        }
    }
}