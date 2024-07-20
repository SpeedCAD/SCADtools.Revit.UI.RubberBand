using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using SCADtools.Revit.UI.DB;
using SCADtools.Revit.UI.Extensions;
using System;

namespace Sample
{
    [Transaction(TransactionMode.Manual)]
    internal class RubberBandCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;
            View activeView = doc.ActiveView;

            try
            {
                XYZPair pickPoints = uidoc.PickPoints();
                if (pickPoints != null)
                {
                    using (Transaction tr = new Transaction(doc, "Detail Curve"))
                    {
                        tr.Start();

                        doc.Create.NewDetailCurve(activeView, Line.CreateBound(pickPoints.StartPoint, pickPoints.EndPoint));

                        tr.Commit();
                    }
                }

                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }
            finally
            {

            }
        }
    }
}
