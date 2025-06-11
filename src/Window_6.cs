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
    public partial class Window_6 : Form
    {
        private Image clueImage_7;
        private bool isDrawingMode = false;
        private bool isDrawingDist_W;
        private bool isFirstPointSet = false;
        private Point firstPoint;
        private Point currentMousePosition;
        private Bitmap drawingLayer;
        public Window_6()
        {
            InitializeComponent();
            LoadClueImage();
            pictureBox6.Image = SharedData.Radiograph_1;
            label_6_4.Visible = false;
            pictureBox6.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox6.MouseDown += PictureBox6_MouseDown;
            pictureBox6.MouseMove += PictureBox6_MouseMove;
            pictureBox6.Paint += PictureBox6_Paint;
        }

        private void btn_switch_6_1_Click(object sender, EventArgs e)
        {
            pictureBox6.Image = SharedData.Radiograph_1;
        }

        private void btn_switch_6_2_Click(object sender, EventArgs e)
        {
            pictureBox6.Image = clueImage_7;
            drawingLayer?.Dispose();
            drawingLayer = null;
            pictureBox6.Invalidate();
        }

        private void btn_help_6_Click(object sender, EventArgs e)
        {
            label_6_4.Visible = !label_6_4.Visible;
        }
        private void LoadClueImage()
        {
            try
            {
                string clueImagePath = Path.Combine(Application.StartupPath, "clue_7.png");

                if (File.Exists(clueImagePath))
                {
                    clueImage_7 = Image.FromFile(clueImagePath);
                }
                else
                {
                    MessageBox.Show("Файл clue_7.png не найден в папке с программой!",
                                  "Предупреждение",
                                  MessageBoxButtons.OK,
                                  MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке clue_7.png: {ex.Message}",
                              "Ошибка",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Error);
            }
        }

        private void btn_measure_6_1_Click(object sender, EventArgs e)
        {
            if (pictureBox6.Image != SharedData.Radiograph_1)
            {
                MessageBox.Show("Рентгенограммы 1 нет на экране!",
                      "Ошибка",
                      MessageBoxButtons.OK,
                      MessageBoxIcon.Error);
            }
            else
            {
                // Сброс состояния и включение режима рисования
                isDrawingDist_W = false;
                isDrawingMode = true;
                isFirstPointSet = false;
                textBox_6_1_Dist_D.Text = "0";

                // Очистка предыдущего рисунка
                if (drawingLayer != null)
                {
                    drawingLayer.Dispose();
                    drawingLayer = null;
                }

                // Перерисовка PictureBox
                pictureBox6.Invalidate();
            }
        }

        private void btn_measure_6_2_Click(object sender, EventArgs e)
        {
            if (pictureBox6.Image != SharedData.Radiograph_1)
            {
                MessageBox.Show("Рентгенограммы 1 нет на экране!",
                      "Ошибка",
                      MessageBoxButtons.OK,
                      MessageBoxIcon.Error);
            }
            else
            {
                // Сброс состояния и включение режима рисования
                isDrawingDist_W = true;
                isDrawingMode = true;
                isFirstPointSet = false;
                textBox_6_2_Dist_W.Text = "0";

                // Очистка предыдущего рисунка
                if (drawingLayer != null)
                {
                    drawingLayer.Dispose();
                    drawingLayer = null;
                }

                // Перерисовка PictureBox
                pictureBox6.Invalidate();
            }
        }
        private void PictureBox6_MouseDown(object sender, MouseEventArgs e)
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

        private void PictureBox6_MouseMove(object sender, MouseEventArgs e)
        {
            if (!isDrawingMode || !isFirstPointSet)
                return;

            // Обновление текущей позиции мыши
            currentMousePosition = GetImageCoordinates(e.Location);

            // Расчёт и отображение длины/диаметра
            double length = CalculateDistance(firstPoint, currentMousePosition);

            if (!isDrawingDist_W)
            {
                textBox_6_1_Dist_D.Text = Math.Round(length, 2).ToString();
            }
            else textBox_6_2_Dist_W.Text = Math.Round(length, 2).ToString();

            // Перерисовка для отображения временной фигуры
            pictureBox6.Invalidate();
        }

        private void PictureBox6_Paint(object sender, PaintEventArgs e)
        {
            // Отрисовка сохранённой фигуры
            if (drawingLayer != null && pictureBox6.Image != null)
            {
                // Получение коэффициентов масштабирования
                float scaleX = (float)pictureBox6.Width / pictureBox6.Image.Width;
                float scaleY = (float)pictureBox6.Height / pictureBox6.Image.Height;
                float scale = Math.Min(scaleX, scaleY);

                // Вычисление смещения для центрирования
                int offsetX = (int)((pictureBox6.Width - pictureBox6.Image.Width * scale) / 2);
                int offsetY = (int)((pictureBox6.Height - pictureBox6.Image.Height * scale) / 2);

                // Отрисовка слоя с фигурой
                e.Graphics.DrawImage(drawingLayer, offsetX, offsetY,
                    pictureBox6.Image.Width * scale, pictureBox6.Image.Height * scale);
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
            if (pictureBox6.Image == null)
                return;

            // Создание слоя для рисования
            drawingLayer = new Bitmap(pictureBox6.Image.Width, pictureBox6.Image.Height);

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
            if (!isDrawingDist_W)
            {
                textBox_6_1_Dist_D.Text = Math.Round(length, 2).ToString();
            }
            else textBox_6_2_Dist_W.Text = Math.Round(length, 2).ToString();

            pictureBox6.Invalidate();
        }

        private Point GetImageCoordinates(Point pictureBoxPoint)
        {
            if (pictureBox6.Image == null)
                return Point.Empty;

            // Получение размеров и позиции изображения в PictureBox
            float scaleX = (float)pictureBox6.Width / pictureBox6.Image.Width;
            float scaleY = (float)pictureBox6.Height / pictureBox6.Image.Height;
            float scale = Math.Min(scaleX, scaleY);

            int offsetX = (int)((pictureBox6.Width - pictureBox6.Image.Width * scale) / 2);
            int offsetY = (int)((pictureBox6.Height - pictureBox6.Image.Height * scale) / 2);

            // Преобразование координат
            int imageX = (int)((pictureBoxPoint.X - offsetX) / scale);
            int imageY = (int)((pictureBoxPoint.Y - offsetY) / scale);

            // Ограничение координат размерами изображения
            imageX = Math.Max(0, Math.Min(imageX, pictureBox6.Image.Width - 1));
            imageY = Math.Max(0, Math.Min(imageY, pictureBox6.Image.Height - 1));

            return new Point(imageX, imageY);
        }

        private Point GetDisplayCoordinates(Point imagePoint)
        {
            if (pictureBox6.Image == null)
                return Point.Empty;

            float scaleX = (float)pictureBox6.Width / pictureBox6.Image.Width;
            float scaleY = (float)pictureBox6.Height / pictureBox6.Image.Height;
            float scale = Math.Min(scaleX, scaleY);

            int offsetX = (int)((pictureBox6.Width - pictureBox6.Image.Width * scale) / 2);
            int offsetY = (int)((pictureBox6.Height - pictureBox6.Image.Height * scale) / 2);

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

        private void btn_measure_6_3_Click(object sender, EventArgs e)
        {
            if (textBox_6_1_Dist_D.Text == "" || textBox_6_2_Dist_W.Text == "")
            {
                MessageBox.Show("Требуемые значения не измерены!",
                                      "Ошибка",
                                      MessageBoxButtons.OK,
                                      MessageBoxIcon.Error);
            }
            else
            {
                SharedData.Dist_D = Convert.ToDouble(textBox_6_1_Dist_D.Text);
                SharedData.Dist_W = Convert.ToDouble(textBox_6_2_Dist_W.Text);
                SharedData.AK = (SharedData.Dist_D / SharedData.Dist_W) * 1000;
                textBox_6_3_AK.Text = Convert.ToString((float)SharedData.AK);
            }
        }

        private void btn_next_6_Click(object sender, EventArgs e)
        {
            if (textBox_6_3_AK.Text == "")
            {
                MessageBox.Show("Ацетабулярный коэффициент не измерен!",
                                      "Ошибка",
                                      MessageBoxButtons.OK,
                                      MessageBoxIcon.Error);
            }
            else
            {
                Window_7 w7 = new Window_7();
                w7.Show();
                Hide();
            }
        }
    }
}
