using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace Test
{
    public abstract class Model
    {
        private Column primaryKey = null;

        private Column createdAtColumn = null;

        private Column updatedAtColumn = null;

        private List<Column> columns = new List<Column>();

        private bool exists = false;

        public Model()
        {
            var properties = this.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (var property in properties)
            {
                var columnAttribute = property.GetCustomAttribute<ColumnAttribute>();
                if (columnAttribute == null) continue;
                var column = new Column(property, columnAttribute.Name);
                if (columnAttribute is PrimaryKeyAttribute)
                {
                    this.primaryKey = column;
                }
                else
                {
                    this.columns.Add(column);

                    if (columnAttribute is CreatedAtAttribute)
                        this.createdAtColumn = column;
                    else if (columnAttribute is UpdatedAtAttribute)
                        this.updatedAtColumn = column;
                }
            }
        }

        public virtual string GetTableName()
        {
            return this.GetType().Name;
        }

        public void Fill(DataRow row)
        {
            foreach (var column in this.columns)
            {
                column.OriginalValue = row[column.ColumnName];
                column.Property.SetValue(this, column.OriginalValue);
            }
        }

        protected IDictionary<string, object> GetAttributesForInsert()
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            foreach (var column in this.columns)
            {
                data[column.ColumnName] = column.Property.GetValue(this);
            }
            return data;
        }

        protected IDictionary<string, object> GetDirtyAttributes()
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            foreach (var column in this.columns)
            {
                object currentValue = column.Property.GetValue(this);
                if (! Object.Equals(column.OriginalValue, currentValue))
                {
                    data[column.ColumnName] = currentValue;
                }
            }
            return data;
        }

        protected void SyncOriginalColumnValues()
        {
            foreach (var column in this.columns)
            {
                column.OriginalValue = column.Property.GetValue(this);
            }
        }

        public bool Save()
        {
            bool saved = (this.exists) ? this.PerformUpdate() : this.PerformInsert();
            this.exists = this.exists || saved;
            this.SyncOriginalColumnValues();
            return saved;
        }

        protected DateTime FreshTimestamp()
        {
            return DateTime.Now;
        }

        public HasOne<TRelated> HasOne<TRelated>(string foreignKey, string localKey) where TRelated : Model, new()
        {
            return new HasOne<TRelated>(this, foreignKey, localKey);
        }

        protected bool PerformInsert()
        {
            var now = this.FreshTimestamp();
            this.createdAtColumn?.Property.SetValue(this, now);
            this.updatedAtColumn?.Property.SetValue(this, now);

            var attributes = this.GetAttributesForInsert();
            foreach (var attr in attributes)
            {
                Console.WriteLine($" Inserted ==> {attr.Key} = {attr.Value}");
            }

            return true;
        }

        protected bool PerformUpdate()
        {
            var dirty = this.GetDirtyAttributes();

            if (dirty.Count == 0) return true;

            if (this.updatedAtColumn != null)
            {
                var now = this.FreshTimestamp();
                this.updatedAtColumn.Property.SetValue(this, now);
                dirty[this.updatedAtColumn.ColumnName] = now;
            }

            foreach (var attr in dirty)
            {
                Console.WriteLine($" Updated ==> {attr.Key} = {attr.Value}");
            }

            return true;
        }
    }
}