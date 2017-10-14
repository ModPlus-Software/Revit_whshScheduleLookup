using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace whshScheduleLookup.Model
{
    public class RevitModel
    {
        public UIApplication UiApplication { get; set; }
        public UIDocument UiDocument { get; set; }
        public Document Document { get; set; }
        public ExternalCommand Command { get; set; } = new ExternalCommand();
        public bool IsRus { get; set; }

        public RevitModel(ExternalCommandData commandData)
        {
            UiApplication = commandData.Application;
            UiDocument = UiApplication.ActiveUIDocument;
            Document = UiDocument.Document;
            IsRus = UiApplication.Application.Language == LanguageType.Russian;
        }
    }
}
