using System.Web.UI;
using Composite.Core.WebClient;

namespace Garaio.CompositeC1Packages.IssuuDocument.Package.Composite.Controls
{
	public partial class StyleLoaderControl : UserControl
	{
		public string Directive { get; set; }

		protected override void Render(HtmlTextWriter writer)
		{
			writer.Write(StyleLoader.Render(Directive));
		}
	}
}