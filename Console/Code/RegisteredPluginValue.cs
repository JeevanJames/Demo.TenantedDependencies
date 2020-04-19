using System;
using System.Diagnostics;

namespace Console.Code
{
    [DebuggerDisplay("{Tenant} ==> {ImplType.FullName}")]
    public sealed class RegisteredPluginValue
    {
        public RegisteredPluginValue(string tenant, Type implType)
        {
            Tenant = tenant;
            ImplType = implType;
        }

        public string Tenant { get; }

        public Type ImplType { get; }
    }
}
 