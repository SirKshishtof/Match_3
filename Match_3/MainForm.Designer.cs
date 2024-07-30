namespace Match_3
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            Play_Button = new Button();
            Ok_Button = new Button();
            GameOver_label = new Label();
            SuspendLayout();
            // 
            // Play_Button
            // 
            Play_Button.Font = new Font("Segoe UI", 36F, FontStyle.Regular, GraphicsUnit.Point, 204);
            Play_Button.Location = new Point(264, 164);
            Play_Button.Name = "Play_Button";
            Play_Button.Size = new Size(429, 100);
            Play_Button.TabIndex = 0;
            Play_Button.Text = "Play";
            Play_Button.UseVisualStyleBackColor = true;
            Play_Button.Click += Play_Button_Click;
            // 
            // Ok_Button
            // 
            Ok_Button.Font = new Font("Segoe UI", 36F, FontStyle.Regular, GraphicsUnit.Point, 204);
            Ok_Button.Location = new Point(264, 387);
            Ok_Button.Name = "Ok_Button";
            Ok_Button.Size = new Size(429, 100);
            Ok_Button.TabIndex = 1;
            Ok_Button.Text = "Ok";
            Ok_Button.UseVisualStyleBackColor = true;
            Ok_Button.Visible = false;
            Ok_Button.Click += Ok_Button_Click;
            // 
            // GameOver_label
            // 
            GameOver_label.AutoSize = true;
            GameOver_label.BackColor = Color.White;
            GameOver_label.Font = new Font("Segoe UI", 48F, FontStyle.Regular, GraphicsUnit.Point, 204);
            GameOver_label.Location = new Point(264, 278);
            GameOver_label.Name = "GameOver_label";
            GameOver_label.Size = new Size(429, 106);
            GameOver_label.TabIndex = 2;
            GameOver_label.Text = "Game over";
            GameOver_label.Visible = false;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(951, 653);
            Controls.Add(GameOver_label);
            Controls.Add(Ok_Button);
            Controls.Add(Play_Button);
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            Name = "MainForm";
            Text = "Match3";
            WindowState = FormWindowState.Maximized;
            Load += MainForm_Load;
            Paint += MainForm_Paint;
            MouseClick += MainForm_MouseClick;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button Play_Button;
        private Button Ok_Button;
        private Label GameOver_label;
    }
}
