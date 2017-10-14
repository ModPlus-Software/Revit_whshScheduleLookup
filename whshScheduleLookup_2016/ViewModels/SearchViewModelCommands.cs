using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using Autodesk.Revit.DB;
using ModPlusAPI.Windows;
using whshScheduleLookup.Views;
using whshScheduleLookup.Model;
using ResultsWindow = whshScheduleLookup.Views.ResultsWindow;

namespace whshScheduleLookup.ViewModels
{
    public partial class SearchViewModel
    {
        public RelayCommandGeneric<string> FindSchdeduleCommand
            => new RelayCommandGeneric<string>(FindSchdedule, IsFindSchdeduleAvailable);

        public void FindSchdedule(string obj)
        {
            Action go = () =>
            {
                SearchFinished = false;
                EnteredNames = obj.Split(new string[] { Delimeter },
                        StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim())
                    .Where(x => !string.IsNullOrEmpty(x))
                    .ToList();
                List<ViewSchedule> foundSchedules = new List<ViewSchedule>();
                List<ViewScheduleSearchResult> foundResults = new List<ViewScheduleSearchResult>();
                if (IsParameterName)
                {
                    //foundResults = ViewScheduleSearchResult.FilterResultsParameterNames(AllModelSchedules,
                    //    EnteredNames, IgnoreCase, PartialSearch);
                    var n = EnteredNames.Count;
                    var total = AllModelSchedules.Count;
                    List<ViewScheduleSearchResult> filteredResults = new List<ViewScheduleSearchResult>();
                    for (var index = 0; index < total; index++)
                    {
                        var viewScheduleSearchResults = ViewScheduleSearchResult.OfParameterNames(AllModelSchedules[index],
                            EnteredNames, PartialSearch, IgnoreCase);
                        if (viewScheduleSearchResults != null && viewScheduleSearchResults.Count == n)
                        {
                            filteredResults.AddRange(viewScheduleSearchResults);
                        }
                        Progress = (int)Math.Round(index * 1.0 / total * 100, 0);
                        StateMessage = Progress + PercentCompleteMess;
                    }
                    foundResults = filteredResults;
                }
                if (IsHeadingName)
                {
                    //foundResults = ViewScheduleSearchResult.FilterResultsByCellValues(AllModelSchedules,
                    //    true, EnteredNames, IgnoreCase, PartialSearch);
                    var n = EnteredNames.Count;
                    var total = AllModelSchedules.Count;
                    List<ViewScheduleSearchResult> filteredResults = new List<ViewScheduleSearchResult>();
                    for (var index = 0; index < total; index++)
                    {
                        var viewScheduleSearchResults = ViewScheduleSearchResult.OfCellValues(AllModelSchedules[index],
                            true, EnteredNames, SectionType.Body ,PartialSearch, IgnoreCase);
                        if (viewScheduleSearchResults != null && viewScheduleSearchResults.Count == n)
                        {
                            filteredResults.AddRange(viewScheduleSearchResults);
                        }
                        Progress = (int)Math.Round(index * 1.0 / total * 100, 0);
                        StateMessage = Progress + PercentCompleteMess;
                    }
                    foundResults = filteredResults;
                }
                if (IsCellValue)
                {
                    //foundResults = ViewScheduleSearchResult.FilterResultsByCellValues(AllModelSchedules,
                    //    false, EnteredNames, IgnoreCase, PartialSearch);
                    var n = EnteredNames.Count;
                    var total = AllModelSchedules.Count;
                    List<ViewScheduleSearchResult> filteredResults = new List<ViewScheduleSearchResult>();
                    for (var index = 0; index < total; index++)
                    {
                        var viewScheduleSearchResults = ViewScheduleSearchResult.OfCellValues(AllModelSchedules[index],
                            false, EnteredNames, SectionType.Body, PartialSearch, IgnoreCase);
                        if (viewScheduleSearchResults != null && viewScheduleSearchResults.Count == n)
                        {
                            filteredResults.AddRange(viewScheduleSearchResults);
                        }
                        Progress = (int)Math.Round(index * 1.0 / total * 100, 0);
                        StateMessage = Progress + PercentCompleteMess;
                    }
                    foundResults = filteredResults;
                }
                if (IsHeader)
                {
                    //foundResults = ViewScheduleSearchResult.FilterResultsByCellValues(AllModelSchedules,
                    //    false, EnteredNames, IgnoreCase, PartialSearch);
                    var n = EnteredNames.Count;
                    var total = AllModelSchedules.Count;
                    List<ViewScheduleSearchResult> filteredResults = new List<ViewScheduleSearchResult>();
                    for (var index = 0; index < total; index++)
                    {
                        var viewScheduleSearchResults = ViewScheduleSearchResult.OfCellValues(AllModelSchedules[index],
                            false, EnteredNames, SectionType.Header, PartialSearch, IgnoreCase);
                        if (viewScheduleSearchResults != null && viewScheduleSearchResults.Count == n)
                        {
                            filteredResults.AddRange(viewScheduleSearchResults);
                        }
                        Progress = (int)Math.Round(index * 1.0 / total * 100, 0);
                        StateMessage = Progress + PercentCompleteMess;
                    }
                    foundResults = filteredResults;
                }
                Progress = 100;
                FoundResultsNumber = foundResults.Count;
                if (FoundResultsNumber > 0)
                {
                    FoundResults = foundResults.OrderBy(v => v.ViewScheduleName).ToList();
                    //FoundSchedules = foundResults.Select(vsr => vsr.ViewScheduleId)
                    //        .Distinct()
                    //        .Select(e => RevitModel.Document.GetElement(e))
                    //        .Cast<ViewSchedule>()
                    //        .ToList()
                    //    ;
                    Thread newWindowThread = new Thread(() =>
                    {
                        try
                        {
                            ResultsWindow resultsWindow = new ResultsWindow();
                            resultsWindow.DataContext = FoundResults;
                            //resultsWindow.Topmost = true;
                            resultsWindow.ShowDialog();
                            //resultsWindow.Topmost = false;
                            //throw new PluginException("gotcha");
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
                else
                {
                    //var s = EnteredNames.Aggregate("\n", (workingSentence, next) => workingSentence + next + "\n");
                    //TaskDialog.Show("deb", $"Не было найдено таблиц, содержащих одновременно следующее: {s}");
                    StateMessage = _noSchedulesFoundMess;
                    Progress = 0;
                }
                SearchFinished = true;
            };
            RevitModel.Command.SetAction(go);
        }

        public bool IsFindSchdeduleAvailable(string obj)
        {
            return (!string.IsNullOrEmpty(obj) && obj != Delimeter);
        }

    }
}