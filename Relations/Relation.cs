using System.Collections.Generic;

namespace Test
{
    public abstract class Relation<TRelated> where TRelated : Model<TRelated>, new()
    {
        protected ModelQuery<TRelated> query;

        protected IModel parent;

        public Relation(ModelQuery<TRelated> query, IModel parent)
        {
            this.query = query;
            this.parent = parent;
        }

        public abstract bool IsRelatedToMany { get; }
        public abstract IEnumerable<TRelated> GetResults();
        public abstract TRelated GetResult();
    }
}