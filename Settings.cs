using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sitecore.Labs.Snippets
{

    public static class Settings
    {
        
        public static string SnippetStyle
        {
            get { return Configuration.Settings.GetSetting("Snippet.Style", "color:black;text-decoration:underline"); }
        }

        public static bool DoNotExpandInLiveSite 
        {
            get { return Configuration.Settings.GetBoolSetting("Snippet.DoNotExpandInLiveSite", false); }
        } 

        public static string SnippetDialogRoot
        {
            get { return Configuration.Settings.GetSetting("Snippet.SnippetDialogRoot", "/sitecore/content/home"); }
        }
    }
}