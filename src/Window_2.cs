using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Medic
{
    public partial class Window_2 : Form
    {
        private Image clueImage_2;
        private bool isDrawingMode = false;
        private bool isDrawingDist_B;
        private bool isFirstPointSet = false;
        private Point firstPoint;
        private Point currentMousePosition;
        private Bitmap drawingLayer;

        public Window_2()
        {
            InitializeComponent();
            LoadClueImage();
            pictureBox2.Image = SharedData.Radiograph_1;
            label_2_7.Visible = false;
            pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox2.MouseDown += PictureBox2_MouseDown;
            pictureBox2.MouseMove += PictureBox2_MouseMove;
            pictureBox2.Paint += PictureBox2_Paint;
        }

        private void LoadClueImage()
        {
            try
            {
                string clueImagePath = Path.Combine(Application.StartupPath, "clue_2.png");

                if (File.Exists(clueImagePath))
                {
                    clueImage_2 = Image.FromFile(clueImagePath);
                }
                else
                {
                    MessageBox.Show("Файл clue_2.png не найден в папке с программой!",
                                  "Предупреждение",
                                  MessageBoxButtons.OK,
                                  MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке clue_2.png: {ex.Message}",
                              "Ошибка",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Error);
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void btn_next_2_Click(object sender, EventArgs e)
        {
            if (textBox_2_3_Okano.Text == "")
            {
                MessageBox.Show("Индекс Окано не измерен!",
                                      "Ошибка",
                                      MessageBoxButtons.OK,
                                      MessageBoxIcon.Error);
            }
            else
            {
                if (SharedData.Okano >= 43 && SharedData.Okano <= 58)
                {
                    Window_3 w3 = new Window_3();
                    w3.Show();
                    Hide();
                }
                else
                {
                    MessageBox.Show("Индекс Окано выходит за пределы требуемого диапазона.\r\nПрименение программы невозможно",
                      "Ошибка",
                      MessageBoxButtons.OK,
                      MessageBoxIcon.Error);
                }
            }
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void btn_switch_2_1_Click(object sender, EventArgs e)
        {
            pictureBox2.Image = SharedData.Radiograph_1;
        }

        private void btn_switch_2_2_Click(object sender, EventArgs e)
        {
            pictureBox2.Image = clueImage_2;
            drawingLayer?.Dispose();
            drawingLayer = null;
            pictureBox2.Invalidate();
        }

        private void btn_help_2_Click(object sender, EventArgs e)
        {
            label_2_7.Visible = !label_2_7.Visible;
        }

        private void btn_measure_2_1_Click(object sender, EventArgs e)
        {
            if (pictureBox2.Image != SharedData.Radiograph_1)
            {
                MessageBox.Show("Рентгенограммы 1 нет на экране!",
                      "Ошибка",
                      MessageBoxButtons.OK,
                      MessageBoxIcon.Error);
            }
            else
            {
                // Сброс состояния и включение режима рисования
                isDrawingDist_B = false;
                isDrawingMode = true;
                isFirstPointSet = false;
                textBox_2_1_Dist_A.Text = "0";

                // Очистка предыдущего рисунка
                if (drawingLayer != null)
                {
                    drawingLayer.Dispose();
                    drawingLayer = null;
                }

                // Перерисовка PictureBox
                pictureBox2.Invalidate();
            }
        }

        private void textBox_2_1_Dist_A_TextChanged(object sender, EventArgs e)
        {

        }

        private void PictureBox2_MouseDown(object sender, MouseEventArgs e)
        {
            if (!isDrawingMode || e.Button != MouseButtons.Left)
                return;

            // Получение координат относительно изображения
            Point imagePoint = GetImageCoordinates(e.Location);

            if (!isFirstPointSet)
            {
                // Установка первой точки
                firstPoint = imagePoint;
                isFirstPointSet = true;
                currentMousePosition = imagePoint;
            }
            else
            {
                CreateFinalLine(imagePoint);
                isDrawingMode = false;
                isFirstPointSet = false;
            }
        }

        private void PictureBox2_MouseMove(object sender, MouseEventArgs e)
        {
            if (!isDrawingMode || !isFirstPointSet)
                return;

            // Обновление текущей позиции мыши
            currentMousePosition = GetImageCoordinates(e.Location);

            // Расчёт и отображение длины/диаметра
            double length = CalculateDistance(firstPoint, currentMousePosition);

            if (!isDrawingDist_B)
            {
                textBox_2_1_Dist_A.Text = Math.Round(length, 2).ToString();
            }
            else textBox_2_2_Dist_B.Text = Math.Round(length, 2).ToString();

            // Перерисовка для отображения временной фигуры
            pictureBox2.Invalidate();
        }

        private void PictureBox2_Paint(object sender, PaintEventArgs e)
        {
            // Отрисовка сохранённой фигуры
            if (drawingLayer != null && pictureBox2.Image != null)
            {
                // Получение коэффициентов масштабирования
                float scaleX = (float)pictureBox2.Width / pictureBox2.Image.Width;
                float scaleY = (float)pictureBox2.Height / pictureBox2.Image.Height;
                float scale = Math.Min(scaleX, scaleY);

                // Вычисление смещения для центрирования
                int offsetX = (int)((pictureBox2.Width - pictureBox2.Image.Width * scale) / 2);
                int offsetY = (int)((pictureBox2.Height - pictureBox2.Image.Height * scale) / 2);

                // Отрисовка слоя с фигурой
                e.Graphics.DrawImage(drawingLayer, offsetX, offsetY,
                    pictureBox2.Image.Width * scale, pictureBox2.Image.Height * scale);
            }

            // Отрисовка временной фигуры при рисовании
            if (isDrawingMode && isFirstPointSet)
            {
                using (Pen pen = new Pen(Color.Red, 2))
                {
                    Point displayFirst = GetDisplayCoordinates(firstPoint);
                    Point displayCurrent = GetDisplayCoordinates(currentMousePosition);
                    // Рисование линии
                    e.Graphics.DrawLine(pen, displayFirst, displayCurrent);

                    // Отрисовка точек
                    e.Graphics.FillEllipse(Brushes.Red, displayFirst.X - 3, displayFirst.Y - 3, 6, 6);
                    e.Graphics.FillEllipse(Brushes.Red, displayCurrent.X - 3, displayCurrent.Y - 3, 6, 6);
                }
            }
        }

        private void CreateFinalLine(Point secondPoint)
        {
            if (pictureBox2.Image == null)
                return;

            // Создание слоя для рисования
            drawingLayer = new Bitmap(pictureBox2.Image.Width, pictureBox2.Image.Height);

            using (Graphics g = Graphics.FromImage(drawingLayer))
            {
                g.Clear(Color.Transparent);
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                using (Pen pen = new Pen(Color.Red, 2))
                {
                    g.DrawLine(pen, firstPoint, secondPoint);

                    // Отрисовка точек
                    g.FillEllipse(Brushes.Red, firstPoint.X - 3, firstPoint.Y - 3, 6, 6);
                    g.FillEllipse(Brushes.Red, secondPoint.X - 3, secondPoint.Y - 3, 6, 6);
                }
            }

            // Расчёт финальной длины
            double length = CalculateDistance(firstPoint, secondPoint);
            if (!isDrawingDist_B)
            {
                textBox_2_1_Dist_A.Text = Math.Round(length, 2).ToString();
            }
            else textBox_2_2_Dist_B.Text = Math.Round(length, 2).ToString();

            pictureBox2.Invalidate();
        }

        private Point GetImageCoordinates(Point pictureBoxPoint)
        {
            if (pictureBox2.Image == null)
                return Point.Empty;

            // Получение размеров и позиции изображения в PictureBox
            float scaleX = (float)pictureBox2.Width / pictureBox2.Image.Width;
            float scaleY = (float)pictureBox2.Height / pictureBox2.Image.Height;
            float scale = Math.Min(scaleX, scaleY);

            int offsetX = (int)((pictureBox2.Width - pictureBox2.Image.Width * scale) / 2);
            int offsetY = (int)((pictureBox2.Height - pictureBox2.Image.Height * scale) / 2);

            // Преобразование координат
            int imageX = (int)((pictureBoxPoint.X - offsetX) / scale);
            int imageY = (int)((pictureBoxPoint.Y - offsetY) / scale);

            // Ограничение координат размерами изображения
            imageX = Math.Max(0, Math.Min(imageX, pictureBox2.Image.Width - 1));
            imageY = Math.Max(0, Math.Min(imageY, pictureBox2.Image.Height - 1));

            return new Point(imageX, imageY);
        }

        private Point GetDisplayCoordinates(Point imagePoint)
        {
            if (pictureBox2.Image == null)
                return Point.Empty;

            float scaleX = (float)pictureBox2.Width / pictureBox2.Image.Width;
            float scaleY = (float)pictureBox2.Height / pictureBox2.Image.Height;
            float scale = Math.Min(scaleX, scaleY);

            int offsetX = (int)((pictureBox2.Width - pictureBox2.Image.Width * scale) / 2);
            int offsetY = (int)((pictureBox2.Height - pictureBox2.Image.Height * scale) / 2);

            int displayX = (int)(imagePoint.X * scale + offsetX);
            int displayY = (int)(imagePoint.Y * scale + offsetY);

            return new Point(displayX, displayY);
        }

        private double CalculateDistance(Point p1, Point p2)
        {
            double dx = p2.X - p1.X;
            double dy = p2.Y - p1.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        private void btn_measure_2_2_Click(object sender, EventArgs e)
        {
            if (pictureBox2.Image != SharedData.Radiograph_1)
            {
                MessageBox.Show("Рентгенограммы 1 нет на экране!",
                      "Ошибка",
                      MessageBoxButtons.OK,
                      MessageBoxIcon.Error);
            }
            else
            {
                // Сброс состояния и включение режима рисования
                isDrawingDist_B = true;
                isDrawingMode = true;
                isFirstPointSet = false;
                textBox_2_2_Dist_B.Text = "0";

                // Очистка предыдущего рисунка
                if (drawingLayer != null)
                {
                    drawingLayer.Dispose();
                    drawingLayer = null;
                }

                // Перерисовка PictureBox
                pictureBox2.Invalidate();
            }
        }

        private void btn_measure_2_3_Click(object sender, EventArgs e)
        {
            if (textBox_2_1_Dist_A.Text == "" || textBox_2_2_Dist_B.Text == "")
            {
                MessageBox.Show("Требуемые значения не измерены!",
                                      "Ошибка",
                                      MessageBoxButtons.OK,
                                      MessageBoxIcon.Error);
            }
            else
            {
                SharedData.Dist_A = Convert.ToDouble(textBox_2_1_Dist_A.Text);
                SharedData.Dist_B = Convert.ToDouble(textBox_2_2_Dist_B.Text);
                textBox_2_3_Okano.Text = Convert.ToString((float)(SharedData.Dist_A / SharedData.Dist_B) * 100) + "%";
                SharedData.Okano = (SharedData.Dist_A / SharedData.Dist_B) * 100;
            }
        }

        private void textBox_2_3_Okano_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
