using System;
using System.Web.UI;
using Composite.Core.WebClient.Presentation;

namespace Garaio.CompositeC1Packages.IssuuDocument.Package.Composite.Controls
{
	public partial class HttpHeadersControl : UserControl
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			ViewServices.RegisterCommonTransformations();
			ViewServices.RegisterMimeType();

			// Force NET mumbojumbo scripts to appear - needed for UpdateManager.js
			Page.ClientScript.GetPostBackEventReference ( this, string.Empty );
		}
    
		protected override void Render(HtmlTextWriter writer)
		{
			// Make hidden field "__EVENTVALIDATION" appear - needed for UpdateManager.js
			Page.ClientScript.RegisterForEventValidation ( "TemporaryIdForEventValidation" );
			base.Render (writer);
		}
	}
}