// -------------------------------------------------------------------------------------------------
// This type exists solely to support modern C# language features (C# 9.0+) when targeting
// older frameworks such as .NET Framework 4.6.2.
//
// C# records and `init`-only setters rely on a compiler-known marker type called
// `System.Runtime.CompilerServices.IsExternalInit`. In newer frameworks, this type is
// provided by the runtime itself. However, .NET Framework versions prior to 4.7.2 do not
// include it, which causes compilation errors when using records or `init` properties.
//
// The C# compiler only requires the presence of this type at compile time; it has no
// runtime behavior and contains no logic. Defining it here is a safe, widely-used shim
// that enables modern language features without changing the target framework.
//
// IMPORTANT:
// - This file must NOT be removed while using records or `init` setters.
// - Do NOT add any members or logic to this type.
// - Do NOT reference this type directly in code.
// -------------------------------------------------------------------------------------------------
namespace System.Runtime.CompilerServices
{
    using System.ComponentModel;

    [EditorBrowsable(EditorBrowsableState.Never)]
    internal static class IsExternalInit
    {
    }
}
