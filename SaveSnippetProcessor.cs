using HtmlAgilityPack;

namespace Sitecore.Labs.Snippets
{
    using Pipelines.Save;
    
    public class SaveSnippetProcessor
    {
        public void Process([NotNull] SaveArgs args)
        {
            var item = args.Items[0];

            if (item == null) return;

            foreach (var saveField in item.Fields)
            {
                var document = new HtmlDocument();
                document.LoadHtml(saveField.Value);
                var htmlNodeCollection = document.DocumentNode.SelectNodes("//span[@itemid and @field]");
                if (htmlNodeCollection == null)
                    continue;

                foreach (var node in htmlNodeCollection)
                {
                    var targetitem = Context.ContentDatabase.GetItem(node.Attributes["itemid"].Value);
                    // todo: do something if the target item does not exist?
                    
                    if (targetitem == null)
                    {                        
                        continue;
                    }
                    
                    var targetfield = node.Attributes["field"].Value;

                    node.InnerHtml = targetitem[targetfield];

                    //todo: do something if field has no value?
                    //if (string.IsNullOrWhiteSpace(node.InnerHtml))
                    //{                        
                       
                    //}

                    var style = node.Attributes["style"];
                    if (style != null) style.Remove();
                }
                saveField.Value = document.DocumentNode.OuterHtml;
            }
        }
    }
}
