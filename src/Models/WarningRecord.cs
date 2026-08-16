namespace RevitWarningReporter.Models
{
    /// <summary>
    /// Represents a single failure/warning record extracted from the Revit document.
    /// </summary>
    public class WarningRecord
    {
        public int Index { get; set; }
        public string Severity { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ElementIds { get; set; } = string.Empty;
        public string Categories { get; set; } = string.Empty;
        public string QcStatus { get; set; } = string.Empty;
        public string ResolutionNotes { get; set; } = string.Empty;
    }
}
