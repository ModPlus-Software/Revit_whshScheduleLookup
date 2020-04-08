namespace whshScheduleLookup.Model
{
    using Autodesk.Revit.ApplicationServices;
    using Autodesk.Revit.DB;
    using Autodesk.Revit.UI;

    public class RevitModel
    {
        public RevitModel(ExternalCommandData commandData)
        {
            UiApplication = commandData.Application;
            UiDocument = UiApplication.ActiveUIDocument;
            Document = UiDocument.Document;
            IsRus = UiApplication.Application.Language == LanguageType.Russian;
        }

        public UIApplication UiApplication { get; set; }

        public UIDocument UiDocument { get; set; }

        public Document Document { get; set; }

        public ExternalCommand Command { get; set; } = new ExternalCommand();

        public bool IsRus { get; set; }
    }
}
