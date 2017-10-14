using System.Collections.Generic;
using System.Collections.ObjectModel;
using whshScheduleLookup.Model;

namespace whshScheduleLookup.ViewModels
{
    public class ResultsViewModel
    {
        public ObservableCollection<ViewScheduleSearchResult> FoundResults { get; set; } // = new ObservableCollection<ViewScheduleSearchResult>();

        public ResultsViewModel(List<ViewScheduleSearchResult> viewScheduleSearchResults)
        {
            FoundResults = new ObservableCollection<ViewScheduleSearchResult>(viewScheduleSearchResults);
        }

        public ResultsViewModel()
        {
            
        }
    }
}
