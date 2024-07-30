using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Match_3
{
    public partial class MainForm : Form
    {
        Drawing drawing;
        Gameplay gameplay;
        InputHandler inputHandler;
        public MainForm()
        {
            InitializeComponent();
            gameplay = new Gameplay();
            BufferedGraphics bufferedGraphics = BufferedGraphicsManager.Current.Allocate(CreateGraphics(), DisplayRectangle);
            drawing = new Drawing(gameplay);
            drawing.BufferedGraphics = bufferedGraphics;
            inputHandler = new InputHandler(drawing);
        }
        private void MainForm_Load(object sender, EventArgs e)
        {
            Play_Button.Location = new Point(Width / 2 - Play_Button.Size.Width / 2, Height / 2 - Play_Button.Size.Height / 2);
            GameOver_label.Location = new Point(Width / 2 - Ok_Button.Size.Width / 2,
                                                Height / 2 - Ok_Button.Size.Height / 2 - 2*GameOver_label.Height);
            Ok_Button.Location = new Point(Width / 2 - Ok_Button.Size.Width / 2, Height / 2 - Ok_Button.Size.Height / 2);
            drawing.SetMetodInTick(new EventHandler(UpdateImage));
            drawing.LoadImage();
        }

        private void MainForm_Paint(object sender, PaintEventArgs e)
        {
            if (gameplay.IsGameStart)
            {
                drawing.Draw();
                if (gameplay.RemainingTime == 0)
                {
                    drawing.Clean();
                    gameplay.IsGameStart = false;
                    gameplay.StopTimer();
                    Ok_Button.Visible = true;
                    GameOver_label.Text = $"{GameOver_label.Text }\nScore:{gameplay.Score}";
                    GameOver_label.Visible = true;
                }
            }
        }
        private void UpdateImage(object? sender, EventArgs e) => Invalidate(new Rectangle(0, 0, 1, 1));
        private void UpdateGame(object? sender, EventArgs e) => Invalidate(new Rectangle(0, 0, 1, 1));
        private void MainForm_MouseClick(object sender, MouseEventArgs e)
        {
            inputHandler.HendelMouseClick(e);
        }
        private void Play_Button_Click(object sender, EventArgs e)
        {
            gameplay.StartTimer();
            drawing.StartTimer();
            Play_Button.Visible = false;
        }
        private void Ok_Button_Click(object sender, EventArgs e)
        {
            Ok_Button.Visible = false;
            GameOver_label.Visible=false;
            Play_Button.Visible = true;
            drawing.Clean();
            gameplay.ResetGame();
        }

    }
}
