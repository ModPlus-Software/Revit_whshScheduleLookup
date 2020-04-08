namespace whshScheduleLookup.Model
{
    using ModPlusAPI.Mvvm;

    public class PropertyValuePair : VmBase
    {
        public string PropertyName { get; set; }

        public string PropertyDisplayName { get; set; }

        public string PropertyValue { get; set; }

        public string PropertyValueName { get; set; }
    }
}
