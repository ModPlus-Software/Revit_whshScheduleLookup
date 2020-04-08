namespace whshScheduleLookup.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using Autodesk.Revit.DB;
    using Model;
    using ModPlusAPI.Mvvm;
    using ModPlusAPI.Windows;
    using Views;

    public partial class SearchViewModel
    {
        public RelayCommand<string> FindSchdeduleCommand
            => new RelayCommand<string>(FindSchdedule, o => IsFindSchdeduleAvailable(o));

        public void FindSchdedule(string obj)
        {
            Action go = () =>
            {
                SearchFinished = false;
                EnteredNames = obj.Split(
                    new[] { Delimeter },
                    StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim())
                    .Where(x => !string.IsNullOrEmpty(x))
                    .ToList();
                var foundResults = new List<ViewScheduleSearchResult>();
                if (IsParameterName)
                {
                    var n = EnteredNames.Count;
                    var total = AllModelSchedules.Count;
                    var filteredResults = new List<ViewScheduleSearchResult>();
                    for (var index = 0; index < total; index++)
                    {
                        var viewScheduleSearchResults = ViewScheduleSearchResult.OfParameterNames(
                            AllModelSchedules[index],
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
                    var n = EnteredNames.Count;
                    var total = AllModelSchedules.Count;
                    var filteredResults = new List<ViewScheduleSearchResult>();
                    for (var index = 0; index < total; index++)
                    {
                        var viewScheduleSearchResults = ViewScheduleSearchResult.OfCellValues(
                            AllModelSchedules[index],
                            true, EnteredNames, SectionType.Body, PartialSearch, IgnoreCase);
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
                    var n = EnteredNames.Count;
                    var total = AllModelSchedules.Count;
                    var filteredResults = new List<ViewScheduleSearchResult>();
                    for (var index = 0; index < total; index++)
                    {
                        var viewScheduleSearchResults = ViewScheduleSearchResult.OfCellValues(
                            AllModelSchedules[index],
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
                    var n = EnteredNames.Count;
                    var total = AllModelSchedules.Count;
                    var filteredResults = new List<ViewScheduleSearchResult>();
                    for (var index = 0; index < total; index++)
                    {
                        var viewScheduleSearchResults = ViewScheduleSearchResult.OfCellValues(
                            AllModelSchedules[index],
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

                    var newWindowThread = new Thread(() =>
                    {
                        try
                        {
                            var resultsWindow = new ResultsWindow();
                            resultsWindow.DataContext = FoundResults;
                            resultsWindow.ShowDialog();
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
                    StateMessage = _noSchedulesFoundMess;
                    Progress = 0;
                }

                SearchFinished = true;
            };
            RevitModel.Command.SetAction(go);
        }

        public bool IsFindSchdeduleAvailable(object obj)
        {
            return !string.IsNullOrEmpty(obj?.ToString()) && obj.ToString() != Delimeter;
        }
    }
}