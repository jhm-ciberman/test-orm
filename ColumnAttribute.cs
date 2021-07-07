using System;
using System.Reflection;

namespace Test
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class ColumnAttribute : System.Attribute
    {
        public string Name { get; set; }

        public ColumnAttribute()
        {
            //
        }

        public ColumnAttribute(string name)
        {
            this.Name = name;
        }
    }
}