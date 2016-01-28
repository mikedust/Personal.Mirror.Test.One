using System;
using System.Web.UI;
using Composite.Core.WebClient;

namespace Garaio.CompositeC1Packages.IssuuDocument.Package.Composite.Controls
{
	public partial class ScriptLoaderControl : UserControl
	{
		public string Type;
		public string Directive;
		public bool UpdateManagerDisabled;

		private ScriptLoader _scriptloader; 

		/*
		 * Notice that automatic compression doesn't work!
		 */
		protected void Page_Load(object sender, EventArgs e)
		{
			_scriptloader = new ScriptLoader(Type, Directive, UpdateManagerDisabled);
		}

		protected override void Render(HtmlTextWriter writer)
		{
			writer.Write(_scriptloader.Render());
		}
	}
}