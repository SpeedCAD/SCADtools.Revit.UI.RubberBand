using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Windows;
using SCADtools.Revit.UI.DB;
using SCADtools.Revit.UI.Handles;
using SCADtools.Revit.UI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace SCADtools.Revit.UI.Extensions
{
    public static class UIDocumentExtensions
    {
        internal const string _RubberBand = "_RubberBand";

        private static IWin32Window revitWindow;
        private static List<ElementId> elementIds;

        /// <summary>
        /// Prompts the user to pick two points in the Revit model and returns them as an <see cref="XYZPair"/>.
        /// </summary>
        /// <param name="uiDocument">The UIDocument instance representing the current Revit document.</param>
        /// <returns>An <see cref="XYZPair"/> containing the start and end points selected by the user.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="uiDocument"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the FamilySymbol required for picking points cannot be found or loaded.</exception>
        public static XYZPair PickPoints(this UIDocument uiDocument)
        {
            if (uiDocument == null)
                throw new ArgumentNullException(nameof(uiDocument));

            XYZPair points = null;
            elementIds = new List<ElementId>();

            Document document = uiDocument.Document;
            Autodesk.Revit.ApplicationServices.Application application = document.Application;

            FamilySymbol symbol = document.GetFamilySymbolByName(_RubberBand);
            if (symbol == null)
            {
                Family family = document.LoadRubberBandAndDelete();
                if (family != null)
                    symbol = document.GetElement(family.GetFamilySymbolIds().First()) as FamilySymbol;
            }

            if (symbol == null)
                throw new InvalidOperationException("FamilySymbol '" + _RubberBand + "' not found.");

            revitWindow = new WindowHandle(ComponentManager.ApplicationWindow);

            try
            {
                application.DocumentChanged += Application_DocumentChanged;
                uiDocument.PromptForFamilyInstancePlacement(symbol);
            }
            catch { }
            finally
            {
                application.DocumentChanged -= Application_DocumentChanged;
            }

            if (elementIds.Count > 0)
            {
                ElementId rubberBandId = elementIds.First();
                FamilyInstance rubberBand = (FamilyInstance)document.GetElement(rubberBandId);
                if (rubberBand != null)
                {
                    if (rubberBand.Location is LocationCurve locationCurve)
                    {
                        Curve curve = locationCurve.Curve;
                        points = new XYZPair(curve.GetEndPoint(0), curve.GetEndPoint(1));

                        UIFrameworkServices.QuickAccessToolBarService.performMultipleUndoRedoOperations(true, 1);
                    }
                }
            }

            return points;
        }

        private static void Application_DocumentChanged(object sender, Autodesk.Revit.DB.Events.DocumentChangedEventArgs e)
        {
            elementIds.AddRange(e.GetAddedElementIds());

            if (elementIds.Count == 1)
            {
                PressService.PostMessage(revitWindow.Handle, (uint)PressService.KEYBOARD_MSG.WM_KEYDOWN, (uint)Keys.Escape, 0);
                PressService.PostMessage(revitWindow.Handle, (uint)PressService.KEYBOARD_MSG.WM_KEYDOWN, (uint)Keys.Escape, 0);
            }
        }
    }
}
