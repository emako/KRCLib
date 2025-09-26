using System;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace KRC.KRCLib;

/// <summary>
/// KRC文件行字符
/// </summary>
[DebuggerDisplay("{DebuggerDisplay}")]
public class KRCLyricsChar
{
    /// <summary>
    /// 字符
    /// </summary>
    public char Char { get; set; }

    /// <summary>
    /// 字符KRC字符串
    /// </summary>
    public string KRCCharString
        => string.Format(@"<{0},{1},{2}>{3}", CharStart.TotalMilliseconds, CharDuring.TotalMilliseconds, 0, Char);

    /// <summary>
    /// 字符起始时间(计算时加上字符所属行的起始时间)
    /// </summary>
    public TimeSpan CharStart { get; set; }

    /// <summary>
    /// 字符时长
    /// </summary>
    public TimeSpan CharDuring { get; set; }

    public KRCLyricsChar()
    {
        CharStart = TimeSpan.Zero;
        CharDuring = TimeSpan.Zero;
    }

    public KRCLyricsChar(string krcCharString) : this()
    {
        var chars = Regex.Match(krcCharString, @"<(\d+),(\d+),(\d+)>(.?)");

        if (chars.Success)
        {
            if (chars.Groups.Count >= 4)
            {
                var charstart = chars.Groups[1].Value;
                var charduring = chars.Groups[2].Value;
                var unknowAlwaysZero = chars.Groups[3].Value;

                CharStart = TimeSpan.FromMilliseconds(double.Parse(charstart));
                CharDuring = TimeSpan.FromMilliseconds(double.Parse(charduring));

                if (chars.Groups.Count >= 5)
                {
                    var charchar = chars.Groups[4].Value;
                    Char = char.Parse(charchar);
                }
                else
                {
                    Char = char.Parse(" ");
                }
            }
        }
    }

    public string DebuggerDisplay
        => string.Format(@"{0:hh\:mm\:ss\.fff} {1:hh\:mm\:ss\.fff} {2}", CharStart, CharDuring, Char);
}
