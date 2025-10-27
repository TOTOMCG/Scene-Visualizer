using Leopotam.Ecs;
/// <summary>
/// Менеджер ECS сцены, координирующий системы и мир ECS
/// </summary>
/// <leoecs>
/// <remarks>
/// Связывает объекты сцены с ECS сущностями и управляет выполнением систем
/// </remarks>
public class EcsSceneManager
{
    private EcsWorld _world;
    private EcsSystems _systems;
    private Scene.Scene _scene;

    // <summary>
    /// Создает новый менеджер ECS сцены
    /// </summary>
    /// <param name="scene">Сцена для управления</param>
    public EcsSceneManager(Scene.Scene scene)
    {
        _world = new EcsWorld();
        _systems = new EcsSystems(_world);
        _scene = scene;
    }

    /// <summary>
    /// Инициализирует ECS системы
    /// </summary>
    public void Initialize()
    {
        _systems
            .Add(new WallInitSystem())
            .Add(new BallInitSystem())
            .Add(new CollisionSystem())
            .Add(new SceneSyncSystem())
            .Add(new MovementSystem())
            .Inject(_scene)
            .Inject(_world)
            .Init();
    }

    /// <summary>
    /// Выполняет один шаг симуляции ECS
    /// </summary>
    public void Update()
    {
        _systems?.Run();
    }

    /// <summary>
    /// Освобождает ресурсы ECS
    /// </summary>
    public void Destroy()
    {
        _systems?.Destroy();
        _systems = null;
        _world?.Destroy();
        _world = null;
    }

    /// <summary>
    /// Возвращает ECS мир
    /// </summary>
    /// <returns>Текущий EcsWorld</returns>
    public EcsWorld GetWorld() => _world;
}

