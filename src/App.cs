using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Media.Imaging;
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
                string panelName = "Warning Reporter";

                // Create ribbon panel directly on built-in Revit "Add-Ins" tab
                RibbonPanel panel = GetOrCreateAddInPanel(application, panelName);

                // Add PushButton for Warning Reporter Command
                string assemblyPath = Assembly.GetExecutingAssembly().Location;

                PushButtonData buttonData = new PushButtonData(
                    "btnWarningExporter",
                    "Export to\nCSV",
                    assemblyPath,
                    "RevitWarningReporter.Command"
                )
                {
                    ToolTip = "Extract all warnings from active model and export to structured CSV for BIM QC & auditing."
                };

                // Assign button icons from resources with explicit decoding size (32x32 for LargeImage, 16x16 for Image)
                BitmapImage? icon32 = LoadImage("icon32.png", 32, 32) ?? LoadImage("icon64.png", 32, 32) ?? LoadImage("icon128.png", 32, 32);
                BitmapImage? icon16 = LoadImage("icon16.png", 16, 16) ?? LoadImage("icon32.png", 16, 16);

                if (icon32 != null)
                {
                    buttonData.LargeImage = icon32;
                }
                if (icon16 != null)
                {
                    buttonData.Image = icon16;
                }

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

        private BitmapImage? LoadImage(string fileName, int decodeWidth = 32, int decodeHeight = 32)
        {
            try
            {
                Assembly assembly = Assembly.GetExecutingAssembly();

                // 1. Try embedded manifest resource streams
                string[] resourceNames = assembly.GetManifestResourceNames();
                foreach (string name in resourceNames)
                {
                    if (name.EndsWith(fileName, StringComparison.OrdinalIgnoreCase))
                    {
                        using (Stream? stream = assembly.GetManifestResourceStream(name))
                        {
                            if (stream != null)
                            {
                                BitmapImage image = new BitmapImage();
                                image.BeginInit();
                                image.StreamSource = stream;
                                if (decodeWidth > 0) image.DecodePixelWidth = decodeWidth;
                                if (decodeHeight > 0) image.DecodePixelHeight = decodeHeight;
                                image.CacheOption = BitmapCacheOption.OnLoad;
                                image.EndInit();
                                image.Freeze();
                                return image;
                            }
                        }
                    }
                }

                // 2. Try file path relative to assembly location
                string assemblyDir = Path.GetDirectoryName(assembly.Location) ?? string.Empty;
                string localFilePath = Path.Combine(assemblyDir, "resources", fileName);
                if (!File.Exists(localFilePath))
                {
                    localFilePath = Path.Combine(assemblyDir, fileName);
                }

                if (File.Exists(localFilePath))
                {
                    BitmapImage image = new BitmapImage();
                    image.BeginInit();
                    image.UriSource = new Uri(localFilePath, UriKind.Absolute);
                    if (decodeWidth > 0) image.DecodePixelWidth = decodeWidth;
                    if (decodeHeight > 0) image.DecodePixelHeight = decodeHeight;
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.EndInit();
                    image.Freeze();
                    return image;
                }

                // 3. Try Pack URI fallback
                string packUriStr = $"pack://application:,,,/RevitWarningReporter;component/resources/{fileName}";
                BitmapImage packImage = new BitmapImage();
                packImage.BeginInit();
                packImage.UriSource = new Uri(packUriStr, UriKind.Absolute);
                if (decodeWidth > 0) packImage.DecodePixelWidth = decodeWidth;
                if (decodeHeight > 0) packImage.DecodePixelHeight = decodeHeight;
                packImage.CacheOption = BitmapCacheOption.OnLoad;
                packImage.EndInit();
                packImage.Freeze();
                return packImage;
            }
            catch
            {
                return null;
            }
        }
    }
}
