# MathParser-CS
Simple Mathematical string parser.

```csharp
string mathString = $"2+(3+8)√(2+2)*5^4^5";
MathBuffer buffer = new MathBuffer(mathString);

double val = buffer.Eval();
```
