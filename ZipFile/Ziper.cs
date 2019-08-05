using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ICSharpCode.SharpZipLib.Checksums;
using ICSharpCode.SharpZipLib.Zip;
using System.IO;

namespace ZipFile
{
    public class Ziper
    {
        /// <summary>
        /// 压缩文件（支持多个文件打包压缩）
        /// </summary>
        /// <param name="fileNames">文件路径集合</param>
        /// <param name="level">压缩级别</param>
        /// <returns>压缩后二进制字节数组</returns>
        public static byte[] PackFiles(List<string> fileNames, int level = 1)
        {
            MemoryStream ms = null;
            ZipOutputStream s = null;
            ZipEntry entry = null;
            FileStream fs = null;
            Crc32 crc = new Crc32();
            try
            {
                ms = new MemoryStream();
                s = new ZipOutputStream(ms);
                s.SetLevel(level);

                foreach (string fileName in fileNames)
                {
                    fs = File.OpenRead(fileName);
                    byte[] buffer = new byte[fs.Length];
                    fs.Read(buffer, 0, buffer.Length);

                    entry = new ZipEntry(Path.GetFileName(fileName));
                    entry.IsUnicodeText = true;
                    entry.DateTime = DateTime.Now;
                    entry.Size = fs.Length;
                    fs.Close();
                    crc.Reset();
                    crc.Update(buffer);
                    entry.Crc = crc.Value;
                    s.PutNextEntry(entry);
                    s.Write(buffer, 0, buffer.Length);
                }

                s.Finish();
                s.Close();

                return ms.ToArray();
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                if (fs != null)
                {
                    fs.Close();
                    fs.Dispose();
                    fs = null;
                }
                if (ms != null)
                {
                    ms.Close();
                    ms.Dispose();
                    ms = null;
                }
                if (s != null)
                {
                    s.Close();
                    s.Dispose();
                    s = null;
                }
                if (entry != null)
                {
                    entry = null;
                }
                GC.Collect();
                GC.Collect(1);
            }
        }
        
        /// <summary>
        /// 解压缩文件（支持解压多个文件）
        /// </summary>
        /// <param name="bytes">压缩文件字节数组</param>
        /// <param name="dirPath">待解压的目录</param>
        /// <returns>成功或失败</returns>
        public static bool UnpackFiles(byte[] bytes, string dirPath)
        {
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }
            ZipInputStream s = null;
            ZipEntry theEntry = null;
            string fileName;
            FileStream streamWriter = null;
            try
            {
                s = new ZipInputStream(new MemoryStream(bytes));
                while ((theEntry = s.GetNextEntry()) != null)
                {
                    if (theEntry.Name != String.Empty)
                    {
                        fileName = Path.Combine(dirPath, theEntry.Name);
                        streamWriter = File.Create(fileName);
                        int size = 2048 * 10;
                        byte[] data = new byte[2048 * 10];
                        while (true)
                        {
                            size = s.Read(data, 0, data.Length);
                            if (size > 0)
                            {
                                streamWriter.Write(data, 0, size);
                            }
                            else
                            {
                                break;
                            }
                        }

                        streamWriter.Flush();
                        streamWriter.Close();
                    }
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                if (streamWriter != null)
                {
                    streamWriter.Close();
                    streamWriter.Dispose();
                    streamWriter = null;
                }
                if (s != null)
                {
                    s.Close();
                    s.Dispose();
                    s = null;
                }
                if (theEntry != null)
                {
                    theEntry = null;
                }
                if (s != null)
                {
                    s.Close();
                    s = null;
                }
                GC.Collect();
                GC.Collect(1);
            }
        }
    }
}
