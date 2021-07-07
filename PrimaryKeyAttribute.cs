using System;

namespace Test
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class PrimaryKeyAttribute : ColumnAttribute
    {
        public PrimaryKeyAttribute() : base()
        {

        }
        
        public PrimaryKeyAttribute(string name) : base(name)
        {
            //
        }
    }
}