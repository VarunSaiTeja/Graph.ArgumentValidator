using System;

namespace Graph.ArgumentValidator
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = true, AllowMultiple = false)]
    public class ValidatableAttribute : Attribute
    {
        public ValidatableAttribute()
        {
        }
    }
}
