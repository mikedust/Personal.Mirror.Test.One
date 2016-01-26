using System;

namespace Garaio.CompositeC1Packages.IssuuDocument.DTO
{
	public class DocumentEmbed : DocumentCommon
	{
		public DocumentEmbed(DocumentCommon documentCommon) : base(documentCommon)
		{}

		public string Created { get; set; }

		public string DataConfigId { get; set; }

		public string EmbedId { get; set; }

		public string Height { get; set; }

		public string Width { get; set; }
	}
}
