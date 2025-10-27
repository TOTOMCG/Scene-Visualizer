namespace Scene;

/// <summary>
///     Компонент для связи ECS сущности с объектом сцены
/// </summary>
/// <component>
public struct SceneObjectComponent
{
    /// <summary>
    ///     Ссылка на объект сцены
    /// </summary>
    public SceneObject SceneObject;
}

/// <summary>
///     Компонент стены, содержащий начальную и конечную точки
/// </summary>
/// <component>
public struct WallComponent
{
    /// <summary>
    ///     Начальная точка стены в нормализованных координатах (0-1)
    /// </summary>
    public Point2D Start;

    /// <summary>
    ///     Конечная точка стены в нормализованных координатах (0-1)
    /// </summary>
    public Point2D End;
}

/// <summary>
///     Компонент шара, содержащий параметры для физической симуляции
/// </summary>
/// <component>
public struct BallComponent
{
    /// <summary>
    ///     Центр шара в нормализованных координатах (0-1)
    /// </summary>
    public Point2D Center;

    /// <summary>
    ///     Радиус шара в нормализованных единицах
    /// </summary>
    public float Radius;

    /// <summary>
    ///     Скорость движения шара
    /// </summary>
    public float Speed;
}

/// <summary>
///     Компонент направления движения
/// </summary>
/// <component>
public struct MovementDirection
{
    /// <summary>
    ///     X компонент направления движения
    /// </summary>
    public float X;

    /// <summary>
    ///     Y компонент направления движения
    /// </summary>
    public float Y;

    /// <summary>
    ///     Создает новый компонент направления движения
    /// </summary>
    public MovementDirection(float x, float y)
    {
        X = x;
        Y = y;
    }
}