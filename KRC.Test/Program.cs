using KRCLib;
using System;

namespace KRC.Test;

internal class Program
{
    public static void Main(string[] args)
    {
        KRCLyrics krc = KRCLyrics.LoadFromFile("test.krc");
        Console.WriteLine("解码 [{0}] 完毕。", "test.krc");
        Console.WriteLine(krc.SaveToString());
        krc.SaveToFile("test_out.krc");
        Console.WriteLine("另存为 [{0}] 完毕。", "test_out.krc");
        Console.ReadLine();
    }
}
