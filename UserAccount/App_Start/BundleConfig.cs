using System.Web.Optimization;

namespace Zidium.UserAccount
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            // Css

            bundles.Add(new StyleBundle("~/Content/CommonCss").Include(
                "~/Content/bootstrap.css",
                "~/Content/bootstrap-datetimepicker.css",
                "~/Content/bootstrap-chosen.css",
                "~/Content/site.css",
                "~/Content/timeline.css",
                "~/Content/css-treeview.css",
                "~/Content/smart.css",
                "~/Content/jquery-ui-1.11.4/jquery-ui.css",
                "~/Content/jquery-ui-1.11.4/jquery-ui.structure.css",
                "~/Content/jquery-ui-1.11.4/jquery-ui.theme.css",
                "~/Content/errors.css",
                "~/Content/tooltipster.css",
                "~/Content/smart-blocks.css"
                ));

            bundles.Add(new StyleBundle("~/Content/GridMvc").Include(
                "~/Content/Gridmvc.css"));

            // Scripts

            bundles.Add(new ScriptBundle("~/Scripts/CommonScripts").Include(
                "~/Scripts/jquery-{version}.js",
                "~/Scripts/jquery.validate*",
                "~/Scripts/jquery.unobtrusive-ajax.js",
                "~/Scripts/bootstrap.js",
                "~/Scripts/bootbox.js",
                "~/Scripts/mainmenu.js",
                "~/Scripts/moment-with-locales.js",
                "~/Scripts/bootstrap-datetimepicker.js",
                "~/Scripts/chosen.jquery.js",
                "~/Scripts/jquery.inputmask/inputmask.js",
                "~/Scripts/jquery.inputmask/jquery.inputmask.js",
                "~/Scripts/jquery.inputmask/inputmask.date.extensions.js",
                "~/Scripts/jquery.inputmask/inputmask.numeric.extensions.js",
                "~/Scripts/jquery.inputmask/inputmask.phone.extensions.js",
                "~/Scripts/jquery.inputmask/phone-codes/phone.js",
                "~/Scripts/utils.js",
                "~/Content/jquery-ui-1.11.4/jquery-ui.js",
                "~/Scripts/errors.js",
                "~/Scripts/ajaxtable.js",
                "~/Scripts/jquery.tooltipster.min.js",
                "~/Scripts/smart-blocks/smart-blocks.js",
                "~/Scripts/smart-blocks/smart-buttons.js",
                "~/Scripts/smart-blocks/smart-config.js",
                "~/Scripts/dialogs.js",
                "~/Scripts/fixed-header.js",
                "~/Scripts/floatThead/jquery.floatThead.js",
                "~/Scripts/global.js"
               ));

            bundles.Add(new ScriptBundle("~/Scripts/AllControls").Include(
                "~/Scripts/Controls/ColorStatusSelector.js",
                "~/Scripts/Controls/ComponentSelector.js",
                "~/Scripts/Controls/ComponentSelectorNew.js",
                "~/Scripts/Controls/DropdownSelectorBase.js",
                "~/Scripts/Controls/DateSelector.js",
                "~/Scripts/Controls/ComponentTypeSelector.js",
                "~/Scripts/Controls/EventTypeSelector.js",
                "~/Scripts/Controls/UserSelector.js",
                "~/Scripts/Controls/CounterSelector.js",
                "~/Scripts/Controls/UnitTestTypeSelector.js",
                "~/Scripts/Controls/ComponentStatusSelector.js",
                "~/Scripts/Controls/FilterButton.js",
                "~/Scripts/Controls/BooleanButton.js",
                "~/Scripts/Controls/TextFilter.js"
                ));

            bundles.Add(new ScriptBundle("~/Scripts/GridMvc").Include(
                "~/Scripts/gridmvc.js"));

            bundles.Add(new ScriptBundle("~/Scripts/HighChartsScripts").Include(
                "~/Scripts/HighCharts/highcharts.js",
                "~/Scripts/HighCharts/modules/exporting.js")
                );

        }
    }
}
