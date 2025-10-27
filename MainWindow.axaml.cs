using Avalonia.Controls;
using Avalonia.Threading;
using System;
using Scene;

namespace SceneVisualizer
{
    public partial class MainWindow : Window
    {
        private Scene.Scene _scene;
        private EcsSceneManager _ecsManager;
        private DispatcherTimer _gameTimer;
        
        public MainWindow()
        {
            InitializeComponent();
            Opened += OnWindowOpened;
        }
        
        private void OnWindowOpened(object sender, EventArgs e)
        {
            InitializeScene();
            InitializeEcs();
            StartGameLoop();
        }
        
        private void InitializeScene()
        {
            _scene = new Scene.Scene();

            Point2D p1 = new Point2D(0.1f, 0.1f);
            Point2D p2 = new Point2D(0.7f, 0.1f);
            Point2D p3 = new Point2D(0.1f, 0.7f);
            Point2D p4 = new Point2D(0.9f, 0.9f);
            Point2D p5 = new Point2D(0.3f, 0.3f);
            Point2D p6 = new Point2D(0.7f, 0.7f);
            
            Wall wall1 = new Wall(p1, p2, "TopWall");
            Wall wall2 = new Wall(p1, p3, "LeftWall");
            Wall wall3 = new Wall(p3, p4, "BottomWall");
            Wall wall4 = new Wall(p2, p4, "RightWall");
            Wall diagonal1 = new Wall(p5, p6, "Diagonal1");

            Ball ball = new Ball(new Point2D(0.5f, 0.7f), 0.05f, 0.5f, "Ball");

            _scene.AddObject(wall1);
            _scene.AddObject(wall2);
            _scene.AddObject(wall3);
            _scene.AddObject(wall4);
            _scene.AddObject(diagonal1);
            _scene.AddObject(ball);
            
            MainCanvas.Scene = _scene;
            
            Console.WriteLine("Scene initialized with " + _scene.GetAllObjects().Count + " objects");
        }
        
        private void InitializeEcs()
        {
            _ecsManager = new EcsSceneManager(_scene);
            _ecsManager.Initialize();
            Console.WriteLine("ECS initialized");
        }
        
        private void StartGameLoop()
        {
            _gameTimer = new DispatcherTimer();
            _gameTimer.Interval = TimeSpan.FromMilliseconds(16);
            _gameTimer.Tick += OnGameTimerTick;
            _gameTimer.Start();
            Console.WriteLine("Game loop started");
        }
        
        private void OnGameTimerTick(object sender, EventArgs e)
        {
            _ecsManager.Update();
            
            MainCanvas.UpdateVisualization();
        }
    }
}