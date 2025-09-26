krc
===

KRC file format parse

酷狗KRC文件解析

## 示例 Example

```csharp
using KRC.KRCLib;
using System;

class Program
{
    static void Main(string[] args)
    {
        KRCLyrics krc = KRCLyrics.LoadFromFile("test.krc");
        Console.WriteLine($"test.krc Loaded");
        krc.SaveToFile("test_out.krc");
        Console.WriteLine($"test_out.krc Saved");
    }
}
```
