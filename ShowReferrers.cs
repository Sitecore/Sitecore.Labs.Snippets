using System;
using Sitecore.Data.Items;
using Sitecore.Links;
using Sitecore.Web;
using Sitecore.Web.UI.HtmlControls;
using Sitecore.Web.UI.Pages;
using Sitecore.Web.UI.Sheer;

namespace Sitecore.Labs.Snippets
{
    public class ShowReferrers : DialogForm
    {
        private Listview Listview;
        
        private readonly UrlOptions UrlOptions = new UrlOptions
                                 {
                                     Site = Configuration.Factory.GetSite("website")
                                 };

        private readonly string DatasourceQuery = "fast:/sitecore/content/home//*[@__renderings = '%{0}%{1}%']";
        private readonly string SnippetRenderingID = "{F2401E37-EE82-4D24-81F7-2848A0CE184C}";

        protected override void OnLoad(EventArgs e)
        {            
            base.OnLoad(e);
            if (Context.ClientPage.IsEvent)
                return;

            Cancel.Visible = false;

            var id = WebUtil.GetQueryString("id");
            var fieldid = WebUtil.GetQueryString("field");

            
            Listview.DblClick = "PreviewItem";
            

            var item = Context.ContentDatabase.GetItem(id);
            var fieldname = item.Database.GetItem(fieldid).Key;

            var linksdb = Configuration.Factory.GetLinkDatabase();
            var referrers = linksdb.GetReferrers(item);
            
            foreach (var itemLink in referrers)
            {
                var sourceitem = itemLink.GetSourceItem();
                var splitpath = itemLink.TargetPath.ToLower().Split('^');
                
                if (splitpath.Length == 3)
                {
                    var sourcefield = splitpath[1];
                    var targetfield = splitpath[2];
                    if (!sourcefield.Contains(fieldname)) continue;
                    CreateControl(sourceitem, targetfield, language: itemLink.SourceItemLanguage.ToString(), version: itemLink.SourceItemVersion.ToString());                
                }               
            }

            var items = item.Database.SelectItems(string.Format(DatasourceQuery,item.Paths.FullPath,SnippetRenderingID));
            foreach (var datasourceItem in items)
            {
                CreateControl(datasourceItem,"used in presentation", color: "blue");
            }

        }
        
        protected virtual void  CreateControl(Item item, string fields, string color = "Black", string version = "-", string language = "-" )
        {
            var lbi = new ListviewItem
                              {
                                  Header =  item.Paths.FullPath,
                                  Icon = item.Appearance.Icon,                                  
                                  Foreground  = color,
                                  ID = Control.GetUniqueID("lvi")
                              };
                lbi.ColumnValues.Add("Path",item.Paths.FullPath);
                lbi.ColumnValues.Add("Url", LinkManager.GetItemUrl(item,UrlOptions));
                lbi.ColumnValues.Add("Fields",fields);
                lbi.ColumnValues.Add("Version", version);
                lbi.ColumnValues.Add("Language", language);
            Listview.Controls.Add(lbi);
        }
       
        protected virtual void PreviewItem()
        {
            var selectedItems = Listview.GetSelectedItems();
            if (selectedItems.Length <= 0) return;

            var selected = selectedItems[0];
            var url = selected.ColumnValues["Url"];

            SheerResponse.Eval("window.open('" + url + "')");

        }
    }
}