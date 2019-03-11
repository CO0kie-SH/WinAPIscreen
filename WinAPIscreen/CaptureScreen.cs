using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace WinAPIscreen
{
    public class CaptureScreen
    {
        public static Bitmap GetDesktopImage()
        {
            //保存截屏的尺寸
            SIZE size;
            //指向bitmap的句柄
            IntPtr hBitmap;
            //通过GetDC函数获得指向桌面设备上下文的句柄
            IntPtr hDC = PlatformInvokeUSER32.GetDC(PlatformInvokeUSER32.GetDesktopWindow());
            //在内存中创建一个兼容的设备上下文
            IntPtr hMemDC = PlatformInvokeGDI32.CreateCompatibleDC(hDC);
            //传递一个常数到GetSystemMetrics，并返回屏幕的x坐标
            size.cx = PlatformInvokeUSER32.GetSystemMetrics(0);
            //传递一个常数到GetSystemMetrics，并返回屏幕的y坐标
            size.cy = PlatformInvokeUSER32.GetSystemMetrics(1);
            //创建与指定的设备环境相关的设备兼容的位图。
            hBitmap = PlatformInvokeGDI32.CreateCompatibleBitmap(hDC, size.cx, size.cy);
            //As hBitmap is IntPtr we can not check it against null. For this purspose IntPtr.Zero is used.
            if (hBitmap != IntPtr.Zero)
            {
                //Here we select the compatible bitmap in memeory device context and keeps the refrence to Old bitmap.
                IntPtr hOld = (IntPtr)PlatformInvokeGDI32.SelectObject(hMemDC, hBitmap);
                //We copy the Bitmap to the memory device context.
                PlatformInvokeGDI32.BitBlt(hMemDC, 0, 0, size.cx, size.cy, hDC, 0, 0, PlatformInvokeGDI32.SRCCOPY);
                //We select the old bitmap back to the memory device context.
                PlatformInvokeGDI32.SelectObject(hMemDC, hOld);
                //We delete the memory device context.
                PlatformInvokeGDI32.DeleteDC(hMemDC);
                //We release the screen device context.
                PlatformInvokeUSER32.ReleaseDC(PlatformInvokeUSER32.GetDesktopWindow(), hDC);
                //Image is created by Image bitmap handle and stored in local variable.
                Bitmap bmp = System.Drawing.Image.FromHbitmap(hBitmap);
                //Release the memory for compatible bitmap.
                PlatformInvokeGDI32.DeleteObject(hBitmap);
                //This statement runs the garbage collector manually.
                GC.Collect();

                //Return the bitmap
                return bmp;
            }
            //If hBitmap is null retunrn null.
            return null;
        }

        public static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                    return codec;
            }
            return null;
        }

        public static void saveJPG(string jpgPath)
        {
            Bitmap im = GetDesktopImage();
            //转成jpg
            var eps = new EncoderParameters(1);
            var ep = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 50L);
            eps.Param[0] = ep;
            var jpsEncodeer = GetEncoder(ImageFormat.Jpeg);
            //保存图片
            //string imgurl = @"/Content/Upload/" + guid + extension;
            im.Save("D:\\aa\\1.jpg", jpsEncodeer, eps);
            //释放资源
            im.Dispose();
            ep.Dispose();
            eps.Dispose();
        }
    }
}
}
