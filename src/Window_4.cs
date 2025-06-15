using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Medic
{
    public partial class Window_4 : Form
    {
        private Image clueImage_5;
        private bool isDrawingMode = false;
        private bool isDrawingCircleMode = true;
        private bool isFirstPointSet = false;
        private Point firstPoint;
        private Point currentMousePosition;
        private Bitmap drawingLayer;

        public Window_4()
        {
            InitializeComponent();
            LoadClueImage();
            pictureBox4.Image = SharedData.Radiograph_1;
            label_4_3.Visible = false;
            pictureBox4.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox4.MouseDown += PictureBox4_MouseDown;
            pictureBox4.MouseMove += PictureBox4_MouseMove;
            pictureBox4.Paint += PictureBox4_Paint;
        }

        private void LoadClueImage()
        {
            try
            {
                string clueImagePath = Path.Combine(Application.StartupPath, "clue_5.png");

                if (File.Exists(clueImagePath))
                {
                    clueImage_5 = Image.FromFile(clueImagePath);
                }
                else
                {
                    MessageBox.Show("Файл clue_5.png не найден в папке с программой!",
                                  "Предупреждение",
                                  MessageBoxButtons.OK,
                                  MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке clue_5.png: {ex.Message}",
                              "Ошибка",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Error);
            }
        }

        private void btn_switch_4_1_Click(object sender, EventArgs e)
        {
            pictureBox4.Image = SharedData.Radiograph_1;
        }

        private void btn_switch_4_2_Click(object sender, EventArgs e)
        {
            pictureBox4.Image = clueImage_5;
            drawingLayer?.Dispose();
            drawingLayer = null;
            pictureBox4.Invalidate();
        }

        private void btn_help_4_Click(object sender, EventArgs e)
        {
            label_4_3.Visible = !label_4_3.Visible;
        }

        private void btn_measure_4_1_Click(object sender, EventArgs e)
        {
            if (pictureBox4.Image != SharedData.Radiograph_1)
            {
                MessageBox.Show("Рентгенограммы 1 нет на экране!",
                      "Ошибка",
                      MessageBoxButtons.OK,
                      MessageBoxIcon.Error);
            }
            else
            {
                // Сброс состояния и включение режима рисования
                isDrawingMode = true;
                isDrawingCircleMode = true;
                isFirstPointSet = false;
                textBox_4_1_DA.Text = "0";

                // Очистка предыдущего рисунка
                if (drawingLayer != null)
                {
                    drawingLayer.Dispose();
                    drawingLayer = null;
                }

                // Перерисовка PictureBox
                pictureBox4.Invalidate();
            }
        }
        private void PictureBox4_MouseDown(object sender, MouseEventArgs e)
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
                // Установка второй точки и завершение рисования
                if (isDrawingCircleMode)
                    CreateFinalCircle(imagePoint);
                else
                    CreateFinalLine(imagePoint);

                isDrawingMode = false;
                isDrawingCircleMode = false;
                isFirstPointSet = false;
            }
        }

        private void PictureBox4_MouseMove(object sender, MouseEventArgs e)
        {
            if (!isDrawingMode || !isFirstPointSet)
                return;

            // Обновление текущей позиции мыши
            currentMousePosition = GetImageCoordinates(e.Location);

            // Расчёт и отображение длины/диаметра
            double length = CalculateDistance(firstPoint, currentMousePosition);

            textBox_4_1_DA.Text = Math.Round(length, 2).ToString();

            // Перерисовка для отображения временной фигуры
            pictureBox4.Invalidate();
        }

        private void PictureBox4_Paint(object sender, PaintEventArgs e)
        {
            // Отрисовка сохранённой фигуры
            if (drawingLayer != null && pictureBox4.Image != null)
            {
                // Получение коэффициентов масштабирования
                float scaleX = (float)pictureBox4.Width / pictureBox4.Image.Width;
                float scaleY = (float)pictureBox4.Height / pictureBox4.Image.Height;
                float scale = Math.Min(scaleX, scaleY);

                // Вычисление смещения для центрирования
                int offsetX = (int)((pictureBox4.Width - pictureBox4.Image.Width * scale) / 2);
                int offsetY = (int)((pictureBox4.Height - pictureBox4.Image.Height * scale) / 2);

                // Отрисовка слоя с фигурой
                e.Graphics.DrawImage(drawingLayer, offsetX, offsetY,
                    pictureBox4.Image.Width * scale, pictureBox4.Image.Height * scale);
            }

            // Отрисовка временной фигуры при рисовании
            if (isDrawingMode && isFirstPointSet)
            {
                using (Pen pen = new Pen(Color.Red, 2))
                {
                    Point displayFirst = GetDisplayCoordinates(firstPoint);
                    Point displayCurrent = GetDisplayCoordinates(currentMousePosition);

                    if (isDrawingCircleMode)
                    {
                        // Рисование окружности
                        DrawCircleByDiameter(e.Graphics, pen, displayFirst, displayCurrent);
                    }
                    else
                    {
                        // Рисование линии
                        e.Graphics.DrawLine(pen, displayFirst, displayCurrent);

                        // Отрисовка точек
                        e.Graphics.FillEllipse(Brushes.Red, displayFirst.X - 3, displayFirst.Y - 3, 6, 6);
                        e.Graphics.FillEllipse(Brushes.Red, displayCurrent.X - 3, displayCurrent.Y - 3, 6, 6);
                    }
                }
            }
        }

        private void CreateFinalLine(Point secondPoint)
        {
            if (pictureBox4.Image == null)
                return;

            // Создание слоя для рисования
            drawingLayer = new Bitmap(pictureBox4.Image.Width, pictureBox4.Image.Height);

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
            textBox_4_1_DA.Text = Math.Round(length, 2).ToString();

            pictureBox4.Invalidate();
        }

        private void CreateFinalCircle(Point secondPoint)
        {
            if (pictureBox4.Image == null)
                return;

            // Создание слоя для рисования
            drawingLayer = new Bitmap(pictureBox4.Image.Width, pictureBox4.Image.Height);

            using (Graphics g = Graphics.FromImage(drawingLayer))
            {
                g.Clear(Color.Transparent);
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                using (Pen pen = new Pen(Color.Red, 2))
                {
                    // Рисование окружности на слое изображения
                    DrawCircleByDiameterOnImage(g, pen, firstPoint, secondPoint);
                }
            }

            // Расчёт финального диаметра
            double diameter = CalculateDistance(firstPoint, secondPoint);
            textBox_4_1_DA.Text = Math.Round(diameter, 2).ToString();

            pictureBox4.Invalidate();
        }

        private void DrawCircleByDiameter(Graphics g, Pen pen, Point p1, Point p2)
        {
            // Вычисление центра окружности (середина диаметра)
            int centerX = (p1.X + p2.X) / 2;
            int centerY = (p1.Y + p2.Y) / 2;

            // Вычисление радиуса
            double diameter = CalculateDistance(p1, p2);
            float radius = (float)(diameter / 2);

            // Рисование окружности
            if (radius > 0)
            {
                g.DrawEllipse(pen, centerX - radius, centerY - radius, radius * 2, radius * 2);
            }

            // Рисование диаметра
            g.DrawLine(pen, p1, p2);

            // Отрисовка точек на концах диаметра
            g.FillEllipse(Brushes.Red, p1.X - 3, p1.Y - 3, 6, 6);
            g.FillEllipse(Brushes.Red, p2.X - 3, p2.Y - 3, 6, 6);

            // Отрисовка центра окружности
            using (Brush centerBrush = new SolidBrush(Color.Blue))
            {
                g.FillEllipse(centerBrush, centerX - 2, centerY - 2, 4, 4);
            }
        }

        private void DrawCircleByDiameterOnImage(Graphics g, Pen pen, Point p1, Point p2)
        {
            // Вычисление центра окружности (середина диаметра)
            int centerX = (p1.X + p2.X) / 2;
            int centerY = (p1.Y + p2.Y) / 2;

            // Вычисление радиуса
            double diameter = CalculateDistance(p1, p2);
            float radius = (float)(diameter / 2);

            // Рисование окружности
            if (radius > 0)
            {
                g.DrawEllipse(pen, centerX - radius, centerY - radius, radius * 2, radius * 2);
            }

            // Рисование диаметра
            g.DrawLine(pen, p1, p2);

            // Отрисовка точек на концах диаметра
            g.FillEllipse(Brushes.Red, p1.X - 3, p1.Y - 3, 6, 6);
            g.FillEllipse(Brushes.Red, p2.X - 3, p2.Y - 3, 6, 6);

            // Отрисовка центра окружности
            using (Brush centerBrush = new SolidBrush(Color.Blue))
            {
                g.FillEllipse(centerBrush, centerX - 2, centerY - 2, 4, 4);
            }
        }

        private Point GetImageCoordinates(Point pictureBoxPoint)
        {
            if (pictureBox4.Image == null)
                return Point.Empty;

            // Получение размеров и позиции изображения в PictureBox
            float scaleX = (float)pictureBox4.Width / pictureBox4.Image.Width;
            float scaleY = (float)pictureBox4.Height / pictureBox4.Image.Height;
            float scale = Math.Min(scaleX, scaleY);

            int offsetX = (int)((pictureBox4.Width - pictureBox4.Image.Width * scale) / 2);
            int offsetY = (int)((pictureBox4.Height - pictureBox4.Image.Height * scale) / 2);

            // Преобразование координат
            int imageX = (int)((pictureBoxPoint.X - offsetX) / scale);
            int imageY = (int)((pictureBoxPoint.Y - offsetY) / scale);

            // Ограничение координат размерами изображения
            imageX = Math.Max(0, Math.Min(imageX, pictureBox4.Image.Width - 1));
            imageY = Math.Max(0, Math.Min(imageY, pictureBox4.Image.Height - 1));

            return new Point(imageX, imageY);
        }

        private Point GetDisplayCoordinates(Point imagePoint)
        {
            if (pictureBox4.Image == null)
                return Point.Empty;

            float scaleX = (float)pictureBox4.Width / pictureBox4.Image.Width;
            float scaleY = (float)pictureBox4.Height / pictureBox4.Image.Height;
            float scale = Math.Min(scaleX, scaleY);

            int offsetX = (int)((pictureBox4.Width - pictureBox4.Image.Width * scale) / 2);
            int offsetY = (int)((pictureBox4.Height - pictureBox4.Image.Height * scale) / 2);

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

        private void btn_measure_4_2_Click(object sender, EventArgs e)
        {
            if (textBox_4_1_DA.Text == "")
            {
                MessageBox.Show("Требуемые значения не измерены!",
                                      "Ошибка",
                                      MessageBoxButtons.OK,
                                      MessageBoxIcon.Error);
            }
            else
            {
                SharedData.Da = Convert.ToDouble(textBox_4_1_DA.Text);
                SharedData.ISA = SharedData.Da / (SharedData.R * 0.5);
                textBox_4_2_ISA.Text = Convert.ToString((float)SharedData.ISA);
                SharedData.ICAS = SharedData.ISA / SharedData.ISh;
                textBox_4_3_ICAS.Text = Convert.ToString((float)SharedData.ICAS);
            }
        }

        private void btn_next_4_Click(object sender, EventArgs e)
        {
            if (textBox_4_2_ISA.Text == "" || textBox_4_3_ICAS.Text == "")
            {
                MessageBox.Show("Требуемые значения не измерены!",
                                      "Ошибка",
                                      MessageBoxButtons.OK,
                                      MessageBoxIcon.Error);
            }
            else
            {
                Window_5 w5 = new Window_5();
                w5.Show();
                Hide();
            }
        }
    }   
}
