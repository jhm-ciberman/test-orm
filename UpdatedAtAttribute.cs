using System;

namespace Test
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class UpdatedAtAttribute : ColumnAttribute
    {
        public UpdatedAtAttribute() : base()
        {

        }
        
        public UpdatedAtAttribute(string name) : base(name)
        {
            //
        }
    }
}