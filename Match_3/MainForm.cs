
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
                    GameOver_label.Text = $"Gameover\nScore:{gameplay.Score}";
                    GameOver_label.Visible = true;
                }


            }
        }
        private void UpdateImage(object? sender, EventArgs e) => Invalidate(new Rectangle(0, 0, 1, 1));
        private void MainForm_MouseClick(object sender, MouseEventArgs e)
        {
            bool canMove = true;
            for (int x = 0; x < GameSettings.MatrixSizeX; x++)
            {
                for (int y = 0; y < GameSettings.MatrixSizeX; y++)
                {
                    if (!gameplay.ElemMatrix[x][y].OnPosition) { canMove = false; break; }
                }
            }
            if (canMove)
            {
                inputHandler.HendelMouseClick(e);
            }
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
