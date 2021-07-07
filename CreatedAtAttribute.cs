using System;

namespace Test
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class CreatedAtAttribute : ColumnAttribute
    {
        public CreatedAtAttribute() : base()
        {

        }
        
        public CreatedAtAttribute(string name) : base(name)
        {
            //
        }
    }
}