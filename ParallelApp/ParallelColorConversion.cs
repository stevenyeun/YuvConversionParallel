using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParallelApp
{
    unsafe class ParallelColorConversion
    {
        private static byte[] rgb;
        private static int width;
        private static int height;
        private static int total;
        private static ParallelOptions options = null;
        public static void Init(int width, int height, int logicalCoreNum)
        {   
            ParallelColorConversion.width = width;
            ParallelColorConversion.height = height;
            total = width * height;
            rgb = new byte[total * 3];
            options = new ParallelOptions();
            options.MaxDegreeOfParallelism = logicalCoreNum;
        }
        public static byte[] ConvertYUV420toRGB888_Parallel(byte* yuv)
        {
            Parallel.For(0, height, options, (posY) =>
            {
                //Console.WriteLine("{0}: {1}",
                //       Thread.CurrentThread.ManagedThreadId, posY);
                for (int posX = 0; posX < width; posX++)
                {
                    //Parallel.For(0, width, (posX) =>
                    //{
                    int yPos = (posY * width + posX);
                    byte y = yuv[yPos];
                    byte u = yuv[(posY / 2) * (width / 2) + (posX / 2) + total];
                    byte v = yuv[(posY / 2) * (width / 2) + (posX / 2) + total + (total / 4)];
                    //byte u = 0;
                    //byte v = 0;


                    int rTmp = y + (int)(1.370705 * (v - 128));
                    int gTmp = y - (int)((0.698001 * (v - 128)) - (0.337633 * (u - 128)));
                    int bTmp = y + (int)(1.732446 * (u - 128));

                    rgb[yPos * 3] = CLAMP(rTmp);
                    rgb[yPos * 3 + 1] = CLAMP(gTmp);
                    rgb[yPos * 3 + 2] = CLAMP(bTmp);
                }
                //});
            });


            return rgb;

        }
        public static byte[] ConvertYUV420toRGB888_Seq(byte* yuv)
        {
            for (int posY = 0; posY < height; posY++)
            {
                //Console.WriteLine("{0}: {1}",
                //       Thread.CurrentThread.ManagedThreadId, posY);
                for (int posX = 0; posX < width; posX++)
                {
                    //Parallel.For(0, width, (posX) =>
                    //{
                    int yPos = (posY * width + posX);
                    byte y = yuv[yPos];
                    byte u = yuv[(posY / 2) * (width / 2) + (posX / 2) + total];
                    byte v = yuv[(posY / 2) * (width / 2) + (posX / 2) + total + (total / 4)];
                    //byte u = 0;
                    //byte v = 0;


                    int rTmp = y + (int)(1.370705 * (v - 128));
                    int gTmp = y - (int)((0.698001 * (v - 128)) - (0.337633 * (u - 128)));
                    int bTmp = y + (int)(1.732446 * (u - 128));

                    rgb[yPos * 3] = CLAMP(rTmp);
                    rgb[yPos * 3 + 1] = CLAMP(gTmp);
                    rgb[yPos * 3 + 2] = CLAMP(bTmp);
                }
                //});
            }


            return rgb;

        }
        static byte CLAMP(int x)
        {
            //((x > 255) ? 255 : (x < 0) ? 0 : x);

            if (x > 255)
            {
                return 255;
            }
            else
            {
                if (x < 0)
                    return 0;
                else
                    return (byte)x;
            }
        }
    }
}
