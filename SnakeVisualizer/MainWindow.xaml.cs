using Microsoft.AspNet.SignalR.Client;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
        private string json;
        private readonly int rows = 20, cols = 20;
        private readonly Image[,] gridImages;
        private GameState gameState;
        HubConnection connection;
        public MainWindow()
        {
            InitializeComponent();
            gridImages = SetupGrid();
            connection = new HubConnectionBuilder()
            .WithUrl("https://localhost:7059/snakehub")
            .WithAutomaticReconnect()
            .Build();


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
            //gameState = GetGameStateFromServer();
            GetGameState();
            gameState = new GameState(20,20);
            
            try
            {
            gameState = (GameState)JsonConvert.DeserializeObject(json);

            }
            catch(Exception ex)
            {
                
            }
            DrawGrid();
            
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
        private async Task GetMessage()
        {
            connection.On<string, string>("ReceiveMessage", (user, message) =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    var newMessage = $"{user}: {message}";
                    messages.Items.Add(newMessage);
                    this.json = newMessage;
                });
            });

            try
            {
                await connection.StartAsync();
                messages.Items.Add("Connection started");
                //connectButton.IsEnabled = false;
                //sendButton.IsEnabled = true;
            }
            catch (Exception ex)
            {
                messages.Items.Add(ex.Message);
            }
        }
        private async Task GetGameState()
        {
            connection.On<string>("SendGameState", (message) =>
            {
                this.Dispatcher.Invoke(() =>
                {

                    messages.Items.Add(message);
                    this.json = message;
                });
            });

        }
    }
}
