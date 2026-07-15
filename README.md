# OperationResult
__Rust-style error handling for C#__

Removed most of the upstream documentation because I made it basically a full re-impl
of the Result type, including member functions. Also included a type that serves as
a convenience re-impl of the anyhow crate's context functionality, for simplified
context management in a C# context (i.e. getting the error string returns both the
context and any contained stack trace)
