using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Autodesk.Revit.UI;
using RevitWarningReporter.Models;
using TaskDialog = Autodesk.Revit.UI.TaskDialog;

namespace RevitWarningReporter
{
    /// <summary>
    /// Handles exporting warning records to a structured CSV file with UTF-8 BOM encoding.
    /// </summary>
    public static class CsvExporter
    {
        private static readonly string[] CsvHeaders = new[]
        {
            "No",
            "Severity",
            "Warning Description",
            "Element IDs",
            "Element Categories",
            "QC Status",
            "Resolution Notes"
        };

        /// <summary>
        /// Prompts user to select file location and exports records to CSV.
        /// </summary>
        /// <param name="records">List of warning records to export.</param>
        /// <param name="docTitle">Title of the active document for default naming.</param>
        public static void Export(List<WarningRecord> records, string docTitle)
        {
            if (records == null || records.Count == 0)
            {
                TaskDialog.Show("Revit Warning Reporter", "No warning data to export.");
                return;
            }

            string sanitizedDocTitle = SanitizeFileName(docTitle);
            string defaultFileName = $"Revit_Warnings_{sanitizedDocTitle}_{DateTime.Now:yyyyMMdd_HHmmss}.csv";

            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "CSV File (*.csv)|*.csv|All Files (*.*)|*.*";
                saveFileDialog.Title = "Export Warning Report to CSV";
                saveFileDialog.FileName = defaultFileName;

                if (saveFileDialog.ShowDialog() != DialogResult.OK)
                {
                    return; // User cancelled
                }

                string filePath = saveFileDialog.FileName;

                try
                {
                    WriteCsvFile(filePath, records);
                    ShowSuccessTaskDialog(filePath, records.Count);
                }
                catch (Exception ex)
                {
                    TaskDialog.Show("CSV Export Error", $"Failed to export CSV file:\n{ex.Message}");
                }
            }
        }

        /// <summary>
        /// Writes records to a CSV file with UTF-8 BOM encoding and proper field escaping.
        /// </summary>
        private static void WriteCsvFile(string filePath, List<WarningRecord> records)
        {
            // UTF-8 with BOM (Byte Order Mark) ensures Microsoft Excel detects UTF-8 correctly
            Encoding utf8WithBom = new UTF8Encoding(true);

            using (StreamWriter writer = new StreamWriter(filePath, false, utf8WithBom))
            {
                // Write Header
                writer.WriteLine(string.Join(",", CsvHeaders.Select(EscapeCsvField)));

                // Write Data Rows
                foreach (var rec in records)
                {
                    string[] row = new[]
                    {
                        rec.Index.ToString(),
                        rec.Severity,
                        rec.Description,
                        rec.ElementIds,
                        rec.Categories,
                        rec.QcStatus,
                        rec.ResolutionNotes
                    };

                    writer.WriteLine(string.Join(",", row.Select(EscapeCsvField)));
                }
            }
        }

        /// <summary>
        /// Escapes CSV fields handling commas, double quotes, and newlines.
        /// </summary>
        private static string EscapeCsvField(string field)
        {
            if (string.IsNullOrEmpty(field))
            {
                return "";
            }

            bool mustQuote = field.Contains(",") || field.Contains("\"") || field.Contains("\r") || field.Contains("\n");

            if (mustQuote)
            {
                // Escape existing quotes by doubling them
                string escaped = field.Replace("\"", "\"\"");
                return $"\"{escaped}\"";
            }

            return field;
        }

        /// <summary>
        /// Replaces invalid characters in document title for safe filename usage.
        /// </summary>
        private static string SanitizeFileName(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                return "Untitled";
            }

            char[] invalidChars = Path.GetInvalidFileNameChars();
            string sanitized = new string(fileName.Where(c => !invalidChars.Contains(c)).ToArray());
            return string.IsNullOrWhiteSpace(sanitized) ? "Document" : sanitized;
        }

        /// <summary>
        /// Shows success TaskDialog with option to open CSV file or containing folder.
        /// </summary>
        private static void ShowSuccessTaskDialog(string filePath, int totalCount)
        {
            TaskDialog dialog = new TaskDialog("Revit Warning Reporter")
            {
                MainInstruction = "Export Completed!",
                MainContent = $"Successfully exported {totalCount} warning(s) to CSV file:\n{filePath}",
                CommonButtons = TaskDialogCommonButtons.Close,
                DefaultButton = TaskDialogResult.Close
            };

            dialog.AddCommandLink(TaskDialogCommandLinkId.CommandLink1, "Open CSV File", "Open exported CSV file directly in Microsoft Excel.");
            dialog.AddCommandLink(TaskDialogCommandLinkId.CommandLink2, "Open Destination Folder", "Open folder containing the exported CSV file.");

            TaskDialogResult result = dialog.Show();

            if (result == TaskDialogResult.CommandLink1)
            {
                Process.Start(new ProcessStartInfo(filePath) { UseShellExecute = true });
            }
            else if (result == TaskDialogResult.CommandLink2)
            {
                Process.Start(new ProcessStartInfo("explorer.exe", $"/select,\"{filePath}\""));
            }
        }
    }
}
