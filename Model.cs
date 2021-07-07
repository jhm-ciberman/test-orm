using System;
using System.Collections.Generic;
using System.Data;

namespace Test
{
    public abstract class Model
    {
        private bool exists = false;

        private ModelPrototype prototype;

        private object[] original;

        public Model()
        {
            this.prototype = ModelPrototype.Get(this);
            this.original = new object[this.prototype.ColumnsCount];
        }

        public virtual string GetTableName()
        {
            return this.GetType().Name;
        }

        public void Fill(DataRow row)
        {
            foreach (var column in this.prototype.Columns)
            {
                object value = row[column.ColumnName];
                this.original[column.Index] = value;
                column.Property.SetValue(this, value);
            }
        }

        protected IDictionary<string, object> GetAttributesForInsert()
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            foreach (var column in this.prototype.Columns)
            {
                data[column.ColumnName] = column.Property.GetValue(this);
            }
            return data;
        }

        protected IDictionary<string, object> GetDirtyAttributes()
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            foreach (var column in this.prototype.Columns)
            {
                object currentValue = column.Property.GetValue(this);
                object originalValue = this.original[column.Index];
                if (! Object.Equals(originalValue, currentValue))
                {
                    data[column.ColumnName] = currentValue;
                }
            }
            return data;
        }

        protected void SyncOriginalColumnValues()
        {
            foreach (var column in this.prototype.Columns)
            {
                Console.WriteLine($"{column.Index} of {this.original.Length}");
                this.original[column.Index] = column.Property.GetValue(this);
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
            this.prototype.CreatedAtColumn?.Property.SetValue(this, now);
            this.prototype.UpdatedAtColumn?.Property.SetValue(this, now);

            var attributes = this.GetAttributesForInsert();
            var tableName = this.GetTableName();

            if (this.prototype.AutoIncrements)
            {
                object id = DB.InsertGetId(tableName, attributes);
                this.prototype.PrimaryKey.Property.SetValue(this, id);
            }
            else
            {
                DB.Insert(tableName, attributes);
            }

            return true;
        }

        protected bool PerformUpdate()
        {
            var dirty = this.GetDirtyAttributes();

            if (dirty.Count == 0) return true;

            if (this.prototype.UpdatedAtColumn != null)
            {
                var now = this.FreshTimestamp();
                this.prototype.UpdatedAtColumn.Property.SetValue(this, now);
                dirty[this.prototype.UpdatedAtColumn.ColumnName] = now;
            }

            var id = this.prototype.PrimaryKey.Property.GetValue(this);
            var tableName = this.GetTableName();
            DB.Update(tableName, id, dirty);

            return true;
        }
    }
}