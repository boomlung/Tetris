﻿using System.Data.Common;
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
using System.Xml.Serialization;

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

        private readonly Image[,] graphics, graphics_2P;
        private int mode_selection = 0;

        private GameState gameState = new GameState(false);
        private GameState_2P gameState_2P = new GameState_2P(false);

        public MainWindow()
        {
            InitializeComponent();
            graphics = SetupGameCanvas(gameState.GameGrid);
            if(mode_selection == 1)
            {
               graphics_2P = SetupGameCanvas(gameState_2P.GameGrid);
            }
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

        private void DrawOpponent_Grid(Grid grid)
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
            NextImage1.Source = blockImages[blockQueue.Queue[0].Id];
            NextImage2.Source = blockImages[blockQueue.Queue[1].Id];
            NextImage3.Source = blockImages[blockQueue.Queue[2].Id];
        }

        private void DrawHoldBlock(Block Held)
        {
            if (Held != null)
            {
                HoldImage.Source = blockImages[gameState.Held.Id];
            }
            else
            {
                HoldImage.Source = null;
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
            ScoreText.Text = $"Score: {gameState.Score}";
        }

        private void Draw_2P()
        {
            DrawOpponent_Grid(gameState_2P.GameGrid);
            DrawBlock(gameState_2P.CurrentBlock);
            DrawGhostBlock(gameState_2P.CurrentBlock);
            DrawNextBlock(gameState_2P.BlockQueue);
            DrawHoldBlock(gameState_2P.Held);
            ScoreText.Text = $"Score: {gameState_2P.Score}";
        }

        private async Task GameLoop()
        {
            Draw();
            while (!gameState.GameOver)
            {
                if (!gameState.IsPaused)
                {
                    gameState.MoveBlockDown();
                    Draw();
                }
                await Task.Delay(500);
            }
            GameOverMenu.Visibility = Visibility.Visible;
            FinalScoreText.Text = $"Score: {gameState.Score}";
        }

    
        private async void RestartButton_Click(object sender, RoutedEventArgs e)
        {
            gameState = new GameState(true);
            GameOverMenu.Visibility = Visibility.Hidden;
            await GameLoop();
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            InstructionPage.Visibility = Visibility.Hidden;
            GameMainMenu.Visibility = Visibility.Hidden;
            ModeSelectionPage.Visibility = Visibility.Visible;
        }

        private async void SinglePlayer_Click(object sender, RoutedEventArgs e)
        {
            gameState = new GameState(true);
            GameGrid.Visibility = Visibility.Visible;
            ModeSelectionPage.Visibility = Visibility.Hidden;
            GameCanvas.Loaded += GameCanvas_Loaded;
            await GameLoop();
        }
        
        private async void LocalBattle_Click(object sender, RoutedEventArgs e)
        {
            mode_selection = 1;
            gameState = new GameState(true);
            gameState_2P = new GameState_2P(true);
            GameGrid.Visibility = Visibility.Visible;
            ModeSelectionPage.Visibility = Visibility.Hidden;
            GameCanvas.Loaded += GameCanvas_Loaded;
            await GameLoop();
        }

        private void OnlineBattle_Click(object sender, RoutedEventArgs e)
        {
            //
        }

        private void InstructButton_Click(object sender, RoutedEventArgs e)
        {
            GameMainMenu.Visibility= Visibility.Hidden;
            GameGrid.Visibility= Visibility.Hidden;
            InstructionPage.Visibility = Visibility.Visible;
        }

        private void ReturnButton_Click(object sender, RoutedEventArgs e)
        {
            GameGrid.Visibility= Visibility.Hidden;
            InstructionPage.Visibility = Visibility.Hidden;
            GameMainMenu.Visibility = Visibility.Visible;
            GameOverMenu.Visibility = Visibility.Hidden;
            ModeSelectionPage.Visibility = Visibility.Hidden;
        }


        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Left:
                    if (!gameState.IsPaused && gameState.IsPlaying) gameState.MoveBlockLeft();
                    break;
                case Key.Right:
                    if (!gameState.IsPaused && gameState.IsPlaying) gameState.MoveBlockRight();
                    break;
                case Key.Down:
                    if (!gameState.IsPaused && gameState.IsPlaying) gameState.MoveBlockDown();
                    break;
                case Key.Up:
                    if (!gameState.IsPaused && gameState.IsPlaying) gameState.RotateBlockCW();
                    break;
                case Key.Enter:
                    if (!gameState.IsPaused && gameState.IsPlaying) gameState.DropBlock();
                    break;
                case Key.RightShift:
                    if (!gameState.IsPaused && gameState.IsPlaying) gameState.HoldBlock();
                    break;
                case Key.Escape:
                    if (gameState.IsPlaying)
                    {
                        gameState.IsPaused = !gameState.IsPaused;
                        if (gameState.IsPaused)
                        {
                            PauseMenu.Visibility = Visibility.Visible;
                        }
                        else 
                        {
                            PauseMenu.Visibility = Visibility.Hidden;
                        }
                    }
                    break;
            }
            Draw();

            if(mode_selection == 1)
            {
                switch (e.Key)
                {
                    case Key.A:
                        if (!gameState_2P.IsPaused && gameState_2P.IsPlaying) gameState.MoveBlockLeft();
                        break;
                    case Key.D:
                        if (!gameState_2P.IsPaused && gameState_2P.IsPlaying) gameState.MoveBlockRight();
                        break;
                    case Key.S:
                        if (!gameState_2P.IsPaused && gameState_2P.IsPlaying) gameState.MoveBlockDown();
                        break;
                    case Key.W:
                        if (!gameState_2P.IsPaused && gameState_2P.IsPlaying) gameState.RotateBlockCW();
                        break;
                    case Key.Space:
                        if (!gameState_2P.IsPaused && gameState_2P.IsPlaying) gameState.DropBlock();
                        break;
                    case Key.LeftShift:
                        if (!gameState_2P.IsPaused && gameState_2P.IsPlaying) gameState.HoldBlock();
                        break;
                    case Key.Escape:
                        if (gameState_2P.IsPlaying)
                        {
                            gameState_2P.IsPaused = !gameState_2P.IsPaused;
                            if (gameState_2P.IsPaused)
                            {
                                PauseMenu.Visibility = Visibility.Visible;
                            }
                            else if (!gameState_2P.IsPaused)
                            {
                                PauseMenu.Visibility = Visibility.Hidden;
                            }
                        }
                        break;
                }
                Draw_2P();
            }
        }
    }
}