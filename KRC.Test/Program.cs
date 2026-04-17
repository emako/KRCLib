using KRCLib;
using System;
using System.IO;

namespace KRC.Test;

internal class Program
{
    public static void Main(string[] args)
    {
        {
            KRCLyrics krc = KRCLyrics.LoadFromFile("test.krc");
            Console.WriteLine("解码 [{0}] 完毕。", "test.krc");
            Console.WriteLine(krc.SaveToString());
            krc.SaveToFile("test_out.krc");
            Console.WriteLine("另存为 [{0}] 完毕。", "test_out.krc");
        }

        // ---

        {
            KRCLyrics krc = KRCLyrics.LoadFromStream(new FileStream("test.krc", FileMode.Open));
            Console.WriteLine("解码 [{0}] 完毕。", "test.krc");
            Console.WriteLine(krc.SaveToString());
            krc.SaveToFile("test_out.krc");
            Console.WriteLine("另存为 [{0}] 完毕。", "test_out.krc");
        }

        // --- KRC转ASS ---

        {
            KRCLyrics krc = KRCLyrics.LoadFromFile("test.krc");
            Console.WriteLine("转换 [{0}] 为ASS格式：", "test.krc");
            Console.WriteLine(krc.SaveToAss());
            krc.SaveToAssFile("test_out.ass");
            Console.WriteLine("另存为 [{0}] 完毕。", "test_out.ass");
            Console.ReadLine();
        }
    }
}
