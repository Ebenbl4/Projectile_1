using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Medic
{
    public partial class Window_5 : Form
    {
        private Image clueImage_6;
        private enum DrawingPhase
        {
            None,           // Ничего не рисуется
            WaitingForP1,   // Ожидание первой точки
            WaitingForP2,   // Ожидание второй точки (первая линия динамическая)
            WaitingForP3,   // Ожидание третьей точки (вторая линия и угол динамические)
            DrawingCompleted // Все три точки установлены
        }

        private DrawingPhase currentPhase = DrawingPhase.None;
        private List<PointF> drawnPoints = new List<PointF>();
        private PointF currentCursorPoint; // Текущая позиция курсора для динамического рисования
        private TextBox activeAngleTextBox = null; // TextBox для вывода текущего угла
        private Pen drawingPen; // Перо для рисования

        private const float ARC_RADIUS = 30f; // Радиус дуги для отображения угла
        public Window_5()
        {
            InitializeComponent();
            LoadClueImage();
            pictureBox5.Image = SharedData.Radiograph_1;
            label_5_3.Visible = false;
            DoubleBuffered = true;
            drawingPen = new Pen(Color.Yellow, 3);
        }

        private void btn_switch_5_1_Click(object sender, EventArgs e)
        {
            pictureBox5.Image = SharedData.Radiograph_1;
        }

        private void btn_switch_5_2_Click(object sender, EventArgs e)
        {
            pictureBox5.Image = clueImage_6;
            drawnPoints.Clear();
            pictureBox5.Invalidate();
        }

        private void btn_help_5_Click(object sender, EventArgs e)
        {
            label_5_3.Visible = !label_5_3.Visible;
        }
        private void LoadClueImage()
        {
            try
            {
                string clueImagePath = Path.Combine(Application.StartupPath, "clue_6.png");

                if (File.Exists(clueImagePath))
                {
                    clueImage_6 = Image.FromFile(clueImagePath);
                }
                else
                {
                    MessageBox.Show("Файл clue_6.png не найден в папке с программой!",
                                  "Предупреждение",
                                  MessageBoxButtons.OK,
                                  MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке clue_6.png: {ex.Message}",
                              "Ошибка",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Error);
            }
        }

        private void btn_measure_5_1_Click(object sender, EventArgs e)
        {
            if (pictureBox5.Image != SharedData.Radiograph_1)
            {
                MessageBox.Show("Рентгенограммы 1 нет на экране!",
                      "Ошибка",
                      MessageBoxButtons.OK,
                      MessageBoxIcon.Error);
            }
            else StartMeasurement(textBox_5_1_Angle_V);
        }

        private void StartMeasurement(TextBox targetTextBox)
        {
            currentPhase = DrawingPhase.WaitingForP1;
            drawnPoints.Clear();
            activeAngleTextBox = targetTextBox;
            if (activeAngleTextBox != null)
            {
                activeAngleTextBox.Text = ""; // Очищаем поле угла
            }
            pictureBox5.Invalidate(); // Перерисовать PictureBox (очистит предыдущие линии)
            // Убедимся, что PictureBox может получать фокус для событий мыши, если нужно
            // pictureBox3.Focus(); // Обычно не требуется для PictureBox
        }

        // Событие нажатия кнопки мыши на PictureBox
        private void pictureBox3_MouseDown(object sender, MouseEventArgs e)
        {
            if (activeAngleTextBox == null || e.Button != MouseButtons.Left)
                return;

            switch (currentPhase)
            {
                case DrawingPhase.WaitingForP1:
                    drawnPoints.Add(e.Location);
                    currentPhase = DrawingPhase.WaitingForP2;
                    currentCursorPoint = e.Location; // Инициализируем для первого кадра
                    break;
                case DrawingPhase.WaitingForP2:
                    drawnPoints.Add(e.Location);
                    currentPhase = DrawingPhase.WaitingForP3;
                    currentCursorPoint = e.Location; // Инициализируем для первого кадра
                    break;
                case DrawingPhase.WaitingForP3:
                    drawnPoints.Add(e.Location);
                    currentPhase = DrawingPhase.DrawingCompleted;
                    UpdateAngleDisplay(true); // Обновить угол в TextBox окончательно
                    break;
            }
            pictureBox5.Invalidate(); // Запросить перерисовку
        }

        // Событие перемещения мыши над PictureBox
        private void pictureBox3_MouseMove(object sender, MouseEventArgs e)
        {
            if (activeAngleTextBox == null)
                return;

            currentCursorPoint = e.Location;

            if (currentPhase == DrawingPhase.WaitingForP2 || currentPhase == DrawingPhase.WaitingForP3)
            {
                if (currentPhase == DrawingPhase.WaitingForP3 && drawnPoints.Count == 2)
                {
                    UpdateAngleDisplay(false); // Динамическое обновление угла в TextBox
                }
                pictureBox5.Invalidate(); // Запросить перерисовку для динамического отображения
            }
        }

        // Событие перерисовки PictureBox
        private void pictureBox3_Paint(object sender, PaintEventArgs e)
        {
            // Устанавливаем высокое качество рендеринга
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            if (activeAngleTextBox == null && currentPhase == DrawingPhase.None)
            {
                // Если не в режиме измерения, ничего не рисуем (кроме базового изображения PictureBox)
                return;
            }

            // Рисуем первую линию (если есть первая точка и мы ждем вторую)
            if (drawnPoints.Count >= 1 && currentPhase == DrawingPhase.WaitingForP2)
            {
                e.Graphics.DrawLine(drawingPen, drawnPoints[0], currentCursorPoint);
            }
            // Рисуем первую зафиксированную линию и вторую динамическую + дугу
            else if (drawnPoints.Count >= 2 && currentPhase == DrawingPhase.WaitingForP3)
            {
                // Первая линия (P0-P1)
                e.Graphics.DrawLine(drawingPen, drawnPoints[0], drawnPoints[1]);
                // Вторая линия (P1-Cursor)
                e.Graphics.DrawLine(drawingPen, drawnPoints[1], currentCursorPoint);
                // Рисуем дугу и угол
                DrawAngleArc(e.Graphics, drawnPoints[0], drawnPoints[1], currentCursorPoint);
            }
            // Рисуем обе зафиксированные линии и дугу
            else if (drawnPoints.Count == 3 && (currentPhase == DrawingPhase.DrawingCompleted || currentPhase == DrawingPhase.WaitingForP1)) // WaitingForP1 - если пользователь снова нажал кнопку
            {
                // Первая линия (P0-P1)
                e.Graphics.DrawLine(drawingPen, drawnPoints[0], drawnPoints[1]);
                // Вторая линия (P1-P2)
                e.Graphics.DrawLine(drawingPen, drawnPoints[1], drawnPoints[2]);
                // Рисуем дугу и угол
                DrawAngleArc(e.Graphics, drawnPoints[0], drawnPoints[1], drawnPoints[2]);
            }
            // Если только первая точка поставлена, но фаза уже не WaitingForP2 (например, сразу нажали кнопку сброса)
            // или если просто очистили и еще ничего не начали рисовать, то ничего дополнительно не рисуем.
        }

        // Метод для расчета угла и обновления TextBox
        private void UpdateAngleDisplay(bool isFinal)
        {
            if (activeAngleTextBox == null) return;

            PointF p0, pVertex, pEndArm;

            if (drawnPoints.Count == 2 && currentPhase == DrawingPhase.WaitingForP3) // Динамический расчет для второй линии
            {
                p0 = drawnPoints[0];
                pVertex = drawnPoints[1];
                pEndArm = currentCursorPoint;
            }
            else if (drawnPoints.Count == 3 && isFinal) // Финальный расчет
            {
                p0 = drawnPoints[0];
                pVertex = drawnPoints[1];
                pEndArm = drawnPoints[2];
            }
            else
            {
                // activeAngleTextBox.Text = ""; // Очищаем, если недостаточно точек
                return;
            }

            double angle = CalculateAngleBetweenPoints(p0, pVertex, pEndArm);
            activeAngleTextBox.Text = angle.ToString("F3");
        }


        // Метод для рисования дуги угла
        private void DrawAngleArc(Graphics g, PointF p0, PointF pVertex, PointF pEndArm)
        {
            // Проверка, что точки не совпадают с вершиной, чтобы избежать ошибок Math.Atan2
            if (pVertex.Equals(p0) || pVertex.Equals(pEndArm))
            {
                return; // Невозможно построить угол
            }

            // Угол между вектором (pVertex -> p0) и осью X
            double startAngleRad = Math.Atan2(p0.Y - pVertex.Y, p0.X - pVertex.X);
            // Угол между вектором (pVertex -> pEndArm) и осью X
            double endAngleRad = Math.Atan2(pEndArm.Y - pVertex.Y, pEndArm.X - pVertex.X);

            float startAngleDeg = (float)(startAngleRad * 180.0 / Math.PI);
            float endAngleDeg = (float)(endAngleRad * 180.0 / Math.PI);

            float sweepAngleDeg = endAngleDeg - startAngleDeg;

            // Нормализация sweepAngle, чтобы он был в диапазоне (-360, 360)
            // и соответствовал наименьшему углу между линиями
            if (sweepAngleDeg > 180f)
            {
                sweepAngleDeg -= 360f;
            }
            else if (sweepAngleDeg < -180f)
            {
                sweepAngleDeg += 360f;
            }

            // Если угол очень маленький, дуга может не нарисоваться или выглядеть странно.
            // Также избегаем рисования дуги, если линии коллинеарны (угол 0 или 180).
            // Для CalculateAngleBetweenPoints это не проблема, но для DrawArc может быть.
            double calculatedAngle = CalculateAngleBetweenPoints(p0, pVertex, pEndArm);
            if (calculatedAngle > 0.1 && calculatedAngle < 179.9) // Не рисуем дугу для слишком маленьких/больших углов
            {
                RectangleF arcRect = new RectangleF(
                   pVertex.X - ARC_RADIUS,
                   pVertex.Y - ARC_RADIUS,
                   2 * ARC_RADIUS,
                   2 * ARC_RADIUS);
                g.DrawArc(drawingPen, arcRect, startAngleDeg, sweepAngleDeg);
            }
        }

        // Метод для вычисления угла между тремя точками (pVertex - вершина угла)
        public static double CalculateAngleBetweenPoints(PointF p0, PointF pVertex, PointF pEndArm)
        {
            // Векторы от вершины угла
            double v1x = p0.X - pVertex.X;
            double v1y = p0.Y - pVertex.Y;
            double v2x = pEndArm.X - pVertex.X;
            double v2y = pEndArm.Y - pVertex.Y;

            // Скалярное произведение
            double dotProduct = v1x * v2x + v1y * v2y;

            // Длины векторов
            double mag1 = Math.Sqrt(v1x * v1x + v1y * v1y);
            double mag2 = Math.Sqrt(v2x * v2x + v2y * v2y);

            // Если длина одного из векторов равна 0, угол не определен (или 0)
            if (mag1 == 0 || mag2 == 0)
                return 0.0;

            // Косинус угла
            double cosTheta = dotProduct / (mag1 * mag2);

            // Иногда из-за ошибок округления cosTheta может быть чуть > 1 или < -1
            cosTheta = Math.Max(-1.0, Math.Min(1.0, cosTheta));

            // Угол в радианах
            double angleRad = Math.Acos(cosTheta);

            // Угол в градусах
            return angleRad * (180.0 / Math.PI);
        }

        private void btn_next_5_Click(object sender, EventArgs e)
        {
            if (textBox_5_1_Angle_V.Text == "")
            {
                MessageBox.Show("Угол В не измерен!",
                                      "Ошибка",
                                      MessageBoxButtons.OK,
                                      MessageBoxIcon.Error);
            }
            else
            {
                SharedData.Angle_V = Convert.ToDouble(textBox_5_1_Angle_V.Text);
                Window_6 w6 = new Window_6();
                w6.Show();
                Hide();
            }
        }
    }
}
