using System;
using System.Web.UI;

using Garaio.CompositeC1Packages.IssuuDocument.Configuration;
using Garaio.CompositeC1Packages.IssuuDocument.Package.App_GlobalResources;

namespace Garaio.CompositeC1Packages.IssuuDocument.Package.Composite.InstalledPackages.IssuuDocument
{
	/// <summary>
	/// Issuu Configuration page.
	/// </summary>
	public partial class IssuuDocumentConfig : Page
	{
		/// <summary>
		///     ASP.NET OnLoad Page Lifecycle Event.
		/// </summary>
		/// <param name="e">Die Event-Argumente.</param>
		protected override void OnLoad(EventArgs e)
		{
			SaveButton.Text = Resources_Text.IssuuDocument_Configuration_SaveButton_Label;

			if (!IsPostBack)
			{
				BindUi();
			}

			base.OnLoad(e);

			DataBind();
		}

		protected void BindUi()
		{
			ApiKey.Text = IssuuDocumentConfigurationFacade.Configuration.APIKey;
			ApiSecretKey.Text = IssuuDocumentConfigurationFacade.Configuration.APISecretKey;
			ApiUrl.Text = IssuuDocumentConfigurationFacade.Configuration.APIUrl;
			RequestsPerSec.Text = IssuuDocumentConfigurationFacade.Configuration.RequestsProSecond;
		}

		protected void SaveSettings(object sender, EventArgs e)
		{
			if (Page.IsValid)
			{
				IssuuDocumentConfigurationFacade.Configuration.APIKey = ApiKey.Text;
				IssuuDocumentConfigurationFacade.Configuration.APISecretKey = ApiSecretKey.Text;
				IssuuDocumentConfigurationFacade.Configuration.APIUrl = ApiUrl.Text.EndsWith(@"/", StringComparison.Ordinal) ? ApiUrl.Text.Remove(ApiUrl.Text.Length - 1) : ApiUrl.Text;
				IssuuDocumentConfigurationFacade.Configuration.RequestsProSecond = RequestsPerSec.Text;

				IssuuDocumentConfigurationFacade.SaveCurrentConfiguration();
			}
		}
	}
}