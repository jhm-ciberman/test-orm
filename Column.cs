using System.Reflection;

namespace Test
{
    public class Column
    {
        public PropertyInfo Property { get; private set; }

        public string ColumnName { get; private set; }

        public int Index { get; private set; }

        public Column(PropertyInfo property, string columnName, int index)
        {
            this.Property = property;
            this.ColumnName = columnName ?? property.Name;
            this.Index = index;
        }
    }
}