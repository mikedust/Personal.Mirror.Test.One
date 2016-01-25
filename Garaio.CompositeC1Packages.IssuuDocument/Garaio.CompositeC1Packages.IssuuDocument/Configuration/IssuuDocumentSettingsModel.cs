namespace Garaio.CompositeC1Packages.IssuuDocument.Configuration
{
	public class IssuuDocumentSettingsModel
	{
		public string APIKey { get; set; }

		public string APISecretKey { get; set; }

		public string APIUrl { get; set; }

		public string Name { get; set; }

		public int RequestsPerSecond { get; set; }
	}
}
