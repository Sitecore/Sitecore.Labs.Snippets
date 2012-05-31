
using HtmlAgilityPack;


namespace Sitecore.Labs.Snippets
{
    using Pipelines.RenderField;


    public class ExpandSnippets
    {
        public void Process([NotNull] RenderFieldArgs args)
        {
            if (args.FieldTypeKey != "rich text") return;

            if (Settings.DoNotExpandInLiveSite && Context.PageMode.IsNormal) return;
            
            var document = new HtmlDocument();
            document.LoadHtml(args.Result.FirstPart);
            var htmlNodeCollection = document.DocumentNode.SelectNodes("//span[@itemid]");
            
            if (htmlNodeCollection == null)
                return;
            
            foreach (var node in htmlNodeCollection)
            {
                var item = Context.Database.GetItem(node.Attributes["itemid"].Value);
                if (item == null) continue;

                var fieldValue = item[node.Attributes["field"].Value];

                if(Context.PageMode.IsNormal)
                {
                    var newnode = document.CreateTextNode(fieldValue);
                    node.ParentNode.ReplaceChild(newnode, node);
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(fieldValue)) node.InnerHtml = fieldValue;

                    if (Context.PageMode.IsPageEditorEditing)
                    {
                        node.SetAttributeValue("style",Settings.SnippetStyle);
                    }    
                }
                
            }
            args.Result.FirstPart = document.DocumentNode.OuterHtml;
        }

        
        

       
    }
}