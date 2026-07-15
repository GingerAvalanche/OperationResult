using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OperationResult;

public class ErrorStack
{
    private string _message;
    private ErrorStack? _child;
    
    private ErrorStack() {}

    public ErrorStack(string message)
    {
        _message = message;
    }
    public static implicit operator ErrorStack(string message) => new(message);

    public ErrorStack(Exception e)
    {
        _message = $"{e.GetType().ToString().Split(".").Last()}: {e.Message}";
    }
    public static implicit operator ErrorStack(Exception e) => new(e);

    public void Attach(ErrorStack child)
    {
        _child = child;
    }

    public void AttachAll(IEnumerable<ErrorStack> children)
    {
        ErrorStack parent = this;
        foreach (var child in children)
        {
            parent._child = child;
            parent = child;
        }
    }

    public IEnumerable<string> GetStackMessages()
    {
        ErrorStack? child = _child;
        while (child != null)
        {
            yield return child._message;
            child = child._child;
        }
    }

    public override string ToString()
    {
        StringBuilder str = new();
        str.AppendLine(_message)
            .AppendLine()
            .AppendLine()
            .AppendLine("Caused by:")
            .AppendLine($"  0. {_message}");
        int i = 0;
        foreach (var value in GetStackMessages())
        {
            str.AppendLine($"  {++i}. {value}");
        }
        return str.ToString();
    }

    public ErrorStack Context(string message)
    {
        ErrorStack parent = message;
        parent.Attach(this);
        return parent;
    }
}