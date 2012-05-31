using Sitecore.Web.UI.Sheer;

namespace Sitecore.Labs.Snippets
{
   using Shell.Framework.Commands;

   public class ShowReferrersCommand : Command
    {
        public override void Execute([NotNull] CommandContext context)
        {
            string url =
                string.Format(@"/sitecore/shell/default.aspx?xmlcontrol=ShowReferrers&id={0}&field={1}", context.Items[0].ID,context.Parameters["fieldid"]);
            
            SheerResponse.ShowModalDialog(url);            
        }
    }
}