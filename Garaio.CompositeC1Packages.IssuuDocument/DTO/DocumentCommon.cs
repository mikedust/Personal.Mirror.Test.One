using System;
using System.Globalization;

namespace Garaio.CompositeC1Packages.IssuuDocument.DTO
{
	public class DocumentCommon
	{
		public DocumentCommon()
		{}

		public DocumentCommon(DocumentCommon documentCommon)
		{
			if (documentCommon == null)
			{
				throw new ArgumentException("documentCommen has to be provided.", "documentCommon");
			}

			this.DocumentId = documentCommon.DocumentId;
			this.DocumentName = documentCommon.DocumentName;
			this.OrgDocName = documentCommon.OrgDocName;
			this.PublishDate = documentCommon.PublishDate;
		}

		public string DocumentId { get; set; }

		public string DocumentName { get; set; }

		public string LabelForDropdown
		{
			get
			{
				return string.Format(CultureInfo.InvariantCulture, "{0} ", this.OrgDocName);
			}
		}

		public string OrgDocName { get; set; }

		public string PublishDate { get; set; }
	}
}
