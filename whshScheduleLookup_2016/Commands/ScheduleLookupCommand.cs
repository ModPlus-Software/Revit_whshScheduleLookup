using System;
using System.Linq;
using System.Threading;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using ModPlusAPI;
using ModPlusAPI.Windows;
using whshScheduleLookup.Model;
using whshScheduleLookup.ViewModels;
using SearchViewModel = whshScheduleLookup.ViewModels.SearchViewModel;
using SearchWindow = whshScheduleLookup.Views.SearchWindow;
using YesNoCancelWindow = whshScheduleLookup.Views.YesNoCancelWindow;

namespace whshScheduleLookup.Commands
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class ScheduleLookupCommand : IExternalCommand
    {
        private const string LangItem = "whshScheduleLookup";
        private RevitModel _revitModel;

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                Statistic.SendCommandStarting(new Interface());

                MessageViewModel yesNoCancelViewModel = new MessageViewModel();
                YesNoCancelWindow yesNoCancelWindow = new YesNoCancelWindow(yesNoCancelViewModel);

                _revitModel = new RevitModel(commandData);

//#if DEBUG
                //var allModelSchedules = DBDocument.FilterInstancesByClass<ViewSchedule>(revitModel.Document,
                //    methodResult: MethodResult.ReturnNonNull);
//#else
                var allModelSchedules = new FilteredElementCollector(_revitModel.Document)
                    .OfClass(typeof(ViewSchedule)).WhereElementIsNotElementType().ToElements()
                    .Where(e => e is ViewSchedule).Cast<ViewSchedule>().ToList();
//#endif

                if (allModelSchedules.Count == 0)
                {
                    yesNoCancelViewModel.Title = Language.GetItem(LangItem, "h8");
                    yesNoCancelViewModel.Message = Language.GetItem(LangItem, "h7");
                    yesNoCancelWindow.ShowDialog();
                    return Result.Succeeded;
                }

                #region Have to show settings window

                SearchViewModel searchViewModel =
                    new SearchViewModel(_revitModel) {AllModelSchedules = allModelSchedules};

                if (true)
                {
                    Thread newWindowThread = new Thread(() =>
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

                //searchDocViewModel = null;
                //revitModel = null;

                Thread.Sleep(200);

                return Result.Succeeded;
            }
            catch (OperationCanceledException)
            {
                return Result.Cancelled;
            }
            catch (PluginException exception)
            {
                message = exception.Message;
                return Result.Failed;
            }
#if !DEBUG
            catch (Exception exception)
            {
                message = exception.Message;
                return Result.Failed;
            }
#endif
        }
    }
}
