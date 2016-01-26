using System;
using System.Web.UI;

using Garaio.CompositeC1Packages.IssuuDocument.Configuration;

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
			if (!this.IsPostBack)
			{
				this.BindUi();
			}

			base.OnLoad(e);

			this.DataBind();
		}

		protected void BindUi()
		{
			this.ApiKey.Text = IssuuDocumentConfigurationFacade.Configuration.APIKey;
			this.ApiSecretKey.Text = IssuuDocumentConfigurationFacade.Configuration.APISecretKey;
			this.ApiUrl.Text = IssuuDocumentConfigurationFacade.Configuration.APIUrl;
			this.RequestsPerSec.Text = IssuuDocumentConfigurationFacade.Configuration.RequestsProSecond;
		}

		protected void SaveSettings(object sender, EventArgs e)
		{
			if (this.Page.IsValid)
			{
				IssuuDocumentConfigurationFacade.Configuration.APIKey = this.ApiKey.Text;
				IssuuDocumentConfigurationFacade.Configuration.APISecretKey = this.ApiSecretKey.Text;
				IssuuDocumentConfigurationFacade.Configuration.APIUrl = this.ApiUrl.Text.EndsWith(@"/", StringComparison.Ordinal) ? this.ApiUrl.Text.Remove(this.ApiUrl.Text.Length - 1) : this.ApiUrl.Text;
				IssuuDocumentConfigurationFacade.Configuration.RequestsProSecond = this.RequestsPerSec.Text;

				IssuuDocumentConfigurationFacade.SaveCurrentConfiguration();
			}
		}
	}
}