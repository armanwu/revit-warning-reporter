using System;
using System.Collections.Generic;
using System.Reflection;
using Autodesk.Revit.UI;
using TaskDialog = Autodesk.Revit.UI.TaskDialog;

namespace RevitWarningReporter
{
    public class App : IExternalApplication
    {
        public Result OnStartup(UIControlledApplication application)
        {
            try
            {
                string panelName = "Warning Exporter";

                // Create ribbon panel directly on built-in Revit "Add-Ins" tab
                RibbonPanel panel = GetOrCreateAddInPanel(application, panelName);

                // Add PushButton for Warning Exporter Command
                string assemblyPath = Assembly.GetExecutingAssembly().Location;

                PushButtonData buttonData = new PushButtonData(
                    "btnWarningExporter",
                    "Warning\nExporter",
                    assemblyPath,
                    "RevitWarningReporter.Command"
                )
                {
                    ToolTip = "Extract all warnings from active model and export to structured CSV for BIM QC & auditing."
                };

                panel.AddItem(buttonData);

                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Revit Warning Reporter Startup Error", $"Failed to load Ribbon Add-in UI:\n{ex.Message}");
                return Result.Failed;
            }
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

        private RibbonPanel GetOrCreateAddInPanel(UIControlledApplication app, string panelName)
        {
            List<RibbonPanel> panels = app.GetRibbonPanels(Tab.AddIns);
            foreach (RibbonPanel p in panels)
            {
                if (p.Name.Equals(panelName, StringComparison.OrdinalIgnoreCase))
                {
                    return p;
                }
            }

            return app.CreateRibbonPanel(Tab.AddIns, panelName);
        }
    }
}
