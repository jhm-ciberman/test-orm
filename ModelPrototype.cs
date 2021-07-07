using System;
using System.Collections.Generic;
using System.Reflection;

namespace Test
{
    public class ModelPrototype
    {
        private static Dictionary<Guid, ModelPrototype> cache = new Dictionary<Guid, ModelPrototype>();

        public static ModelPrototype Get(Model model)
        {
            Type type = model.GetType();
            Guid guid = type.GUID;
            if (! ModelPrototype.cache.TryGetValue(guid, out ModelPrototype modelPrototype))
            {
                modelPrototype = new ModelPrototype(type);
                ModelPrototype.cache[guid] = modelPrototype;
            }

            return modelPrototype;
        }

        public Column PrimaryKey { get; set; } = null;

        public Column CreatedAtColumn { get; set; } = null;

        public Column UpdatedAtColumn { get; set; } = null;

        public bool AutoIncrements { get; set; } = true;

        private List<Column> columns = new List<Column>();
        public IEnumerable<Column> Columns => this.columns;
        public int ColumnsCount => this.columns.Count;

        public ModelPrototype(Type modelType)
        {
            var properties = modelType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            for (int i = 0; i < properties.Length; i++)
            {
                var property = properties[i];
                var columnAttribute = property.GetCustomAttribute<ColumnAttribute>();
                if (columnAttribute == null) continue;
                var column = new Column(property, columnAttribute.Name, this.columns.Count);
                if (columnAttribute is PrimaryKeyAttribute pkAttribute)
                {
                    this.PrimaryKey = column;
                    this.AutoIncrements = pkAttribute.AutoIncrement;
                    if (! this.AutoIncrements) this.columns.Add(column);
                }
                else
                {
                    this.columns.Add(column);

                    if (columnAttribute is CreatedAtAttribute)
                        this.CreatedAtColumn = column;
                    else if (columnAttribute is UpdatedAtAttribute)
                        this.UpdatedAtColumn = column;
                }
            }
        }
    }
}