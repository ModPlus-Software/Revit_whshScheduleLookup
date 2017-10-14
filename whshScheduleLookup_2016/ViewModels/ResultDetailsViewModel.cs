using System;
using System.Collections.Generic;
using whshScheduleLookup.Model;

namespace whshScheduleLookup.ViewModels
{
    public class ResultDetailsViewModel
    {
        public List<PropertyValuePair> PropertyValuePairs { get; set; } = new List<PropertyValuePair>();

        public ResultDetailsViewModel(object someClassInstance)
        {
            Type type = someClassInstance.GetType();
            var propertyInfos = type.GetProperties();
            foreach (var propertyInfo in propertyInfos)
            {
                var pvp = new PropertyValuePair();
                pvp.PropertyName = propertyInfo.Name;
                if (Properties.Settings.Default.PropertyFriendlyNamesEN != null
                    && Properties.Settings.Default.PropertyFriendlyNamesEN.ContainsKey(pvp.PropertyName))
                {
                    pvp.PropertyDisplayName = Properties.Settings.Default.PropertyFriendlyNamesEN[pvp.PropertyName];
                }
                else
                {
                    pvp.PropertyDisplayName = pvp.PropertyName;
                }
                var value = propertyInfo.GetValue(someClassInstance);
                if (value == null) pvp.PropertyValue = "n/a";
                else if (string.IsNullOrEmpty(value.ToString())) pvp.PropertyValue = "-";
                else pvp.PropertyValue = value.ToString();

                //Debug.Print("Property: " + pvp.PropertyName + " Value: " + pvp.PropertyValue);
                //pvp.PropertyDisplayName = propertyInfo.GetCustomAttributes()
                PropertyValuePairs.Add(pvp);
            }
            
        }

        public ResultDetailsViewModel()
        {

        }
    }
}
