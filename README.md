# Revit Warning Reporter

Revit Warning Reporter is an Autodesk Revit 2027 add-in (.NET 8 / .NET 10) that extracts all warning and failure messages from the active Revit model and exports them to a structured CSV file for BIM Quality Control (QC) and model auditing in Microsoft Excel.

## Features

- **Automatic Extraction**: Retrieves all active model warnings using `doc.GetWarnings()`.
- **Structured Fields**: Exports Index, Severity, Description, Failing Element IDs, and Element Categories.
- **Modern Revit API Support**: Uses `ElementId.Value` (`Int64`) for modern Revit API compliance.
- **QC Ready**: Includes empty `QC Status` and `Resolution Notes` columns for model audit workflows.
- **Excel Compatible**: Uses UTF-8 with BOM encoding for seamless Microsoft Excel compatibility.
- **Revit Ribbon Integration**: Adds a **Warning Exporter** button under the built-in **Add-Ins** ribbon tab.
- **One-Click Installation**: Provides `Install.bat` and `Uninstall.bat` batch scripts for quick setup.

## CSV Columns

| Column Name | Description |
| --- | --- |
| `No` | Sequential warning index |
| `Severity` | Warning severity level |
| `Warning Description` | Warning description text |
| `Element IDs` | Semicolon-separated IDs of failing elements |
| `Element Categories` | Semicolon-separated categories of failing elements |
| `QC Status` | Empty column for QC status (e.g., Open, Fixed, Ignored) |
| `Resolution Notes` | Empty column for resolution notes |

## System Requirements

- Autodesk Revit 2027
- .NET 8.0 SDK / .NET 10.0 SDK (for building from source)
- Windows 10 / 11

## Installation

1. Double-click `Install.bat` to build and install the add-in automatically.
2. Open or restart Autodesk Revit 2027.
3. Access **Warning Exporter** under the **Add-Ins** ribbon tab.

To remove the add-in, double-click `Uninstall.bat`.

## Acknowledgments

Developed with AI assistance from **Google Gemini** and **Google Antigravity**.

## License

This project is licensed under the [MIT License](LICENSE).
