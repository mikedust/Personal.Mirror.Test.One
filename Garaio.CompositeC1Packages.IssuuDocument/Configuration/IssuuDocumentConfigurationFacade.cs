using System.ComponentModel;
using System.IO;
using System.Xml.Serialization;

using Composite.Core.Configuration;
using Composite.Core.IO;

namespace Garaio.CompositeC1Packages.IssuuDocument.Configuration
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static class IssuuDocumentConfigurationFacade
	{
		private const string NameOfConfigurationFile = "IssuuDocumentConfiguration.xml";

		private static readonly string ConfigFilePath = Path.Combine(PathUtil.Resolve(GlobalSettingsFacade.ConfigurationDirectory), NameOfConfigurationFile);

		private static IssuuDocumentConfiguration _configuration;

		public static IssuuDocumentConfiguration Configuration
		{
			get
			{
				return _configuration ?? (_configuration = C1File.Exists(ConfigFilePath) ? ReadConfigFile() : InitializeAndSaveNewConfigFile());
			}
		}

		public static void SaveCurrentConfiguration()
		{
			SaveConfigurationToFile(_configuration);
		}

		private static IssuuDocumentConfiguration InitializeAndSaveNewConfigFile()
		{
			IssuuDocumentConfiguration configuration = new IssuuDocumentConfiguration
															{
																Name = "Initial configuration",
																APIKey = "Initial API Key",
																APISecretKey = "Initial API Secret Key",
																APIUrl = "http://api.issuu.com/1_0",
																RequestsProSecond = "1"
															};

			SaveConfigurationToFile(configuration);
			return configuration;
		}

		private static IssuuDocumentConfiguration ReadConfigFile()
		{
			XmlSerializer serializer = new XmlSerializer(typeof(IssuuDocumentConfiguration));
			using (StreamReader reader = new StreamReader(ConfigFilePath))
			{
				_configuration = (IssuuDocumentConfiguration)serializer.Deserialize(reader);
				return _configuration;
			}
		}

		private static void SaveConfigurationToFile(IssuuDocumentConfiguration config)
		{
			string directory = Path.GetDirectoryName(ConfigFilePath);
			if (C1Directory.Exists(directory) == false)
			{
				C1Directory.CreateDirectory(directory);
			}

			XmlSerializer xmlSerializer = new XmlSerializer(typeof(IssuuDocumentConfiguration));
			using (StreamWriter streamWriter = new StreamWriter(ConfigFilePath))
			{
				xmlSerializer.Serialize(streamWriter, config);
			}
		}
	}
}
