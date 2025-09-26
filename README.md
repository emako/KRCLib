KRCLib
===

[![GitHub license](https://img.shields.io/github/license/emako/KRCLib)](https://github.com/emako/KRCLib/blob/master/LICENSE) [![NuGet](https://img.shields.io/nuget/v/KRCLib.svg)](https://nuget.org/packages/KRCLib) [![Actions](https://github.com/emako/KRCLib/actions/workflows/library.nuget.yml/badge.svg)](https://github.com/emako/KRCLib/actions/workflows/library.nuget.yml)

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
