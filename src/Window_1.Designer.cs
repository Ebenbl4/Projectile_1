namespace Medic
{
    partial class Window_1
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Window_1));
            this.label_1_1 = new System.Windows.Forms.Label();
            this.btn_next_1 = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.btn_select_1 = new System.Windows.Forms.Button();
            this.btn_switch_1 = new System.Windows.Forms.Button();
            this.btn_switch_2 = new System.Windows.Forms.Button();
            this.label_1_2 = new System.Windows.Forms.Label();
            this.btn_help_1 = new System.Windows.Forms.Button();
            this.label_1_3 = new System.Windows.Forms.Label();
            this.label_1_4 = new System.Windows.Forms.Label();
            this.label_1_5 = new System.Windows.Forms.Label();
            this.textBox_1_2_R = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox_1_1_Dh = new System.Windows.Forms.TextBox();
            this.btn_measure_1_2 = new System.Windows.Forms.Button();
            this.btn_measure_1_1 = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.btn_measure_1_3 = new System.Windows.Forms.Button();
            this.textBox_1_3_ISh = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // label_1_1
            // 
            this.label_1_1.BackColor = System.Drawing.Color.LightGray;
            this.label_1_1.Font = new System.Drawing.Font("Cambria", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label_1_1.Location = new System.Drawing.Point(28, 83);
            this.label_1_1.Name = "label_1_1";
            this.label_1_1.Size = new System.Drawing.Size(339, 50);
            this.label_1_1.TabIndex = 0;
            this.label_1_1.Text = "Загрузите рентгенограмму таза в прямой проекции";
            this.label_1_1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btn_next_1
            // 
            this.btn_next_1.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.btn_next_1.Font = new System.Drawing.Font("Cambria", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btn_next_1.Location = new System.Drawing.Point(428, 657);
            this.btn_next_1.Name = "btn_next_1";
            this.btn_next_1.Size = new System.Drawing.Size(160, 50);
            this.btn_next_1.TabIndex = 1;
            this.btn_next_1.Text = "Далее";
            this.btn_next_1.UseVisualStyleBackColor = false;
            this.btn_next_1.Click += new System.EventHandler(this.btn_next_1_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.White;
            this.pictureBox1.Location = new System.Drawing.Point(601, 83);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(864, 858);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 2;
            this.pictureBox1.TabStop = false;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            this.openFileDialog1.FileOk += new System.ComponentModel.CancelEventHandler(this.openFileDialog1_FileOk);
            // 
            // btn_select_1
            // 
            this.btn_select_1.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.btn_select_1.Font = new System.Drawing.Font("Cambria", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btn_select_1.Location = new System.Drawing.Point(428, 83);
            this.btn_select_1.Name = "btn_select_1";
            this.btn_select_1.Size = new System.Drawing.Size(160, 50);
            this.btn_select_1.TabIndex = 3;
            this.btn_select_1.Text = "Выбрать файл";
            this.btn_select_1.UseVisualStyleBackColor = false;
            this.btn_select_1.Click += new System.EventHandler(this.btn_select_1_Click);
            // 
            // btn_switch_1
            // 
            this.btn_switch_1.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.btn_switch_1.Font = new System.Drawing.Font("Cambria", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btn_switch_1.Location = new System.Drawing.Point(601, 27);
            this.btn_switch_1.Name = "btn_switch_1";
            this.btn_switch_1.Size = new System.Drawing.Size(180, 50);
            this.btn_switch_1.TabIndex = 4;
            this.btn_switch_1.Text = "Рентгенограмма 1";
            this.btn_switch_1.UseVisualStyleBackColor = false;
            this.btn_switch_1.Click += new System.EventHandler(this.btn_switch_1_Click);
            // 
            // btn_switch_2
            // 
            this.btn_switch_2.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.btn_switch_2.Font = new System.Drawing.Font("Cambria", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btn_switch_2.Location = new System.Drawing.Point(787, 27);
            this.btn_switch_2.Name = "btn_switch_2";
            this.btn_switch_2.Size = new System.Drawing.Size(180, 50);
            this.btn_switch_2.TabIndex = 5;
            this.btn_switch_2.Text = "Шпаргалка 1";
            this.btn_switch_2.UseVisualStyleBackColor = false;
            this.btn_switch_2.Click += new System.EventHandler(this.btn_switch_2_Click);
            // 
            // label_1_2
            // 
            this.label_1_2.BackColor = System.Drawing.Color.LightGray;
            this.label_1_2.Font = new System.Drawing.Font("Cambria", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label_1_2.Location = new System.Drawing.Point(28, 167);
            this.label_1_2.Name = "label_1_2";
            this.label_1_2.Size = new System.Drawing.Size(339, 50);
            this.label_1_2.TabIndex = 0;
            this.label_1_2.Text = "Определение индекса сферичности головки ISh";
            this.label_1_2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btn_help_1
            // 
            this.btn_help_1.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.btn_help_1.Font = new System.Drawing.Font("Cambria", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btn_help_1.Location = new System.Drawing.Point(428, 167);
            this.btn_help_1.Name = "btn_help_1";
            this.btn_help_1.Size = new System.Drawing.Size(160, 50);
            this.btn_help_1.TabIndex = 6;
            this.btn_help_1.Text = "Помощь";
            this.btn_help_1.UseVisualStyleBackColor = false;
            this.btn_help_1.Click += new System.EventHandler(this.btn_help_1_Click);
            // 
            // label_1_3
            // 
            this.label_1_3.BackColor = System.Drawing.Color.LightGray;
            this.label_1_3.Font = new System.Drawing.Font("Cambria", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label_1_3.Location = new System.Drawing.Point(28, 840);
            this.label_1_3.Name = "label_1_3";
            this.label_1_3.Size = new System.Drawing.Size(551, 101);
            this.label_1_3.TabIndex = 0;
            this.label_1_3.Text = "Индекс сферичности головки (ISh) - отношение диаметра окружности, соответствующей" +
    " форме головки (Dh) к половине расстояния между фигурами слезы (R).";
            this.label_1_3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label_1_3.Click += new System.EventHandler(this.label_1_3_Click);
            // 
            // label_1_4
            // 
            this.label_1_4.BackColor = System.Drawing.Color.LightGray;
            this.label_1_4.Font = new System.Drawing.Font("Cambria", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label_1_4.Location = new System.Drawing.Point(28, 258);
            this.label_1_4.Name = "label_1_4";
            this.label_1_4.Size = new System.Drawing.Size(339, 50);
            this.label_1_4.TabIndex = 0;
            this.label_1_4.Text = "Измерьте диаметр головки Dh";
            this.label_1_4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label_1_5
            // 
            this.label_1_5.BackColor = System.Drawing.Color.LightGray;
            this.label_1_5.Font = new System.Drawing.Font("Cambria", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label_1_5.Location = new System.Drawing.Point(28, 386);
            this.label_1_5.Name = "label_1_5";
            this.label_1_5.Size = new System.Drawing.Size(339, 50);
            this.label_1_5.TabIndex = 0;
            this.label_1_5.Text = "Измерьте расстояние между фигурами слезы R";
            this.label_1_5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // textBox_1_2_R
            // 
            this.textBox_1_2_R.Location = new System.Drawing.Point(486, 458);
            this.textBox_1_2_R.Name = "textBox_1_2_R";
            this.textBox_1_2_R.ReadOnly = true;
            this.textBox_1_2_R.Size = new System.Drawing.Size(102, 22);
            this.textBox_1_2_R.TabIndex = 7;
            this.textBox_1_2_R.TextChanged += new System.EventHandler(this.textBox_1_1_Dh_TextChanged);
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Cambria", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(424, 324);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(61, 22);
            this.label1.TabIndex = 8;
            this.label1.Text = "Dh =";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Cambria", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label2.Location = new System.Drawing.Point(428, 457);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(52, 22);
            this.label2.TabIndex = 8;
            this.label2.Text = "R =";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label2.Click += new System.EventHandler(this.label2_Click);
            // 
            // textBox_1_1_Dh
            // 
            this.textBox_1_1_Dh.Location = new System.Drawing.Point(486, 325);
            this.textBox_1_1_Dh.Name = "textBox_1_1_Dh";
            this.textBox_1_1_Dh.ReadOnly = true;
            this.textBox_1_1_Dh.Size = new System.Drawing.Size(102, 22);
            this.textBox_1_1_Dh.TabIndex = 7;
            this.textBox_1_1_Dh.TextChanged += new System.EventHandler(this.textBox_1_2_R_TextChanged);
            // 
            // btn_measure_1_2
            // 
            this.btn_measure_1_2.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.btn_measure_1_2.Font = new System.Drawing.Font("Cambria", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btn_measure_1_2.Location = new System.Drawing.Point(428, 386);
            this.btn_measure_1_2.Name = "btn_measure_1_2";
            this.btn_measure_1_2.Size = new System.Drawing.Size(160, 50);
            this.btn_measure_1_2.TabIndex = 9;
            this.btn_measure_1_2.Text = "Измерить";
            this.btn_measure_1_2.UseVisualStyleBackColor = false;
            this.btn_measure_1_2.Click += new System.EventHandler(this.btn_measure_1_1_Click);
            // 
            // btn_measure_1_1
            // 
            this.btn_measure_1_1.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.btn_measure_1_1.Font = new System.Drawing.Font("Cambria", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btn_measure_1_1.Location = new System.Drawing.Point(428, 258);
            this.btn_measure_1_1.Name = "btn_measure_1_1";
            this.btn_measure_1_1.Size = new System.Drawing.Size(160, 50);
            this.btn_measure_1_1.TabIndex = 9;
            this.btn_measure_1_1.Text = "Измерить";
            this.btn_measure_1_1.UseVisualStyleBackColor = false;
            this.btn_measure_1_1.Click += new System.EventHandler(this.btn_measure_1_2_Click);
            // 
            // label3
            // 
            this.label3.BackColor = System.Drawing.SystemColors.ControlLight;
            this.label3.Font = new System.Drawing.Font("Cambria", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label3.Location = new System.Drawing.Point(420, 575);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(60, 32);
            this.label3.TabIndex = 0;
            this.label3.Text = "ISh =";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btn_measure_1_3
            // 
            this.btn_measure_1_3.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.btn_measure_1_3.Font = new System.Drawing.Font("Cambria", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btn_measure_1_3.Location = new System.Drawing.Point(428, 514);
            this.btn_measure_1_3.Name = "btn_measure_1_3";
            this.btn_measure_1_3.Size = new System.Drawing.Size(160, 50);
            this.btn_measure_1_3.TabIndex = 10;
            this.btn_measure_1_3.Text = "Рассчитать";
            this.btn_measure_1_3.UseVisualStyleBackColor = false;
            this.btn_measure_1_3.Click += new System.EventHandler(this.btn_measure_1_3_Click);
            // 
            // textBox_1_3_ISh
            // 
            this.textBox_1_3_ISh.Location = new System.Drawing.Point(486, 581);
            this.textBox_1_3_ISh.Name = "textBox_1_3_ISh";
            this.textBox_1_3_ISh.ReadOnly = true;
            this.textBox_1_3_ISh.Size = new System.Drawing.Size(102, 22);
            this.textBox_1_3_ISh.TabIndex = 11;
            // 
            // Window_1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLight;
            this.ClientSize = new System.Drawing.Size(1482, 953);
            this.Controls.Add(this.textBox_1_3_ISh);
            this.Controls.Add(this.btn_measure_1_3);
            this.Controls.Add(this.btn_measure_1_1);
            this.Controls.Add(this.btn_measure_1_2);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox_1_1_Dh);
            this.Controls.Add(this.textBox_1_2_R);
            this.Controls.Add(this.btn_help_1);
            this.Controls.Add(this.btn_switch_2);
            this.Controls.Add(this.btn_switch_1);
            this.Controls.Add(this.btn_select_1);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.btn_next_1);
            this.Controls.Add(this.label_1_3);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label_1_5);
            this.Controls.Add(this.label_1_4);
            this.Controls.Add(this.label_1_2);
            this.Controls.Add(this.label_1_1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(1500, 1000);
            this.Name = "Window_1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Medic";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label_1_1;
        private System.Windows.Forms.Button btn_next_1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Button btn_select_1;
        private System.Windows.Forms.Button btn_switch_1;
        private System.Windows.Forms.Button btn_switch_2;
        private System.Windows.Forms.Label label_1_2;
        private System.Windows.Forms.Button btn_help_1;
        private System.Windows.Forms.Label label_1_3;
        private System.Windows.Forms.Label label_1_4;
        private System.Windows.Forms.Label label_1_5;
        private System.Windows.Forms.TextBox textBox_1_2_R;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox_1_1_Dh;
        private System.Windows.Forms.Button btn_measure_1_2;
        private System.Windows.Forms.Button btn_measure_1_1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btn_measure_1_3;
        private System.Windows.Forms.TextBox textBox_1_3_ISh;
    }
}

