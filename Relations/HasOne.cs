using System;
using System.Collections.Generic;

namespace Test
{
    public class HasOne<TRelated> : Relation<TRelated> where TRelated : Model<TRelated>, new()
    {
        private string foreignKey;
        private string localKey;

        public HasOne(ModelQuery<TRelated> query, IModel parent, string foreignKey, string localKey) : base(query, parent)
        {
            this.foreignKey = foreignKey;
            this.localKey = localKey;
        }

        public override bool IsRelatedToMany => false;

        public override TRelated GetResult()
        {
            TRelated related = new TRelated();
            object parentPrimaryKey = this.parent.GetType().GetProperty(this.localKey).GetValue(this.parent);
            return this.query.SelectWhere(this.foreignKey, parentPrimaryKey);
        }

        public override IEnumerable<TRelated> GetResults()
        {
            return new TRelated[] { this.GetResult() };
        }
    }
}