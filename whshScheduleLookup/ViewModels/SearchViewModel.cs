namespace whshScheduleLookup.ViewModels
{
    using System.Collections.Generic;
    using Autodesk.Revit.DB;
    using Model;
    using ModPlusAPI;
    using ModPlusAPI.Mvvm;

    public partial class SearchViewModel : VmBase
    {
        private const string LangItem = "whshScheduleLookup";

        #region Static data

        private readonly string _emptySearchErrMess = " nothing entered yet";
        private readonly string _pressFindMess = " click \"Find\" to start";
        private readonly string _refreshSearchMess = " click 'Find' to refresh search";
        private readonly string _noSchedulesFoundMess = " no schedules match criteria";
        private readonly string _schedulesNumberFoundMess = " schedules found: ";

        public string AssemblyVersion { get; set; }

        public string WindowTitle { get; set; } = "Find in Tables";

        public string AddText { get; set; } = "that must present in table"; // которые вместе должны присутствовать в таблице

        public string FindButtonName { get; set; } = "Find";

        public string ResetButtonName { get; set; } = "Clean";

        public string CancelButtonName { get; set; } = "Cancel";

        public string DelimiterText { get; set; } = "delimiter\n symbol"; // разделитель&#x0a;значений

        public string PartialSearchText { get; set; } = "search substring";

        public string IgnoreCaseText { get; set; } = "ignore case";

        public string SearchText { get; set; } = "Search for";

        public string FieldText { get; set; } = "field name";

        public string ColumnText { get; set; } = "column name";

        public string ValueText { get; set; } = "cell value";

        public string HeaderText { get; set; } = "header value";

        public string PercentCompleteMess { get; set; } = "% completed...";

        public string StatusText { get; set; } = "STATE: ";

        public string SearchBarTooltip { get; set; } = "Press spacebar twice to insert delimeter";

        #endregion

        public RevitModel RevitModel { get; set; }

        private bool _selectionInProgress;
        private string _stateMessage;
        private bool _solutionExists;

        private List<string> _enteredNames;
        private string _enteredNamesString;
        private bool _isParameterName;
        private string _labelName;
        private List<ViewSchedule> _foundSchedules;
        private System.Windows.Visibility _expanderState;
        private bool _enteredNamesAreValid;
        private bool _isHeadingName;
        private bool _isCellValue;
        private bool _ignoreCase;
        private bool _partialSearch;
        private string _delimeter;
        private List<ViewSchedule> _allModelSchedules;
        private List<ViewScheduleSearchResult> _foundResults;
        private int _progress;
        private bool _searchFinished;
        private int? _foundResultsNumber;
        private bool _isHeader;

        #region Common Data

        public bool SolutionExists
        {
            get => _solutionExists;
            set
            {
                if (value == _solutionExists)
                    return;
                _solutionExists = value;
                OnPropertyChanged();
            }
        }

        public string StateMessage
        {
            get => _stateMessage;
            set
            {
                _stateMessage = StatusText + " " + value;
                OnPropertyChanged();
            }
        }

        public bool SelectionInProgress
        {
            get => _selectionInProgress;
            set
            {
                if (value == _selectionInProgress)
                    return;
                _selectionInProgress = value;
                OnPropertyChanged();
            }
        }

        public bool SearchFinished
        {
            get => _searchFinished;
            set
            {
                _searchFinished = value;
                OnPropertyChanged();
            }
        }

        public SearchViewModel(RevitModel revitModel)
        {
            RevitModel = revitModel;
            _emptySearchErrMess = Language.GetItem(LangItem, "h10"); // "ничего не введено для поиска";
            _pressFindMess = Language.GetItem(LangItem, "h11"); // "нажмите \"Найти\" для продолжения";
            _refreshSearchMess = Language.GetItem(LangItem, "h12"); // "Нажмите \"Найти\" для актуализации!";
            _noSchedulesFoundMess = Language.GetItem(LangItem, "h13"); // "не найдено подходящих таблиц!";
            _schedulesNumberFoundMess = Language.GetItem(LangItem, "h14"); // " таблиц найдено: ";

            WindowTitle = Language.GetItem(LangItem, "h15"); // "ПОИСК В ТАБЛИЦАХ";
            AddText = Language.GetItem(LangItem, "h16"); // "\nкоторые должны присутствовать в таблице";
            FindButtonName = Language.GetItem(LangItem, "find"); // "Найти";
            ResetButtonName = Language.GetItem(LangItem, "clean"); // "Очистить";
            CancelButtonName = Language.GetItem(LangItem, "cancel"); // "Отмена";
            DelimiterText = Language.GetItem(LangItem, "h17"); // "символ-\nразделитель"; //
            PartialSearchText = Language.GetItem(LangItem, "h18"); // "подстрока";
            IgnoreCaseText = Language.GetItem(LangItem, "h19"); // "учитывать регистр";
            SearchText = Language.GetItem(LangItem, "h20"); // "Искать";
            FieldText = Language.GetItem(LangItem, "h21"); // "поле";
            ColumnText = Language.GetItem(LangItem, "h22"); // "заголовок";
            ValueText = Language.GetItem(LangItem, "h23"); // "значение";
            HeaderText = Language.GetItem(LangItem, "h24"); // "шапка";
            PercentCompleteMess = Language.GetItem(LangItem, "h25"); // "% обработано...";
            StatusText = Language.GetItem(LangItem, "h26"); // "СТАТУС: ";
            SearchBarTooltip = Language.GetItem(LangItem, "h27") + "\"" + Delimeter + "\""; // $"Нажмите пробел дважды, чтобы вставить \"{Delimeter}\"";

            StateMessage = _emptySearchErrMess;
            SearchFinished = true;

            IsParameterName = !bool.TryParse(
                UserConfigFile.GetValue("whshScheduleLookup", nameof(IsParameterName)), out var b) || b; // default - true
            IsHeadingName = bool.TryParse(
                UserConfigFile.GetValue("whshScheduleLookup", nameof(IsHeadingName)), out b) && b; // default - false
            IsCellValue = bool.TryParse(
                UserConfigFile.GetValue("whshScheduleLookup", nameof(IsCellValue)), out b) && b; // default - false
            IgnoreCase = bool.TryParse(
                UserConfigFile.GetValue("whshScheduleLookup", nameof(IgnoreCase)), out b) && b; // default - false
            PartialSearch = bool.TryParse(
                UserConfigFile.GetValue("whshScheduleLookup", nameof(PartialSearch)), out b) && b; // default - false
            var delimeter = UserConfigFile.GetValue("whshScheduleLookup", nameof(Delimeter));
            Delimeter = string.IsNullOrEmpty(delimeter) ? ";" : delimeter;

            AssemblyVersion = revitModel.GetType().Assembly.GetName().Version.ToString();
            Update();
        }

        #endregion

        public int Progress
        {
            get => _progress;
            set
            {
                _progress = value;
                OnPropertyChanged();
            }
        }

        public List<string> EnteredNames
        {
            get => _enteredNames;
            set
            {
                if (value == _enteredNames)
                    return;
                _enteredNames = value;
                OnPropertyChanged();
            }
        }

        public bool EnteredNamesAreValid
        {
            get => _enteredNamesAreValid;
            set
            {
                if (value == _enteredNamesAreValid)
                    return;
                _enteredNamesAreValid = value;
                OnPropertyChanged();
            }
        }

        public string EnteredNamesString
        {
            get => _enteredNamesString;
            set
            {
                if (value == _enteredNamesString)
                    return;
                _enteredNamesString = value;
                OnPropertyChanged();
                Update();
                if (!string.IsNullOrEmpty(_enteredNamesString))
                {
                    StateMessage = _pressFindMess;
                }
                else
                {
                    StateMessage = _emptySearchErrMess;
                }
            }
        }

        public bool IsParameterName
        {
            get => _isParameterName;
            set
            {
                if (value == _isParameterName)
                    return;
                _isParameterName = value;
                OnPropertyChanged();
                Update();
                UserConfigFile.SetValue("whshScheduleLookup", nameof(IsParameterName), value.ToString(), true);
            }
        }

        private void Update()
        {
            FoundResultsNumber = null;
            if (!string.IsNullOrEmpty(EnteredNamesString))
            {
                StateMessage = _refreshSearchMess;
            }
            else
            {
                StateMessage = _emptySearchErrMess;
            }

            if (IsParameterName)
            {
                LabelName = Language.GetItem(LangItem, "h28") + $" \"{Delimeter}\",\n" + AddText;
            }

            if (IsHeadingName)
            {
                LabelName = Language.GetItem(LangItem, "h29") + $" \"{Delimeter}\",\n" + AddText;
            }

            if (IsCellValue)
            {
                LabelName = Language.GetItem(LangItem, "h30") + $" \"{Delimeter}\",\n" + AddText;
            }
        }

        public string Delimeter
        {
            get => _delimeter;
            set
            {
                if (value == _delimeter)
                    return;
                _delimeter = value;
                OnPropertyChanged();
                Update();
                UserConfigFile.SetValue("whshScheduleLookup", nameof(Delimeter), value, true);
            }
        }

        public bool IsHeadingName
        {
            get => _isHeadingName;
            set
            {
                if (value == _isHeadingName)
                    return;
                _isHeadingName = value;
                OnPropertyChanged();
                Update();
                UserConfigFile.SetValue("whshScheduleLookup", nameof(IsHeadingName), value.ToString(), true);
            }
        }

        public bool IsCellValue
        {
            get => _isCellValue;
            set
            {
                if (value == _isCellValue)
                    return;
                _isCellValue = value;
                OnPropertyChanged();
                Update();
                UserConfigFile.SetValue("whshScheduleLookup", nameof(IsCellValue), value.ToString(), true);
            }
        }

        public bool IsHeader
        {
            get => _isHeader;
            set
            {
                if (value == _isHeader)
                    return;
                _isHeader = value;
                OnPropertyChanged();
                Update();
            }
        }

        public bool IgnoreCase
        {
            get => _ignoreCase;
            set
            {
                if (value == _ignoreCase)
                    return;
                _ignoreCase = value;
                OnPropertyChanged();
                Update();
                UserConfigFile.SetValue("whshScheduleLookup", nameof(IgnoreCase), value.ToString(), true);
            }
        }

        public bool PartialSearch
        {
            get => _partialSearch;
            set
            {
                if (value == _partialSearch)
                    return;
                _partialSearch = value;
                OnPropertyChanged();
                Update();
                UserConfigFile.SetValue("whshScheduleLookup", nameof(PartialSearch), value.ToString(), true);
            }
        }

        public string LabelName
        {
            get => _labelName;
            set
            {
                if (value == _labelName)
                    return;
                _labelName = value;
                OnPropertyChanged();
            }
        }

        public List<ViewSchedule> AllModelSchedules
        {
            get => _allModelSchedules;
            set
            {
                if (value == _allModelSchedules)
                    return;
                _allModelSchedules = value;
                OnPropertyChanged();
            }
        }

        public List<ViewSchedule> FoundSchedules
        {
            get => _foundSchedules;
            set
            {
                _foundSchedules = value;
                OnPropertyChanged();
            }
        }

        public List<ViewScheduleSearchResult> FoundResults
        {
            get => _foundResults;
            set
            {
                _foundResults = value;
                OnPropertyChanged();
                if (_foundResults == null || _foundResults.Count == 0)
                {
                    StateMessage = _noSchedulesFoundMess;
                }
                else
                {
                    StateMessage = _schedulesNumberFoundMess + " " + _foundResults.Count;
                }
            }
        }

        public int? FoundResultsNumber
        {
            get => _foundResultsNumber;
            set
            {
                _foundResultsNumber = value;
                OnPropertyChanged();
            }
        }

        public System.Windows.Visibility ExpanderState
        {
            get => _expanderState;
            set
            {
                if (value == _expanderState)
                    return;
                _expanderState = value;
                OnPropertyChanged();
            }
        }
    }
}
