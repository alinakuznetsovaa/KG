using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CompGraph1
{
    public partial class Form1 : Form
    {
        Bitmap image;

        public Form1()
        {
            InitializeComponent();
        }


        int Clamp(int value, int min, int max) //чтобы привести значения к допустимому диапозону
        {
            if (value < min)
                return min;
            if (value > max)
                return max;
            return value;

        }


        Bitmap Difference(Bitmap resultImage, Bitmap resultImage1)
        {
            Bitmap resIm = new Bitmap(resultImage.Width, resultImage.Height);
            for (int i = 0; i < resultImage.Width; i++)
            {
                for (int j = 0; j < resultImage.Height; j++)
                {
                    Color sourceColor = resultImage.GetPixel(i, j);//получаем цвет исходного пикселя
                    Color sourceColor1 = resultImage1.GetPixel(i, j);//получаем цвет исходного пикселя

                    Color resultColor = Color.FromArgb(Clamp((Int32)(sourceColor.R - sourceColor1.R), 0, 255), Clamp((Int32)(sourceColor.G - sourceColor1.G), 0, 255), Clamp((Int32)(sourceColor.B - sourceColor1.B), 0, 255));


                    resIm.SetPixel(i, j, resultColor);
                }
            }
            return resIm;
        }



        private void открытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //создаем диалог для открытия файла
            OpenFileDialog dialog = new OpenFileDialog();//инициализируем его конструктором по умолчанию
            dialog.Filter = "Image files|*.png;*.jpg;*.bmp|All files(*.*)|*.*"; //фильтр
            //проверка выбрал ли пользователь файл
            if (dialog.ShowDialog() == DialogResult.OK)
            {

            }
            image = new Bitmap(dialog.FileName);//загрузили картинку в программу
            pictureBox1.Image = image;//визуализируем ее на форме
            pictureBox1.Refresh();//обновили pictureBox

        }

        private void инверсияToolStripMenuItem_Click(object sender, EventArgs e)
        {

            InvertFilter filter = new InvertFilter();
            Bitmap resultImage = filter.processImage(image);
            pictureBox2.Image = resultImage;
            pictureBox2.Refresh();
        }

        private void вОттенкахСерогоToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new GrayScaleFilter();
            Bitmap resultImage = filter.processImage(image);
            pictureBox2.Image = resultImage;
            pictureBox2.Refresh();
        }

        private void сепияToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new SepiaFilter();
            Bitmap resultImage = filter.processImage(image);
            pictureBox2.Image = resultImage;
            pictureBox2.Refresh();
        }

        private void увеличитьЯркостьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new IncreaseBrightnessFilter();
            Bitmap resultImage = filter.processImage(image);
            pictureBox2.Image = resultImage;
            pictureBox2.Refresh();
        }

        private void блюрToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new BlurFilter();
            Bitmap resultImage = filter.processImage(image);
            pictureBox2.Image = resultImage;
            pictureBox2.Refresh();
        }

        private void гауссаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new GaussFilter();
            Bitmap resultImage = filter.processImage(image);
            pictureBox2.Image = resultImage;
            pictureBox2.Refresh();
        }

        private void резкостьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new SharpFilter();
            Bitmap resultImage = filter.processImage(image);
            pictureBox2.Image = resultImage;
            pictureBox2.Refresh();
        }

        private void собеляToolStripMenuItem_Click(object sender, EventArgs e)
        {
            float[,] kerx = new float[3, 3] { { -1, 0, 1 }, { -2, 0, 2 }, { -1, 0, 1 } };
            float[,] kery = new float[3, 3] { { -1, -2, -1 }, { 0, 0, 0 }, { 1, 2, 1 } };

            Filters filter = new SobelFilter(kerx);
            Bitmap resultImage = filter.processImage(image);
            pictureBox2.Image = resultImage;

            Filters filter1 = new SobelFilter(kery);
            Bitmap resultImage1 = filter1.processImage(image);



            Bitmap res = SobelFunc(resultImage, resultImage1);
            pictureBox2.Image = res;
            pictureBox2.Refresh();

            Bitmap SobelFunc(Bitmap a, Bitmap b)
            {
                Bitmap resImage = new Bitmap(a.Width, a.Height);
                for (int i = 0; i < a.Width; i++)
                {
                    for (int j = 0; j < a.Height; j++)
                    {
                        Color sourceColor = a.GetPixel(i, j);
                        Color sourceColor1 = b.GetPixel(i, j);
                        double intensR = Math.Sqrt((double)(Math.Pow((Int32)sourceColor.R, 2) + Math.Pow((Int32)sourceColor1.R, 2)));
                        double intensG = Math.Sqrt((double)(Math.Pow((Int32)sourceColor.G, 2) + Math.Pow((Int32)sourceColor1.G, 2)));
                        double intensB = Math.Sqrt((double)(Math.Pow((Int32)sourceColor.B, 2) + Math.Pow((Int32)sourceColor1.B, 2)));
                        double intensity = intensR + intensG + intensB;
                        Color resColor = Color.FromArgb(Clamp((Int32)(intensity), 0, 255), Clamp((Int32)(intensity), 0, 255), Clamp((Int32)(intensity), 0, 255));
                        resImage.SetPixel(i, j, resColor);





                        /*  int intensity = (Int32)(sourceColor.R + sourceColor.G + sourceColor.B);

                          int intensity1 = (Int32)(sourceColor.R + sourceColor.G + sourceColor.B);

                          double intens = Math.Sqrt((double)(Math.Pow(intensity, 2) + Math.Pow(intensity1, 2)));
                          Color resColor = Color.FromArgb(Clamp((Int32)(intens), 0, 255), Clamp((Int32)(intens), 0, 255), Clamp((Int32)(intens), 0, 255));
                          resImage.SetPixel(i, j, resColor);*/
                    }
                }
                return resImage;
            }
        }

        private void тиснениеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //1 шаг 
            Filters filter = new GrayScaleFilter();
            Bitmap resultImage = filter.processImage(image);

            //2 шаг
            Filters filter1 = new TisnenieFilter();
            Bitmap resultImage1 = filter1.processImage(resultImage);

            //3 шаг
            Filters filter2 = new ForTisnenie();
            Bitmap resultImage2 = filter2.processImage(resultImage1);
            pictureBox2.Image = resultImage2;
            pictureBox2.Refresh();
        }

        private void серыйМирToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GrayWorld filter = new GrayWorld();
            Bitmap resultImage = filter.processImage(image);
            pictureBox2.Image = resultImage;
            pictureBox2.Refresh();
        }

        private void идеальныйОтражательToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Perfectreflector filter = new Perfectreflector();
            Bitmap resultImage = filter.processImage(image);
            pictureBox2.Image = resultImage;
            pictureBox2.Refresh();
        }

        private void delationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Delation filter = new Delation();
            Bitmap resultImage = filter.processImage(image);
            pictureBox2.Image = resultImage;
            pictureBox2.Refresh();
        }

        private void erosionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Erosion filter = new Erosion();
            Bitmap resultImage = filter.processImage(image);
            pictureBox2.Image = resultImage;
            pictureBox2.Refresh();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Erosion filter = new Erosion();
            Bitmap resultImage = filter.processImage(image);
            Delation filter1 = new Delation();
            Bitmap resultImage1 = filter1.processImage(resultImage);

            pictureBox2.Image = resultImage1;
            pictureBox2.Refresh();

        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Delation filter = new Delation();
            Bitmap resultImage = filter.processImage(image);
            Erosion filter1 = new Erosion();
            Bitmap resultImage1 = filter1.processImage(resultImage);
            pictureBox2.Image = resultImage1;
            pictureBox2.Refresh();
        }

        private void gradToolStripMenuItem_Click(object sender, EventArgs e)
        {

            Delation filter = new Delation();
            Bitmap resultIm = filter.processImage(image);

            Erosion filter1 = new Erosion();
            Bitmap resultIm1 = filter1.processImage(image);
            Bitmap res = Difference(resultIm, resultIm1);
            pictureBox2.Image = res;
            pictureBox2.Refresh();


        }

        private void topHatToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Delation filter = new Delation();
            Bitmap resultImage = filter.processImage(image);
            Erosion filter1 = new Erosion();
            Bitmap resultImage1 = filter1.processImage(resultImage);
            Bitmap res = Difference(image, resultImage1);
            pictureBox2.Image = res;
            pictureBox2.Refresh();
        }


       

        private void blackHatToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Erosion filter = new Erosion();
            Bitmap resultImage = filter.processImage(image);
            Delation filter1 = new Delation();
            Bitmap resultImage1 = filter1.processImage(resultImage);
            Bitmap res = Difference(resultImage1, image);
            pictureBox2.Image = res;
            pictureBox2.Refresh();
        }

        private void медианныйToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MedianFilter filter = new MedianFilter();
            Bitmap resultImage = filter.create_new_bitmap(5, image);
            pictureBox2.Image = resultImage;
            pictureBox2.Refresh();
        }
    }
}
