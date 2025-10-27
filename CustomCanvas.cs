using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Scene;

namespace SceneVisualizer;

/// <summary>
/// Кастомный канвас для визуализации сцены с объектами
/// </summary>
/// <remarks>
/// Отрисовывает стены и шары, а также информационную панель.
/// Автоматически масштабирует координаты сцены к размерам канваса.
/// </remarks>
/// <avalonia>

public class CustomCanvas : Canvas
{
    /// <summary>
    /// Avalonia свойство для привязки сцены
    /// </summary>
    public static readonly DirectProperty<CustomCanvas, Scene.Scene> SceneProperty =
        AvaloniaProperty.RegisterDirect<CustomCanvas, Scene.Scene>(
            nameof(Scene),
            o => o.Scene,
            (o, v) => o.Scene = v);

    private Scene.Scene _scene;
    private readonly Dictionary<string, Control> _visualElements = new();

    /// <summary>
    /// Текущая сцена для визуализации
    /// </summary>
    public Scene.Scene Scene
    {
        get => _scene;
        set
        {
            SetAndRaise(SceneProperty, ref _scene, value);
            InitializeVisualization();
        }
    }

    /// <summary>
    /// Инициализирует визуализацию на основе текущей сцены
    /// </summary>
    public void InitializeVisualization()
    {
        if (_scene == null) return;

        Children.Clear();
        _visualElements.Clear();

        foreach (var wall in _scene.GetObjectsOfType<Wall>()) CreateWallVisual(wall);

        foreach (var ball in _scene.GetObjectsOfType<Ball>()) CreateBallVisual(ball);

        CreateInfoPanel();
    }
    /// <summary>
    /// Обновляет визуализацию
    /// </summary>
    public void UpdateVisualization()
    {
        if (_scene == null) return;

        foreach (var ball in _scene.GetObjectsOfType<Ball>()) UpdateBallVisual(ball);

        UpdateInfoPanel();
    }

    /// <summary>
    /// Визуализирует стены
    /// </summary>
    /// <param name="wall"></param>
    private void CreateWallVisual(Wall wall)
    {
        var line = new Line
        {
            Stroke = Brushes.Blue,
            StrokeThickness = 3,
            StartPoint = ScalePoint(wall.StartPoint),
            EndPoint = ScalePoint(wall.EndPoint)
        };

        Children.Add(line);
        _visualElements[wall.Name + "_line"] = line;

        var label = new TextBlock
        {
            Text = wall.Name,
            Foreground = Brushes.DarkBlue,
            FontSize = 10,
            Background = Brushes.White,
            Padding = new Thickness(2)
        };

        var midPoint = new Point(
            (line.StartPoint.X + line.EndPoint.X) / 2,
            (line.StartPoint.Y + line.EndPoint.Y) / 2
        );

        Children.Add(label);
        SetLeft(label, midPoint.X);
        SetTop(label, midPoint.Y - 15);
        _visualElements[wall.Name + "_label"] = label;
    }

    /// <summary>
    /// Визуализирует шары
    /// </summary>
    /// <param name="ball"></param>
    private void CreateBallVisual(Ball ball)
    {
        var center = ScalePoint(ball.Center);
        var radius = ScaleLength(ball.Radius);

        var ellipse = new Ellipse
        {
            Width = radius * 2,
            Height = radius * 2,
            Fill = Brushes.HotPink,
            Stroke = Brushes.DeepPink,
            StrokeThickness = 2
        };

        Children.Add(ellipse);
        SetLeft(ellipse, center.X - radius);
        SetTop(ellipse, center.Y - radius);
        _visualElements[ball.Name + "_ellipse"] = ellipse;

        var label = new TextBlock
        {
            Text = $"{ball.Name}\n({ball.Center.X:F2}, {ball.Center.Y:F2})",
            Foreground = Brushes.DeepPink,
            FontSize = 8,
            Background = Brushes.White,
            Padding = new Thickness(2),
            TextAlignment = TextAlignment.Center
        };

        Children.Add(label);
        SetLeft(label, center.X - 20);
        SetTop(label, center.Y + radius + 5);
        _visualElements[ball.Name + "_label"] = label;
    }

    /// <summary>
    /// Обновляет визуализацию шаров
    /// </summary>
    /// <param name="ball"></param>
    private void UpdateBallVisual(Ball ball)
    {
        var center = ScalePoint(ball.Center);
        var radius = ScaleLength(ball.Radius);

        if (_visualElements.TryGetValue(ball.Name + "_ellipse", out var ellipseControl) &&
            ellipseControl is Ellipse ellipse)
        {
            SetLeft(ellipse, center.X - radius);
            SetTop(ellipse, center.Y - radius);
        }

        if (_visualElements.TryGetValue(ball.Name + "_label", out var labelControl) &&
            labelControl is TextBlock label)
        {
            label.Text = $"{ball.Name}\n({ball.Center.X:F2}, {ball.Center.Y:F2})";
            SetLeft(label, center.X - 20);
            SetTop(label, center.Y + radius + 5);
        }
    }

    /// <summary>
    /// Создаёт панель с информацией
    /// </summary>
    private void CreateInfoPanel()
    {
        var panel = new Border
        {
            Background = new SolidColorBrush(Color.FromArgb(200, 255, 255, 255)),
            BorderBrush = Brushes.Gray,
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(5),
            Padding = new Thickness(10)
        };

        var stackPanel = new StackPanel();
        panel.Child = stackPanel;

        Children.Add(panel);
        SetLeft(panel, 10);
        SetTop(panel, 10);
        _visualElements["info_panel"] = panel;
        _visualElements["info_stack"] = stackPanel;

        UpdateInfoPanel();
    }

    /// <summary>
    /// Обновляет панель с информацией
    /// </summary>
    private void UpdateInfoPanel()
    {
        if (_scene == null) return;

        if (_visualElements.TryGetValue("info_stack", out var stackControl) &&
            stackControl is StackPanel stackPanel)
        {
            stackPanel.Children.Clear();

            var objectsCount = _scene.GetAllObjects().Count;
            var ballsCount = _scene.GetObjectsOfType<Ball>().Count();
            var wallsCount = _scene.GetObjectsOfType<Wall>().Count();

            var title = new TextBlock
            {
                Text = "Scene Visualizer",
                FontSize = 14,
                FontWeight = FontWeight.Bold,
                Foreground = Brushes.Black
            };

            var objectsText = new TextBlock
            {
                Text = $"Objects: {objectsCount}",
                FontSize = 10,
                Foreground = Brushes.Black
            };

            var ballsText = new TextBlock
            {
                Text = $"Balls: {ballsCount}",
                FontSize = 10,
                Foreground = Brushes.Black
            };

            var wallsText = new TextBlock
            {
                Text = $"Walls: {wallsCount}",
                FontSize = 10,
                Foreground = Brushes.Black
            };

            stackPanel.Children.Add(title);
            stackPanel.Children.Add(objectsText);
            stackPanel.Children.Add(ballsText);
            stackPanel.Children.Add(wallsText);
        }
    }

    /// <summary>
    /// Переводит точку в точку на экране
    /// </summary>
    /// <param name="point"></param>
    /// <returns></returns>
    private Point ScalePoint(Point2D point)
    {
        return new Point(
            point.X * Bounds.Width,
            point.Y * Bounds.Height
        );
    }
    /// <summary>
    /// Переводит длину в длину на экране
    /// </summary>
    /// <param name="length"></param>
    /// <returns></returns>
    private double ScaleLength(float length)
    {
        return length * Math.Min(Bounds.Width, Bounds.Height);
    }
}