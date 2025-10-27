using System.Collections.Generic;
using System.Linq;

namespace Scene;

/// <summary>
/// Базовый класс для всех объектов сцены
/// </summary>
public abstract class SceneObject
{
    /// <summary>
    /// Имя объекта сцены
    /// </summary>
    public string Name { get; set; } = string.Empty;
}

/// <summary>
/// Представляет 2D точку с координатами X и Y
/// </summary>
public class Point2D
{
    /// <summary>
    /// X координата
    /// </summary>
    public float X { get; set; }
    /// <summary>
    /// Y координата
    /// </summary>
    public float Y { get; set; }

    /// <summary>
    /// Создает новую 2D точку
    /// </summary>
    /// <param name="x">X координата</param>
    /// <param name="y">Y координата</param>
    public Point2D(float x, float y)
    {
        X = x;
        Y = y;
    }
    
    /// <summary>
    /// Создает новую 2D точку из double координат
    /// </summary>
    /// <param name="x">X координата</param>
    /// <param name="y">Y координата</param>
    public Point2D(double x, double y)
    {
        X = (float)x;
        Y = (float)y;
    }
}

/// <summary>
/// Представляет стену с начальной и конечной точками
/// </summary>
public class Wall : SceneObject 
{
    /// <summary>
    /// Начальная точка стены
    /// </summary>
    public Point2D StartPoint { get; set; }
    
    /// <summary>
    /// Конечная точка стены
    /// </summary>
    public Point2D EndPoint { get; set; }

    /// <summary>
    /// Создает новую стену
    /// </summary>
    /// <param name="startPoint">Начальная точка</param>
    /// <param name="endPoint">Конечная точка</param>
    /// <param name="name">Имя стены</param>
    public Wall(Point2D startPoint, Point2D endPoint, string name)
    { 
        StartPoint = startPoint; 
        EndPoint = endPoint; 
        Name = name;
    }
}

/// <summary>
/// Представляет шар с центром, радиусом и скоростью
/// </summary>
public class Ball : SceneObject
{
    /// <summary>
    /// Центр шара
    /// </summary>
    public Point2D Center { get; set; }
    /// <summary>
    /// Радиус шара
    /// </summary>
    public float Radius { get; set; }
    /// <summary>
    /// Скорость движения шара
    /// </summary>
    public float Speed { get; set; }

    /// <summary>
    /// Создает новый шар
    /// </summary>
    /// <param name="center">Центр шара</param>
    /// <param name="radius">Радиус</param>
    /// <param name="speed">Скорость</param>
    /// <param name="name">Имя шара</param>
    public Ball(Point2D center, float radius, float speed, string name)
    {
        Center = center;
        Radius = radius;
        Speed = speed;
        Name = name;
    }
}

/// <summary>
/// Контейнер для объектов сцены
/// </summary>
/// <remarks>
/// Управляет коллекцией объектов сцены и предоставляет методы для запросов
/// </remarks>
public class Scene
{
    private List<SceneObject> _objects = new List<SceneObject>();

    /// <summary>
    /// Добавляет объект в сцену
    /// </summary>
    /// <param name="obj">Объект для добавления</param>
    public void AddObject(SceneObject obj)
    { 
        _objects.Add(obj);
    }

    /// <summary>
    /// Удаляет объект из сцены
    /// </summary>
    /// <param name="obj">Объект для удаления</param>
    public void RemoveObject(SceneObject obj)
    { 
        _objects.Remove(obj);
    }
    
    /// <summary>
    /// Возвращает все объекты указанного типа
    /// </summary>
    /// <typeparam name="T">Тип объектов</typeparam>
    /// <returns>Коллекция объектов типа T</returns>
    public IEnumerable<T> GetObjectsOfType<T>() where T : SceneObject
    {
        return _objects.OfType<T>();
    }

    /// <summary>
    /// Возвращает все объекты сцены
    /// </summary>
    /// <returns>Коллекция всех объектов</returns>
    public IReadOnlyList<SceneObject> GetAllObjects() => _objects;
}