using System;

namespace Graph.ArgumentValidator
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class ValidatableAttribute : Attribute
    {
        public ValidatableAttribute()
        {
        }
    }
}
