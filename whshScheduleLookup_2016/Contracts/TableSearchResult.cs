using Autodesk.Revit.DB;

namespace whshScheduleLookup.Contracts
{
    public abstract class TableSearchResult<T> where T : TableView
    {
        public abstract ElementId TableId { get; set; }
        public abstract string TableName { get; set; }
        public abstract SectionType SectionType { get; set; }
        public abstract string SearchedValue { get; set; }
        public abstract string ColumnName { get; set; }
        public abstract int? ColumnNumber { get; set; }
        public abstract int? RowNumber { get; set; }
        public abstract CellType CellType { get; set; }

        //protected TableSearchResult(ViewSchedule viewSchedule, string searchedValue) { }

        //public abstract void CreateBlank();

        public virtual bool Create(T table, SectionType sectionType, string searchedValue)
        {
            TableName = table.Name;
            TableId = table.Id;
            SearchedValue = searchedValue;
            SectionType = sectionType;
            if (SectionType == SectionType.Body)
            {
                FillFromBody();
            }
            if (SectionType == SectionType.Header)
            {
                FillFromBody();
            }
            if (SectionType == SectionType.None)
            {
                Fill();
            }
            return true;
        }

        public abstract bool Fill();
        public abstract bool FillFromBody();
        public abstract bool FillFromHeader();

    }
}
