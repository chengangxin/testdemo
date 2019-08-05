using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ZipFile
{
    class Program
    {
        static void Main(string[] args)
        {
            DateTime dtStart = DateTime.Now;
            //SharpZip.PackFiles("D:\\工作计划及周报.zip", "D:\\工作周报和计划");
            //SharpZip.UnpackFiles("D:\\工作计划及周报.zip", "D:\\ZipTest\\");
            //System.IO.Directory.Delete("D:\\ZipTest\\", true);
            byte[] bytes = Ziper.PackFiles(Directory.GetFiles(@"D:\Eol安装环境\激活工具\").ToList(), 5);
            Ziper.UnpackFiles(bytes, @"D:\ZipTest\");
            DateTime dtEnd = DateTime.Now;
            Console.WriteLine((dtEnd - dtStart).Milliseconds);
            Console.ReadKey();
        }
    }
}
