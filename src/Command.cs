using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using RevitWarningReporter.Models;
using TaskDialog = Autodesk.Revit.UI.TaskDialog;

namespace RevitWarningReporter
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class Command : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                UIApplication uiapp = commandData.Application;
                UIDocument uidoc = uiapp.ActiveUIDocument;

                if (uidoc == null || uidoc.Document == null)
                {
                    TaskDialog.Show("Revit Warning Reporter", "No active Revit document found.");
                    return Result.Cancelled;
                }

                Document doc = uidoc.Document;

                // Extract failure warning messages from active document
                IList<FailureMessage> warnings = doc.GetWarnings();

                if (warnings == null || warnings.Count == 0)
                {
                    TaskDialog.Show("Revit Warning Reporter", "Model is clean! No warnings found in the active document.");
                    return Result.Succeeded;
                }

                List<WarningRecord> records = new List<WarningRecord>();
                int index = 1;

                foreach (FailureMessage msg in warnings)
                {
                    // 1. Severity
                    string severity = msg.GetSeverity().ToString();

                    // 2. Warning Description
                    string description = msg.GetDescriptionText();

                    // 3. Failing Element IDs (using ElementId.Value for Revit modern API)
                    ICollection<ElementId> failingElementIds = msg.GetFailingElements();
                    List<string> elementIdStrings = new List<string>();
                    List<string> categoryNames = new List<string>();

                    if (failingElementIds != null && failingElementIds.Count > 0)
                    {
                        foreach (ElementId id in failingElementIds)
                        {
                            // Modern Revit API uses ElementId.Value (Int64)
                            elementIdStrings.Add(id.Value.ToString());

                            Element elem = doc.GetElement(id);
                            if (elem != null && elem.Category != null && !string.IsNullOrWhiteSpace(elem.Category.Name))
                            {
                                categoryNames.Add(elem.Category.Name);
                            }
                        }
                    }

                    string elementIdsFormatted = string.Join("; ", elementIdStrings);

                    // Get distinct category names for clean formatting
                    string categoriesFormatted = string.Join("; ", categoryNames.Distinct());

                    records.Add(new WarningRecord
                    {
                        Index = index++,
                        Severity = severity,
                        Description = description,
                        ElementIds = elementIdsFormatted,
                        Categories = categoriesFormatted,
                        QcStatus = string.Empty,       // Empty for QC team inspection
                        ResolutionNotes = string.Empty // Empty for QC team inspection
                    });
                }

                // Export collected records to CSV
                CsvExporter.Export(records, doc.Title);

                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                TaskDialog.Show("Revit Warning Reporter Error", $"An error occurred while processing warnings:\n{ex.Message}");
                return Result.Failed;
            }
        }
    }
}
