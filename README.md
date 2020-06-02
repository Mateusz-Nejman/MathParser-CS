# MathParser-CS
Simple Mathematical string parser.

## Installing
```csharp
Install-Package Nejman.MathParser
```

## Using

```csharp
string mathString = $"2+(3+8)√(2+2)*5^4^5";
MathBuffer buffer = new MathBuffer(mathString);

double val = buffer.Eval();
```
