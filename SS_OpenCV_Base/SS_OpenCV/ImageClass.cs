using System;
using System.Collections.Generic;
using System.Text;
using Emgu.CV.Structure;
using Emgu.CV;
using System.Drawing;
using System.Management.Instrumentation;
using System.Windows.Forms;
using Emgu.CV.ImgHash;
using System.Linq;
using Emgu.CV.XFeatures2D;
using System.Diagnostics.Eventing.Reader;
using System.Runtime.InteropServices.ComTypes;

namespace SS_OpenCV
{
    class ImageClass
    {

        /// <summary>
        /// Image Negative using EmguCV library
        /// Slower method
        /// </summary>
        /// <param name="img">Image</param>
        public static void Negativeold(Image<Bgr, byte> img)
        {
            int x, y;

            Bgr aux;
            for (y = 0; y < img.Height; y++)
            {
                for (x = 0; x < img.Width; x++)
                {
                    // acesso pela biblioteca : mais lento 
                    aux = img[y, x];
                    img[y, x] = new Bgr(255 - aux.Blue, 255 - aux.Green, 255 - aux.Red);
                }
            }
        }

        /// <summary>
        /// Image Negative made by student
        /// Faster method
        /// </summary>
        /// <param name="img">Image</param>
        public static void Negative(Image<Bgr, byte> img)
        {
            unsafe
            {// direct access to the image memory(sequencial)
                // direcion top left -> bottom right

                MIplImage m = img.MIplImage;
                byte* dataPtr = (byte*)m.ImageData.ToPointer(); // Pointer to the image
                byte blue, green, red;

                int width = img.Width;
                int height = img.Height;
                int nChan = m.NChannels; // number of channels - 3
                int padding = m.WidthStep - m.NChannels * m.Width; // alinhament bytes (padding)
                int x, y;

                for (y = 0; y < height; y++)
                {
                    for (x = 0; x < width; x++)
                    {
                        blue = dataPtr[0];
                        green = dataPtr[1];
                        red = dataPtr[2];
                        // convert to the negative color
                        dataPtr[0] = (byte)(255 - blue);
                        dataPtr[1] = (byte)(255 - green);
                        dataPtr[2] = (byte)(255 - red);

                        // advance the pointer to the next pixel
                        dataPtr += nChan;
                    }
                    //at the end of the line advance the pointer by the aligment bytes (padding)
                    dataPtr += padding;
                }
            }
        }
        /// <summary>
        /// Convert to gray
        /// Direct access to memory - faster method
        /// </summary>
        /// <param name="img">image</param>
        public static void ConvertToGray(Image<Bgr, byte> img)
        {
            unsafe
            {
                // direct access to the image memory(sequencial)
                // direcion top left -> bottom right

                MIplImage m = img.MIplImage;
                byte* dataPtr = (byte*)m.ImageData.ToPointer(); // Pointer to the image
                byte blue, green, red, gray;

                int width = img.Width;
                int height = img.Height;
                int nChan = m.NChannels; // number of channels - 3
                int padding = m.WidthStep - m.NChannels * m.Width; // alinhament bytes (padding)
                int x, y;

                if (nChan == 3) // image in RGB
                {
                    for (y = 0; y < height; y++)
                    {
                        for (x = 0; x < width; x++)
                        {
                            //retrieve 3 colour components
                            blue = dataPtr[0];
                            green = dataPtr[1];
                            red = dataPtr[2];

                            // convert to gray
                            gray = (byte)Math.Round(((int)blue + green + red) / 3.0);

                            // store in the image
                            dataPtr[0] = gray;
                            dataPtr[1] = gray;
                            dataPtr[2] = gray;

                            // advance the pointer to the next pixel
                            dataPtr += nChan;
                        }

                        //at the end of the line advance the pointer by the aligment bytes (padding)
                        dataPtr += padding;
                    }
                }
            }
        }
        public static void RedChannel(Image<Bgr, byte> img)
        {
            unsafe
            {
                // direct access to the image memory(sequencial)
                // direcion top left -> bottom right

                MIplImage m = img.MIplImage;
                byte* dataPtr = (byte*)m.ImageData.ToPointer(); // Pointer to the image
                byte red;

                int width = img.Width;
                int height = img.Height;
                int nChan = m.NChannels; // number of channels - 3
                int padding = m.WidthStep - m.NChannels * m.Width; // alinhament bytes (padding)
                int x, y;

                if (nChan == 3) // image in RGB
                {
                    for (y = 0; y < height; y++)
                    {
                        for (x = 0; x < width; x++)
                        {
                            //retrieve 3 colour components                       
                            red = dataPtr[2];

                            // store in the image
                            dataPtr[0] = red;
                            dataPtr[1] = red;
                            dataPtr[2] = red;

                            // advance the pointer to the next pixel
                            dataPtr += nChan;
                        }

                        //at the end of the line advance the pointer by the aligment bytes (padding)
                        dataPtr += padding;
                    }
                }
            }

        }
        public static void BrightContrast(Image<Bgr, byte> img, int bright, double contrast)
        {
            unsafe
            {
                // direct access to the image memory(sequencial)
                // direcion top left -> bottom 
                MIplImage m = img.MIplImage;
                byte* dataPtr = (byte*)m.ImageData.ToPointer(); // Pointer to the image
                byte blue, green, red;
                double b, g, r;
                int width = img.Width;
                int height = img.Height;
                int nChan = m.NChannels; // number of channels - 3
                int padding = m.WidthStep - m.NChannels * m.Width; // alinhament bytes (padding)
                int x, y;
                if (nChan == 3)
                {
                    for (y = 0; y < height; y++)
                    {
                        for (x = 0; x < width; x++)
                        { 
                            b= Math.Round(contrast * dataPtr[0] + bright);
                            g = Math.Round(contrast * dataPtr[1] + bright);
                            r = Math.Round(contrast * dataPtr[2] + bright);
                            blue = (byte)Math.Round(contrast * dataPtr[0] + bright);
                            green = (byte)Math.Round(contrast * dataPtr[1] + bright);
                            red = (byte)Math.Round(contrast * dataPtr[2] + bright);

                            dataPtr[0] = blue;
                            dataPtr[1] = green;
                            dataPtr[2] = red;

                            if (b <= 0){
                                dataPtr[0] = 0;
                            } else if (b >= 255)
                                {
                                    dataPtr[0] = 255;
                                }

                            if (g <= 0){
                                dataPtr[1] = 0;
                            } else if (g >= 255)
                                {
                                    dataPtr[1] = 255;
                                }
                            if (r <= 0){
                                dataPtr[2] = 0;
                            }else if (r >= 255)
                                {
                                    dataPtr[2] = 255;
                                }

                            // advance the pointer to the next pixel
                            dataPtr += nChan;
                        }

                        //at the end of the line advance the pointer by the aligment bytes (padding)
                        dataPtr += padding;
                    }
                }
            }
        }
        public static void Rotation(Image<Bgr, byte> imgDestino, Image<Bgr, byte> imgOrigem, float angle)
        {
            unsafe
            {
                MIplImage m_destino = imgDestino.MIplImage;
                MIplImage m_origem = imgOrigem.MIplImage;
                byte* dataPtr_origem = (byte*)m_origem.ImageData.ToPointer();
                byte* dataPtr_destino = (byte*)m_destino.ImageData.ToPointer();// Pointer to the image

                byte blue, green, red;
                int widthD = imgDestino.Width;
                int widthO = m_origem.Width;
                int heightO = imgOrigem.Height;
                int heightD = imgDestino.Height;
                int x_origem, y_origem;
                int widthstep = m_origem.WidthStep;
                int nChan = m_destino.NChannels; // number of channels - 3
                int padding = m_destino.WidthStep - m_destino.NChannels * m_destino.Width; // alinhament bytes (padding)
                int x_destino, y_destino;

                if (nChan == 3)
                {
                    for (y_destino = 0; y_destino < heightD; y_destino++)
                    {
                        for (x_destino = 0; x_destino < widthD; x_destino++)
                        {
                            x_origem = (int)Math.Round((x_destino - widthD/2.0)*Math.Cos(angle)-(heightD/ 2.0-y_destino)*Math.Sin(angle)+widthD /2.0);
                            y_origem = (int)Math.Round(heightD / 2.0 - (x_destino - widthD / 2.0) * Math.Sin(angle) - (heightD/ 2.0 - y_destino)*Math.Cos(angle));



                            if (x_origem < 0 || x_origem >= widthO || y_origem < 0 || y_origem >= heightO)
                            {
                                blue = 0;
                                green = 0;
                                red = 0;
                            }
                            else
                            {
                                blue = (byte)(dataPtr_origem + (y_origem * widthstep) + x_origem * nChan)[0];
                                green = (byte)(dataPtr_origem + (y_origem * widthstep) + x_origem * nChan)[1];
                                red = (byte)(dataPtr_origem + (y_origem * widthstep) + x_origem * nChan)[2];
                            }
                            dataPtr_destino[0] = blue; 
                            dataPtr_destino[1] = green;
                            dataPtr_destino[2] = red;


                            dataPtr_destino += nChan;
                        }
                        dataPtr_destino += padding;
                    }
                }
            }
        }
        public static void Translation(Image<Bgr, byte> imgDestino, Image<Bgr, byte> imgOrigem, int dx, int dy)
        {
            unsafe
            {
                MIplImage m_destino = imgDestino.MIplImage;
                MIplImage m_origem = imgOrigem.MIplImage;
                byte* dataPtr_origem = (byte*)m_origem.ImageData.ToPointer();
                byte* dataPtr_destino = (byte*)m_destino.ImageData.ToPointer();// Pointer to the image

                byte blue, green, red;
                int widthD = imgDestino.Width;
                int widthO = m_origem.Width;
                int heightO = imgOrigem.Height;
                int heightD = imgDestino.Height;
                int x_origem, y_origem;
                int widthstep = m_origem.WidthStep;
                int nChan = m_destino.NChannels; // number of channels - 3
                int padding = m_destino.WidthStep - m_destino.NChannels * m_destino.Width; // alinhament bytes (padding)
                int x_destino, y_destino;

                if (nChan == 3)
                {
                    for (y_destino = 0; y_destino < heightD; y_destino++)
                    {
                        for (x_destino = 0; x_destino < widthD; x_destino++)
                        {
                            x_origem = (x_destino-dx);
                            y_origem = (y_destino-dy);



                            if (x_origem < 0 || x_origem >= widthO || y_origem < 0 || y_origem >= heightO)
                            {
                                blue = 0;
                                green = 0;
                                red = 0;
                            }
                            else
                            {
                                blue = (byte)(dataPtr_origem + (y_origem * widthstep) + x_origem * nChan)[0];
                                green = (byte)(dataPtr_origem + (y_origem * widthstep) + x_origem * nChan)[1];
                                red = (byte)(dataPtr_origem + (y_origem * widthstep) + x_origem * nChan)[2];
                            }
                            dataPtr_destino[0] = blue;
                            dataPtr_destino[1] = green;
                            dataPtr_destino[2] = red;


                            dataPtr_destino += nChan;
                        }
                        dataPtr_destino += padding;
                    }
                }
            }
        }
        public static void Scale(Image<Bgr, byte> imgDestino, Image<Bgr, byte> imgOrigem, float scaleFactor)
        {
            unsafe
            {
                MIplImage m_destino = imgDestino.MIplImage;
                MIplImage m_origem = imgOrigem.MIplImage;
                byte* dataPtr_origem = (byte*)m_origem.ImageData.ToPointer();
                byte* dataPtr_destino = (byte*)m_destino.ImageData.ToPointer();// Pointer to the image

                byte blue, green, red;
                int widthD = imgDestino.Width;
                int widthO = m_origem.Width;
                int heightO = imgOrigem.Height;
                int heightD = imgDestino.Height;
                int x_origem, y_origem;
                int widthstep = m_origem.WidthStep;
                int nChan = m_destino.NChannels; // number of channels - 3
                int padding = m_destino.WidthStep - m_destino.NChannels * m_destino.Width; // alinhament bytes (padding)
                int x_destino, y_destino;

                if (nChan == 3)
                {
                    for (y_destino = 0; y_destino < heightD; y_destino++)
                    {
                        for (x_destino = 0; x_destino < widthD; x_destino++)
                        {
                            x_origem = (int)Math.Round(x_destino / scaleFactor);
                            y_origem = (int)Math.Round(y_destino / scaleFactor);



                            if (x_origem < 0 || x_origem >= widthO || y_origem < 0 || y_origem >= heightO)
                            {
                                blue = 0;
                                green = 0;
                                red = 0;
                            }
                            else
                            {
                                blue = (byte)(dataPtr_origem + (y_origem * widthstep) + x_origem * nChan)[0];
                                green = (byte)(dataPtr_origem + (y_origem * widthstep) + x_origem * nChan)[1];
                                red = (byte)(dataPtr_origem + (y_origem * widthstep) + x_origem * nChan)[2];
                            }
                            dataPtr_destino[0] = blue;
                            dataPtr_destino[1] = green;
                            dataPtr_destino[2] = red;


                            dataPtr_destino += nChan;
                        }
                        dataPtr_destino += padding;
                    }
                }
            }
        }
        public static void Scale_point_xy(Image<Bgr, byte> imgDestino, Image<Bgr, byte> imgOrigem, float scaleFactor, int centerX, int centerY)
        {
            unsafe
            {
                MIplImage m_destino = imgDestino.MIplImage;
                MIplImage m_origem = imgOrigem.MIplImage;
                byte* dataPtr_origem = (byte*)m_origem.ImageData.ToPointer();
                byte* dataPtr_destino = (byte*)m_destino.ImageData.ToPointer();// Pointer to the image

                byte blue, green, red;
                int widthD = imgDestino.Width;
                int widthO = m_origem.Width;
                int heightO = imgOrigem.Height;
                int heightD = imgDestino.Height;
                int x_origem, y_origem;
                int widthstep = m_origem.WidthStep;
                int nChan = m_destino.NChannels; // number of channels - 3
                int padding = m_destino.WidthStep - m_destino.NChannels * m_destino.Width; // alinhament bytes (padding)
                int x_destino, y_destino;

                if (nChan == 3)
                {
                    for (y_destino = 0; y_destino < heightD; y_destino++)
                    {
                        for (x_destino = 0; x_destino < widthD; x_destino++)
                        {
                            x_origem = (int)Math.Round((x_destino / scaleFactor) - ((widthO/2)/scaleFactor) + centerX);
                            y_origem = (int)Math.Round((y_destino / scaleFactor) - ((heightO / 2)/ scaleFactor) + centerY);


                            if (x_origem < 0 || x_origem >= widthO || y_origem < 0 || y_origem >= heightO)
                            {
                                blue = 0;
                                green = 0;
                                red = 0;
                            }
                            else
                            {
                                blue = (byte)(dataPtr_origem + (y_origem * widthstep) + x_origem * nChan)[0];
                                green = (byte)(dataPtr_origem + (y_origem * widthstep) + x_origem * nChan)[1];
                                red = (byte)(dataPtr_origem + (y_origem * widthstep) + x_origem * nChan)[2];
                            }
                            dataPtr_destino[0] = blue;
                            dataPtr_destino[1] = green;
                            dataPtr_destino[2] = red;


                            dataPtr_destino += nChan;
                        }
                        dataPtr_destino += padding;
                    }
                }
            }
        }
    }
    
}
