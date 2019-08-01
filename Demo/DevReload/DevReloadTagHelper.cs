using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Abiosoft.DotNet.DevReload
{
	[HtmlTargetElement("devreload")]
	public class DevReloadTagHelper : TagHelper
	{
		public override void Process(TagHelperContext context, TagHelperOutput output) => output.Content.AppendHtml(Js.Tag);
	}
}