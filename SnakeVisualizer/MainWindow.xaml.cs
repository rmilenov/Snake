using Microsoft.AspNet.SignalR.Client;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using HubConnection = Microsoft.AspNetCore.SignalR.Client.HubConnection;

namespace SnakeVisualizer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
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
        private string json = "";
        private bool isDirty = true;
        private readonly int rows = 20, cols = 20;
        internal Image[,] gridImages;
        private GameState gameState;// = new GameState(20, 20);
        HubConnection connection;
        public MainWindow()
        {
          
            InitializeComponent();
            gridImages = SetupGrid();
            gameState = new GameState(rows, cols);
            connection = new HubConnectionBuilder()
            .WithUrl("https://localhost:7059/snakehub")
            .WithAutomaticReconnect()
            .Build();
            ReceiveGameState();

            try
            {
                connection.StartAsync();
                messages.Items.Add("Connection started");

            }
            catch (Exception ex)
            {
                messages.Items.Add(ex.Message);
            }
            connection.Reconnecting += (sender) =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    var newMessage = "Attempting to reconnect...";
                    messages.Items.Add(newMessage);
                });

                return Task.CompletedTask;
            };

            connection.Reconnected += (sender) =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    var newMessage = "Reconnected to the server";
                    messages.Items.Clear();
                    messages.Items.Add(newMessage);
                });

                return Task.CompletedTask;
            };

            connection.Closed += (sender) =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    var newMessage = "Connection Closed";
                    messages.Items.Add(newMessage);

                });

                return Task.CompletedTask;
            };



            Draw();
            //_ = GetMessage();
            _ = GameLoop();
        }

        private Image[,] SetupGrid()
        {
            Image[,] images = new Image[rows, cols];
            GameGrid.Rows = rows;
            GameGrid.Columns = cols;
            GameGrid.Width = GameGrid.Height * (cols / (double)rows);
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
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
        private void DrawGrid()
        {
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    GridValue gridVal = gameState.Grid[row, col];
                    gridImages[row, col].Source = gridValToImage[gridVal];
                    gridImages[row, col].RenderTransform = Transform.Identity;
                }
            }
        }
        //private  Task<GameState> GetGameStateFromServer()
        //{
        //     connection.On<GameState>("ReceiveGameState", gameState =>
        //    this.Dispatcher.Invoke(() =>
        //    {
        //        this.gameState = gameState;
        //        return Task.CompletedTask;
        //    }));
        //    return;
        //}
        //private Task GetMessage()
        //{
        //    connection.On<string, string>("ReceiveMessage", (user, message) =>
        //    {
        //        Dispatcher.Invoke(() =>
        //        {
        //            var newMessage = $"{user}: {message}";
        //            messages.Items.Add(newMessage);
        //            json = newMessage;

        //        });
        //    });
        //    return Task.CompletedTask;
        //}

        private async Task ReceiveGameState()
        {
            connection.On<string>("ReceiveGameState", (message) =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    message = message.Replace("\r\n", "");// - Fehler bei der übertragung
                    JsonConvert.PopulateObject(message, gameState);
                    Console.WriteLine(message);
                    messages.Items.Add(message);
                    gameState.Move();
                    Draw();
                    gameState.Move();
                });
            });

        }

        private async Task GameLoop()
        {
            await Task.Delay(gameState.Speed);

            //do
            //{
            //sendButton_Click(gameStateJson);
            //await ReceiveGameState();
            //GameState gameState = new GameState(rows, cols);
            //gameState = JsonConvert.DeserializeObject<GameState>(json);

            //gridImages = SetupGrid();
            gameState.Move();
            Draw();

            //} while (!gameState.GameOver);
            //return Task.CompletedTask;
        }

        private void Draw()
        {
            DrawGrid();
            DrawSnakeHead();

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            while (isDirty)
            {
                Draw();
                GameLoop();
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
    }
}