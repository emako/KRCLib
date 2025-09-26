using ICSharpCode.SharpZipLib.Zip.Compression;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using System.IO;
using System.Linq;
using System.Text;

namespace KRCLib;

public static class KRCFile
{
    /// <summary>
    /// 异或加密 密钥
    /// </summary>
    public static readonly char[] KRCFileXorKey = ['@', 'G', 'a', 'w', '^', '2', 't', 'G', 'Q', '6', '1', '-', 'Î', 'Ò', 'n', 'i'];

    /// <summary>
    /// KRC 文件头
    /// </summary>
    public static readonly char[] KRCFileHead = ['k', 'r', 'c', '1'];

    /// <summary>
    /// KRC 文件头的字节
    /// </summary>
    public static readonly byte[] KRCFileHeadBytes = [0x6B, 0x72, 0x63, 0x31];

    /// <summary>
    /// 解码
    /// </summary>
    public static string DecodeFileToString(string krcFilePath)
    {
        // krc1
        var headBytes = new byte[4];
        byte[] encodedBytes;
        byte[] zipedBytes;

        using (var krcfs = new FileStream(krcFilePath, FileMode.Open))
        {
            encodedBytes = new byte[krcfs.Length - headBytes.Length];
            zipedBytes = new byte[krcfs.Length - headBytes.Length];

            // 读文件头标记
            _ = krcfs.Read(headBytes, 0, headBytes.Length);

            // 读XOR加密的内容
            _ = krcfs.Read(encodedBytes, 0, encodedBytes.Length);

            // 关闭文件
            krcfs.Close();
        }

        for (var i = 0; i < encodedBytes.Length; i++)
        {
            zipedBytes[i] = (byte)(encodedBytes[i] ^ KRCFileXorKey[i % 16]);
        }

        // 前面3字节是 UTF-8 的 BOM
        var unzipedBytes = Decompress(zipedBytes);

        // 编码器带有BOM输出时多了3字节，所以跳过开头的3字节bom
        var text = RemoveBom(Encoding.UTF8.GetString(unzipedBytes));

        return text;
    }

    /// <summary>
    /// 编码到字节数组
    /// </summary>
    /// <param name="inText"></param>
    /// <returns></returns>
    public static byte[] EncodeStringToBytes(string inText)
    {
        // 用默认的，编码时带有UTF-8的BOM
        byte[] inbytes = Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(inText)).ToArray();

        byte[] zipedBytes = Compress(inbytes);

        int encodedBytesLength = zipedBytes.Length;

        var encodedBytes = new byte[zipedBytes.Length];

        for (int i = 0; i < encodedBytesLength; i++)
        {
            int l = i % 16;

            encodedBytes[i] = (byte)(zipedBytes[i] ^ KRCFileXorKey[l]);
        }

        byte[] byets = null;

        using (var ms = new MemoryStream())
        {
            ms.Write(KRCFileHeadBytes, 0, KRCFileHeadBytes.Length);
            ms.Write(encodedBytes, 0, encodedBytes.Length);
            ms.Flush();
            byets = ms.ToArray();
        }

        return byets;
    }

    /// <summary>
    /// 移除UTF-8 BOM
    /// </summary>
    /// <param name="p"></param>
    /// <returns></returns>
    private static string RemoveBom(string p)
    {
        string bomMarkUtf8 = Encoding.UTF8.GetString(Encoding.UTF8.GetPreamble());
        if (p.StartsWith(bomMarkUtf8))
            p = p.Remove(0, bomMarkUtf8.Length);
        return p.Replace("\0", "");
    }

    /// <summary>
    /// 解码流
    /// </summary>
    /// <param name="inputStream"></param>
    /// <returns></returns>
    public static string DecodeStreamToString(Stream inputStream)
    {
        var headBytes = new byte[4];
        byte[] encodedBytes;
        byte[] zipedBytes;

        // 读文件头标记
        _ = inputStream.Read(headBytes, 0, headBytes.Length);

        // 读XOR加密的内容
        encodedBytes = new byte[inputStream.Length - headBytes.Length];
        _ = inputStream.Read(encodedBytes, 0, encodedBytes.Length);

        zipedBytes = new byte[encodedBytes.Length];
        for (var i = 0; i < encodedBytes.Length; i++)
        {
            zipedBytes[i] = (byte)(encodedBytes[i] ^ KRCFileXorKey[i % 16]);
        }

        var unzipedBytes = Decompress(zipedBytes);
        var text = RemoveBom(Encoding.UTF8.GetString(unzipedBytes));
        return text;
    }

    #region 压缩 解压缩

    private static byte[] Compress(byte[] pBytes)
    {
        byte[] outdata = null;
        using (var mMemory = new MemoryStream(pBytes))
        using (var mStream = new DeflaterOutputStream(mMemory, new Deflater(Deflater.DEFAULT_COMPRESSION), 131072))
        {
            mStream.Write(pBytes, 0, pBytes.Length);
            mStream.Flush();
            mMemory.Flush();
            outdata = mMemory.ToArray();
        }
        return outdata;
    }

    /// <summary>
    /// 解压缩
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    private static byte[] Decompress(byte[] data)
    {
        byte[] outdata = null;
        using (var ms = new MemoryStream())
        using (var inputStream = new InflaterInputStream(new MemoryStream(data), new Inflater(false)))
        {
            inputStream.CopyTo(ms);
            ms.Flush();

            outdata = ms.ToArray();
            ms.Close();
        }
        return outdata;
    }

    #endregion 压缩 解压缩
}
