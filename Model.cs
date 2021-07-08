using System;
using System.Collections.Generic;
using System.Data;

namespace Test
{
    public abstract class Model<TModel> : IModel where TModel : Model<TModel>, new()
    {
        private static ModelPrototype cached = null;

        internal static ModelPrototype GetPrototype()
        {
            if (Model<TModel>.cached != null) 
                return Model<TModel>.cached;

            Model<TModel>.cached = new ModelPrototype(typeof(TModel));

            return Model<TModel>.cached;
        }


        private ModelPrototype prototype;
        private bool exists = false;
        private object[] original;

        public Model()
        {
            this.prototype = GetPrototype();
            this.original = new object[this.prototype.ColumnsCount];
        }

        public virtual string GetTableName()
        {
            return this.prototype.TableName;
        }

        public void Fill(IDictionary<string, object> row)
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

        public static ModelQuery<TModel> NewQuery()
        {
            return new ModelQuery<TModel>();
        }

        public HasOne<TRelated> HasOne<TRelated>(string foreignKey, string localKey) where TRelated : Model<TRelated>, new()
        {
            return new HasOne<TRelated>(Model<TRelated>.NewQuery(), this, foreignKey, localKey);
        }

        private Dictionary<object, object> relations = new Dictionary<object, object>();

        protected TRelated One<TRelated>(string relationName) where TRelated : Model<TRelated>, new()
        {
            if (! this.relations.TryGetValue(relationName, out object result))
            {
                result = this.GetRelationMethod<TRelated>(relationName).GetResult();
                this.relations[relationName] = result;
            }

            return (TRelated) result;
        }

        private Relation<TRelated> GetRelationMethod<TRelated>(string relationName) where TRelated : Model<TRelated>, new()
        {
            return (Relation<TRelated>) this.prototype.GetRelation(relationName).Invoke(this, null);
        }

        protected IEnumerable<TRelated> Many<TRelated>(string relationName) where TRelated : Model<TRelated>, new()
        {
            
            if (! this.relations.TryGetValue(relationName, out object result))
            {
                result = this.GetRelationMethod<TRelated>(relationName).GetResults();
                this.relations[relationName] = result;
            }

            return (IEnumerable<TRelated>) result;
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