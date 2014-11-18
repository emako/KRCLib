using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using KRC.KRCLib;

namespace KRC.Test
{
	class Program
	{
		static void Main(string[] args)
		{
			string inputFile = @"杨钰莹.桃花运-b0c4014bd991a6a637445defa56822f9.krc";
			string outputFile = @"123.krc";
			KRCLyrics krc = KRCLyrics.LoadFromFile(inputFile);
			Console.WriteLine("解码 [{0}] 完毕。", inputFile);
			krc.SaveToFile(outputFile);
			Console.WriteLine("另存为 [{0}] 完毕。", outputFile);
			Console.ReadLine();
		}
	}
}
