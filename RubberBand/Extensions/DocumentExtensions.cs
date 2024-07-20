using Autodesk.Revit.DB;
using System;
using System.IO;
using System.Linq;

namespace SCADtools.Revit.UI.Extensions
{
    internal static class DocumentExtensions
    {
        /// <summary>
        /// Retrieves the <see cref="FamilySymbol"/> with the specified name from the document.
        /// </summary>
        /// <param name="document">The Revit document from which to retrieve the family symbol.</param>
        /// <param name="symbolName">The name of the family symbol to retrieve.</param>
        /// <returns>The <see cref="FamilySymbol"/> with the specified name, or <c>null</c> if no such symbol is found.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="document"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when the <paramref name="symbolName"/> is <c>null</c> or empty.</exception>
        internal static FamilySymbol GetFamilySymbolByName(this Document document, string symbolName)
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            if (string.IsNullOrEmpty(symbolName))
                throw new ArgumentException("Symbol name cannot be null or empty.", nameof(symbolName));

            return new FilteredElementCollector(document)
                .OfClass(typeof(FamilySymbol))
                .OfCategory(BuiltInCategory.OST_DetailComponents)
                .FirstOrDefault(x => x.Name == symbolName) as FamilySymbol;
        }

        internal static Family LoadRubberBandAndDelete(this Document document)
        {
            Family family = null;

            string tempFilePath = Path.Combine(Path.GetTempPath(), UIDocumentExtensions._RubberBand + ".rfa");

            try
            {
                if (File.Exists(tempFilePath))
                    File.Delete(tempFilePath);

                byte[] data = Properties.Resources._RubberBand;
                using (FileStream fileStream = new FileStream(tempFilePath, FileMode.CreateNew))
                {
                    fileStream.Write(data, 0, data.Length);
                }

                using (Transaction tr = new Transaction(document, "Load RubberBand"))
                {
                    tr.Start();
                    document.LoadFamily(tempFilePath, out family);
                    tr.Commit();
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error loading or deleting the family file.", ex);
            }
            finally
            {
                if (File.Exists(tempFilePath))
                {
                    try
                    {
                        File.Delete(tempFilePath);
                    }
                    catch (IOException ex)
                    {
                        throw new InvalidOperationException("Failed to delete the temporary Family file.", ex);
                    }
                }
            }

            return family;
        }
    }
}
