using System;


namespace MonoInjection
{
    /// <summary>
    /// Attribute to mark fields for injection
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class InjectAttribute : Attribute { }
}
