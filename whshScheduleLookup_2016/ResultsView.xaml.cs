namespace whshScheduleLookup
{
    public partial class ResultsView
    {
        private const string LangItem = "whshScheduleLookup";
        public ResultsView()
        {
            InitializeComponent();
            Title = ModPlusAPI.Language.GetItem(LangItem, "h1");
        }
    }
}
