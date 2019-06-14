using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Remote_Client_fubao.Helper
{
    /// <summary>
    /// 图片差异比较类
    /// </summary>
    public class ImageComparer
    {
        /// <summary>
        /// 图像颜色
        /// </summary>
        [StructLayout(LayoutKind.Explicit)]
        private struct ICColor
        {
            [FieldOffset(0)]
            public byte B;
            [FieldOffset(1)]
            public byte G;
            [FieldOffset(2)]
            public byte R;
        }

        /// <summary>
        /// 按20*20大小进行分块比较两个图像
        /// </summary>
        /// <param name="bmp1"></param>
        /// <param name="bmp2"></param>
        /// <returns></returns>
        public static List<Rectangle> Compare(Bitmap bmp1, Bitmap bmp2)
        {
            return Compare(bmp1, bmp2, new Size(80, 45));
        }
        /// <summary>
        /// 比较两个图像
        /// </summary>
        /// <param name="bmp1"></param>
        /// <param name="bmp2"></param>
        /// <param name="block"></param>
        /// <returns></returns>
        public static List<Rectangle> Compare(Bitmap bmp1, Bitmap bmp2, Size block)
        {
            List<Rectangle> rects = new List<Rectangle>();
            PixelFormat pf = PixelFormat.Format24bppRgb;

            BitmapData bd1 = bmp1.LockBits(new Rectangle(0, 0, bmp1.Width, bmp1.Height), ImageLockMode.ReadOnly, pf);
            BitmapData bd2 = bmp2.LockBits(new Rectangle(0, 0, bmp2.Width, bmp2.Height), ImageLockMode.ReadOnly, pf);
         
            try
            {
                unsafe
                {
                    int w = 0, h = 0;

                    while (h < bd1.Height && h < bd2.Height)
                    {
                        byte* p1 = (byte*)bd1.Scan0 + h * bd1.Stride;
                        byte* p2 = (byte*)bd2.Scan0 + h * bd2.Stride;

                        w = 0;
                        while (w < bd1.Width && w < bd2.Width)
                        {
                            //按块大小进行扫描
                            for (int i = 0; i < block.Width; i++)
                            {
                                int wi = w + i;
                                if (wi >= bd1.Width || wi >= bd2.Width) break;

                                for (int j = 0; j < block.Height; j++)
                                {
                                    int hj = h + j;
                                    if (hj >= bd1.Height || hj >= bd2.Height) break;

                                    ICColor* pc1 = (ICColor*)(p1 + wi * 3 + bd1.Stride * j);
                                    ICColor* pc2 = (ICColor*)(p2 + wi * 3 + bd2.Stride * j);

                                    if (pc1->R != pc2->R || pc1->G != pc2->G || pc1->B != pc2->B)
                                    {
                                        //当前块有某个象素点颜色值不相同.也就是有差异.

                                        int bw = Math.Min(block.Width, bd1.Width - w);
                                        int bh = Math.Min(block.Height, bd1.Height - h);
                                        rects.Add(new Rectangle(w, h, bw, bh));

                                        goto E;
                                    }
                                }
                            }
                        E:
                            w += block.Width;
                        }

                        h += block.Height;
                    }
                }
            }
            finally
            {
                bmp1.UnlockBits(bd1);
                bmp2.UnlockBits(bd2);
            }

            return rects;
        }
    }
}
