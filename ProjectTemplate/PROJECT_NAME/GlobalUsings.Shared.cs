global using System;
global using System.Collections.Generic;
global using System.IO;
global using System.Linq;
global using System.Diagnostics.CodeAnalysis;
global using System.Numerics;
global using System.Collections;
global using System.Collections.Specialized;
global using System.Diagnostics;

namespace PROJECT_NAME;
internal static class SelfNote
{
    [DoesNotReturn]
    public static void ThrowNotImplemented() => throw new NotImplementedException();
    [DoesNotReturn]
    public static T ThrowNotImplemented<T>() => throw new NotImplementedException();
    /// <summary>
    /// Notes that the following code has the code that is not allowed in UWP certification.
    /// </summary>
    public static void HasDisallowedPInvoke() { }
    public static void DebugBreakOnShift()
    {
#if DEBUG
        if (WinWrapper.Input.Keyboard.IsShiftDown)
            Debugger.Break();
#endif
    }
}