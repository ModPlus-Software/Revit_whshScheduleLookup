using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using Autodesk.Revit.DB;
using ModPlusAPI;

namespace whshScheduleLookup.Model
{
    public class ViewScheduleSearchResult
    {
        private const string LangItem = "whshScheduleLookup";
        private static Dictionary<string, string> _paramsNamesAndKeySchedulesNames;
        private int? _columnNumber;
        private int? _rowNumber;

        public string SearchedValue { get; set; } = string.Empty;
        public string FoundIn { get; set; } = string.Empty;
        public string ViewScheduleName { get; set; } = string.Empty;
        public ElementId ViewScheduleId { get; set; }
        public bool? IsKeySchedule { get; set; }
        public string FieldName { get; set; } = string.Empty;
        public string FieldParameterName { get; set; } = string.Empty;
        public string ColumnHeadingName { get; set; } = string.Empty;
        public string FromKeyScheduleNamed { get; set; } = string.Empty;
        public string KeyScheduleParameterName { get; set; } = string.Empty;
        public int? ColumnNumber {
            get => _columnNumber;
            set { if (value != null) { _columnNumber = value + 1; } }
        }
        public bool IsHiddenColumn { get; set; }
        public int? RowNumber
        {
            get => _rowNumber;
            set { if (value != null) { _rowNumber = value + 1; } }
        }
        public bool IsHeadingsRow { get; set; }
        public SectionType SectionType { get; set; }
        public string CellType { get; set; } = string.Empty;
        //public bool? IncludingHeadings { get; set; }
        public bool SearchExecuted { get; set; }
        public bool? Found { get; set; }
        public bool PartialSearch { get; set; }
        public bool IgnoreCase { get; set; }



        public ViewScheduleSearchResult(ViewSchedule viewSchedule)
        {
            SearchExecuted = false;
            if (_paramsNamesAndKeySchedulesNames == null)
            {
                var docViewSchedules = new FilteredElementCollector(viewSchedule.Document)
                    .OfClass(typeof(ViewSchedule)).WhereElementIsNotElementType().ToElements()
                    .Where(e => e is ViewSchedule).Cast<ViewSchedule>().ToList();
                foreach (var docViewSchedule in docViewSchedules)
                {
                    string ksParamName = string.Empty;
                    try
                    {
                        ksParamName = docViewSchedule.KeyScheduleParameterName;
                    }
                    catch (Autodesk.Revit.Exceptions.InvalidOperationException e) { }
                    if (!string.IsNullOrEmpty(ksParamName))
                    {
                        if (_paramsNamesAndKeySchedulesNames == null)
                        {
                            _paramsNamesAndKeySchedulesNames = new Dictionary<string, string>();
                        }
                        if (!_paramsNamesAndKeySchedulesNames.ContainsKey(ksParamName))
                        {
                            _paramsNamesAndKeySchedulesNames.Add(ksParamName, docViewSchedule.ViewName);
                        }
                    }
                }
            }
        }

        public static List<ViewScheduleSearchResult> FilterResultsByCellValues(List<ViewSchedule> viewSchedules,
            bool onlyHeadings, List<string> searchValues, SectionType section
            , bool partialSearch = false, bool ignoreCase = false)
        {
            var n = searchValues.Count;
            List<ViewScheduleSearchResult> filteredResults = new List<ViewScheduleSearchResult>();
            foreach (var viewSchedule in viewSchedules)
            {
                var viewScheduleSearchResults = OfCellValues(viewSchedule,
                    onlyHeadings, searchValues, section, partialSearch, ignoreCase);
                if (viewScheduleSearchResults != null && viewScheduleSearchResults.Count == n)
                {
                    filteredResults.AddRange(viewScheduleSearchResults);
                }
            }
            return filteredResults;
        }

        public static List<ViewScheduleSearchResult> FilterResultsParameterNames(List<ViewSchedule> viewSchedules,
            List<string> searchValues, bool partialSearch = false, bool ignoreCase = false)
        {
            var n = searchValues.Count;
            List<ViewScheduleSearchResult> filteredResults = new List<ViewScheduleSearchResult>();
            foreach (var viewSchedule in viewSchedules)
            {
                var viewScheduleSearchResults = OfParameterNames(viewSchedule,
                    searchValues, partialSearch, ignoreCase);
                if (viewScheduleSearchResults != null && viewScheduleSearchResults.Count == n)
                {
                    filteredResults.AddRange(viewScheduleSearchResults);
                }
            }
            return filteredResults;
        }

        public static List<ViewSchedule> FilterSchedulesByCellValues(List<ViewSchedule> viewSchedules,
            bool onlyHeadings, List<string> searchValues, SectionType section,
            bool partialSearch = false, bool ignoreCase = false)
        {
            var n = searchValues.Count;
            List<ViewSchedule> filteredSchedules = new List<ViewSchedule>();
            foreach (var viewSchedule in viewSchedules)
            {
                var viewScheduleSearchResults = OfCellValues(viewSchedule,
                    onlyHeadings, searchValues, section, partialSearch, ignoreCase);
                if (viewScheduleSearchResults != null && viewScheduleSearchResults.Count == n)
                {
                    filteredSchedules.Add(viewSchedule);
                }
            }
            return filteredSchedules;
        }

        public static List<ViewSchedule> FilterSchedulesByParameterNames(List<ViewSchedule> viewSchedules,
            List<string> searchValues, bool partialSearch = false, bool ignoreCase = false)
        {
            var n = searchValues.Count;
            List<ViewSchedule> filteredSchedules = new List<ViewSchedule>();
            foreach (var viewSchedule in viewSchedules)
            {
                var viewScheduleSearchResults = OfParameterNames(viewSchedule,
                    searchValues, partialSearch, ignoreCase);
                if (viewScheduleSearchResults != null && viewScheduleSearchResults.Count == n)
                {
                    filteredSchedules.Add(viewSchedule);
                }
            }
            return filteredSchedules;
        }

        public static List<ViewScheduleSearchResult> OfCellValues(ViewSchedule viewSchedule, bool onlyHeadings,
            List<string> searchValues, SectionType section, bool partialSearch = false, bool ignoreCase = false)
        {
            //if (viewSchedule.ViewName != "00_ROMOV.Расчет воздух_Заполнение") { return null; }
            var scheduleDefinition = viewSchedule.Definition;
            bool isKeySchedule = false;
            var scheduleFieldsCount = scheduleDefinition.GetFieldCount();
            searchValues = searchValues.Select(x => x.Trim()).ToList();
            var viewScheduleSearchResults = new List<ViewScheduleSearchResult>();
            var foundsArray = new bool[searchValues.Count];
            var founds = foundsArray.ToList();
            if (scheduleFieldsCount < 1) { return null; }
            var tableData = viewSchedule.GetTableData();
            var sectionData = tableData.GetSectionData(section);
            if (sectionData == null || sectionData.LastColumnNumber < 0 || sectionData.LastRowNumber < 0) { return null; }

            List<string> headingsRowValues = null;
            bool canCheckHeaders = false;
            HashSet<string> headingsRowSet = null;
            var scheduleFieldsColumnsNums = Enumerable.Range(0, scheduleFieldsCount).ToList();
            var paramIdandField = new List<Tuple<ElementId, ScheduleField>>(); // new Dictionary<ElementId, ScheduleField>();

            foreach (var scheduleFieldId in scheduleDefinition.GetFieldOrder())
            {
                var field = scheduleDefinition.GetField(scheduleFieldId);
                if (field.IsHidden) continue;
                Debug.WriteLine(field.GetName() + field.ParameterId);
                var fieldParameterId = field.ParameterId;
                paramIdandField.Add(new Tuple<ElementId, ScheduleField>(fieldParameterId, field));
                if (fieldParameterId.IntegerValue == (int)BuiltInParameter.REF_TABLE_ELEM_NAME)
                {
                    isKeySchedule = true;
                }
            }


            if (onlyHeadings)
            {
                headingsRowValues = GetHeadingsByColumnNumbers(viewSchedule, scheduleFieldsColumnsNums);
                var headingsRowAggregateString = headingsRowValues
                    .Aggregate((workingSentence, next) => next + workingSentence);
                if (!string.IsNullOrEmpty(headingsRowAggregateString)) { canCheckHeaders = true; }
                if (canCheckHeaders)
                {
                    headingsRowSet = new HashSet<string>(headingsRowValues);
                }
            }

            var bodyLastColumnNumber = sectionData.LastColumnNumber;
            for (int row = 0; row <= sectionData.LastRowNumber; row++)
            {
                var rowCellsValues = new List<string>();
                for (int col = 0; col <= bodyLastColumnNumber; col++)
                {
                    if (sectionData.IsValidColumnNumber(col) && sectionData.IsValidRowNumber(row))
                    {
                        var cellText = viewSchedule.GetCellText(section, row, col).Trim();
                        //if (!string.IsNullOrEmpty(cellText))
                        //{
                        rowCellsValues.Add(cellText);
                        //}
                    }
                }
                if (rowCellsValues.Count <= 0) continue;

                var rowAggregateString = rowCellsValues
                    .Aggregate((workingSentence, next) => next + workingSentence);
                if (string.IsNullOrEmpty(rowAggregateString)) continue;

                HashSet<string> rowValuesSet = new HashSet<string>(rowCellsValues);

                if (canCheckHeaders && onlyHeadings && !rowValuesSet.IsSubsetOf(headingsRowSet)) { continue; }

                for (var col = 0; col < rowCellsValues.Count; col++)
                {
                    ViewScheduleSearchResult res = null;
                    var cellValue = rowCellsValues[col];
                    int columnNumber = col;
                    //var cellValue = viewSchedule.GetCellText(SectionType.Body, row, col).Trim();
                    if (string.IsNullOrEmpty(cellValue)) { continue; }
                    //Debug.Write(cellValue.ToString());
                    for (var index = 0; index < searchValues.Count; index++)
                    {
                        if (founds[index]) continue;
                        var searchValue = searchValues[index];
                        if (partialSearch)
                        {
                            if (!ignoreCase && cellValue.Contains(searchValue)
                                || ignoreCase && cellValue.ToLower().Contains(searchValue.ToLower())
                            )
                            {
                                founds[index] = true;
                                res = new ViewScheduleSearchResult(viewSchedule) { SearchedValue = searchValue };
                                res.FoundIn = cellValue;
                                break;
                            }
                        }
                        else
                        {
                            if (!ignoreCase && cellValue == searchValue
                                || ignoreCase && cellValue.ToLower() == searchValue.ToLower()
                            )
                            {
                                founds[index] = true;
                                res = new ViewScheduleSearchResult(viewSchedule) { SearchedValue = searchValue };
                                res.FoundIn = cellValue;
                                break;
                            }
                        }
                    }
                    //Debug.WriteLine("");
                    if (res != null)
                    {
                        res.ColumnNumber = columnNumber;
                        res.RowNumber = row;
                        res.IsHeadingsRow = onlyHeadings;
                        res.CellType = sectionData.GetCellType(row, columnNumber).ToString();
                        var paramId = sectionData.GetCellParamId(row, columnNumber);
                        if (paramId == ElementId.InvalidElementId) paramId = sectionData.GetCellParamId(columnNumber);
                        if (paramId == ElementId.InvalidElementId)
                        {
                            for (int i = sectionData.LastRowNumber; i >= 0; i--)
                            {
                                paramId = sectionData.GetCellParamId(i, columnNumber);
                                if (paramId != ElementId.InvalidElementId) break;
                            }
                        }

                        // if paramId is still undefined data comes from field not body
                        var field = paramIdandField[columnNumber].Item2;
                        var fieldParamId = paramIdandField[columnNumber].Item1;
                        res.FieldName = field.GetName();
                        res.ColumnHeadingName = field.ColumnHeading;
                        if (paramId == ElementId.InvalidElementId)
                        {
                            paramId = field.ParameterId;
                            if (paramId == ElementId.InvalidElementId)
                            {
                                res.FieldParameterName = field.FieldType.ToString();
                            }
                        }
                        
                        if (paramId != ElementId.InvalidElementId)
                        {
                            res.FieldParameterName = GetNameById(viewSchedule.Document, fieldParamId);
                            if (paramId.IntegerValue == (int)BuiltInParameter.REF_TABLE_ELEM_NAME)
                            {
                                res.FromKeyScheduleNamed = viewSchedule.ViewName;
                                res.KeyScheduleParameterName = viewSchedule.KeyScheduleParameterName;
                                res.FieldParameterName = viewSchedule.KeyScheduleParameterName;
                            }
                            else
                            {
                                if (_paramsNamesAndKeySchedulesNames != null
                                    && _paramsNamesAndKeySchedulesNames.ContainsKey(res.FieldParameterName))
                                {
                                    res.FromKeyScheduleNamed = _paramsNamesAndKeySchedulesNames[res.FieldParameterName];
                                    res.KeyScheduleParameterName = res.FieldParameterName;
                                }
                            }
                        }

                        res.ViewScheduleName = viewSchedule.Name;
                        res.IsKeySchedule = isKeySchedule;
                        res.ViewScheduleId = viewSchedule.Id;
                        res.SectionType = section;
                        res.PartialSearch = partialSearch;
                        res.IgnoreCase = ignoreCase;
                        res.SearchExecuted = true;
                        res.Found = true;
                        viewScheduleSearchResults.Add(res);
                    }
                }
                if (founds.TrueForAll(x => x)) { break; }
            }
            return viewScheduleSearchResults;
        }

        public static List<ViewScheduleSearchResult> OfParameterNames(ViewSchedule viewSchedule,
            List<string> searchValues, bool partialSearch = false, bool ignoreCase = false)
        {
            //if (viewSchedule.ViewName != "00_ROMOV.Расчет воздух_Заполнение") { return null; }
            bool isKeySchedule = false;
            searchValues = searchValues.Select(x => x.Trim()).ToList();
            var viewScheduleSearchResults = new List<ViewScheduleSearchResult>();
            var foundsArray = new bool[searchValues.Count];
            var founds = foundsArray.ToList();

            var scheduleDefinition = viewSchedule.Definition;
            for (int columnNumber = 0; columnNumber < scheduleDefinition.GetFieldCount(); columnNumber++)
            {
                if (!scheduleDefinition.IsValidFieldIndex(columnNumber)) { continue; }
                var field = scheduleDefinition.GetField(columnNumber);
                var fieldName = field.GetName();
                if (field.ParameterId.IntegerValue == (int)BuiltInParameter.REF_TABLE_ELEM_NAME)
                {
                    isKeySchedule = true;
                }
                ViewScheduleSearchResult res = null;
                for (var index = 0; index < searchValues.Count; index++)
                {
                    if (founds[index]) continue;
                    var searchValue = searchValues[index];
                    if (partialSearch)
                    {
                        if (!ignoreCase && fieldName.Contains(searchValue)
                            || ignoreCase && fieldName.ToLower().Contains(searchValue.ToLower())
                        )
                        {
                            founds[index] = true;
                            res = new ViewScheduleSearchResult(viewSchedule) { SearchedValue = searchValue };
                            res.FoundIn = fieldName;
                            break;
                        }
                    }
                    else
                    {
                        if (!ignoreCase && fieldName == searchValue
                            || ignoreCase && fieldName.ToLower() == searchValue.ToLower()
                        )
                        {
                            founds[index] = true;
                            res = new ViewScheduleSearchResult(viewSchedule) { SearchedValue = searchValue };
                            res.FoundIn = fieldName;
                            break;
                        }
                    }
                }
                if (res != null)
                {
                    res.ColumnNumber = columnNumber;
                    res.FieldName = fieldName;
                    if (field.ParameterId != ElementId.InvalidElementId)
                    {
                        res.FieldParameterName = GetNameById(viewSchedule.Document, field.ParameterId);
                    }
                    else
                    {
                        res.FieldParameterName = field.FieldType.ToString();
                    }
                    if (field.ParameterId.IntegerValue == (int)BuiltInParameter.REF_TABLE_ELEM_NAME)
                    {
                        //res.FromKeyScheduleNamed = viewSchedule.ViewName;
                        res.KeyScheduleParameterName = viewSchedule.KeyScheduleParameterName;
                        res.FieldParameterName = viewSchedule.KeyScheduleParameterName;
                    }
                    res.ColumnHeadingName = field.ColumnHeading;
                    res.ViewScheduleName = viewSchedule.Name;
                    res.ViewScheduleId = viewSchedule.Id;
                    res.SectionType = SectionType.None;
                    res.CellType = "n/a";
                    res.IsHiddenColumn = field.IsHidden;
                    res.IsHeadingsRow = false;
                    res.PartialSearch = partialSearch;
                    res.IgnoreCase = ignoreCase;
                    res.SearchExecuted = true;
                    res.Found = true;
                    viewScheduleSearchResults.Add(res);
                }
                if (founds.TrueForAll(x => x) && isKeySchedule) { break; }
            }
            viewScheduleSearchResults.ForEach(res => res.IsKeySchedule = isKeySchedule);
            return viewScheduleSearchResults;
        }

        private static string GetNameById(Document doc, ElementId elementId, bool human = true)
        {
            string res = string.Empty;

            if (Enum.IsDefined(typeof(BuiltInParameter), elementId.IntegerValue) && elementId != ElementId.InvalidElementId)
            {
                if (human)
                {
                    res = LabelUtils.GetLabelFor((BuiltInParameter)elementId.IntegerValue);
                }
                else
                {
                    res = Enum.GetName(typeof(BuiltInParameter), (BuiltInParameter)elementId.IntegerValue);
                }

            }
            if (Enum.IsDefined(typeof(BuiltInCategory), elementId.IntegerValue))
            {
                if (human)
                {
                    var category = Category.GetCategory(doc, (BuiltInCategory)elementId.IntegerValue);
                    res = category?.Name;
                }
                else
                {
                    res = Enum.GetName(typeof(BuiltInCategory), (BuiltInCategory)elementId.IntegerValue);
                }
            }

            object element = doc.GetElement(elementId);

            if (string.IsNullOrEmpty(res) && element != null)
            {
                if (element.GetType() == typeof(InternalDefinition))
                {
                    res = ((InternalDefinition)element).Name;
                }
                else if (element.GetType() == typeof(Definition))
                {
                    res = ((Definition)element).Name;
                }
                else if (element.GetType() == typeof(SharedParameterElement))
                {
                    res = ((SharedParameterElement)element).Name;
                }
                else if (element.GetType() == typeof(ParameterElement))
                {
                    res = ((ParameterElement)element).Name;
                }
            }

            return res;
        }

        private static List<string> GetHeadingsByColumnNumbers(ViewSchedule viewSchedule, List<int> scheduleFieldsColumnsNums)
        {
            var scheduleDefinition = viewSchedule?.Definition;
            if (scheduleDefinition == null) { return null; }

            var valid = scheduleFieldsColumnsNums.TrueForAll(c => scheduleDefinition.IsValidFieldIndex(c));

            if (!valid) { return null; }

            var res = new List<string>();
            scheduleFieldsColumnsNums.ForEach(c => res.Add(scheduleDefinition.GetField(c).ColumnHeading));

            return res;
        }
    }
}
