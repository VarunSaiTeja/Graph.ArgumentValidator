using System;

namespace Graph.ArgumentValidator
{
    [AttributeUsage(AttributeTargets.Parameter, Inherited = true, AllowMultiple = false)]
    public class ValidatableAttribute : Attribute
    {
        public ValidatableAttribute()
        {
        }
    }
}
