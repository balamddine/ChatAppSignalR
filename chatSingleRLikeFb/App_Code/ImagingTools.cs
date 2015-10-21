
using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Drawing.Drawing2D;

public class ImagingTools
{

    public static byte[] ResizeFromByteArray(int MaxSideSize, Byte[] byteArrayIn, string fileName)
    {

        byte[] byteArray = null;  // really make this an error gif
        MemoryStream ms = new MemoryStream(byteArrayIn);
        //byteArray = imageResize.ResizeFromStream(MaxSideSize, ms, fileName);

        return byteArray;
    }

    public static string ConvertBinImageToFile(byte[] BinImage, string path, string pictureName)
    {
        string imageUrl = "";
        try
        {
            ImagingTools.SaveFromBytes(BinImage, pictureName, path, 180,100,180,100);
        }
        catch (Exception) { }
        imageUrl = path + pictureName;
        return imageUrl;
    }

    public static byte[] ConvertImgToBinary(string imageFileName)
    {
        System.Drawing.Image img = System.Drawing.Image.FromFile(System.Web.HttpContext.Current.Server.MapPath(imageFileName));
        return ImagingTools.imageToByteArray(img);
    }

    public static byte[] ResizeFromStream(int MaxSideSize, Stream Buffer, string fileName)
    {
        byte[] byteArray = null;  // really make this an error gif

        try
        {
            Bitmap bitMap = new Bitmap(Buffer);
            int intOldWidth = bitMap.Width;
            int intOldHeight = bitMap.Height;

            int intNewWidth;
            int intNewHeight;
            int intMaxSide;

            if (intOldWidth >= intOldHeight)
            {
                intMaxSide = intOldWidth;
            }
            else
            {
                intMaxSide = intOldHeight;
            }

            if (intMaxSide > MaxSideSize)
            {
                //set new width and height                    
                double dblCoef = MaxSideSize / (double)intMaxSide;
                intNewWidth = Convert.ToInt32(dblCoef * intOldWidth);
                intNewHeight = Convert.ToInt32(dblCoef * intOldHeight);
            }
            else
            {

                intNewWidth = intOldWidth;
                intNewHeight = intOldHeight;
            }

            Size ThumbNailSize = new Size(intNewWidth, intNewHeight);
            System.Drawing.Image oImg = System.Drawing.Image.FromStream(Buffer);
            System.Drawing.Image oThumbNail = new Bitmap(ThumbNailSize.Width, ThumbNailSize.Height);
            Graphics oGraphic = Graphics.FromImage(oThumbNail);
            oGraphic.CompositingQuality = CompositingQuality.HighQuality;
            oGraphic.SmoothingMode = SmoothingMode.HighQuality;
            oGraphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
            Rectangle oRectangle = new Rectangle
                (0, 0, ThumbNailSize.Width, ThumbNailSize.Height);

            oGraphic.DrawImage(oImg, oRectangle);

            MemoryStream ms = new MemoryStream();
            oThumbNail.Save(ms, ImageFormat.Jpeg);
            byteArray = new byte[ms.Length];
            ms.Position = 0;
            ms.Read(byteArray, 0, Convert.ToInt32(ms.Length));
            oGraphic.Dispose();
            oImg.Dispose();
            ms.Close();
            ms.Dispose();
        }
        catch (Exception)
        {
            int newSize = MaxSideSize - 20;
            Bitmap bitMap = new Bitmap(newSize, newSize);
            Graphics g = Graphics.FromImage(bitMap);
            g.FillRectangle(new SolidBrush(Color.Gray), new Rectangle(0, 0, newSize, newSize));

            Font font = new Font("Courier", 8);
            SolidBrush solidBrush = new SolidBrush( Color.Red);
            g.DrawString("Failed File", font, solidBrush, 10, 5);
            g.DrawString(fileName, font, solidBrush, 10, 50);

            MemoryStream ms = new MemoryStream();
            bitMap.Save(ms, ImageFormat.Jpeg);
            byteArray = new byte[ms.Length];
            ms.Position = 0;
            ms.Read(byteArray, 0, Convert.ToInt32(ms.Length));

            ms.Close();
            ms.Dispose();
            bitMap.Dispose();
            solidBrush.Dispose();
            g.Dispose();
            font.Dispose();

        }
        return byteArray;
    }

    /// <summary>
    /// Saves the resized image to specified file name and path as JPEG
    /// and also returns the bytearray for any other use you may need it for
    /// </summary>
    /// <param name="MaxSideSize"></param>
    /// <param name="Buffer"></param>
    /// <param name="fileName">No Extension</param>
    /// <param name="filePath">Examples: "images/dir1/dir2" or "images" or "images/dir1"</param>

    public static byte[] SaveFromStream(int MaxWidth, int MaxHeight, int MinWidth, int MinHeight, Stream Buffer, string fileName, string filePath)
    {
        byte[] byteArray = null;  // really make this an error gif

        try
        {
            Bitmap bitMap = new Bitmap(Buffer);
            int intOldWidth = bitMap.Width;
            int intOldHeight = bitMap.Height;

            int intNewWidth;
            int intNewHeight;
            int intMaxSide;
            bool WidthIsBiggerThanHeight;

            if (intOldWidth >= intOldHeight)
            {
                intMaxSide = intOldWidth;
                WidthIsBiggerThanHeight = true;
            }
            else
            {
                intMaxSide = intOldHeight;
                WidthIsBiggerThanHeight = false;
            }

            //if (intMaxSide > MaxSideSize)
            if(WidthIsBiggerThanHeight)
                if (intOldWidth > MaxWidth)
                {
                    //set new width and height
                    double dblCoef; //MaxSideSize / (double)intMaxSide;
                    if (intOldWidth - intOldHeight > 10)
                    {
                        dblCoef = MaxWidth / (double)intOldWidth;
                        intNewWidth = Convert.ToInt32(dblCoef * intOldWidth);
                        intNewHeight = Convert.ToInt32(dblCoef * intOldHeight);
                    }
                    else
                    {
                        dblCoef = MinWidth / (double)intOldWidth;
                        intNewWidth = Convert.ToInt32(dblCoef * intOldWidth);
                        intNewHeight = Convert.ToInt32(dblCoef * intOldHeight);
                    }
                }
                else
                {
                    intNewWidth = intOldWidth;
                    intNewHeight = intOldHeight;
                }
            else
                if (intOldHeight > MaxHeight)
                {
                    //set new width and height
                    double dblCoef = MaxHeight / (double)intOldHeight; //MaxSideSize / (double)intMaxSide;
                    if (intOldHeight - intOldWidth > 10)
                    {
                        intNewWidth = Convert.ToInt32(dblCoef * intOldWidth);
                        intNewHeight = Convert.ToInt32(dblCoef * intOldHeight);
                    }
                    else
                    {
                        dblCoef = MinHeight / (double)intOldHeight;
                        intNewWidth = Convert.ToInt32(dblCoef * intOldWidth);
                        intNewHeight = Convert.ToInt32(dblCoef * intOldHeight);
                    }
                }
                else
                {
                    intNewWidth = intOldWidth;
                    intNewHeight = intOldHeight;
                }

            Size ThumbNailSize = new Size(intNewWidth, intNewHeight);
            System.Drawing.Image oImg = System.Drawing.Image.FromStream(Buffer);
            System.Drawing.Image oThumbNail = new Bitmap(ThumbNailSize.Width, ThumbNailSize.Height);

            Graphics oGraphic = Graphics.FromImage(oThumbNail);
            oGraphic.CompositingQuality = CompositingQuality.HighQuality;
            oGraphic.SmoothingMode = SmoothingMode.HighQuality;
            oGraphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
            Rectangle oRectangle = new Rectangle
                (0, 0, ThumbNailSize.Width, ThumbNailSize.Height);

            oGraphic.DrawImage(oImg, oRectangle);

            //Save File
            string newFileName = string.Format(System.Web.HttpContext.Current.Server.MapPath(filePath + fileName/*"~/{0}*{1}.jpg"*/), filePath, fileName);
            oThumbNail.Save(newFileName, ImageFormat.Png);

            MemoryStream ms = new MemoryStream();
            oThumbNail.Save(ms, ImageFormat.Png);
            byteArray = new byte[ms.Length];
            ms.Position = 0;
            ms.Read(byteArray, 0, Convert.ToInt32(ms.Length));

            oGraphic.Dispose();
            oImg.Dispose();
            ms.Close();
            ms.Dispose();
        }
        catch (Exception)
        {
            int newSize = MaxWidth - 20;
            Bitmap bitMap = new Bitmap(newSize, newSize);
            Graphics g = Graphics.FromImage(bitMap);
            g.FillRectangle(new SolidBrush(Color.Gray), new Rectangle(0, 0, newSize, newSize));

            Font font = new Font("Courier", 8);
            SolidBrush solidBrush = new SolidBrush(Color.Red);
            g.DrawString("Failed To Save File or Failed File", font, solidBrush, 10, 5);
            g.DrawString(fileName, font, solidBrush, 10, 50);

            MemoryStream ms = new MemoryStream();
            bitMap.Save(ms, ImageFormat.Png);
            byteArray = new byte[ms.Length];
            ms.Position = 0;
            ms.Read(byteArray, 0, Convert.ToInt32(ms.Length));

            ms.Close();
            ms.Dispose();
            bitMap.Dispose();
            solidBrush.Dispose();
            g.Dispose();
            font.Dispose();

        }
        return byteArray;
    }

    public static void SaveFromBytes(byte[] ImageBytes, string fileName, string filePath, int MaxWidth, int MaxHeight, int MinWidth, int MinHeight)
    {
        MemoryStream ms = new MemoryStream(ImageBytes);
        SaveFromStream(MaxWidth, MaxHeight, MinWidth, MinHeight, ms, fileName, filePath);
        //int intNewWidth = 10, intNewHeight = 10;
        ////Size ThumbNailSize = new Size(intNewWidth, intNewHeight);
        //System.Drawing.Image oImg = ByteArrayToImage(ImageBytes);
        //System.Drawing.Image oThumbNail = new Bitmap(ms); //new Bitmap(ThumbNailSize.Width, ThumbNailSize.Height);

        //Graphics oGraphic = Graphics.FromImage(oThumbNail);
        //oGraphic.CompositingQuality = CompositingQuality.HighQuality;
        //oGraphic.SmoothingMode = SmoothingMode.HighQuality;
        //oGraphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
        //Rectangle oRectangle = new Rectangle(0, 0, ThumbNailSize.Width, ThumbNailSize.Height);

        //oGraphic.DrawImage(oImg, oRectangle);

        ////Save File
        //string newFileName = string.Format(System.Web.HttpContext.Current.Server.MapPath(filePath + fileName), filePath, fileName);//("~/{0}*{1}.jpg"), filePath, fileName);
        //oThumbNail.Save(newFileName, ImageFormat.Jpeg);
        
        //MemoryStream ms = new MemoryStream();
        //oThumbNail.Save(ms, ImageFormat.Jpeg);
        ////byteArray = new byte[ms.Length];
        //ms.Position = 0;
        //ms.Read(ImageBytes, 0, Convert.ToInt32(ms.Length));

        //oGraphic.Dispose();
        //oImg.Dispose();
        //ms.Close();
        //ms.Dispose();
    }
    public static void CreateTextPicture(string text, string fileName, string filePath, int width, int height)
    {
        Bitmap bitMap = new Bitmap(width, height);
        Graphics g = Graphics.FromImage(bitMap);
        g.FillRectangle(new SolidBrush(Color.Gray), new Rectangle(0, 0, width, height));
        g.FillRectangle(new SolidBrush(Color.LightGray), new Rectangle(1, 1, width-2, height-2));
        
        Font font = new Font("Arial", 10);
        SolidBrush solidBrush = new SolidBrush(Color.White);
        g.DrawString(text, font, solidBrush, 5, 5);
        //g.DrawString(fileName, font, solidBrush, 10, 50);

        MemoryStream ms = new MemoryStream();
        bitMap.Save(ms, ImageFormat.Png);
        byte[] byteArray = new byte[ms.Length];
        ms.Position = 0;
        ms.Read(byteArray, 0, Convert.ToInt32(ms.Length));

        string newFileName = string.Format(System.Web.HttpContext.Current.Server.MapPath(filePath + fileName/*"~/{0}*{1}.jpg"*/), filePath, fileName);
        System.Drawing.Image oThumbNail = bitMap;
        oThumbNail.Save(newFileName, ImageFormat.Png);
            
        ms.Close();
        ms.Dispose();
        bitMap.Dispose();
        solidBrush.Dispose();
        g.Dispose();
        font.Dispose();
    }
    public static byte[] imageToByteArray(System.Drawing.Image imageIn)
    {
        MemoryStream ms = new MemoryStream();
        imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
        imageIn.Dispose();
        return ms.ToArray();
    }
    public static System.Drawing.Image ByteArrayToImage(byte[] byteArrayIn)
    {
        MemoryStream ms = new MemoryStream(byteArrayIn);
        System.Drawing.Image returnImage = System.Drawing.Image.FromStream(ms);
        return returnImage;
    }

    enum BWMode { By_Lightness, By_RGB_Value };
    public static void CreateBlackImageFromFile(string SourceImageFile)
    {
        System.Drawing.Bitmap SourceImage = new System.Drawing.Bitmap(System.Web.HttpContext.Current.Server.MapPath(SourceImageFile));
        System.Drawing.Bitmap BlackImage = new System.Drawing.Bitmap(SourceImage, SourceImage.Width, SourceImage.Height);
        BWMode Mode = BWMode.By_Lightness;
        Single tolerance = 0;
        int x;
        int y;

        if (tolerance > 1 || tolerance < -1)
            throw new ArgumentOutOfRangeException();

        for (x = 0; x < BlackImage.Width - 1; x++)
            for (y = 0; y < BlackImage.Height - 1; y++)
            {
                Color clr = BlackImage.GetPixel(x, y);
                if (Mode == BWMode.By_RGB_Value)
                    if ((clr.R + clr.G + clr.B) > 383 - (tolerance * 383))
                        BlackImage.SetPixel(x, y, Color.White);
                    else
                        BlackImage.SetPixel(x, y, Color.Black);

                else
                    if (clr.GetBrightness() > 0.8 - (tolerance / 2))
                        BlackImage.SetPixel(x, y, Color.White);
                    else
                        BlackImage.SetPixel(x, y, Color.Black);
            }
        string BlackImageName = Path.GetFileNameWithoutExtension(SourceImageFile) + "_black" + Path.GetExtension(SourceImageFile);
        string BlackFileName = string.Format(System.Web.HttpContext.Current.Server.MapPath(Path.GetDirectoryName(SourceImageFile) + "/" + BlackImageName), Path.GetDirectoryName(SourceImageFile), BlackImageName);
        BlackImage.Save(BlackFileName, ImageFormat.Png);
    }
    public static string GetImageType(string path)
    {
        string headerCode = GetHeaderInfo(path).ToUpper();

        if (headerCode.StartsWith("FFD8FFE0"))
        {
            return "JPG";
        }
        else if (headerCode.StartsWith("49492A"))
        {
            return "TIFF";
        }
        else if (headerCode.StartsWith("424D"))
        {
            return "BMP";
        }
        else if (headerCode.StartsWith("474946"))
        {
            return "GIF";
        }
        else if (headerCode.StartsWith("89504E470D0A1A0A"))
        {
            return "PNG";
        }
        else
        {
            return ""; //UnKnown
        }
    }
    public static string GetHeaderInfo(string path)
    {
        byte[] buffer = new byte[8];

        BinaryReader reader = new BinaryReader(new FileStream(path, FileMode.Open));
        reader.Read(buffer, 0, buffer.Length);
        reader.Close();

        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        foreach (byte b in buffer)
            sb.Append(b.ToString("X2"));

        return sb.ToString();
    }

    public static string GetImageType(byte[] image)
    {
        string headerCode = GetHeaderInfo(image).ToUpper();

        if (headerCode.StartsWith("FFD8FFE0"))
        {
            return "JPG";
        }
        else if (headerCode.StartsWith("49492A"))
        {
            return "TIFF";
        }
        else if (headerCode.StartsWith("424D"))
        {
            return "BMP";
        }
        else if (headerCode.StartsWith("474946"))
        {
            return "GIF";
        }
        else if (headerCode.StartsWith("89504E470D0A1A0A"))
        {
            return "PNG";
        }
        else
        {
            return ""; //UnKnown
        }
    }
    public static string GetHeaderInfo(byte[] image)
    {
        byte[] buffer = new byte[8];

        BinaryReader reader = new BinaryReader(new MemoryStream(image));
        reader.Read(buffer, 0, buffer.Length);
        reader.Close();

        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        foreach (byte b in buffer)
            sb.Append(b.ToString("X2"));

        return sb.ToString();
    }
}