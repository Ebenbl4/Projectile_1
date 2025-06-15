using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Medic
{
    public partial class Window_9 : Form
    {
        private Image clueImage_10;
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

        public Window_9()
        {
            InitializeComponent();
            pictureBox9.Image = SharedData.Radiograph_4;
            LoadClueImage();
            btn_switch_9_1.Enabled = false;
            btn_switch_9_2.Enabled = false;
            label_9_4.Visible = false;
            DoubleBuffered = true;
            drawingPen = new Pen(Color.Yellow, 3);
        }

        private void LoadClueImage()
        {
            try
            {
                string clueImagePath = Path.Combine(Application.StartupPath, "clue_10.png");

                if (File.Exists(clueImagePath))
                {
                    clueImage_10 = Image.FromFile(clueImagePath);
                }
                else
                {
                    MessageBox.Show("Файл clue_10.png не найден в папке с программой!",
                                  "Предупреждение",
                                  MessageBoxButtons.OK,
                                  MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке clue_10.png: {ex.Message}",
                              "Ошибка",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Error);
            }
        }

        private void btn_select_4_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "Выберите изображение";
                openFileDialog.Filter = "Файлы изображений|*.png;*.jpg;*.jpeg|PNG файлы|*.png|JPEG файлы|*.jpg;*.jpeg";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        SharedData.Radiograph_4 = (Bitmap)Image.FromFile(openFileDialog.FileName);
                        pictureBox9.Image = SharedData.Radiograph_4;

                        btn_switch_9_1.Enabled = true;
                        btn_switch_9_2.Enabled = (clueImage_10 != null);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при загрузке изображения: {ex.Message}",
                                      "Ошибка",
                                      MessageBoxButtons.OK,
                                      MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btn_switch_9_1_Click(object sender, EventArgs e)
        {
            pictureBox9.Image = SharedData.Radiograph_4;
        }

        private void btn_switch_9_2_Click(object sender, EventArgs e)
        {
            pictureBox9.Image = clueImage_10;
            drawnPoints.Clear();
            pictureBox9.Invalidate();
        }

        private void btn_help_9_Click(object sender, EventArgs e)
        {
            label_9_4.Visible = !label_9_4.Visible;
        }

        private void btn_measure_9_1_Click(object sender, EventArgs e)
        {
            if (SharedData.Radiograph_4 != null)
            {
                if (pictureBox9.Image != SharedData.Radiograph_4)
                {
                    MessageBox.Show("Рентгенограммы 4 нет на экране!",
                          "Ошибка",
                          MessageBoxButtons.OK,
                          MessageBoxIcon.Error);
                }
                else StartMeasurement(textBox_9_1_Angle_E);
            }
            else
            {
                MessageBox.Show("Сначала загрузите рентгенограмму!",
                        "Ошибка",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
            }
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
            pictureBox9.Invalidate(); // Перерисовать PictureBox (очистит предыдущие линии)
            // Убедимся, что PictureBox может получать фокус для событий мыши, если нужно
            // pictureBox3.Focus(); // Обычно не требуется для PictureBox
        }

        // Событие нажатия кнопки мыши на PictureBox
        private void pictureBox9_MouseDown(object sender, MouseEventArgs e)
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
            pictureBox9.Invalidate(); // Запросить перерисовку
        }

        // Событие перемещения мыши над PictureBox
        private void pictureBox9_MouseMove(object sender, MouseEventArgs e)
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
                pictureBox9.Invalidate(); // Запросить перерисовку для динамического отображения
            }
        }

        // Событие перерисовки PictureBox
        private void pictureBox9_Paint(object sender, PaintEventArgs e)
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

        public static string FormatFloat(double value)
        {
            return value.ToString("F2");
        }

        public static string GenerateVerdictFor13AndOlder(string PHIO, int year, double ICAS, double angle_A, double angle_B, double angle_V, double angle_Vibert, double angle_D, double angle_E, double dist_D, double dist_W, double UOB, double Dh, double AK, double SLN, double SPN, double HP, double BP, double trans)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{\\rtf1\\ansi\\ansicpg1251\\deff0\n");
            sb.Append("{\\fonttbl{\\f0 Times New Roman;}}\n");
            sb.Append("\\pard\\sl360\\slmult1\n");
            sb.Append("\\f0\\fs28\n");

            if (ICAS >= 1.0 && ICAS <= 1.7)
            {
                if (angle_A <= 35)
                {
                    if (angle_B >= 15)
                    {
                        sb.Append("\\qc \\fs32 Вердикт 1\\par \\par");
                        sb.Append("\\ql \\fs28     " + PHIO + "\\par");
                        sb.Append("\\ql \\fs28     Год рождения - " + year.ToString() + "\\par \\par");
                        sb.Append("\\ql \\fs28 Угол наклона опорной поверхности (Б) равен " + FormatFloat(angle_B) + "\\par");
                        sb.Append("\\ql \\fs28 Ацетабулярный коэффициент АК равен " + FormatFloat(AK) + "\\par Показана остеотомия таза.\\par");
                        sb.Append("\\ql \\fs28 Степень латерального наклона ацетабулярного фрагмента равна " + FormatFloat(SLN) + "\\par");
                        sb.Append("\\ql \\fs28 Степень переднего наклона ацетабулярного фрагмента равна " + FormatFloat(SPN) + "\\par");
                        if (angle_V >= 10 && angle_V <= 20)
                        {
                        }
                        else if (angle_V > 20)
                        {
                            sb.Append("\\ql \\fs28 А Наружная ротация ацетабулярного фрагмента НР равна " + FormatFloat(HP) + "\\par");
                        }
                        else if (angle_V < 10)
                        {
                            sb.Append("\\ql \\fs28 Б Внутренняя ротация ацетабулярного фрагмента ВР равна " + FormatFloat(BP) + "\\par");
                        }
                    }
                    else
                    {
                        sb.Append("\\qc \\fs32 Вердикт 0\\par \\par");
                        sb.Append("\\ql \\fs28     " + PHIO + "\\par");
                        sb.Append("\\ql \\fs28     Год рождения - " + year.ToString() + "\\par \\par");
                        sb.Append("\\qc \\fs32 Угол наклона опорной поверхности Б < 15°\\par В данном случае реконструктивная операция не показана\\par");
                    }
                }
                else
                {
                    sb.Append("\\qc \\fs32 Вердикт 0\\par \\par");
                    sb.Append("\\ql \\fs28     " + PHIO + "\\par");
                    sb.Append("\\ql \\fs28     Год рождения - " + year.ToString() + "\\par \\par");
                    sb.Append("\\qc \\fs32 Угол наклона опорной поверхности Б < 15°\\par В данном случае реконструктивная операция не показана\\par");
                }
            }
            else if (ICAS > 1.7)
            {
                string text = "\\ql \\fs28 Ацетабулярный коэффициент АК равен " + FormatFloat(AK) + "\\par";
                if (angle_A > 35)
                {
                    text += "\\ql \\fs28 Угол наклона опорной поверхности (Б) равен " + FormatFloat(angle_B) + "\\par";
                    if (angle_V >= 10 && angle_V <= 20)
                    {
                        sb.Append("\\qc \\fs32 Вердикт 2\\par\\par");
                        sb.Append("\\ql \\fs28     " + PHIO + "\\par");
                        sb.Append("\\ql \\fs28     Год рождения - " + year.ToString() + "\\par\\par");
                        sb.Append(text);
                        sb.Append("\\ql \\fs28 Показана неполная периацетабулярная остеотомия (ПАО)\\par");
                        sb.Append("\\ql \\fs28 Наклон ацетабулярного фрагмента во фронтальной и сагиттальной плоскости: \\par");
                        sb.Append("\\ql \\fs28 Степень латерального наклона (фронтальная плоскость) равна " + FormatFloat(angle_B) + "\\par");
                        sb.Append("\\ql \\fs28 Степень латерального наклона соответствует углу наклона опорной поверхности Б\\par");
                        sb.Append("\\ql \\fs28 Степень переднего наклона (сагиттальная плоскость) равна " + FormatFloat(SPN) + "\\par");
                    }
                    else
                    {
                        sb.Append("\\qc \\fs32 Вердикт 3\\par\\par");
                        sb.Append("\\ql \\fs28     " + PHIO + "\\par");
                        sb.Append("\\ql \\fs28     Год рождения - " + year.ToString() + "\\par\\par");
                        sb.Append(text);
                        sb.Append("Показана остеотомия таза + неполная периацетабулярная остеотомия (ПАО)\\par");
                        sb.Append("За счет остеотомии таза осуществляется наклон в сагиттальной и горизонтальной плоскости\\par");
                        sb.Append("Сагиттальная плоскость (передний наклон) Степень переднего наклона фрагмента равна " + FormatFloat(SPN) + "\\par");
                        if (angle_V > 20)
                        {
                            sb.Append("\\ql \\fs28 А Горизонтальная плоскость. \\par");
                            sb.Append("\\ql \\fs28 Наружная ротация ацетабулярного фрагмента НР равна " + FormatFloat(HP) + "\\par");
                        }
                        else if (angle_V < 10)
                        {
                            sb.Append("\\ql \\fs28 Б Горизонтальная плоскость.\\par");
                            sb.Append("\\ql \\fs28 Внутренняя ротация ацетабулярного фрагмента ВР равна " + FormatFloat(BP) + "\\par");
                        }
                        sb.Append("\\ql \\fs28 За счет неполной ПАО осуществляется латеральный наклон\\par");
                        sb.Append("\\ql \\fs28 Степень латерального наклона соответствует углу наклона опорной поверхности Б = " + FormatFloat(angle_B) + "\\par"); //
                    }
                }
                else if (angle_A <= 35)
                {
                    sb.Append("\\qc \\fs32 Вердикт 4\\par\\par");
                    sb.Append("\\ql \\fs28     " + PHIO + "\\par");
                    sb.Append("\\ql \\fs28     Год рождения - " + year.ToString() + "\\par\\par");
                    sb.Append(text);
                    sb.Append("\\ql \\fs28 Угол наклона опорной поверхности (Б) равен " + FormatFloat(angle_B) + "\\par");
                    sb.Append("\\ql \\fs28 Показана остеотомия таза + неполная периацетабулярная остеотомия\\par");
                    if (angle_V >= 10 && angle_V <= 20)
                    {
                        sb.Append("\\ql \\fs28 А. За счет остеотомии таза осуществляется наклон во фронтальной и сагиттальной плоскости\\par");
                        sb.Append("\\ql \\fs28 Во фронтальной плоскости (латеральный наклон): \\par");
                        sb.Append("\\ql \\fs28 Степень латерального наклона ацетабулярного фрагмента (СЛН) равна " + FormatFloat(SLN) + "\\par");
                        sb.Append("\\ql \\fs28 В сагиттальной плоскости (передний наклон): \\par");
                        sb.Append("\\ql \\fs28 Степень переднего наклона ацетабулярного фрагмента (СПН) равна " + FormatFloat(SPN) + "\\par");
                    }
                    else if (angle_V > 20)
                    {
                        sb.Append("\\ql \\fs28 Б. За счет остеотомии таза осуществляется наклон во фронтальной, сагиттальной и горизонтальной плоскости\\par");
                        sb.Append("\\ql \\fs28 Во фронтальной плоскости (латеральный наклон):\\par");
                        sb.Append("\\ql \\fs28 Степень латерального наклона ацетабулярного фрагмента (СЛН) равна " + FormatFloat(SLN) + "\\par");
                        sb.Append("\\ql \\fs28 В сагиттальной плоскости (передний наклон):\\par");
                        sb.Append("\\ql \\fs28 Степень переднего наклона ацетабулярного фрагмента (СПН) равна " + FormatFloat(SPN) + "\\par");
                        sb.Append("\\ql \\fs28 В горизонтальной плоскости:\\par");
                        sb.Append("\\ql \\fs28 Наружная ротация ацетабулярного фрагмента НР равна " + FormatFloat(HP) + "\\par");
                    }
                    else if (angle_V < 10)
                    {
                        sb.Append("\\ql \\fs28 В. За счет остеотомии таза осуществляется наклон во фронтальной, сагиттальной и горизонтальной плоскости\\par");
                        sb.Append("\\ql \\fs28 Во фронтальной плоскости (латеральный наклон):\\par");
                        sb.Append("\\ql \\fs28 Степень латерального наклона ацетабулярного фрагмента равна " + FormatFloat(SLN) + "\\par");
                        sb.Append("\\ql \\fs28 В сагиттальной плоскости (передний наклон):\\par");
                        sb.Append("\\ql \\fs28 Степень переднего наклона ацетабулярного фрагмента (СПН) равна " + FormatFloat(SPN) + "\\par");
                        sb.Append("\\ql \\fs28 В горизонтальной плоскости:\\par");
                        sb.Append("\\ql \\fs28 Внутренняя ротация ацетабулярного фрагмента ВР равна " + FormatFloat(BP) + "\\par");
                    }
                    sb.Append("\\ql \\fs28 За счет неполной периацетабулярной остеотомии (ПАО) осуществляется дополнительный латеральный наклон\\par");
                    sb.Append("\\ql \\fs28 Степень дополнительного латерального наклона = " + FormatFloat((angle_B - angle_E)) + "\\par");
                }
            }
            else if (ICAS >= 0.8 && ICAS <= 0.99)
            {
                if (angle_A > 35)
                {
                    if (angle_B >= 15 && angle_B <= 20)
                    {
                        sb.Append("\\qc \\fs32 Вердикт 5\\par\\par");
                        sb.Append("\\ql \\fs28     " + PHIO + "\\par");
                        sb.Append("\\ql \\fs28     Год рождения - " + year.ToString() + "\\par\\par");
                        sb.Append("\\ql \\fs28 Г. Ацетабулярный коэффициент равен " + FormatFloat(AK) + "\\par");
                        sb.Append("\\ql \\fs28 Показана ацетабулопластика\\par");
                        sb.Append("\\ql \\fs28 За счет ацетабулопластики осуществляется дополнительное латеральное покрытие наклона\\par");
                        sb.Append("\\ql \\fs28 Величина трансплантата: " + FormatFloat(trans) + "\\par");
                    }
                    else
                    {
                        sb.Append("\\qc \\fs32 Вердикт 0\\par\\par");
                        sb.Append("\\ql \\fs28     " + PHIO + "\\par");
                        sb.Append("\\ql \\fs28     Год рождения - " + year.ToString() + "\\par\\par");
                        sb.Append("\\qc \\fs32 Угол наклона опорной поверхности Б < 15°\\par В данном случае реконструктивная операция не показана\\par");
                    }
                }
                else if (angle_A <= 35)
                {
                    if (angle_B >= 15)
                    {
                        sb.Append("\\qc \\fs32 Вердикт 5\\par\\par");
                        sb.Append("\\ql \\fs28     " + PHIO + "\\par");
                        sb.Append("\\ql \\fs28     Год рождения - " + year.ToString() + "\\par\\par");
                        sb.Append("\\ql \\fs28 Ацетабулярный коэффициент равен " + FormatFloat(AK) + "\\par");
                        sb.Append("\\ql \\fs28 Показана остеотомия таза + ацетабулопластика\\par");
                        if (angle_V >= 10 && angle_V <= 20)
                        {
                            sb.Append("\\ql \\fs28 А. За счет остеотомии таза осуществляется наклон во фронтальной и сагиттальной плоскости\\par");
                            sb.Append("\\ql \\fs28 Во фронтальной плоскости (латеральный наклон):\\par");
                            sb.Append("\\ql \\fs28 Степень латерального наклона ацетабулярного фрагмента соответствует величине угла наклона опорной поверхности Б = " + FormatFloat(angle_B) + "\\par");
                            sb.Append("\\ql \\fs28 В сагиттальной плоскости (передний наклон):\\par");
                            sb.Append("\\ql \\fs28 Степень переднего наклона фрагмента равна величине угла Е = " + FormatFloat(angle_E) + "\\par");
                            sb.Append("\\ql \\fs28 За счет ацетабулопластики осуществляется дополнительное латеральное покрытие наклона\\par");
                            sb.Append("\\ql \\fs28 Величина трансплантата: " + FormatFloat(trans) + "\\par");
                        }
                        else if (angle_V > 20)
                        {
                            sb.Append("\\ql \\fs28 Б. За счет остеотомии таза осуществляется наклон во фронтальной, сагиттальной и горизонтальной плоскости\\par");
                            sb.Append("\\ql \\fs28 Во фронтальной плоскости (латеральный наклон):\\par");
                            sb.Append("\\ql \\fs28 Степень латерального наклона ацетабулярного фрагмента соответствует величине угла наклона опорной поверхности Б = " + FormatFloat(angle_B) + "\\par");
                            sb.Append("\\ql \\fs28 В сагиттальной плоскости (передний наклон):\\par");
                            sb.Append("\\ql \\fs28 Степень переднего наклона фрагмента равна величине угла Е = " + FormatFloat(angle_E) + "\\par");
                            sb.Append("\\ql \\fs28 Горизонтальная плоскость:\\par");
                            sb.Append("\\ql \\fs28 Наружная ротация ацетабулярного фрагмента: " + FormatFloat(HP) + "\\par");
                            sb.Append("\\ql \\fs28 За счет ацетабулопластики осуществляется дополнительное латеральное покрытие наклона\\par");
                            sb.Append("\\ql \\fs28 Величина трансплантата: " + FormatFloat(trans) + "\\par");
                        }
                        else if (angle_V < 10)
                        {
                            sb.Append("\\ql \\fs28 В. За счет остеотомии таза осуществляется наклон во фронтальной, сагиттальной и горизонтальной плоскости\\par");
                            sb.Append("\\ql \\fs28 Во фронтальной плоскости (латеральный наклон):\\par");
                            sb.Append("\\ql \\fs28 Степень латерального наклона ацетабулярного фрагмента соответствует величине угла наклона опорной поверхности Б = " + FormatFloat(angle_B) + "\\par");
                            sb.Append("\\ql \\fs28 В сагиттальной плоскости (передний наклон):\\par");
                            sb.Append("\\ql \\fs28 Степень переднего наклона фрагмента равна величине угла Е = " + FormatFloat(angle_E) + "\\par");
                            sb.Append("\\ql \\fs28 Горизонтальная плоскость:\\par");
                            sb.Append("\\ql \\fs28 Внутренняя ротация ацетабулярного фрагмента: " + FormatFloat(BP) + "\\par");
                            sb.Append("\\ql \\fs28 За счет ацетабулопластики осуществляется дополнительное латеральное покрытие наклона\\par");
                            sb.Append("\\ql \\fs28 Величина трансплантата: " + FormatFloat(trans) + "\\par");
                        }
                    }
                    else
                    {
                        sb.Append("\\qc \\fs32 Вердикт 0\\par\\par");
                        sb.Append("\\ql \\fs28     " + PHIO + "\\par");
                        sb.Append("\\ql \\fs28     Год рождения - " + year.ToString() + "\\par\\par");
                        sb.Append("\\qc \\fs32 Угол наклона опорной поверхности Б < 15°\\par В данном случае реконструктивная операция не показана\\par");
                    }
                }
            }

            sb.Append("\\par");
            sb.Append("}");
            return sb.ToString();
        }

        public static string GenerateVerdictFor10To13(string PHIO, int year, double ICAS, double angle_A, double angle_B, double angle_V, double angle_Vibert, double angle_D, double angle_E, double dist_D, double dist_W, double UOB, double Dh, double AK, double SLN, double SPN, double HP, double BP, double trans)
        {
            double FPN = angle_B - 10;
            StringBuilder sb = new StringBuilder();
            sb.Append("{\\rtf1\\ansi\\ansicpg1251\\deff0\n");
            sb.Append("{\\fonttbl{\\f0 Times New Roman;}}\n");
            sb.Append("\\pard\\sl360\\slmult1\n");
            sb.Append("\\f0\\fs28\n");

            if (ICAS >= 1.0 && ICAS <= 1.7)
            {
                if (angle_A <= 35)
                {
                    if (angle_B >= 15)
                    {
                        sb.Append("\\qc \\fs32 Вердикт 1\\par \\par");
                        sb.Append("\\ql \\fs28     " + PHIO + "\\par");
                        sb.Append("\\ql \\fs28     Год рождения - " + year.ToString() + "\\par \\par");
                        sb.Append("\\ql \\fs28 Угол наклона опорной поверхности (Б) равен " + FormatFloat(angle_B) + "\\par");
                        sb.Append("\\ql \\fs28 Ацетабулярный коэффициент АК равен " + FormatFloat(AK) + "\\par Показана остеотомия таза (лонно-подвздошная остеотомия при сохранении ЛСС). \\par");
                        sb.Append("\\ql \\fs28 Степень латерального наклона ацетабулярного фрагмента равна " + FormatFloat(SLN) + "\\par");
                        sb.Append("\\ql \\fs28 Степень переднего наклона ацетабулярного фрагмента равна " + FormatFloat(SPN) + "\\par");
                        if (angle_V >= 10 && angle_V <= 20)
                        {
                        }
                        else if (angle_V > 20)
                        {
                            sb.Append("\\ql \\fs28 А Наружная ротация ацетабулярного фрагмента НР равна " + FormatFloat(HP) + "\\par");
                        }
                        else if (angle_V < 10)
                        {
                            sb.Append("\\ql \\fs28 Б Внутренняя ротация ацетабулярного фрагмента ВР равна " + FormatFloat(BP) + "\\par");
                        }
                    }
                    else
                    {
                        sb.Append("\\qc \\fs32 Вердикт 0\\par \\par");
                        sb.Append("\\ql \\fs28     " + PHIO + "\\par");
                        sb.Append("\\ql \\fs28     Год рождения - " + year.ToString() + "\\par \\par");
                        sb.Append("\\qc \\fs32 Угол наклона опорной поверхности Б < 15°\\par В данном случае реконструктивная операция не показана\\par");
                    }
                }
                else
                {
                    sb.Append("\\qc \\fs32 Вердикт 0\\par \\par");
                    sb.Append("\\ql \\fs28     " + PHIO + "\\par");
                    sb.Append("\\ql \\fs28     Год рождения - " + year.ToString() + "\\par \\par");
                    sb.Append("\\qc \\fs32 Угол наклона опорной поверхности Б < 15°\\par В данном случае реконструктивная операция не показана\\par");
                }
            }
            else if (ICAS > 1.7)
            {
                string text = "\\ql \\fs28 Ацетабулярный коэффициент АК равен " + FormatFloat(AK) + "\\par";
                if (angle_A > 35)
                {
                    text += "\\ql \\fs28 Угол наклона опорной поверхности (Б) равен " + FormatFloat(angle_B) + "\\par";
                    if (angle_V >= 10 && angle_V <= 20)
                    {
                        sb.Append("\\qc \\fs32 Вердикт 2\\par\\par");
                        sb.Append("\\ql \\fs28     " + PHIO + "\\par");
                        sb.Append("\\ql \\fs28     Год рождения - " + year.ToString() + "\\par\\par");
                        sb.Append(text);
                        sb.Append("\\ql \\fs28 Показана неполная периацетабулярная остеотомия (ПАО)\\par");
                        sb.Append("\\ql \\fs28 Наклон ацетабулярного фрагмента во фронтальной и сагиттальной плоскости: \\par");
                        sb.Append("\\ql \\fs28 Степень латерального наклона (фронтальная плоскость) равна " + FormatFloat(FPN) + "\\par");
                        sb.Append("\\ql \\fs28 Степень переднего наклона (сагиттальная плоскость) равна " + FormatFloat(SPN) + "\\par");
                    }
                    else
                    {
                        sb.Append("\\qc \\fs32 Вердикт 3\\par\\par");
                        sb.Append("\\ql \\fs28     " + PHIO + "\\par");
                        sb.Append("\\ql \\fs28     Год рождения - " + year.ToString() + "\\par\\par");
                        sb.Append(text);
                        sb.Append("Показана остеотомия таза (лонно-подвздошная остеотомия при сохранении ЛСС) + неполная периацетабулярная остеотомия (ПАО)\\par");
                        sb.Append("За счет остеотомии таза осуществляется наклон в сагиттальной и горизонтальной плоскости\\par");
                        sb.Append("Сагиттальная плоскость (передний наклон) Степень переднего наклона фрагмента равна " + FormatFloat(SPN) + "\\par");
                        if (angle_V > 20)
                        {
                            sb.Append("\\ql \\fs28 А Горизонтальная плоскость. \\par");
                            sb.Append("\\ql \\fs28 Наружная ротация ацетабулярного фрагмента НР равна " + FormatFloat(HP) + "\\par");
                        }
                        else if (angle_V < 10)
                        {
                            sb.Append("\\ql \\fs28 Б Горизонтальная плоскость.\\par");
                            sb.Append("\\ql \\fs28 Внутренняя ротация ацетабулярного фрагмента ВР равна " + FormatFloat(BP) + "\\par");
                        }
                        sb.Append("\\ql \\fs28 За счет неполной ПАО осуществляется латеральный наклон\\par");
                        sb.Append("\\ql \\fs28 Степень латерального наклона соответствует углу наклона опорной поверхности Б = " + FormatFloat(FPN) + "\\par"); //
                    }
                }
                else if (angle_A <= 35)
                {
                    sb.Append("\\qc \\fs32 Вердикт 4\\par\\par");
                    sb.Append("\\ql \\fs28     " + PHIO + "\\par");
                    sb.Append("\\ql \\fs28     Год рождения - " + year.ToString() + "\\par\\par");
                    sb.Append(text);
                    sb.Append("\\ql \\fs28 Угол наклона опорной поверхности (Б) равен " + FormatFloat(angle_B) + "\\par");
                    sb.Append("\\ql \\fs28 Показана остеотомия таза (лонно-подвздошная остеотомия при сохранении ЛСС) + неполная периацетабулярная остеотомия\\par");
                    if (angle_V >= 10 && angle_V <= 20)
                    {
                        sb.Append("\\ql \\fs28 А. За счет остеотомии таза осуществляется наклон во фронтальной и сагиттальной плоскости\\par");
                        sb.Append("\\ql \\fs28 Во фронтальной плоскости (латеральный наклон): \\par");
                        sb.Append("\\ql \\fs28 Степень латерального наклона ацетабулярного фрагмента (СЛН) равна " + FormatFloat(SLN) + "\\par");
                        sb.Append("\\ql \\fs28 В сагиттальной плоскости (передний наклон): \\par");
                        sb.Append("\\ql \\fs28 Степень переднего наклона ацетабулярного фрагмента (СПН) равна " + FormatFloat(SPN) + "\\par");
                    }
                    else if (angle_V > 20)
                    {
                        sb.Append("\\ql \\fs28 Б. За счет остеотомии таза осуществляется наклон во фронтальной, сагиттальной и горизонтальной плоскости\\par");
                        sb.Append("\\ql \\fs28 Во фронтальной плоскости (латеральный наклон):\\par");
                        sb.Append("\\ql \\fs28 Степень латерального наклона ацетабулярного фрагмента (СЛН) равна " + FormatFloat(SLN) + "\\par");
                        sb.Append("\\ql \\fs28 В сагиттальной плоскости (передний наклон):\\par");
                        sb.Append("\\ql \\fs28 Степень переднего наклона ацетабулярного фрагмента (СПН) равна " + FormatFloat(SPN) + "\\par");
                        sb.Append("\\ql \\fs28 В горизонтальной плоскости:\\par");
                        sb.Append("\\ql \\fs28 Наружная ротация ацетабулярного фрагмента НР равна " + FormatFloat(HP) + "\\par");
                    }
                    else if (angle_V < 10)
                    {
                        sb.Append("\\ql \\fs28 В. За счет остеотомии таза осуществляется наклон во фронтальной, сагиттальной и горизонтальной плоскости\\par");
                        sb.Append("\\ql \\fs28 Во фронтальной плоскости (латеральный наклон):\\par");
                        sb.Append("\\ql \\fs28 Степень латерального наклона ацетабулярного фрагмента равна " + FormatFloat(SLN) + "\\par");
                        sb.Append("\\ql \\fs28 В сагиттальной плоскости (передний наклон):\\par");
                        sb.Append("\\ql \\fs28 Степень переднего наклона ацетабулярного фрагмента (СПН) равна " + FormatFloat(SPN) + "\\par");
                        sb.Append("\\ql \\fs28 В горизонтальной плоскости:\\par");
                        sb.Append("\\ql \\fs28 Внутренняя ротация ацетабулярного фрагмента ВР равна " + FormatFloat(BP) + "\\par");
                    }
                    sb.Append("\\ql \\fs28 За счет неполной периацетабулярной остеотомии (ПАО) осуществляется дополнительный латеральный наклон\\par");
                    sb.Append("\\ql \\fs28 Степень дополнительного латерального наклона = " + FormatFloat((angle_B - angle_E)) + "\\par");
                }
            }
            else if (ICAS >= 0.8 && ICAS <= 0.99)
            {
                if (angle_A > 35)
                {
                    if (angle_B >= 15 && angle_B <= 20)
                    {
                        sb.Append("\\qc \\fs32 Вердикт 5\\par\\par");
                        sb.Append("\\ql \\fs28     " + PHIO + "\\par");
                        sb.Append("\\ql \\fs28     Год рождения - " + year.ToString() + "\\par\\par");
                        sb.Append("\\ql \\fs28 Г. Ацетабулярный коэффициент равен " + FormatFloat(AK) + "\\par");
                        sb.Append("\\ql \\fs28 Показана ацетабулопластика\\par");
                        sb.Append("\\ql \\fs28 За счет ацетабулопластики осуществляется дополнительное латеральное покрытие наклона\\par");
                        sb.Append("\\ql \\fs28 Величина трансплантата: " + FormatFloat(trans) + "\\par");
                    }
                    else
                    {
                        sb.Append("\\qc \\fs32 Вердикт 0\\par\\par");
                        sb.Append("\\ql \\fs28     " + PHIO + "\\par");
                        sb.Append("\\ql \\fs28     Год рождения - " + year.ToString() + "\\par\\par");
                        sb.Append("\\qc \\fs32 Угол наклона опорной поверхности Б < 15°\\par В данном случае реконструктивная операция не показана\\par");
                    }
                }
                else if (angle_A <= 35)
                {
                    if (angle_B >= 15)
                    {
                        sb.Append("\\qc \\fs32 Вердикт 5\\par\\par");
                        sb.Append("\\ql \\fs28     " + PHIO + "\\par");
                        sb.Append("\\ql \\fs28     Год рождения - " + year.ToString() + "\\par\\par");
                        sb.Append("\\ql \\fs28 Ацетабулярный коэффициент равен " + FormatFloat(AK) + "\\par");
                        sb.Append("\\ql \\fs28 Показана остеотомия таза (лонно-подвздошная остеотомия при сохранении ЛСС) + ацетабулопластика\\par");
                        if (angle_V >= 10 && angle_V <= 20)
                        {
                            sb.Append("\\ql \\fs28 А. За счет остеотомии таза осуществляется наклон во фронтальной и сагиттальной плоскости\\par");
                            sb.Append("\\ql \\fs28 Во фронтальной плоскости (латеральный наклон):\\par");
                            sb.Append("\\ql \\fs28 Степень латерального наклона ацетабулярного фрагмента = " + FormatFloat(FPN) + "\\par");
                            sb.Append("\\ql \\fs28 В сагиттальной плоскости (передний наклон):\\par");
                            sb.Append("\\ql \\fs28 Степень переднего наклона ацетабулярного фрагмента равна величине угла Е = " + FormatFloat(angle_E) + "\\par");
                            sb.Append("\\ql \\fs28 За счет ацетабулопластики осуществляется дополнительное латеральное покрытие наклона\\par");
                            sb.Append("\\ql \\fs28 Величина трансплантата: " + FormatFloat(trans) + "\\par");
                        }
                        else if (angle_V > 20)
                        {
                            sb.Append("\\ql \\fs28 Б. За счет остеотомии таза осуществляется наклон во фронтальной, сагиттальной и горизонтальной плоскости\\par");
                            sb.Append("\\ql \\fs28 Во фронтальной плоскости (латеральный наклон):\\par");
                            sb.Append("\\ql \\fs28 Степень латерального наклона ацетабулярного фрагмента = " + FormatFloat(FPN) + "\\par");
                            sb.Append("\\ql \\fs28 В сагиттальной плоскости (передний наклон):\\par");
                            sb.Append("\\ql \\fs28 Степень переднего наклона ацетабулярного фрагмента равна величине угла Е = " + FormatFloat(angle_E) + "\\par");
                            sb.Append("\\ql \\fs28 Горизонтальная плоскость:\\par");
                            sb.Append("\\ql \\fs28 Наружная ротация ацетабулярного фрагмента: " + FormatFloat(HP) + "\\par");
                            sb.Append("\\ql \\fs28 За счет ацетабулопластики осуществляется дополнительное латеральное покрытие наклона\\par");
                            sb.Append("\\ql \\fs28 Величина трансплантата: " + FormatFloat(trans) + "\\par");
                        }
                        else if (angle_V < 10)
                        {
                            sb.Append("\\ql \\fs28 В. За счет остеотомии таза осуществляется наклон во фронтальной, сагиттальной и горизонтальной плоскости\\par");
                            sb.Append("\\ql \\fs28 Во фронтальной плоскости (латеральный наклон):\\par");
                            sb.Append("\\ql \\fs28 Степень латерального наклона ацетабулярного фрагмента = " + FormatFloat(FPN) + "\\par");
                            sb.Append("\\ql \\fs28 В сагиттальной плоскости (передний наклон):\\par");
                            sb.Append("\\ql \\fs28 Степень переднего наклона ацетабулярного фрагмента равна величине угла Е = " + FormatFloat(angle_E) + "\\par");
                            sb.Append("\\ql \\fs28 Горизонтальная плоскость:\\par");
                            sb.Append("\\ql \\fs28 Внутренняя ротация ацетабулярного фрагмента: " + FormatFloat(BP) + "\\par");
                            sb.Append("\\ql \\fs28 За счет ацетабулопластики осуществляется дополнительное латеральное покрытие наклона\\par");
                            sb.Append("\\ql \\fs28 Величина трансплантата: " + FormatFloat(trans) + "\\par");
                        }
                    }
                    else
                    {
                        sb.Append("\\qc \\fs32 Вердикт 0\\par\\par");
                        sb.Append("\\ql \\fs28     " + PHIO + "\\par");
                        sb.Append("\\ql \\fs28     Год рождения - " + year.ToString() + "\\par\\par");
                        sb.Append("\\qc \\fs32 Угол наклона опорной поверхности Б < 15°\\par В данном случае реконструктивная операция не показана\\par");
                    }
                }
            }

            sb.Append("\\par");
            sb.Append("}");
            return sb.ToString();
        }

        public static string GenerateVerdictFor7To9(string PHIO, int year, double ICAS, double angle_A, double angle_B, double angle_V, double angle_Vibert, double angle_D, double angle_E, double dist_D, double dist_W, double UOB, double Dh, double AK, double SLN, double SPN, double HP, double BP, double trans)
        {
            double FPN = angle_B - 10;
            StringBuilder sb = new StringBuilder();
            sb.Append("{\\rtf1\\ansi\\ansicpg1251\\deff0\n");
            sb.Append("{\\fonttbl{\\f0 Times New Roman;}}\n");
            sb.Append("\\pard\\sl360\\slmult1\n");
            sb.Append("\\f0\\fs28\n");

            if (ICAS >= 1.0 && ICAS <= 1.7)
            {
                if (angle_A <= 35 && angle_A >= 32)
                {
                    if (angle_B >= 25 && angle_B <= 35)
                    {
                        sb.Append("\\qc \\fs32 Вердикт 1\\par \\par");
                        sb.Append("\\ql \\fs28     " + PHIO + "\\par");
                        sb.Append("\\ql \\fs28     Год рождения - " + year.ToString() + "\\par \\par");
                        sb.Append("\\ql \\fs28 Угол наклона опорной поверхности (Б) равен " + FormatFloat(angle_B) + "\\par");
                        sb.Append("\\ql \\fs28 Ацетабулярный коэффициент АК равен " + FormatFloat(AK) + "\\par");
                        sb.Append("\\ql \\fs28 Показана  Г-образная остеотомия подвздошной кости. \\par");
                        sb.Append("\\ql \\fs28 Наклон впадины во фронтальной (латеральный наклон) и сагиттальной (передний наклон) плоскости \\par");
                        sb.Append("\\ql \\fs28 Степень латерального наклона ацетабулярного фрагмента равна " + FormatFloat(SLN) + "\\par");
                        sb.Append("\\ql \\fs28 Степень переднего наклона ацетабулярного фрагмента (СПН) равна " + FormatFloat(SPN) + "\\par");
                    }
                    else
                    {
                        sb.Append("\\qc \\fs32 Вердикт 0\\par \\par");
                        sb.Append("\\ql \\fs28     " + PHIO + "\\par");
                        sb.Append("\\ql \\fs28     Год рождения - " + year.ToString() + "\\par \\par");
                        sb.Append("\\qc \\fs32 Измеренные параметры выходят за пределы допустимого диапазона \\par В данном случае реконструктивная операция не показана\\par");
                    }
                }
                else if (angle_B > 35)
                {
                    sb.Append("\\ql \\fs28 Угол наклона опорной поверхности (Б) равен " + FormatFloat(angle_B) + "\\par");
                    sb.Append("\\ql \\fs28 Ацетабулярный коэффициент АК равен " + FormatFloat(AK) + "\\par");
                    sb.Append("\\ql \\fs28 Показана остеотомия таза (лонно-подвздошная остеотомия при сохранении ЛСС). \\par");
                    sb.Append("\\ql \\fs28 Наклон впадины во фронтальной (латеральный наклон), сагиттальной (передний наклон) и горизонтальной плоскости \\par");
                    sb.Append("\\ql \\fs28 Степень латерального наклона ацетабулярного фрагмента равна " + FormatFloat(SLN) + "\\par");
                    sb.Append("\\ql \\fs28 Степень переднего наклона ацетабулярного фрагмента (СПН) равна " + FormatFloat(SPN) + "\\par");
                }
                else
                {
                    sb.Append("\\qc \\fs32 Вердикт 0\\par \\par");
                    sb.Append("\\ql \\fs28     " + PHIO + "\\par");
                    sb.Append("\\ql \\fs28     Год рождения - " + year.ToString() + "\\par \\par");
                    sb.Append("\\qc \\fs32 Угол наклона опорной поверхности Б < 15°\\par В данном случае реконструктивная операция не показана\\par");
                }
            }
            else if (ICAS > 1.7)
            {
                string text = "\\ql \\fs28 Ацетабулярный коэффициент АК равен " + FormatFloat(AK) + "\\par";
                text += "\\ql \\fs28 Угол наклона опорной поверхности (Б) равен " + FormatFloat(angle_B) + "\\par";
                if (angle_A > 35)
                {
                    sb.Append("\\qc \\fs32 Вердикт 2\\par\\par");
                    sb.Append("\\ql \\fs28     " + PHIO + "\\par");
                    sb.Append("\\ql \\fs28     Год рождения - " + year.ToString() + "\\par\\par");
                    sb.Append(text);
                    sb.Append("\\ql \\fs28 Показана неполная периацетабулярная остеотомия (ПАО)\\par");
                    sb.Append("\\ql \\fs28 Наклон ацетабулярного фрагмента во фронтальной и сагиттальной плоскости: \\par");
                    sb.Append("\\ql \\fs28 Степень латерального наклона (фронтальная плоскость) равна " + FormatFloat(FPN) + "\\par");
                    sb.Append("\\ql \\fs28 Степень переднего наклона (сагиттальная плоскость) равна " + FormatFloat(SPN) + "\\par");
                }
                else if (angle_A <= 35 && angle_A >= 30)
                {
                    sb.Append("\\qc \\fs32 Вердикт 3\\par\\par");
                    sb.Append("\\ql \\fs28     " + PHIO + "\\par");
                    sb.Append("\\ql \\fs28     Год рождения - " + year.ToString() + "\\par\\par");
                    sb.Append(text);
                    sb.Append("\\ql \\fs28 Показана поперечная остеотомия подвздошной кости + неполная периацетабулярная остеотомия (ПАО) \\par");
                    sb.Append("\\ql \\fs28 За счет поперечной остеотомии подвздошной кости осуществляется наклон во фронтальной и сагиттальной плоскости \\par");
                    sb.Append("\\ql \\fs28 Во фронтальной плоскости (латеральный наклон) \\par");
                    sb.Append("\\ql \\fs28 За счет поперечной остеотомии подвздошной кости осуществляется наклон во фронтальной и сагиттальной плоскости \\par");
                    sb.Append("\\ql \\fs28 Степень латерального наклона ацетабулярного фрагмента = " + FormatFloat(SLN) + "\\par");
                    sb.Append("\\ql \\fs28 В сагиттальной плоскости (передний наклон) \\par");
                    sb.Append("\\ql \\fs28 Степень переднего наклона фрагмента равна " + FormatFloat(SPN) + "\\par");
                    sb.Append("\\ql \\fs28 За счет неполной ПАО осуществляется дополнительный латеральный наклон \\par");
                    sb.Append("\\ql \\fs28 Степень дополнительного латерального наклона:" + FormatFloat((angle_B - angle_E)) + "\\par");
                }
                else if (angle_A < 30)
                {
                    text += "\\ql \\fs28 Показана остеотомия таза (лонно-подвздошная остеотомия при сохранении ЛСС) + неполная периацетабулярная остеотомия \\par";
                    if (angle_V >= 10 && angle_V <= 20)
                    {
                        sb.Append("\\ql \\fs28 А. За счет остеотомии таза осуществляется наклон во фронтальной и сагиттальной плоскости\\par");
                        sb.Append("\\ql \\fs28 Во фронтальной плоскости (латеральный наклон): \\par");
                        sb.Append("\\ql \\fs28 Степень латерального наклона ацетабулярного фрагмента (СЛН) равна " + FormatFloat(SLN) + "\\par");
                        sb.Append("\\ql \\fs28 В сагиттальной плоскости (передний наклон): \\par");
                        sb.Append("\\ql \\fs28 Степень переднего наклона ацетабулярного фрагмента (СПН) равна " + FormatFloat(SPN) + "\\par");
                    }
                    else if (angle_V > 20)
                    {
                        sb.Append("\\ql \\fs28 Б. За счет остеотомии таза осуществляется наклон во фронтальной, сагиттальной и горизонтальной плоскости\\par");
                        sb.Append("\\ql \\fs28 Во фронтальной плоскости (латеральный наклон):\\par");
                        sb.Append("\\ql \\fs28 Степень латерального наклона ацетабулярного фрагмента (СЛН) равна " + FormatFloat(SLN) + "\\par");
                        sb.Append("\\ql \\fs28 В сагиттальной плоскости (передний наклон):\\par");
                        sb.Append("\\ql \\fs28 Степень переднего наклона ацетабулярного фрагмента (СПН) равна " + FormatFloat(SPN) + "\\par");
                        sb.Append("\\ql \\fs28 В горизонтальной плоскости:\\par");
                        sb.Append("\\ql \\fs28 Наружная ротация ацетабулярного фрагмента НР равна " + FormatFloat(HP) + "\\par");
                    }
                    else if (angle_V < 10)
                    {
                        sb.Append("\\ql \\fs28 В. За счет остеотомии таза осуществляется наклон во фронтальной, сагиттальной и горизонтальной плоскости\\par");
                        sb.Append("\\ql \\fs28 Во фронтальной плоскости (латеральный наклон):\\par");
                        sb.Append("\\ql \\fs28 Степень латерального наклона ацетабулярного фрагмента равна " + FormatFloat(SLN) + "\\par");
                        sb.Append("\\ql \\fs28 В сагиттальной плоскости (передний наклон):\\par");
                        sb.Append("\\ql \\fs28 Степень переднего наклона ацетабулярного фрагмента (СПН) равна " + FormatFloat(SPN) + "\\par");
                        sb.Append("\\ql \\fs28 В горизонтальной плоскости:\\par");
                        sb.Append("\\ql \\fs28 Внутренняя ротация ацетабулярного фрагмента ВР равна " + FormatFloat(BP) + "\\par");
                    }
                    sb.Append("\\ql \\fs28 За счет неполной периацетабулярной остеотомии (ПАО) осуществляется дополнительный латеральный наклон\\par");
                    sb.Append("\\ql \\fs28 Степень дополнительного латерального наклона = " + FormatFloat((angle_B - angle_E)) + "\\par");
                }
            }
            else if (ICAS >= 0.8 && ICAS <= 0.99)
            {
                if (angle_A > 35)
                {
                    sb.Append("\\qc \\fs32 Вердикт 5\\par\\par");
                    sb.Append("\\ql \\fs28     " + PHIO + "\\par");
                    sb.Append("\\ql \\fs28     Год рождения - " + year.ToString() + "\\par\\par");
                    sb.Append("\\ql \\fs28 Г. Ацетабулярный коэффициент равен " + FormatFloat(AK) + "\\par");
                    sb.Append("\\ql \\fs28 Показана ацетабулопластика\\par");
                    sb.Append("\\ql \\fs28 За счет ацетабулопластики осуществляется дополнительное латеральное покрытие наклона\\par");
                    sb.Append("\\ql \\fs28 Величина трансплантата: " + FormatFloat(trans) + "\\par");
                }
                else if (angle_A < 32)
                {
                    if (angle_B > 35)
                    {
                        sb.Append("\\qc \\fs32 Вердикт 5\\par\\par");
                        sb.Append("\\ql \\fs28     " + PHIO + "\\par");
                        sb.Append("\\ql \\fs28     Год рождения - " + year.ToString() + "\\par\\par");
                        sb.Append("\\ql \\fs28 Угол наклона опорной поверхности (Б) равен " + FormatFloat(angle_B) + "\\par");
                        sb.Append("\\ql \\fs28 Ацетабулярный коэффициент равен " + FormatFloat(AK) + "\\par");
                        sb.Append("\\ql \\fs28 Показана остеотомия таза (лонно-подвздошная остеотомия при сохранении ЛСС) + ацетабулопластика \\par");
                        if (angle_V >= 10 && angle_V <= 20)
                        {
                            sb.Append("\\ql \\fs28 А. За счет остеотомии таза осуществляется наклон во фронтальной и сагиттальной плоскости\\par");
                            sb.Append("\\ql \\fs28 Во фронтальной плоскости (латеральный наклон):\\par");
                            sb.Append("\\ql \\fs28 Степень латерального наклона ацетабулярного фрагмента = " + FormatFloat(FPN) + "\\par");
                            sb.Append("\\ql \\fs28 В сагиттальной плоскости (передний наклон)\\par");
                            sb.Append("\\ql \\fs28 Степень переднего наклона ацетабулярного фрагмента = " + FormatFloat(angle_E) + "\\par");
                            sb.Append("\\ql \\fs28 За счет ацетабулопластики осуществляется дополнительное латеральное покрытие наклона\\par");
                            sb.Append("\\ql \\fs28 Величина трансплантата: " + FormatFloat(trans) + "\\par");
                        }
                        else if (angle_V > 20)
                        {
                            sb.Append("\\ql \\fs28 Б. За счет остеотомии таза осуществляется наклон во фронтальной, сагиттальной и горизонтальной плоскости\\par");
                            sb.Append("\\ql \\fs28 Во фронтальной плоскости (латеральный наклон):\\par");
                            sb.Append("\\ql \\fs28 Степень латерального наклона ацетабулярного фрагмента = " + FormatFloat(FPN) + "\\par");
                            sb.Append("\\ql \\fs28 В сагиттальной плоскости (передний наклон)\\par");
                            sb.Append("\\ql \\fs28 Степень переднего наклона ацетабулярного фрагмента = " + FormatFloat(angle_E) + "\\par");
                            sb.Append("\\ql \\fs28 Горизонтальная плоскость:\\par");
                            sb.Append("\\ql \\fs28 Наружная ротация ацетабулярного фрагмента: " + FormatFloat(HP) + "\\par");
                            sb.Append("\\ql \\fs28 За счет ацетабулопластики осуществляется дополнительное латеральное покрытие наклона\\par");
                            sb.Append("\\ql \\fs28 Величина трансплантата: " + FormatFloat(trans) + "\\par");
                        }
                        else if (angle_V < 10)
                        {
                            sb.Append("\\ql \\fs28 В. За счет остеотомии таза осуществляется наклон во фронтальной, сагиттальной и горизонтальной плоскости\\par");
                            sb.Append("\\ql \\fs28 Во фронтальной плоскости (латеральный наклон):\\par");
                            sb.Append("\\ql \\fs28 Степень латерального наклона ацетабулярного фрагмента = " + FormatFloat(FPN) + "\\par");
                            sb.Append("\\ql \\fs28 В сагиттальной плоскости (передний наклон)\\par");
                            sb.Append("\\ql \\fs28 Степень переднего наклона ацетабулярного фрагмента = " + FormatFloat(angle_E) + "\\par");
                            sb.Append("\\ql \\fs28 Горизонтальная плоскость:\\par");
                            sb.Append("\\ql \\fs28 Внутренняя ротация ацетабулярного фрагмента: " + FormatFloat(BP) + "\\par");
                            sb.Append("\\ql \\fs28 За счет ацетабулопластики осуществляется дополнительное латеральное покрытие наклона\\par");
                            sb.Append("\\ql \\fs28 Величина трансплантата: " + FormatFloat(trans) + "\\par");
                        }
                    }
                    else
                    {
                        sb.Append("\\qc \\fs32 Вердикт 0\\par\\par");
                        sb.Append("\\ql \\fs28     " + PHIO + "\\par");
                        sb.Append("\\ql \\fs28     Год рождения - " + year.ToString() + "\\par\\par");
                        sb.Append("\\qc \\fs32 Измеренные параметры выходят за пределы допустимого диапазона \\par В данном случае реконструктивная операция не показана\\par");
                    }
                }
                else if (angle_A >= 32 && angle_A <= 35)
                {
                    if (angle_B >= 25 && angle_B <= 33)
                    {
                        sb.Append("\\qc \\fs32 Вердикт 5\\par\\par");
                        sb.Append("\\ql \\fs28     " + PHIO + "\\par");
                        sb.Append("\\ql \\fs28     Год рождения - " + year.ToString() + "\\par\\par");
                        sb.Append("\\ql \\fs28 Д. Угол наклона опорной поверхности (Б) равен " + FormatFloat(angle_B) + "\\par");
                        sb.Append("\\ql \\fs28 Ацетабулярный коэффициент равен " + FormatFloat(AK) + "\\par");
                        sb.Append("\\ql \\fs28 Показана  Г-образная остеотомия подвздошной кости +ацетабулопластика \\par");
                        sb.Append("\\ql \\fs28 За счет остеотомии подвздошной кости осуществляется наклон впадины во фронтальной (латеральный наклон) и сагиттальной (передний наклон) плоскости \\par");
                        sb.Append("\\ql \\fs28 Степень латерального наклона ацетабулярного фрагмента = " + FormatFloat(FPN) + "\\par");
                        sb.Append("\\ql \\fs28 В сагиттальной плоскости (передний наклон)\\par");
                        sb.Append("\\ql \\fs28 Степень переднего наклона ацетабулярного фрагмента = " + FormatFloat(angle_E) + "\\par");
                        sb.Append("\\ql \\fs28 За счет ацетабулопластики осуществляется дополнительное  латеральное покрытие головки \\par");
                        sb.Append("\\ql \\fs28 Величина трансплантата: " + FormatFloat(trans) + "\\par");
                    }
                    else
                    {
                        sb.Append("\\qc \\fs32 Вердикт 0\\par\\par");
                        sb.Append("\\ql \\fs28     " + PHIO + "\\par");
                        sb.Append("\\ql \\fs28     Год рождения - " + year.ToString() + "\\par\\par");
                        sb.Append("\\qc \\fs32 Измеренные параметры выходят за пределы допустимого диапазона \\par В данном случае реконструктивная операция не показана\\par");
                    }
                }
            }

            sb.Append("\\par");
            sb.Append("}");
            return sb.ToString();
        }

        public static void PrintVerdict(string PHIO, int year, string output, string filename)
        {
            try
            {
                File.WriteAllText(filename, output, Encoding.GetEncoding(1251));
                Console.WriteLine($"Файл успешно сохранен: {filename}");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Не удалось открыть файл для записи: " + ex.Message);
            }
        }
        private void btn_complete_Click(object sender, EventArgs e)
        {
            if (textBox_9_2_SDLN.Text == "" || textBox_9_3_PHIO.Text == "" || textBox_9_4_year.Text == "" || textBox_9_5_age.Text == "")
            {
                MessageBox.Show("Требуемые поля не заполнены!",
                                      "Ошибка",
                                      MessageBoxButtons.OK,
                                      MessageBoxIcon.Error);
            }
            else
            {
                SharedData.PHIO = textBox_9_3_PHIO.Text;
                SharedData.year = Convert.ToInt32(textBox_9_4_year.Text);
                SharedData.age = Convert.ToInt32(textBox_9_5_age.Text);
                SharedData.HP = SharedData.Angle_V - 20;
                SharedData.BP = 15 - SharedData.Angle_V;
                SharedData.trans = 15 + SharedData.Dh * (1 - SharedData.ICAS);
                string output;
                if (SharedData.age >= 13) 
                {
                    output = GenerateVerdictFor13AndOlder(SharedData.PHIO,
                    SharedData.year,
                    SharedData.ICAS,
                    SharedData.Angle_A,
                    SharedData.Angle_B,
                    SharedData.Angle_V,
                    SharedData.Angle_Viberg,
                    SharedData.Angle_D,
                    SharedData.Angle_E,
                    SharedData.Dist_D,
                    SharedData.Dist_W,
                    SharedData.UOB,
                    SharedData.Dh,
                    SharedData.AK,
                    SharedData.SLN,
                    SharedData.SPN_1,
                    SharedData.HP,
                    SharedData.BP,
                    SharedData.trans);
                }
                else if (SharedData.age >= 10)
                {
                    output = GenerateVerdictFor10To13(SharedData.PHIO,
                    SharedData.year,
                    SharedData.ICAS,
                    SharedData.Angle_A,
                    SharedData.Angle_B,
                    SharedData.Angle_V,
                    SharedData.Angle_Viberg,
                    SharedData.Angle_D,
                    SharedData.Angle_E,
                    SharedData.Dist_D,
                    SharedData.Dist_W,
                    SharedData.UOB,
                    SharedData.Dh,
                    SharedData.AK,
                    SharedData.SLN,
                    SharedData.SPN_1,
                    SharedData.HP,
                    SharedData.BP,
                    SharedData.trans);
                }
                else
                {
                    output = GenerateVerdictFor7To9(SharedData.PHIO,
                    SharedData.year,
                    SharedData.ICAS,
                    SharedData.Angle_A,
                    SharedData.Angle_B,
                    SharedData.Angle_V,
                    SharedData.Angle_Viberg,
                    SharedData.Angle_D,
                    SharedData.Angle_E,
                    SharedData.Dist_D,
                    SharedData.Dist_W,
                    SharedData.UOB,
                    SharedData.Dh,
                    SharedData.AK,
                    SharedData.SLN,
                    SharedData.SPN_1,
                    SharedData.HP,
                    SharedData.BP,
                    SharedData.trans);
                }

                string filename = SharedData.PHIO + " " + SharedData.year.ToString() + ".rtf";
                PrintVerdict(SharedData.PHIO, SharedData.year, output, filename);
                MessageBox.Show($"Файл {filename} сохранён в папке с программой.",
                      "Информация",
                      MessageBoxButtons.OK,
                      MessageBoxIcon.Information);
            }
        }

        private void btn_measure_9_2_Click(object sender, EventArgs e)
        {
            if (textBox_9_1_Angle_E.Text == "")
            {
                MessageBox.Show("Угол Е не измерен!",
                                      "Ошибка",
                                      MessageBoxButtons.OK,
                                      MessageBoxIcon.Error);
            }
            else
            {
                SharedData.Angle_E = Convert.ToDouble(textBox_9_1_Angle_E.Text);
                SharedData.SDLN = SharedData.Angle_B - SharedData.Angle_E;
                textBox_9_2_SDLN.Text = Convert.ToString((float)SharedData.SDLN);
            }
        }
    }
}
