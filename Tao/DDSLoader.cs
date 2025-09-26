using System;
using System.Drawing;
using System.Drawing.Imaging;
using Tao.DevIl;

namespace rMap.Tao
{
    class DDSLoader
    {
        /// <summary>
        /// Converts an in-memory image in DDS format to a System.Drawing.Bitmap
        /// object for easy display in Windows forms.
        /// </summary>
        /// <param name="DDSData">Byte array containing DDS image data</param>
        /// <returns>A Bitmap object that can be displayed</returns>
        public static Bitmap DDSDataToBMP(byte[ ] DDSData) 
        {  
            // Create a DevIL image "name" (which is actually a number)  
            int img_name;
            Il.ilInit();
            Il.ilGenImages(1, out img_name);  
            Il.ilBindImage(img_name);  
            
            // Load the DDS file into the bound DevIL image  
            Il.ilLoadL(Il.IL_DDS, DDSData, DDSData.Length);
            int err = Il.ilGetError();
            if (err != Il.IL_NO_ERROR)
                throw new Exception("Image load failed with error: " + err.ToString());
            // Set a few size variables that will simplify later code  
            
            int ImgWidth = Il.ilGetInteger(Il.IL_IMAGE_WIDTH);  
            int ImgHeight = Il.ilGetInteger(Il.IL_IMAGE_HEIGHT);  
            Rectangle rect = new Rectangle(0, 0, ImgWidth, ImgHeight);  
            
            // Convert the DevIL image to a pixel byte array to copy into Bitmap  
            Il.ilConvertImage(Il.IL_BGRA, Il.IL_UNSIGNED_BYTE);  
            
            // Create a Bitmap to copy the image into, and prepare it to get data  
            Bitmap bmp = new Bitmap(ImgWidth, ImgHeight);  
            BitmapData bmd =    
                bmp.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);  
            
            // Copy the pixel byte array from the DevIL image to the Bitmap  
            Il.ilCopyPixels(0, 0, 0,    
                Il.ilGetInteger(Il.IL_IMAGE_WIDTH),    
                Il.ilGetInteger(Il.IL_IMAGE_HEIGHT),    
                1, Il.IL_BGRA, Il.IL_UNSIGNED_BYTE,    
                bmd.Scan0);  
            
            // Clean up and return Bitmap  
            Il.ilDeleteImages(1, ref img_name);
            Il.ilShutDown();
            bmp.UnlockBits(bmd);  
            return bmp;
        }

        public static unsafe Bitmap LoadFile(string file)
        {
            // Create a DevIL image "name" (which is actually a number)  
            int img_name;
            Il.ilInit();
            Il.ilGenImages(1, out img_name);
            Il.ilBindImage(img_name);

            Il.ilLoadImage(file);
            int err = Il.ilGetError();
            if (err != Il.IL_NO_ERROR)
                throw new Exception("Image load failed with error: " + err.ToString());
            // Set a few size variables that will simplify later code  

            int ImgWidth = Il.ilGetInteger(Il.IL_IMAGE_WIDTH);
            int ImgHeight = Il.ilGetInteger(Il.IL_IMAGE_HEIGHT);
            Rectangle rect = new Rectangle(0, 0, ImgWidth, ImgHeight);

            System.GC.Collect();
            //IntPtr ptr = System.Runtime.InteropServices.Marshal.AllocHGlobal(ImgWidth * ImgHeight * 4);
 
            // Create a Bitmap to copy the image into, and prepare it to get data  
            Bitmap bmp = new Bitmap(ImgWidth, ImgHeight); //, ImgWidth * 4, PixelFormat.Format32bppArgb, ptr);
            BitmapData bmd = bmp.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

            // Copy the pixel byte array from the DevIL image to the Bitmap  
            Il.ilCopyPixels(0, 0, 0,
                Il.ilGetInteger(Il.IL_IMAGE_WIDTH),
                Il.ilGetInteger(Il.IL_IMAGE_HEIGHT),
                1, Il.IL_BGRA, Il.IL_UNSIGNED_BYTE,
                bmd.Scan0);


            // Clean up and return Bitmap  
            Il.ilDeleteImages(1, ref img_name);
            Il.ilShutDown();
            bmp.UnlockBits(bmd);
            return bmp;
        }

        public static void SaveBitmapToDDS(string path, Bitmap bmp, Rectangle rect)
        {
            Il.ilInit();
            int ilImageID = 0;
            Il.ilGenImages(1, out ilImageID);
            Il.ilBindImage(ilImageID);

            //Rectangle rect = new Rectangle(new Point(0, 0), bmp.Size);

            System.Drawing.Imaging.BitmapData bmpData = bmp.LockBits(rect,
                    System.Drawing.Imaging.ImageLockMode.ReadWrite,
                    System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            if (!Il.ilTexImage(rect.Width, rect.Height, 1, 4,
                    Il.IL_BGRA,
                    Il.IL_UNSIGNED_BYTE,
                    bmpData.Scan0))
            {
                int errorCode = Il.ilGetError();
                Il.ilDeleteImages(1, ref ilImageID);
                Il.ilShutDown();

                throw new Exception("Dev IL tex copy error: " + errorCode);
            }

            Il.ilSetInteger(Il.IL_DXTC_FORMAT, Il.IL_DXT3);
            Il.ilEnable(Il.IL_FILE_OVERWRITE);
            if (!Il.ilSaveImage(path))
            {
                int errorCode = Il.ilGetError();
                Il.ilDeleteImages(1, ref ilImageID);
                Il.ilShutDown();

                throw new Exception("Dev IL save error: " + errorCode);
            }
           
            Il.ilDeleteImages(1, ref ilImageID);

            bmp.UnlockBits(bmpData);
            Il.ilShutDown();
        }

        public static void SaveBitmapToDDS(string path, System.Drawing.Imaging.BitmapData bmpData)
        {
            Il.ilInit();
            int ilImageID = 0;
            Il.ilGenImages(1, out ilImageID);
            Il.ilBindImage(ilImageID);

            if (!Il.ilTexImage(bmpData.Width, bmpData.Height, 1, 4,
                    Il.IL_BGRA,
                    Il.IL_UNSIGNED_BYTE,
                    bmpData.Scan0))
            {
                int errorCode = Il.ilGetError();
                Il.ilDeleteImages(1, ref ilImageID);
                Il.ilShutDown();

                throw new Exception("Dev IL tex copy error: " + errorCode);
            }

            Il.ilSetInteger(Il.IL_DXTC_FORMAT, Il.IL_DXT3);
            Il.ilEnable(Il.IL_FILE_OVERWRITE);
            if (!Il.ilSaveImage(path))
            {
                int errorCode = Il.ilGetError();
                Il.ilDeleteImages(1, ref ilImageID);
                Il.ilShutDown();

                throw new Exception("Dev IL save error: " + errorCode);
            }

            Il.ilDeleteImages(1, ref ilImageID);
            Il.ilShutDown();
        }

        private static void HandleILError(ref int ilImageID)
        {
            int errorCode = Il.ilGetError();
            Il.ilDeleteImages(1, ref ilImageID);
            Il.ilShutDown();

            throw new Exception("Dev IL error: " + errorCode);
        }
    }
}
