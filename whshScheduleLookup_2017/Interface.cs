using System;
using System.Collections.Generic;
using ModPlusAPI.Interfaces;

namespace whshScheduleLookup
{
    public class Interface : IModPlusFunctionInterface
    {
        public SupportedProduct SupportedProduct => SupportedProduct.Revit;
        public string Name => "whshScheduleLookup";
        public string AvailProductExternalVersion => "2017";
        public string FullClassName => "whshScheduleLookup.Commands.ScheduleLookupCommand";
        public string AppFullClassName => string.Empty;
        public Guid AddInId => Guid.Empty;
        public string LName => "Поиск в таблицах";
        public string Description => "Поиск таблиц в проекте, содержащих указанное значение";
        public string Author => "Игорь Сердюков";
        public string Price => "0";
        public bool CanAddToRibbon => true;
        public string FullDescription => "Ищет заданные строки (целиком или часть), а также числа среди таблиц проекта в названиях полей, в заголовках столбцов или в ячейках, с учётом или без учёта регистра";
        public string ToolTipHelpImage => string.Empty;
        public List<string> SubFunctionsNames => new List<string>();
        public List<string> SubFunctionsLames => new List<string>();
        public List<string> SubDescriptions => new List<string>();
        public List<string> SubFullDescriptions => new List<string>();
        public List<string> SubHelpImages => new List<string>();
        public List<string> SubClassNames => new List<string>();
    }
}