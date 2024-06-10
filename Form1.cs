using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;
using System.Drawing.Imaging;

namespace MozaikaApp
{
    public partial class Form1 : Form
    {
        public class User
        {
            // Пока пустой, но может содержать свойства и методы, связанные с пользователем.
        }

        public class Wall
        {
            public int Width { get; set; }
            public int Height { get; set; }

            public Wall(int width, int height)
            {
                Width = width;
                Height = height;
            }
        }

        public class Chip
        {
            public int Width { get; set; }
            public int Height { get; set; }
            public Color Color { get; set; }

            public Chip(int width, int height, Color color)
            {
                Width = width;
                Height = height;
                Color = color;
            }
        }

        public class Photo
        {
            public Bitmap Image { get; private set; }

            public Photo(Bitmap image)
            {
                Image = image;
            }
        }

        public class Mosaic
        {
            public List<Chip> Chips { get; private set; }

            public Mosaic()
            {
                Chips = new List<Chip>();
            }

            public void AddChip(Chip chip)
            {
                Chips.Add(chip);
            }
        }

        private Bitmap orig_image;
        private Color chipBorderColor;


        public Form1()
        {
            InitializeComponent();

        }


        // добавление цвета (обработка кнопки)
        private void button1_Click(object sender, EventArgs e)
        {

            if (colorDialog1.ShowDialog() == DialogResult.OK) // Проверка, был ли выбран цвет
                chipBorderColor = colorDialog1.Color;
            // Добавление выбранного цвета в список
        }




        // загрузка изображения (обработка кнопки)
        private void button2_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                orig_image = new Bitmap(openFileDialog1.FileName);
                pictureBox2.Image = orig_image;

            }

        }

        // ограничение для пользователя на ввод значений размеров стены и плиток 
        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            char number = e.KeyChar;
            if (!Char.IsDigit(number) && number != 8 && number != 44)
            {
                e.Handled = true;
            }
            
        }

        // обработка нажатия кнопки генерации изображения
        private void button3_Click(object sender, EventArgs e)
        {
            if (orig_image == null)
            {
                MessageBox.Show("Пожалуйста, загрузите изображение.");
                return;
            }

            if (textBox1.Text == "" || textBox2.Text == "" || textBox3.Text == "" || textBox4.Text == "")
            {
                MessageBox.Show("Пожалуйста, введите все значения.");
                return;

            }
            else
            {


                int wallWidth = int.Parse(textBox1.Text);
                int wallHeight = int.Parse(textBox2.Text);
                int chipWidth = int.Parse(textBox3.Text);
                int chipHeight = int.Parse(textBox4.Text);

                Wall wall = new Wall(wallWidth, wallHeight);
                Photo photo = new Photo(orig_image);

                Bitmap mosaicImage = CreateMosaic(wall, chipWidth, chipHeight, chipBorderColor, photo);
                if (mosaicImage != null) 
                    pictureBox1.Image = mosaicImage;

            }
           

            
        }
        private Bitmap CreateMosaic(Wall wall, int chipWidth, int chipHeight, Color chipBorderColor, Photo photo)
        {
            Bitmap mosaic = new Bitmap(wall.Width, wall.Height);

            int chipsPerRow = wall.Width / chipWidth;
            int chipsPerColumn = wall.Height / chipHeight;

            using (Graphics g = Graphics.FromImage(mosaic))
            {
                for (int row = 0; row < chipsPerColumn; row++)
                {
                    for (int col = 0; col < chipsPerRow; col++)
                    {
                        // Отрисовываем заполненный чип
                        Color chipFillColor = GetAverageColor(photo.Image, col * chipWidth, row * chipHeight, chipWidth, chipHeight);
                        using (Brush fillBrush = new SolidBrush(chipFillColor))
                        {
                            g.FillRectangle(fillBrush, col * chipWidth, row * chipHeight, chipWidth, chipHeight);
                        }

                        // Отрисовываем обводку чипа
                        using (Pen borderPen = new Pen(chipBorderColor))
                        {
                            g.DrawRectangle(borderPen, col * chipWidth, row * chipHeight, chipWidth, chipHeight);
                        }
                    }
                }
            }

            return mosaic;
        }

        private Color GetAverageColor(Bitmap image, int startX, int startY, int width, int height)
        {
            int totalRed = 0, totalGreen = 0, totalBlue = 0;

            for (int y = startY; y < startY +height; y++)
            {
                for (int x = startX; x < startX + width; x++)
                {
                    Color pixelColor = image.GetPixel(x % image.Width, y % image.Height);
                    totalRed += pixelColor.R;
                    totalGreen += pixelColor.G;
                    totalBlue += pixelColor.B;
                }
            }

            int pixelCount = width * height; //сколько плиток нам нужно для заполнения изображения
            int avgRed = totalRed / pixelCount; 
            int avgGreen = totalGreen / pixelCount;
            int avgBlue = totalBlue / pixelCount;

            return Color.FromArgb(avgRed, avgGreen, avgBlue);
        }

       
    }
}



