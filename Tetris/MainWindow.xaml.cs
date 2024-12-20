using System.Data.Common;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Tetris
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ImageSource[] tileImages = new ImageSource[]
        {
            new BitmapImage(new Uri("Assets/TileEmpty.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TileYellow.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TilePurple.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TileOrange.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TileBlue.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TileRed.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TileGreen.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TileCyan.png", UriKind.Relative)),
        };

        private readonly ImageSource[] blockImages = new ImageSource[]
        {
            new BitmapImage(new Uri("Assets/Block-Empty.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Block-O.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Block-T.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Block-L.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Block-J.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Block-Z.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Block-S.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Block-I.png", UriKind.Relative)),
        };

        private readonly Image[,] graphics;

        private GameState gameState = new GameState();

        public MainWindow()
        {
            InitializeComponent();
            graphics = SetupGameCanvas(gameState.GameGrid);
        }

        private Image[,] SetupGameCanvas(Grid grid)
        {
            Image[,] graphics = new Image[grid.Rows, grid.Columns];
            int cellSize = 25;

            for(int row = 0; row < grid.Rows; row++)
            {
                for (int column = 0; column < grid.Columns; column++)
                {
                    Image image = new Image();
                    image.Width = cellSize;
                    image.Height = cellSize;
                    //image.Source = tileImages[grid[row, column]];
                    Canvas.SetLeft(image, column * cellSize);
                    Canvas.SetTop(image, row * cellSize);
                    GameCanvas.Children.Add(image);
                    graphics[row, column] = image;
                }
            }

            return graphics;
        }

        private void DrawGrid(Grid grid)
        {
            for (int row = 0; row < grid.Rows; row++)
            {
                for (int column = 0; column < grid.Columns; column++)
                {
                    graphics[row, column].Opacity = 1;
                    graphics[row, column].Source = tileImages[grid[row, column]];
                }
            }
        }

        private void DrawBlock(Block block)
        {
            foreach (Position tile in block.TilePositions())
            {
                graphics[tile.Row, tile.Column].Opacity = 1;
                graphics[tile.Row, tile.Column].Source = tileImages[block.Id];
            }
        }

        private void DrawNextBlock(BlockQueue blockQueue)
        {
            Block next = blockQueue.NextBlock;
            NextImage.Source = blockImages[next.Id];
        }

        private void DrawHoldBlock(Block Held)
        {
            if(Held == null)
            {
                HoldImage.Source = blockImages[0];
            }
            else
            {
                HoldImage.Source = blockImages[gameState.Held.Id];
            }
        }

        private void DrawGhostBlock(Block block)
        {
            int distance = gameState.BlockDropDistance();

            foreach (Position tile in block.TilePositions())
            {
                graphics[tile.Row + distance, tile.Column].Opacity = 0.5;
                graphics[tile.Row + distance, tile.Column].Source = tileImages[block.Id];
            }
        }

        private async void GameCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            await GameLoop();
        }

        private void Draw()
        {
            DrawGrid(gameState.GameGrid);
            DrawBlock(gameState.CurrentBlock);
            DrawGhostBlock(gameState.CurrentBlock);
            DrawNextBlock(gameState.BlockQueue);
            DrawHoldBlock(gameState.Held);
        }

        private async Task GameLoop()
        {
            Draw();
            while (!gameState.GameOver)
            {
                gameState.MoveBlockDown();
                Draw();
                await Task.Delay(500);
            }

            GameOverMenu.Visibility = Visibility.Visible;
        }

        private async void RestartButton_Click(object sender, RoutedEventArgs e)
        {
            gameState = new GameState();
            GameOverMenu.Visibility = Visibility.Hidden;
            await GameLoop();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Left:
                    gameState.MoveBlockLeft();
                    break;
                case Key.Right:
                    gameState.MoveBlockRight();
                    break;
                case Key.Down:
                    gameState.MoveBlockDown();
                    break;
                case Key.Up:
                    gameState.RotateBlockCW();
                    break;
                case Key.Enter:
                    gameState.DropBlock();
                    break;
                case Key.RightShift:
                    gameState.HoldBlock();
                    break;
            }
            Draw();
        }
    }
}