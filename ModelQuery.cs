using System;

namespace Test
{
    public class ModelQuery<TModel> where TModel : Model<TModel>, new()
    {
        private string Table;

        public ModelQuery()
        {
            this.Table = Model<TModel>.GetPrototype().TableName;
        }

        public TModel NewModel()
        {
            return new TModel();
        }

        internal TModel SelectWhere(string columnName, object value)
        {
            var data = DB.Select(this.Table, columnName, value);
            TModel model = this.NewModel();
            model.Fill(data);
            return model;
        }
    }
}