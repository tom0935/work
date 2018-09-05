using System.Web;
using System.Web.Optimization;

namespace IntranetSystem
{
    public class BundleConfig
    {
        // 如需 Bundling 的詳細資訊，請造訪 http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                        "~/Scripts/jquery-ui-1.9.2.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.unobtrusive*",
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/jbval").Include(
                        "~/Scripts/jqBootstrapValidation.js"));

            bundles.Add(new ScriptBundle("~/bundles/colorbox").Include(
            "~/Scripts/jquery.colorbox.js"));

            bundles.Add(new ScriptBundle("~/bundles/ie8down").Include(
                        "~/Scripts/html5shiv.js",
                        "~/Scripts/respond.js"));

            bundles.Add(new ScriptBundle("~/bundles/material").Include(
                        "~/Scripts/material.js"));

            bundles.Add(new ScriptBundle("~/bundles/printer").Include(
                        "~/Scripts/printer.js"));

            bundles.Add(new ScriptBundle("~/bundles/requisition").Include(
                        "~/Scripts/requisition.js"));

            bundles.Add(new ScriptBundle("~/bundles/deliveryadd").Include(
                        "~/Scripts/deliveryadd.js"));

            // 使用開發版本的 Modernizr 進行開發並學習。然後，當您
            // 準備好實際執行時，請使用 http://modernizr.com 上的建置工具，只選擇您需要的測試。
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new StyleBundle("~/Content/css").Include("~/Content/Site.css"));

            bundles.Add(new StyleBundle("~/Content/themes/base/css").Include(
                        "~/Content/themes/base/jquery.ui.core.css",
                        "~/Content/themes/base/jquery.ui.resizable.css",
                        "~/Content/themes/base/jquery.ui.selectable.css",
                        "~/Content/themes/base/jquery.ui.accordion.css",
                        "~/Content/themes/base/jquery.ui.autocomplete.css",
                        "~/Content/themes/base/jquery.ui.button.css",
                        "~/Content/themes/base/jquery.ui.dialog.css",
                        "~/Content/themes/base/jquery.ui.slider.css",
                        "~/Content/themes/base/jquery.ui.tabs.css",
                        "~/Content/themes/base/jquery.ui.datepicker.css",
                        "~/Content/themes/base/jquery.ui.progressbar.css",
                        "~/Content/themes/base/jquery.ui.theme.css"));

            bundles.Add(new StyleBundle("~/Content/themes/jui/css").Include(
                        "~/Content/themes/jui/jquery-ui-1.10.0.custom.css"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include("~/Scripts/bootstrap.js"));
            
            bundles.Add(new StyleBundle("~/Content/bootstrap").Include(
                        "~/Content/bootstrap/bootstrap.css",
                        "~/Content/bootstrap/bootstrap-theme.css"));

            bundles.Add(new ScriptBundle("~/Content/colorbox").Include("~/Content/colorbox.css"));
        }
    }
}