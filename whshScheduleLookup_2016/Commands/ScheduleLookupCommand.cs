namespace whshScheduleLookup.Commands
{
    using System;
    using System.Linq;
    using System.Threading;
    using Autodesk.Revit.Attributes;
    using Autodesk.Revit.DB;
    using Autodesk.Revit.UI;
    using Model;
    using ModPlusAPI;
    using ModPlusAPI.Windows;
    using ViewModels;
    using Views;

    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class ScheduleLookupCommand : IExternalCommand
    {
        private const string LangItem = "whshScheduleLookup";
        private RevitModel _revitModel;

        /// <inheritdoc />
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                Statistic.SendCommandStarting(new ModPlusConnector());

                var yesNoCancelViewModel = new MessageViewModel();
                var yesNoCancelWindow = new YesNoCancelWindow(yesNoCancelViewModel);

                _revitModel = new RevitModel(commandData);

                var allModelSchedules = new FilteredElementCollector(_revitModel.Document)
                    .OfClass(typeof(ViewSchedule)).WhereElementIsNotElementType().ToElements()
                    .Where(e => e is ViewSchedule).Cast<ViewSchedule>().ToList();

                if (allModelSchedules.Count == 0)
                {
                    yesNoCancelViewModel.Title = Language.GetItem(LangItem, "h8");
                    yesNoCancelViewModel.Message = Language.GetItem(LangItem, "h7");
                    yesNoCancelWindow.ShowDialog();
                    return Result.Succeeded;
                }

                #region Have to show settings window

                var searchViewModel =
                    new SearchViewModel(_revitModel)
                    {
                        AllModelSchedules = allModelSchedules
                    };

                if (true)
                {
                    var newWindowThread = new Thread(() =>
                    {
                        try
                        {
                            var mainWindow = new SearchWindow(searchViewModel);
                            mainWindow.ShowDialog();
                        }
                        catch (Exception e)
                        {
                            ExceptionBox.Show(e);
                        }
                    });
                    newWindowThread.SetApartmentState(ApartmentState.STA);
                    newWindowThread.IsBackground = true;
                    newWindowThread.Start();
                }
                #endregion
                
                Thread.Sleep(200);

                return Result.Succeeded;
            }
            catch (OperationCanceledException)
            {
                return Result.Cancelled;
            }
            catch (Exception exception)
            {
                ExceptionBox.Show(exception);
                return Result.Failed;
            }
        }
    }
}
