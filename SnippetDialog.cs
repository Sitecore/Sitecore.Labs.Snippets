using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Shell.Applications.ContentManager;
using Sitecore.Text;
using Sitecore.Web;
using Sitecore.Web.UI.HtmlControls;
using Sitecore.Web.UI.Pages;
using Sitecore.Web.UI.Sheer;
using Sitecore.Web.UI.WebControls;
using Literal = Sitecore.Web.UI.HtmlControls.Literal;

namespace Sitecore.Labs.Snippets
{

    public class SnippetDialog : DialogForm
    {
        private TreeviewEx Treeview;
        private Scrollbox Fields;
        private DataContext DataContext;

        private string[] textfieldtypes = { "single-line text", "multi-line text", "rich text" };

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (Context.ClientPage.IsEvent)
                return;

            DataContext.Root = Settings.SnippetDialogRoot;            

            if (WebUtil.GetQueryString("mode") == "rte")
            {
                var path = WebUtil.GetQueryString("path",DataContext.Root);
                var fields = WebUtil.GetQueryString("fields");
                if (!string.IsNullOrEmpty(path))
                {
                    var item = Sitecore.Context.ContentDatabase.GetItem(path);
                    DataContext.SetFolder(item.Uri);
                    CreateControls(item, fields);
                }
            }
            else
            {
                CreateControls(Sitecore.Context.ContentDatabase.GetItem(DataContext.Root));
            }
        }

        private void CreateControls(Item item, string fields = "")
        {
            var textfields = GetTextFields(item);
            Fields.Controls.Clear();
            
            foreach (var field in textfields)
            {
                var border = new GridPanel()
                {
                    Width = Unit.Percentage(90),
                    Columns = 2
                };
                var checkbox = new Checkbox()
                {                    
                    Header = field.DisplayName,
                    HeaderStyle = "font-weight:bold",                    
                    ID = Control.GetUniqueID("ctl_cb"),                    
                    Checked = fields.Contains(string.Concat("|", field.Name, "|"))
                };
                border.Controls.Add(checkbox);
                border.Controls.Add(new Literal { Text = "<br/>" });
                border.Controls.Add(new HtmlTextArea()
                    {
                        ID = checkbox.ID.Replace("cb","ta"),
                        Rows = 5,
                        Cols = 28,
                        Value = field.Value,
                    }
                    );
                
                Fields.Controls.Add(border);
            }

        }

        protected virtual void TreeViewClick()
        {
            var item = Treeview.GetSelectionItem();
            CreateControls(item);
            SheerResponse.Refresh(Fields);
        }

        private IEnumerable<Field> GetTextFields(Item item)
        {
            item.Fields.ReadAll();
            return item.Fields.Where(f => textfieldtypes.Contains(f.TypeKey) && !string.IsNullOrWhiteSpace(f.Value));
        }

        protected override void OnCancel(object sender, EventArgs args)
        {
            if (WebUtil.GetQueryString("mode") == "rte")
            {
                SheerResponse.Eval("scCancel()");
            }
            else
            {
                base.OnCancel(sender, args);
            }
        }

        protected override void OnOK(object sender, EventArgs args)
        {
            var selectedFields = new ListString();
            var fieldValue = "";
            foreach (var border in Fields.Controls.OfType<GridPanel>())
            {
                foreach (var checkbox in border.Controls.OfType<Checkbox>())
                {
                    if (checkbox.Checked)
                    {
                        selectedFields.Add(checkbox.Header);
                        var control = checkbox.Parent.FindControl(checkbox.ID.Replace("cb", "ta")) as HtmlTextArea;
                        if(control != null) fieldValue = control.Value;
                    }
                    
                }
            }

            if (WebUtil.GetQueryString("mode") == "rte")
            {
                if (selectedFields.Count > 0)
                {

                    var html = string.Format("<span field='{0}' itemid='{1}' style='{2}'>{3}</span>",
                            selectedFields,
                            Treeview.GetSelectionItem().ID,
                            Settings.SnippetStyle,
                            fieldValue
                        );
                    SheerResponse.Eval(
                        string.Concat(
                        "scClose(",                       
                        StringUtil.EscapeJavascriptString(html),
                        ")"
                        ));
                }
                else
                {
                    SheerResponse.Eval("scClose()");
                }
            }

            else
            {
                var fieldEditorOptions = FieldEditorOptions.Parse(WebUtil.GetQueryString("hdl"));
                var fieldField = fieldEditorOptions.Fields.SingleOrDefault(fd => fd.FieldID.Equals(new ID("{AC2FF529-BE03-439C-9EB6-E56A0D5E4123}"))); // Fields 

                if (fieldField != null)
                {
                    fieldField.ContainsStandardValue = false;

                    fieldField.Value = selectedFields.ToString();
                }

                var datasourceField = fieldEditorOptions.Fields.SingleOrDefault(fd => fd.FieldID.Equals(new ID("{0793DCFF-0EE7-48AD-B9D4-EF1DD24EB59B}"))); // Datasource
                if (datasourceField != null)
                {
                    datasourceField.ContainsStandardValue = false;
                    datasourceField.Value = Treeview.GetSelectionItem().Paths.FullPath;
                }


                SheerResponse.SetDialogValue(fieldEditorOptions.ToUrlHandle().ToHandleString());
                SheerResponse.CloseWindow();
            }



        }
    }
}