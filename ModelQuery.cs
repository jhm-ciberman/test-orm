namespace Test
{
    public class ModelQuery<TModel> where TModel : Model, new()
    {

        public ModelQuery()
        {
            //
        }

        public TModel NewModel()
        {
            return new TModel();
        }
    }
}