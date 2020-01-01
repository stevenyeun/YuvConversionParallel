using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Management;

namespace ParallelApp
{
    unsafe class Program
    {
        static void Main(string[] args)
        {
#if false
            // 1. 순차적 실행
            // 동일쓰레드가 0~999 출력
            //
            for (int i = 0; i < 1000; i++)
            {
                Console.WriteLine("{0}: {1}",
                    Thread.CurrentThread.ManagedThreadId, i);
            }
            Console.Read();

            // 2. 병렬 처리
            // 다중쓰레드가 병렬로 출력
            //
            Parallel.For(0, 1000, (i) => {
                Console.WriteLine("{0}: {1}",
                    Thread.CurrentThread.ManagedThreadId, i);
            });

            Console.Read();
#endif

#if false
            int width = 352;
            int height = 288;
            string file = "akiyo_cif.yuv";
#endif
            //int width = 960;
            //int height = 540;
            //int width = 720;
            //int height = 576;

            int width = 1920;
            int height = 1080;
            //string file = "D:\\yuv\\BigBuckBunny_1920_1080_24fps.420";
            string file = "BigBuckBunny_1920_1080_24fps.420";


            int ySize = width * height;
            int uvSize = ySize / 2;
            
            //string file = "videoCam_960x540_420P.i420";
            //string file = "720X576-YV12.yuv";
            



            int logicalCoreNum = Environment.ProcessorCount;
            Console.WriteLine("core = " + logicalCoreNum);

            File.SetAttributes(file, File.GetAttributes(file) | FileAttributes.ReadOnly);
            using (FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read))
            {
                int totalSize = (int)fs.Length;
                int totalNumOfFrame = totalSize / (ySize + uvSize);
                int frameSize = ySize + uvSize;

                byte[] buff = new byte[frameSize];
                long totalTime = 0;

                ParallelColorConversion.Init(width, height, logicalCoreNum);
                for (int i = 0; i < totalNumOfFrame; i++)
                {
                    int n = fs.Read(buff, 0, frameSize);
               
                    fixed (byte* ptr = buff)
                    {
                        Stopwatch swh = new Stopwatch();
                        swh.Start();

                        byte[] RGB = ParallelColorConversion.ConvertYUV420toRGB888_Parallel(ptr);
                        //byte[] RGB = ParallelColorConversion.ConvertYUV420toRGB888_Seq(ptr);

                        swh.Stop();
                        Console.WriteLine(swh.ElapsedMilliseconds.ToString()+"ms");
                        totalTime += swh.ElapsedMilliseconds;
#if false
                        string fileName = String.Format("{0}.bmp", i + 1);
                        Bitmap bitmap = CreateBitmap24bppRgb(RGB, width, height);
                        bitmap.Save(fileName);
#endif
                        //Console.WriteLine(fileName);
                    }
                    Thread.Sleep(1000/15);
                }
                Console.WriteLine("avrProcTime=" + totalTime / totalNumOfFrame);
            }


            Thread.Sleep(5000);
        }

       

        public static Bitmap CreateBitmap24bppRgb(byte[] bmpData, int width, int height)
        {
            if ((width * height * 3) != bmpData.Length)
                throw new ArgumentException();

            // Sometimes rows have an offset to complete a multiple of 4 number of bytes.  
            // In that case:  
            // int offset = 4 - ((width * 3) % 4);  
            // if (offset == 4) offset = 0;  
            // if (((width * 3 + offset) * height) != bmpData.Length)  
            //     throw new ArgumentException();  

            Bitmap bmp = new Bitmap(width, height, PixelFormat.Format24bppRgb);
            int pos = 0;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    bmp.SetPixel(x, y, Color.FromArgb(bmpData[pos], bmpData[pos + 1], bmpData[pos + 2]));
                    pos += 3;
                }
                //    pos += offset;  
            }
            return bmp;
        }




       


   
    }
}
