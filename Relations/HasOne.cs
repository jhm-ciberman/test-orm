using System;

namespace Test
{
    public class HasOne<TRelated> : Relation<TRelated> where TRelated : Model, new()
    {
        private string foreignKey;
        private string localKey;

        public bool loaded;
        private TRelated cachedModel;

        public HasOne(Model parent, string foreignKey, string localKey) : base(parent)
        {
            this.foreignKey = foreignKey;
            this.localKey = localKey;
        }

        public TRelated GetResult()
        {
            if (! this.loaded) {
                TRelated related = new TRelated();
                object parentPrimaryKey = parent.GetType().GetProperty(this.localKey);

                query = new ModelQuery<TRelated>();
                Console.WriteLine($"SELECT * from {related.GetTableName()} where {this.foreignKey} = {parentPrimaryKey}");
                this.cachedModel = related;
            }

            return this.cachedModel;
        }
    }
}