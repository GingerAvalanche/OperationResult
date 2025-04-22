using System;
using System.Collections.Generic;

namespace OperationResult;

public static class Extensions
{
    public static ErrorStack Context(this string message, string context) => new ErrorStack(message).Context(context);

    public static ErrorStack Context(this List<ErrorStack> errors, string context)
    {
        ErrorStack stack = context;
        stack.AttachAll(errors);
        return stack;
    }

    public static ErrorStack Context(this Exception e, string context) => new ErrorStack(e).Context(context);
}