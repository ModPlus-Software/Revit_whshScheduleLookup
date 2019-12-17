namespace whshScheduleLookup.ViewModels
{
    using System;
    using System.Collections.Generic;
    using Model;
    using ModPlusAPI;

    public class ResultDetailsViewModel
    {
        private const string LangItem = "whshScheduleLookup";

        public List<PropertyValuePair> PropertyValuePairs { get; set; } = new List<PropertyValuePair>();

        public ResultDetailsViewModel(object someClassInstance)
        {
            Type type = someClassInstance.GetType();
            var propertyInfos = type.GetProperties();
            foreach (var propertyInfo in propertyInfos)
            {
                var pvp = new PropertyValuePair();
                pvp.PropertyName = propertyInfo.Name;
                if (LocalizationNames.ContainsKey(pvp.PropertyName))
                {
                    pvp.PropertyDisplayName = LocalizationNames[pvp.PropertyName];
                }
                else
                {
                    pvp.PropertyDisplayName = pvp.PropertyName;
                }

                var value = propertyInfo.GetValue(someClassInstance);
                if (value == null)
                    pvp.PropertyValue = "n/a";
                else if (string.IsNullOrEmpty(value.ToString()))
                    pvp.PropertyValue = "-";
                else
                    pvp.PropertyValue = value.ToString();

                PropertyValuePairs.Add(pvp);
            }
        }

        private Dictionary<string, string> LocalizationNames => new Dictionary<string, string>
        {
            { "SearchedValue", Language.GetItem(LangItem, "sr1") },
            { "FoundIn", Language.GetItem(LangItem, "sr2") },
            { "ViewScheduleName", Language.GetItem(LangItem, "sr3") },
            { "IsKeySchedule", Language.GetItem(LangItem, "sr4") },
            { "ViewScheduleId", Language.GetItem(LangItem, "sr5") },
            { "FieldName", Language.GetItem(LangItem, "sr6") },
            { "FieldParameterName", Language.GetItem(LangItem, "sr7") },
            { "ColumnHeadingName", Language.GetItem(LangItem, "sr8") },
            { "FromKeyScheduleNamed", Language.GetItem(LangItem, "sr9") },
            { "KeyScheduleParameterName", Language.GetItem(LangItem, "sr10") },
            { "ColumnNumber", Language.GetItem(LangItem, "sr11") },
            { "IsHiddenColumn", Language.GetItem(LangItem, "sr12") },
            { "RowNumber", Language.GetItem(LangItem, "sr13") },
            { "SectionType", Language.GetItem(LangItem, "sr14") },
            { "CellType", Language.GetItem(LangItem, "sr15") },
            { "IsHeadingsRow", Language.GetItem(LangItem, "sr16") },
            { "SearchExecuted", Language.GetItem(LangItem, "sr17") },
            { "Found", Language.GetItem(LangItem, "sr18") },
            { "PartialSearch", Language.GetItem(LangItem, "sr19") },
            { "IgnoreCase", Language.GetItem(LangItem, "sr20") }
        };
    }
}
