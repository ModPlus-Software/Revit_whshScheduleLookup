namespace whshScheduleLookup.ViewModels
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Model;

    public class ResultsViewModel
    {
        public ObservableCollection<ViewScheduleSearchResult> FoundResults { get; set; } // = new ObservableCollection<ViewScheduleSearchResult>();

        public ResultsViewModel(List<ViewScheduleSearchResult> viewScheduleSearchResults)
        {
            FoundResults = new ObservableCollection<ViewScheduleSearchResult>(viewScheduleSearchResults);
        }
    }
}
