using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using HtmlAgilityPack;
using Sitecore.Data.Fields;
using Sitecore.Diagnostics;

namespace Sitecore.Labs.Snippets
{
    public class CustomHtmlField : HtmlField
    {
        public CustomHtmlField(Field innerField)
            : base(innerField)
        {
            Assert.ArgumentNotNull((object)innerField, "innerField");          
        }

        public override void ValidateLinks(Links.LinksValidationResult result)
       {           
            base.ValidateLinks(result);

            var document = new HtmlDocument();
            document.LoadHtml(this.Value);

            var htmlNodeCollection = document.DocumentNode.SelectNodes("//span[@itemid and @field]");
            if (htmlNodeCollection == null) return;

            var links = htmlNodeCollection.Select(n=> 
                new { ItemID=n.Attributes["itemid"].Value,Field=n.Attributes["field"].Value});
            
            foreach(var link in links)
            {                
                var item = this.InnerField.Database.GetItem(link.ItemID);                
                
                if (item != null && item.Fields.Any(f=> f.Key.Equals(link.Field,StringComparison.InvariantCultureIgnoreCase)))
                {
                    result.AddValidLink(item, string.Concat(link.ItemID,"^",link.Field,"^",this._innerField.Key));
                }
                else
                {
                    result.AddBrokenLink(link.ItemID);
                }
            }
          
        }
    }

    
}