using System.Reflection;

namespace Test
{
    public class Column
    {
        public PropertyInfo Property { get; private set; }

        public string ColumnName { get; private set; }

        public object OriginalValue { get; set; }

        public Column(PropertyInfo property, string columnName)
        {
            this.Property = property;
            this.ColumnName = columnName ?? property.Name;
        }
    }
}