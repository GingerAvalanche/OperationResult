using System;
using System.Collections.Generic;

namespace OperationResult;

public class ErrorStack
{
    private string? _error;
    private List<ErrorStack>? _children;
    
    public ErrorStack() {}

    public ErrorStack(string message)
    {
        _error = message;
    }
    public static implicit operator ErrorStack(string message) => new(message);

    public ErrorStack(Exception e)
    {
        _error = e.ToString();
    }
    public static implicit operator ErrorStack(Exception e) => new(e);

    public void Attach(ErrorStack child)
    {
        _children ??= new List<ErrorStack>();
        _children.Add(child);
    }

    public void AttachAll(IEnumerable<ErrorStack> children)
    {
        _children ??= new List<ErrorStack>();
        _children.AddRange(children);
    }

    public IEnumerable<string> GetAllErrors()
    {
        yield return _error ?? "No context given";
        if (_children == null) yield break;
        foreach (var t in _children)
        {
            foreach (var message in t.GetAllErrors())
                yield return $"  {message}";
        }
    }

    public override string ToString()
    {
        return string.Join('\n', GetAllErrors());
    }

    public ErrorStack Context(string message)
    {
        if (_error == null)
        {
            _error = message;
            return this;
        }
        ErrorStack parent = message;
        parent.Attach(this);
        return parent;
    }
}