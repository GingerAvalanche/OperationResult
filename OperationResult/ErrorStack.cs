using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OperationResult;

[Serializable]
public class ErrorStack : Exception
{
    private LinkedList<string> context = [];
    private string? stackTrace;
    public string? Stack => stackTrace;
    
    private ErrorStack() {}

    public ErrorStack(string message, string? callSite = null) : base(message)
    {
        stackTrace = callSite;
    }
    public static implicit operator ErrorStack(string message) => new(message);

    public ErrorStack(string message, Exception innerException, string? callSite = null) : base(message, innerException)
    {
        stackTrace = callSite ?? innerException.StackTrace;
    }

    public ErrorStack Context(string message)
    {
        context.AddFirst(message);
        return this;
    }

    public override string ToString()
    {
        StringBuilder str = new();
        if (InnerException != null)
        {
            str.Append(InnerException.GetType().ToString().Split(".").Last())
                .Append(": ")
                .Append(InnerException.Message);
        }
        else if (Message != string.Empty)
        {
            str.Append(Message);
        }
        else if (context.Last != null)
        {
            str.Append(context.Last.Value);
        }

        str.AppendLine().AppendLine().Append("Caused by:").AppendLine();

        int i = -1;
        foreach (var value in context)
        {
            str.Append("  ").Append(++i).Append(". ").Append(value).AppendLine();
        }

        if (Message != string.Empty)
        {
            str.Append("  ").Append(++i).Append(". ").Append(Message);
        }
        if (InnerException != null)
        {
            str.AppendLine()
                .Append("  ")
                .Append(++i)
                .Append(". ")
                .Append(InnerException.GetType().ToString().Split(".").Last())
                .Append(": ")
                .Append(InnerException.Message);
        }
        return str.ToString();
    }
}