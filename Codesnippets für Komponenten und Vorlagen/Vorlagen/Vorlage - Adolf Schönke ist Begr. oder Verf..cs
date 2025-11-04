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
		public bool IsTemplateForReference(ConditionalTemplate template, Citation citation)
		{
			//Just fill in those attribute that you would like to search for or leave empty
			//The following information must be filled in exactly as in the "Edit person" dialog
			string lastName = "Schönke";
			string firstName = "Adolf";
			string middleName = "";
			string prefix = "";
			string suffix = "";
			string abbreviation = "";
			


			//If you do not wish the above person to be checked as the publisher of the superordinate work,
			//then enter"= false;" in the following line:
			bool checkParentReference = true;


			if (citation == null) return false;
			if (citation.Reference == null) return false;


			var authors = citation.Reference.AuthorsOrEditorsOrOrganizations;
			var hasAuthor = hasPerson(authors, lastName, firstName, middleName, prefix, suffix, abbreviation);

			if (hasAuthor) return true;
			
			var editors = citation.Reference.AuthorsOrEditorsOrOrganizations;
			var hasEditor = hasPerson(editors, lastName, firstName, middleName, prefix, suffix, abbreviation);

				if (hasEditor) return true;
			
			var parentReference = citation.Reference.ParentReference;
			if (parentReference == null) return false;


			if (checkParentReference)
			{
				var authors2 = citation.Reference.AuthorsOrEditorsOrOrganizations;
				var hasAuthor2 = hasPerson(authors2, lastName, firstName, middleName, prefix, suffix, abbreviation);

				if (hasAuthor2) return true;
				
				var editors2 = parentReference.AuthorsOrEditorsOrOrganizations;
				var hasEditor2 = hasPerson(editors2, lastName, firstName, middleName, prefix, suffix, abbreviation);

				if (hasEditor2) return true;
			}
			else
			{
				return false;
			}
			
			

			return false;
		}
			


		private bool hasPerson(IEnumerable<Person> persons, string lastName, string firstName, string middleName, string prefix, string suffix, string abbreviation)
		{
			foreach (Person person in persons)
			{
				if
				(
					(string.IsNullOrWhiteSpace(lastName) || string.Compare(person.LastName, lastName, StringComparison.Ordinal) == 0) &&
					(string.IsNullOrWhiteSpace(firstName) || string.Compare(person.FirstName, firstName, StringComparison.Ordinal) == 0) &&
					(string.IsNullOrWhiteSpace(middleName) || string.Compare(person.MiddleName, middleName, StringComparison.Ordinal) == 0) &&
					(string.IsNullOrWhiteSpace(prefix) || string.Compare(person.Prefix, prefix, StringComparison.Ordinal) == 0) &&
					(string.IsNullOrWhiteSpace(suffix) || string.Compare(person.Suffix, suffix, StringComparison.Ordinal) == 0) &&
					(string.IsNullOrWhiteSpace(abbreviation) || string.Compare(person.Abbreviation, abbreviation, StringComparison.Ordinal) == 0)
				)
				{
					return true;
				}
			}

			return false;
		}
		
	}
}