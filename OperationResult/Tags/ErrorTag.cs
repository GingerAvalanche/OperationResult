using System;

namespace OperationResult.Tags
{
    public struct ErrorTag { }

    public struct ErrorTag<E>
    {
        internal readonly E Error;

        internal ErrorTag(E error)
        {
            Error = error;
        }
    }
}
