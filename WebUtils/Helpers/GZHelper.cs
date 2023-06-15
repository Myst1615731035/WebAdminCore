using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.GZip;
using System.Text;

namespace WebUtils
{
    /// <summary>
    /// GZIP压缩方式
    /// </summary>
    public class GZHelper
    {
        /// <summary>
        /// zip压缩-文件方式
        /// </summary>
        /// <param name="inFile">源文件</param>
        /// <param name="outFile">压缩后的文件</param>
        public static void Zip(string inFile, string outFile)
        {
            byte[] dataBuffer = new byte[4096];
            FileHelper.CreateDir(outFile);
            using (Stream s = new GZipOutputStream(File.Create(outFile)))
            {
                using (FileStream fs = File.OpenRead(inFile))
                {
                    StreamUtils.Copy(fs, s, dataBuffer);
                }
            }
        }

        /// <summary>
        /// 解压-文件方式
        /// </summary>
        /// <param name="inFile">压缩包</param>
        /// <param name="outFile">解压后的文件</param>
        public static void UnZip(string inFile, string outFile)
        {
            byte[] dataBuffer = new byte[4096];
            FileHelper.CreateDir(outFile);
            using (Stream s = new GZipInputStream(File.OpenRead(inFile)))
            {
                using (FileStream fs = File.Create(outFile))
                {
                    StreamUtils.Copy(s, fs, dataBuffer);
                }
            }
        }

        /// <summary>
        /// 内容zip压缩
        /// </summary>
        /// <param name="content"></param>
        /// <param name="outFile"></param>
        public static void ZipContent(string content, string outFile)
        {
            byte[] dataBuffer = new byte[4096];
            var bytes = Encoding.UTF8.GetBytes(content);
            FileHelper.CreateDir(outFile);
            using (Stream s = new GZipOutputStream(File.Create(outFile)))
            {
                using (Stream fs = new MemoryStream(bytes))
                {
                    StreamUtils.Copy(fs, s, dataBuffer);
                }
            }
        }
    }
}