using System;

namespace Test
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class PrimaryKeyAttribute : ColumnAttribute
    {
        public bool AutoIncrement { get; set; } = true;

        public PrimaryKeyAttribute() : base()
        {

        }
        
        public PrimaryKeyAttribute(string name) : base(name)
        {
            //
        }
    }
}