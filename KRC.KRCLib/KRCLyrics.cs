using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace KRCLib;

/// <summary>
/// KRC歌词文件
/// </summary>
public class KRCLyrics
{
    public List<KRCLyricsLine> Lines => _lines;

    /// <summary>
    /// 歌词文本
    /// </summary>
    public string KRCString { get; set; }

    /// <summary>
    /// ID （总是$00000000，意义未知）
    /// </summary>
    public string ID { get; set; }

    /// <summary>
    /// 艺术家
    /// </summary>
    public string Ar { get; set; }

    /// <summary>
    ///
    /// </summary>
    public string Al { get; set; }

    /// <summary>
    /// 标题
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// 歌词文件作者
    /// </summary>
    public string By { get; set; }

    /// <summary>
    /// 歌曲文件Hash
    /// </summary>
    public string Hash { get; set; }

    /// <summary>
    /// 总时长
    /// </summary>
    public TimeSpan Total
    {
        get
        {
            // 计算总时间=所有行时间
            var sum = Lines.Select(x => x.LineDuring.TotalMilliseconds).Sum();
            return TimeSpan.FromMilliseconds(sum);
        }
    }

    /// <summary>
    /// 偏移
    /// </summary>
    public TimeSpan Offset { get; set; }

    private readonly List<KRCLyricsLine> _lines = [];
    private readonly List<Tuple<Regex, Action<string>>> _properties;
    private readonly Regex _regGetValueFromKeyValuePair = new(@"\[(.*):(.*)\]");

    /// <summary>
    /// 默认构造
    /// </summary>
    public KRCLyrics()
    {
        //this.Total = TimeSpan.Zero;
        this.Offset = TimeSpan.Zero;

        this._properties =
        [
            new Tuple<Regex, Action<string>>(new Regex("\\[id:[^\\]]+\\]"), (s) => { ID = s; }),
            new Tuple<Regex, Action<string>>(new Regex("\\[al:[^\\n]+\\n"), (s) => { Al = s; }),
            new Tuple<Regex, Action<string>>(new Regex("\\[ar:[^\\]]+\\]"), (s) => { Ar = s; }),
            new Tuple<Regex, Action<string>>(new Regex("\\[ti:[^\\]]+\\]"), (s) => { Title = s; }),
            new Tuple<Regex, Action<string>>(new Regex("\\[hash:[^\\n]+\\n"), (s) => { Hash = s; }),
            new Tuple<Regex, Action<string>>(new Regex("\\[by:[^\\n]+\\n"), (s) => { By = s; }),
            new Tuple<Regex, Action<string>>(new Regex("\\[total:[^\\n]+\\n"), (s) =>
            {
				// Total = TimeSpan.FromMilliseconds(double.Parse(s));
			}),
            new Tuple<Regex, Action<string>>(new Regex("\\[offset:[^\\n]+\\n"), (s) =>
            {
                Offset = TimeSpan.FromMilliseconds(double.Parse(s));
            }),
        ];
    }

    /// <summary>
    /// 构造
    /// </summary>
    /// <param name="krcstring">KRC字符文本</param>
    private KRCLyrics(string krcstring) : this()
    {
        KRCString = krcstring;
        LoadProperties();
        LoadLines();
    }

    public override string ToString()
    {
        return SaveToString();
    }

    /// <summary>
    /// 加载KRC属性
    /// </summary>
    private void LoadProperties()
    {
        foreach (var prop in _properties)
        {
            var m = prop.Item1.Match(KRCString);
            if (m.Success)
            {
                var mm = _regGetValueFromKeyValuePair.Match(m.Value);

                if (mm.Success && mm.Groups.Count == 3)
                {
                    prop.Item2(mm.Groups[2].Value);
                }
            }
        }
    }

    /// <summary>
    /// 加载KRC所有行数据
    /// </summary>
    private void LoadLines()
    {
        var linesMachCollection = Regex.Matches(KRCString, @"\[\d{1,}[^\n]+\n");
        foreach (Match m in linesMachCollection)
        {
            Lines.Add(new KRCLyricsLine(m.Value));
        }
    }

    /// <summary>
    /// 保存KRC为字符串
    /// </summary>
    public string SaveToString()
    {
        var sb = new StringBuilder();
        sb.AppendLine(string.Format("[id:{0}]", ID));

        if (!string.IsNullOrEmpty(Al))
        {
            sb.AppendLine(string.Format("[al:{0}]", Al));
        }

        if (!string.IsNullOrEmpty(Ar))
        {
            sb.AppendLine(string.Format("[ar:{0}]", Ar));
        }

        if (!string.IsNullOrEmpty(Title))
        {
            sb.AppendLine(string.Format("[ti:{0}]", Title));
        }

        if (!string.IsNullOrEmpty(Hash))
        {
            sb.AppendLine(string.Format("[hash:{0}]", Hash));
        }

        if (!string.IsNullOrEmpty(By))
        {
            sb.AppendLine(string.Format("[by:{0}]", By));
        }

        if (Total != TimeSpan.Zero)
        {
            sb.AppendLine(string.Format("[total:{0}]", Total.TotalMilliseconds));
        }

        if (Offset != TimeSpan.Zero)
        {
            sb.AppendLine(string.Format("[offset:{0}]", Offset.TotalMilliseconds));
        }

        foreach (var line in Lines)
        {
            sb.AppendLine(line.KRCLineString);
        }

        return sb.ToString();
    }

    /// <summary>
    /// 保存到文件
    /// </summary>
    /// <param name="outputFilePath"></param>
    public void SaveToFile(string outputFilePath)
    {
        var bytes = KRCFile.EncodeStringToBytes(SaveToString());

        File.WriteAllBytes(outputFilePath, bytes);
    }

    /// <summary>
    /// 转换为ASS字幕格式字符串
    /// </summary>
    /// <returns>ASS字幕内容</returns>
    public string SaveToAss()
    {
        var sb = new StringBuilder();

        // ASS Script Info
        sb.AppendLine("[Script Info]");
        sb.AppendLine("; Script generated by KRCLib");
        sb.AppendLine("ScriptType: v4.00+");
        sb.AppendLine("PlayResX: 1280");
        sb.AppendLine("PlayResY: 720");
        sb.AppendLine("Timer: 100.0000");
        sb.AppendLine();

        // V4+ Styles
        sb.AppendLine("[V4+ Styles]");
        sb.AppendLine("Format: Name, Fontname, Fontsize, PrimaryColour, SecondaryColour, OutlineColour, BackColour, Bold, Italic, Underline, StrikeOut, ScaleX, ScaleY, Spacing, Angle, BorderStyle, Outline, Shadow, Alignment, MarginL, MarginR, MarginV, Encoding");
        sb.AppendLine("Style: Default,Arial,48,&H00FFFFFF,&H000000FF,&H00000000,&H00000000,0,0,0,0,100,100,0,0,1,2,1,2,10,10,10,1");
        sb.AppendLine();

        // Events
        sb.AppendLine("[Events]");
        sb.AppendLine("Format: Layer, Start, End, Style, Name, MarginL, MarginR, MarginV, Effect, Text");

        foreach (var line in Lines)
        {
            var lineStart = line.LineStart + Offset;
            var lineEnd = lineStart + line.LineDuring;

            var textPart = new StringBuilder();
            foreach (var ch in line.Chars)
            {
                var centiseconds = (long)Math.Round(ch.CharDuring.TotalMilliseconds / 10.0);
                textPart.Append(string.Format(@"{{\k{0}}}{1}", centiseconds, ch.Char));
            }

            sb.AppendLine(string.Format("Dialogue: 0,{0},{1},Default,,0,0,0,,{2}",
                FormatAssTime(lineStart), FormatAssTime(lineEnd), textPart));
        }

        return sb.ToString();
    }

    /// <summary>
    /// 保存为ASS字幕文件
    /// </summary>
    /// <param name="outputFilePath">输出文件路径</param>
    public void SaveToAssFile(string outputFilePath)
    {
        File.WriteAllText(outputFilePath, SaveToAss(), Encoding.UTF8);
    }

    /// <summary>
    /// 格式化时间为ASS时间格式 h:mm:ss.cc
    /// </summary>
    /// <param name="time">时间</param>
    /// <returns>格式化后的时间字符串</returns>
    private static string FormatAssTime(TimeSpan time)
    {
        int h = (int)time.TotalHours;
        int m = time.Minutes;
        int s = time.Seconds;
        int cs = (int)Math.Round(time.TotalMilliseconds / 10.0) % 100;
        return string.Format("{0}:{1:D2}:{2:D2}.{3:D2}", h, m, s, cs);
    }

    /// <summary>
    /// 从文件加载
    /// </summary>
    /// <param name="inputFilePath"></param>
    /// <returns></returns>
    public static KRCLyrics LoadFromFile(string inputFilePath)
    {
        var str = KRCFile.DecodeFileToString(inputFilePath);

        return LoadFromString(str);
    }

    /// <summary>
    /// 从流加载
    /// </summary>
    /// <param name="inputStream"></param>
    /// <returns></returns>
    public static KRCLyrics LoadFromStream(Stream inputStream)
    {
        var str = KRCFile.DecodeStreamToString(inputStream);
        return LoadFromString(str);
    }

    /// <summary>
    /// 从文本加载
    /// </summary>
    /// <param name="krcstring"></param>
    /// <returns></returns>
    public static KRCLyrics LoadFromString(string krcstring)
    {
        var aa = new KRCLyrics(krcstring);
        return aa;
    }
}
