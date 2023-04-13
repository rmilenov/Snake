using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Snake
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Dictionary<GridValue, ImageSource> gridValToImage = new()
        {
            {GridValue.Empty, Images.Empty }, {GridValue.Snake, Images.Body }, {GridValue.Food, Images.Food }
        };
        private readonly Dictionary<Direction, int> dirToRotation = new()
        {
            {Direction.Up , 0 }, {Direction.Down,180 },
            {Direction.Right,90 }, {Direction.Left, 270}
        };
        private readonly int rows=20, cols=20;
        private readonly Image[,] gridImages;
        private GameState gameState;
        private bool gameRunning;
        public MainWindow()
        {
            InitializeComponent();
            gridImages = SetupGrid();
            gameState = new GameState(rows, cols);
        }
        private async Task GameLoop()
        {
           
            while (!gameState.GameOver)
            {
                await Task.Delay(200);
                gameState.Move();
                Draw();
                
            }
        }
        private Image[,] SetupGrid()
        {
            Image[,] images = new Image[rows, cols];
            GameGrid.Rows = rows;
            GameGrid.Columns = cols;
            GameGrid.Width = GameGrid.Height * (cols / (double)rows);
            for (int row = 0; row < rows;row++)
            {
                for (int col = 0; col < cols;col++)
                {
                    Image image = new Image()
                    {
                        Source = Images.Empty,
                        RenderTransformOrigin = new Point(0.5, 0.5)
                    };
                    images[row, col] = image;
                    GameGrid.Children.Add(image);
                }

            }
            return images;
        }
        private void Draw()
        {
            DrawGrid();
            DrawSnakeHead();
            ScoreText.Text = $"{gameState.Player.ToString()}'s Score: {gameState.Score}"; 
        }

        private async Task RunGame()
        {
            Draw();
            await ShowCountDown();
            Sound player = new Sound();
            if (!player.isPlaying) player.PlayBackgroundMusic();
            else player.UnpauseBackgroundMusic();
            Overlay.Visibility = Visibility.Hidden;
            await GameLoop();
            player.PauseBackgroundMusic();  
            await ShowGameOver();
            gameState = new GameState(rows,cols);
        }

        private async void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (Overlay.Visibility == Visibility.Visible)
            {
                e.Handled = true;
            }
            if (!gameRunning)
            {
                gameRunning = true;
                await RunGame();
                gameRunning = false;
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (gameState.GameOver)
            {
                return;
            }
            switch(e.Key)
            {
                case Key.Left:
                    gameState.ChangeDirection(Direction.Left);
                    break;
                case Key.Right:
                    gameState.ChangeDirection(Direction.Right);
                    break;
                case Key.Up:
                    gameState.ChangeDirection(Direction.Up);
                    break;
                case Key.Down:
                    gameState.ChangeDirection(Direction.Down);
                    break;
               
            }
        }
       

        private void DrawGrid()
        {
            for (int row=0; row < rows; row++)
            {
                for (int col=0;col < cols;col++)
                {
                    GridValue gridVal = gameState.Grid[row, col];
                    gridImages[row, col].Source = gridValToImage[gridVal];
                    gridImages[row, col].RenderTransform = Transform.Identity;
                }
            }
        }
        private void DrawSnakeHead()
        {
            Position headPos = gameState.HeadPosition();
            Image img = gridImages[headPos.Row, headPos.Column];
            img.Source = Images.Head;
            int rotation = dirToRotation[gameState.Direction];
            img.RenderTransform = new RotateTransform(rotation);
        }
        private async Task DrawDeadSnake()
        {
            List<Position> positions = new List<Position>(gameState.SnakePositions());
            for (int i= 0; i < positions.Count; i++)
            {
                Position pos = positions[i];
                ImageSource source = (i==0) ? Images.DeadHead : Images.DeadBody;
                gridImages[pos.Row, pos.Column].Source = source;
                await Task.Delay(50);
            }
        }
        private async Task ShowCountDown()
        {
            OverlayText.FontSize = 26;
            for(int i = 3; i >= 1; i--) 
            {
                OverlayText.Text = i.ToString();
                await Task.Delay(500);
            }
        }
        private async Task ShowGameOver()
        {
            await DrawDeadSnake();
            await Task.Delay(500);
            InputLabel.Visibility = Visibility.Hidden;
            InputField.Visibility = Visibility.Hidden;

            InputButton.Visibility = Visibility.Visible;
            InputButton.Content = "Yes";
            Overlay.Visibility = Visibility.Visible;
            OverlayText.Text = "Try again ?";
            OverlayText.FontSize=22;
        }

        private async void InputButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Overlay.Visibility == Visibility.Visible)
            {
                e.Handled = true;
            }
            if (!gameRunning)
            {
                gameState.Player = InputField.Text;
                gameRunning = true;
                await RunGame();
                gameRunning = false;
            }
        }

        private  void InputButton_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private async void OnClick(object sender, RoutedEventArgs e)
        {
            if (Overlay.Visibility == Visibility.Visible)
            {
                e.Handled = true;

            }
            if (!gameRunning)
            {
                gameState.Player = InputField.Text;
                gameRunning = true;
                await RunGame();
                gameRunning = false;
            }
        }

        private void  ShowGamePaused()
        {
            
            Overlay.Visibility = Visibility.Visible;
            OverlayText.Text = "Game paused! Press space to continue playing!";
        }
    }
}
