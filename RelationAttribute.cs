using System;

namespace Test
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class RelationAttribute : System.Attribute
    {
        public string Name { get; set; }

        public RelationAttribute()
        {
            //
        }

        public RelationAttribute(string name)
        {
            this.Name = name;
        }
    }
}