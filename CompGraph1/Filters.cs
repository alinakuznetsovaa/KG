using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel;
using System.Data;
using System.Drawing;

using System.Windows.Forms;

namespace CompGraph1
{
    abstract class Filters
    {
        protected abstract Color calculateNewPixelColor(Bitmap sourceImage, int x, int y);

        public int Clamp(int value, int min, int max) //чтобы привести значения к допустимому диапозону
        {
            if (value < min)
                return min;
            if (value > max)
                return max;
            return value;

        }

        public Bitmap processImage(Bitmap sourseImage) //создаем новое изображение, обходим все пиксели и устанавливаем им новый цвет
        {
            Bitmap resultImage = new Bitmap(sourseImage.Width, sourseImage.Height);
            for (int i = 0; i < sourseImage.Width; i++)
            {
                for (int j = 0; j < sourseImage.Height; j++)
                {
                    resultImage.SetPixel(i, j, calculateNewPixelColor(sourseImage, i, j));
                }
            }
            return resultImage;
        }


    }



    class ForTisnenie : Filters
    {
        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            Color sourceColor = sourceImage.GetPixel(x, y);//получаем цвет исходного пикселя
            Color resultColor = Color.FromArgb((Int32)((255 + sourceColor.R) / 2), (Int32)((255 + sourceColor.G) / 2), (Int32)((255 + sourceColor.B) / 2));
            return resultColor;
        }
    }

    class InvertFilter : Filters
    {
        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            Color sourceColor = sourceImage.GetPixel(x, y);//получаем цвет исходного пикселя
            Color resultColor = Color.FromArgb(255 - sourceColor.R, 255 - sourceColor.G, 255 - sourceColor.B);//вычисляем инверсию цвета
            return resultColor;
        }
    }

    class GrayScaleFilter : Filters
    {
        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            Color sourceColor = sourceImage.GetPixel(x, y);//получаем цвет исходного пикселя
            Color resultColor = Color.FromArgb((Int32)(0.299 * sourceColor.R + 0.587 * sourceColor.G + 0.114 * sourceColor.B), (Int32)(0.299 * sourceColor.R + 0.587 * sourceColor.G + 0.114 * sourceColor.B), (Int32)(0.299 * sourceColor.R + 0.587 * sourceColor.G + 0.114 * sourceColor.B));//вычисляем инверсию цвета
            return resultColor;
        }
    }

    class SepiaFilter : Filters
    {
        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {

            double k = 20;
            Color sourceColor = sourceImage.GetPixel(x, y);//получаем цвет исходного пикселя
            int intensity = (Int32)(0.299 * sourceColor.R + 0.587 * sourceColor.G + 0.114 * sourceColor.B);
            Color resultColor = Color.FromArgb(Clamp((Int32)(intensity + 2 * k), 0, 255), Clamp((Int32)(intensity + 0.5 * k), 0, 255), Clamp((Int32)(intensity - k), 0, 255));//вычисляем инверсию цвета
            return resultColor;
        }
    }

    class IncreaseBrightnessFilter : Filters
    {


        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {


            int bright = 50;

            Color sourceColor = sourceImage.GetPixel(x, y);//получаем цвет исходного пикселя
            Color resultColor = Color.FromArgb(Clamp(sourceColor.R + bright, 0, 255), Clamp(sourceColor.G + bright, 0, 255), Clamp(sourceColor.B + bright, 0, 255));//вычисляем инверсию цвета
            return resultColor;
        }
    }


    class MatrixFilter : Filters
    {
        protected float[,] kernel = null;
        protected MatrixFilter() { }
        public MatrixFilter(float[,] kernel)
        {
            this.kernel = kernel;
        }



        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            int radiusX = kernel.GetLength(0) / 2;
            int radiusY = kernel.GetLength(1) / 2;
            float resultR = 0;
            float resultG = 0;
            float resultB = 0;

            for (int l = -radiusY; l <= radiusY; l++)
                for (int k = -radiusX; k <= radiusX; k++)
                {
                    int idX = Clamp(x + k, 0, sourceImage.Width - 1);
                    int idY = Clamp(y + l, 0, sourceImage.Height - 1);
                    Color neighborColor = sourceImage.GetPixel(idX, idY);
                    resultR += neighborColor.R * kernel[k + radiusX, l + radiusY];
                    resultG += neighborColor.G * kernel[k + radiusX, l + radiusY];
                    resultB += neighborColor.B * kernel[k + radiusX, l + radiusY];
                }
            Color resultColor = Color.FromArgb(
                Clamp((int)resultR, 0, 255),
                Clamp((int)resultG, 0, 255),
                Clamp((int)resultB, 0, 255)
            );
            return resultColor;

        }
    }

    class SobelFilter : MatrixFilter

    {
        public SobelFilter() { }
        public SobelFilter(float[,] ker)
        {
            this.kernel = ker;
        }


    }

    class TisnenieFilter : MatrixFilter
    {
        public TisnenieFilter()
        {
            kernel = new float[3, 3] { { 0, 1, 0 }, { 1, 0, -1 }, { 0, -1, 0 } };
        }


    }


    class BlurFilter : MatrixFilter
    {
        public BlurFilter()
        {
            int sizeX = 3;
            int sizeY = 3;
            kernel = new float[sizeX, sizeY];
            for (int i = 0; i < sizeX; i++)
                for (int j = 0; j < sizeY; j++)
                    kernel[i, j] = 1.0f / (float)(sizeX * sizeY);


        }
    }

    class SharpFilter : MatrixFilter
    {
        public SharpFilter()
        {
            kernel = new float[3, 3] { { 0, -1, 0 }, { -1, 5, -1 }, { 0, -1, 0 } };

        }
    }

    class MedianFilter
    {
        int ShellSort(int n, int[] mass)
        {
            int i, j, step;
            int tmp1;
            for (step = n / 2; step > 0; step /= 2)
            {
                for (i = step; i < n; i++)
                {
                    tmp1 = mass[i];
                    for (j = i; j >= step; j -= step)
                    {
                        if (tmp1 < mass[j - step])
                        {
                            mass[j] = mass[j - step];
                        }
                        else
                        {
                            break;
                        }
                    }
                    mass[j] = tmp1;
                }
            }
            return (mass[n / 2]);

        }
        public int Clamp(int value, int min, int max) //чтобы привести значения к допустимому диапозону
        {
            if (value < min)
                return min;
            if (value > max)
                return max;
            return value;

        }

        public Bitmap create_new_bitmap(int radius, Bitmap image)
        {
            int[] massR = new Int32[radius * radius];
            int[] massG = new Int32[radius * radius];
            int[] massB = new Int32[radius * radius];
            Bitmap resultImage = new Bitmap(image.Width, image.Height);
            int resultR = 0;
            int resultG = 0;
            int resultB = 0;
            int k = 0;
            for (int x = 0; x < image.Height; x++)
                for (int y = 0; y < image.Width; y++)
                {
                    k = 0;
                    for (int i = 0; i < radius; i++)
                    {
                        for (int j = 0; j < radius; j++)
                        {

                            Color sourceColor = image.GetPixel(i, j);
                            massR[k] = (Int32)(sourceColor.R);
                            massG[k] = (Int32)(sourceColor.G);
                            massB[k] = (Int32)(sourceColor.B);
                            k++;



                        }
                    }
                    resultR = ShellSort(radius * radius, massR);
                    resultG = ShellSort(radius * radius, massG);
                    resultB = ShellSort(radius * radius, massB);
                    Color resultColor = Color.FromArgb(
                    Clamp(resultR, 0, 255),
                    Clamp(resultG, 0, 255),
                    Clamp(resultB, 0, 255)
                    );
                    resultImage.SetPixel(y, x, resultColor);

                }
            return resultImage;

        }
    }

    class GaussFilter : MatrixFilter
    {
        public void createGaussKernel(int radius, float sigma)
        {
            //определяем размер ядра
            int size = 2 * radius + 1;
            //создаем ядро фильтра
            kernel = new float[size, size];
            //коэффициент нормировки ядра
            float norm = 0;
            //расчитываем ядро линейного фильтра
            for (int i = -radius; i <= radius; i++)
                for (int j = -radius; j <= radius; j++)
                {
                    kernel[i + radius, j + radius] = (float)(Math.Exp(-(i * i + j * j) / (2 * sigma * sigma)));
                    norm += kernel[i + radius, j + radius];
                }
            //нормируем ядро
            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                    kernel[i, j] /= norm;



        }

        public GaussFilter()
        {
            createGaussKernel(3, 2);
        }
    }


    class GrayWorld
    {
        public int Clamp(int value, int min, int max) //чтобы привести значения к допустимому диапозону
        {
            if (value < min)
                return min;
            if (value > max)
                return max;
            return value;

        }
        public Bitmap processImage(Bitmap sourceImage)
        {
            int Red = 0;
            int Green = 0;
            int Blue = 0;
            for (int i = 0; i < sourceImage.Width; i++)
            {
                for (int j = 0; j < sourceImage.Height; j++)
                {
                    Color sourcecolor = sourceImage.GetPixel(i, j);
                    Red += (Int32)sourcecolor.R;
                    Green += (Int32)sourcecolor.G;
                    Blue += (Int32)sourcecolor.B;

                }
            }
            double avRed = 0.0;
            double avGreen = 0.0;
            double avBlue = 0.0;
            double N = sourceImage.Width * sourceImage.Height;
            avRed = Red / N;
            avGreen = Green / N;
            avBlue = Blue / N;
            double avg = (avRed + avGreen + avBlue) / 3;
            Bitmap resultIm = new Bitmap(sourceImage.Width, sourceImage.Height);

            for (int i = 0; i < sourceImage.Width; i++)
            {
                for (int j = 0; j < sourceImage.Height; j++)
                {
                    Color sourceColor = sourceImage.GetPixel(i, j);//получаем цвет исходного пикселя
                    Color resultColor = Color.FromArgb(Clamp((Int32)(sourceColor.R * avg / avRed), 0, 255), Clamp((Int32)(sourceColor.G * avg / avGreen), 0, 255), Clamp((Int32)(sourceColor.B * avg / avBlue), 0, 255));

                    resultIm.SetPixel(i, j, resultColor);

                }
            }
            return resultIm;

        }


    }


    class Perfectreflector
    {

        public int Clamp(int value, int min, int max) //чтобы привести значения к допустимому диапозону
        {
            if (value < min)
                return min;
            if (value > max)
                return max;
            return value;

        }
        public Bitmap processImage(Bitmap sourceImage)
        {
            int Red = 0;
            int Green = 0;
            int Blue = 0;
            for (int i = 0; i < sourceImage.Width; i++)
            {
                for (int j = 0; j < sourceImage.Height; j++)
                {
                    Color sourcecolor = sourceImage.GetPixel(i, j);
                    if (Red < (Int32)sourcecolor.R)
                        Red = (Int32)sourcecolor.R;
                    if (Green < (Int32)sourcecolor.G)
                        Green = (Int32)sourcecolor.G;
                    if (Blue < (Int32)sourcecolor.B)
                        Blue = (Int32)sourcecolor.B;

                }
            }

            Bitmap resultIm = new Bitmap(sourceImage.Width, sourceImage.Height);

            for (int i = 0; i < sourceImage.Width; i++)
            {
                for (int j = 0; j < sourceImage.Height; j++)
                {
                    Color sourceColor = sourceImage.GetPixel(i, j);//получаем цвет исходного пикселя
                    Color resultColor = Color.FromArgb(Clamp((Int32)(sourceColor.R * 255 / Red), 0, 255), Clamp((Int32)(sourceColor.G * 255 / Green), 0, 255), Clamp((Int32)(sourceColor.B * 255 / Blue), 0, 255));

                    resultIm.SetPixel(i, j, resultColor);

                }
            }
            return resultIm;

        }



    }

    class Delation
    {
        public Bitmap processImage(Bitmap sourceImage)
        {
            int W = sourceImage.Width;
            int H = sourceImage.Height;
            Bitmap resultImage = new Bitmap(W, H);

            // УСТАНОВИТЬ РАЗМЕРЫ СТРУКТУРНОГО ЭЛЕМЕНТА
            int MW = 3;
            int MH = 3;
            int[,] mask = new int[MW, MH];

            mask[0, 0] = 0;
            mask[0, 1] = 1;
            mask[0, 2] = 0;

            mask[1, 0] = 1;
            mask[1, 1] = 1;
            mask[1, 2] = 1;

            mask[2, 0] = 0;
            mask[2, 1] = 1;
            mask[2, 2] = 0;


            for (int y = MH / 2; y < H - MH / 2; y++)
            {
                for (int x = MW / 2; x < W - MW / 2; x++)
                {
                    int max1 = 0;

                    int max2 = 0;

                    int max3 = 0;
                    for (int j = -MH / 2; j <= MH / 2; j++)
                    {
                        for (int i = -MW / 2; i <= MW / 2; i++)
                        {
                            if ((mask[i + MW / 2, j + MH / 2] == 1) && (sourceImage.GetPixel(x + i, y + j).R > max1))
                            {
                                max1 = sourceImage.GetPixel(x + i, y + j).R;
                            }
                            if ((mask[i + MW / 2, j + MH / 2] == 1) && (sourceImage.GetPixel(x + i, y + j).R > max2))
                            {
                                max2 = sourceImage.GetPixel(x + i, y + j).R;
                            }
                            if ((mask[i + MW / 2, j + MH / 2] == 1) && (sourceImage.GetPixel(x + i, y + j).R > max3))
                            {
                                max3 = sourceImage.GetPixel(x + i, y + j).R;
                            }
                        }
                        resultImage.SetPixel(x, y, Color.FromArgb(max1, max2, max3));
                    }
                }
            }
            return resultImage;
        }
    }


    class Erosion
    {
        public Bitmap processImage(Bitmap sourceImage)
        {
            int W = sourceImage.Width;
            int H = sourceImage.Height;
            Bitmap resultImage = new Bitmap(W, H);

            // УСТАНОВИТЬ РАЗМЕРЫ СТРУКТУРНОГО ЭЛЕМЕНТА
            int MW = 3;
            int MH = 3;
            int[,] mask = new int[MW, MH];

            mask[0, 0] = 0;
            mask[0, 1] = 1;
            mask[0, 2] = 0;

            mask[1, 0] = 1;
            mask[1, 1] = 1;
            mask[1, 2] = 1;

            mask[2, 0] = 0;
            mask[2, 1] = 1;
            mask[2, 2] = 0;


            for (int y = MH / 2; y < H - MH / 2; y++)
            {
                for (int x = MW / 2; x < W - MW / 2; x++)
                {
                    int min1 = 255;

                    int min2 = 255;

                    int min3 = 255;
                    for (int j = -MH / 2; j <= MH / 2; j++)
                    {
                        for (int i = -MW / 2; i <= MW / 2; i++)
                        {
                            if ((mask[i + MW / 2, j + MH / 2] == 1) && (sourceImage.GetPixel(x + i, y + j).R < min1))
                            {
                                min1 = sourceImage.GetPixel(x + i, y + j).R;
                            }
                            if ((mask[i + MW / 2, j + MH / 2] == 1) && (sourceImage.GetPixel(x + i, y + j).R < min2))
                            {
                                min2 = sourceImage.GetPixel(x + i, y + j).R;
                            }
                            if ((mask[i + MW / 2, j + MH / 2] == 1) && (sourceImage.GetPixel(x + i, y + j).R < min3))
                            {
                                min3 = sourceImage.GetPixel(x + i, y + j).R;
                            }
                        }
                        resultImage.SetPixel(x, y, Color.FromArgb(min1, min2, min3));
                    }
                }
            }
            return resultImage;
        }
    }
}
