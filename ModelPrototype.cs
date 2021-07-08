using System;
using System.Collections.Generic;
using System.Reflection;

namespace Test
{
    internal class ModelPrototype
    {
        public Column PrimaryKey { get; set; } = null;

        public Column CreatedAtColumn { get; set; } = null;

        public Column UpdatedAtColumn { get; set; } = null;

        public bool AutoIncrements { get; set; } = true;

        private List<Column> columns = new List<Column>();
        public IEnumerable<Column> Columns => this.columns;
        public int ColumnsCount => this.columns.Count;

        private Dictionary<string, MethodInfo> relations = new Dictionary<string, MethodInfo>();

        public string TableName { get; private set; }

        internal ModelPrototype(Type modelType)
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


            var methods = modelType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            foreach (var method in methods)
            {
                RelationAttribute relationAttribute = method.GetCustomAttribute<RelationAttribute>();
                if (relationAttribute == null) continue;
                string name = relationAttribute.Name ?? method.Name.Replace("Relation", "");

                Console.WriteLine("RELATION YES: " + name);
                this.relations.Add(name, method);
            }
            this.TableName = modelType.Name;
        }

        public MethodInfo GetRelation(string name)
        {
            return this.relations[name];
        }
    }
}