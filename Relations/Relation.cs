namespace Test
{
    public abstract class Relation<T> where T : Model, new()
    {
        protected ModelQuery<T> query = null;

        protected Model parent;

        public Relation(Model parent)
        {
            this.parent = parent;
        }
    }
}