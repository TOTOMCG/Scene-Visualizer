using System;
using System.Collections.Generic;
using Leopotam.Ecs;
using Scene;

public class WallInitSystem : IEcsInitSystem
{
    private EcsWorld _world;
    private Scene.Scene _scene;

    public void Init()
    {   
        foreach (var wall in _scene.GetObjectsOfType<Wall>())
        {
            var entity = _world.NewEntity();
            ref var wallComp = ref entity.Get<WallComponent>();
            ref var sceneObjComp = ref entity.Get<SceneObjectComponent>();
            
            wallComp.Start = new Point2D(wall.StartPoint.X, wall.StartPoint.Y);
            wallComp.End = new Point2D(wall.EndPoint.X, wall.EndPoint.Y);
            sceneObjComp.SceneObject = wall;
            
            Console.WriteLine($"Created wall entity: {wall.Name} from ({wall.StartPoint.X},{wall.StartPoint.Y}) to ({wall.EndPoint.X},{wall.EndPoint.Y})");
        }
    }
}

public class BallInitSystem : IEcsInitSystem
{
    private EcsWorld _world;
    private Scene.Scene _scene;

    public void Init()
    {
        foreach (var ball in _scene.GetObjectsOfType<Ball>())
        {
            var entity = _world.NewEntity();
            ref var ballComp = ref entity.Get<BallComponent>();
            ref var sceneObjComp = ref entity.Get<SceneObjectComponent>();

            ballComp.Center = new Point2D(ball.Center.X, ball.Center.Y);
            ballComp.Radius = ball.Radius;
            ballComp.Speed = ball.Speed;
            sceneObjComp.SceneObject = ball;
            
            entity.Get<MovementDirection>() = new MovementDirection(0f, 1f);
            
            Console.WriteLine($"Created ball entity: {ball.Name} at ({ball.Center.X}, {ball.Center.Y}) with speed {ball.Speed}");
        }
    }
}

public class MovementSystem : IEcsRunSystem
{
    private EcsFilter<BallComponent, MovementDirection, SceneObjectComponent> _movingBalls;
    private const float DeltaTime = 0.016f;

    public void Run()
    {
        foreach (var i in _movingBalls)
        {
            ref var ballComp = ref _movingBalls.Get1(i);
            ref var movement = ref _movingBalls.Get2(i);
            ref var sceneObjComp = ref _movingBalls.Get3(i);
            
            // Обновляем позицию
            ballComp.Center.X += movement.X * ballComp.Speed * DeltaTime;
            ballComp.Center.Y += movement.Y * ballComp.Speed * DeltaTime;

            // Обновляем объект сцены
            if (sceneObjComp.SceneObject is Ball ball)
            {
                ball.Center.X = ballComp.Center.X;
                ball.Center.Y = ballComp.Center.Y;
                
                Console.WriteLine($"ECS moved ball to: ({ball.Center.X:F3}, {ball.Center.Y:F3})");
            }
        }
    }
}

public class CollisionSystem : IEcsRunSystem
{
    private EcsFilter<BallComponent, MovementDirection> _balls;
    private EcsFilter<WallComponent, SceneObjectComponent> _walls;
    
    public void Run()
    {
        float deltaTime = 0.016f;
        
        foreach (var ballIdx in _balls)
        {
            ref var ballComp = ref _balls.Get1(ballIdx);
            ref var movement = ref _balls.Get2(ballIdx);
            
            var previousPosition = new Point2D(
                ballComp.Center.X - movement.X * ballComp.Speed * deltaTime,
                ballComp.Center.Y - movement.Y * ballComp.Speed * deltaTime
            );
            
            var movementVector = new Point2D(
                movement.X * ballComp.Speed * deltaTime,
                movement.Y * ballComp.Speed * deltaTime
            );
            
            // Ищем ближайшее столкновение
            WallComponent? closestWall = null;
            Point2D closestNormal = new Point2D(0, 0);
            float closestCollisionTime = 1.0f;
            
            foreach (var wallIdx in _walls)
            {
                ref var wallComp = ref _walls.Get1(wallIdx);
                
                if (CheckCircleWallCollision(previousPosition, ballComp.Radius, 
                    wallComp, out var collisionNormal, out var collisionTime))
                {
                    if (collisionTime < closestCollisionTime)
                    {
                        closestCollisionTime = collisionTime;
                        closestNormal = collisionNormal;
                        closestWall = wallComp;
                    }
                }
            }
            
            // Обрабатываем ближайшее столкновение
            if (closestWall != null && closestCollisionTime <= 1.0f)
            {
                // Корректируем позицию до точки столкновения
                ballComp.Center.X = previousPosition.X + movementVector.X * closestCollisionTime;
                ballComp.Center.Y = previousPosition.Y + movementVector.Y * closestCollisionTime;
                
                // Отладочная информация
                Console.WriteLine($"Столкновение со стеной! Время: {closestCollisionTime:F3}");
                Console.WriteLine($"Нормаль: [{closestNormal.X:F3}, {closestNormal.Y:F3}]");
                
                // Отражаем движение
                ReflectMovement(ref movement, closestNormal);
                
                // Применяем оставшееся движение после отскока
                var remainingMovement = 1.0f - closestCollisionTime;
                ballComp.Center.X += movement.X * ballComp.Speed * deltaTime * remainingMovement;
                ballComp.Center.Y += movement.Y * ballComp.Speed * deltaTime * remainingMovement;
            }
        }
    }
    
    private bool CheckCircleWallCollision(Point2D circleStart, float radius, 
        WallComponent wall, out Point2D normal, out float collisionTime)
    {
        normal = new Point2D(0, 0);
        collisionTime = 1.0f;
        
        // Вектор стены
        var wallVec = new Point2D(wall.End.X - wall.Start.X, wall.End.Y - wall.Start.Y);
        var wallLength = (float)Math.Sqrt(wallVec.X * wallVec.X + wallVec.Y * wallVec.Y);
        
        if (wallLength < 0.0001f) return false;
        
        // Нормализованный вектор стены
        var wallDir = new Point2D(wallVec.X / wallLength, wallVec.Y / wallLength);
        
        // Вектор от начала стены к центру круга
        var toCircle = new Point2D(circleStart.X - wall.Start.X, circleStart.Y - wall.Start.Y);
        
        // Проекция центра круга на стену
        var projection = wallDir.X * toCircle.X + wallDir.Y * toCircle.Y;
        
        // Ближайшая точка на стене к центру круга
        Point2D closestPoint;
        if (projection <= 0)
        {
            closestPoint = wall.Start;
        }
        else if (projection >= wallLength)
        {
            closestPoint = wall.End;
        }
        else
        {
            closestPoint = new Point2D(
                wall.Start.X + projection * wallDir.X,
                wall.Start.Y + projection * wallDir.Y
            );
        }
        
        // Вектор от ближайшей точки к центру круга
        var toCenter = new Point2D(
            circleStart.X - closestPoint.X,
            circleStart.Y - closestPoint.Y
        );
        
        // Расстояние до стены
        var distance = (float)Math.Sqrt(toCenter.X * toCenter.X + toCenter.Y * toCenter.Y);
        
        if (distance > radius) return false; // Нет столкновения
        
        // Вычисляем нормаль (от стены наружу)
        if (distance > 0.0001f)
        {
            normal.X = toCenter.X / distance;
            normal.Y = toCenter.Y / distance;
        }
        else
        {
            // Если расстояние 0, используем перпендикуляр к стене
            normal.X = -wallDir.Y;
            normal.Y = wallDir.X;
            
            // Убедимся, что нормаль направлена от стены
            var dot = normal.X * toCircle.X + normal.Y * toCircle.Y;
            if (dot < 0)
            {
                normal.X = -normal.X;
                normal.Y = -normal.Y;
            }
        }
        
        // Столкновение происходит в начале движения
        
        collisionTime = 0f;
        
        return true;
    }
    
    private void ReflectMovement(ref MovementDirection movement, Point2D normal)
    {
        // Нормализуем
        var normalLength = (float)Math.Sqrt(normal.X * normal.X + normal.Y * normal.Y);
        if (normalLength > 0)
        {
            normal.X /= normalLength;
            normal.Y /= normalLength;
        }
        
        // Скалярное произведение направления движения и нормали
        var dot = movement.X * normal.X + movement.Y * normal.Y;
        
        // Формула отражения: R = V - 2*(V·N)*N
        movement.X = movement.X - 2 * dot * normal.X;
        movement.Y = movement.Y - 2 * dot * normal.Y;
        
        // Нормализуем результирующий вектор
        var movementLength = (float)Math.Sqrt(movement.X * movement.X + movement.Y * movement.Y);
        if (movementLength > 0)
        {
            movement.X /= movementLength;
            movement.Y /= movementLength;
        }
        
        Console.WriteLine($"Новое направление: [{movement.X:F3}, {movement.Y:F3}]");
    }
}

public class SceneSyncSystem : IEcsRunSystem
{
    private EcsFilter<BallComponent, SceneObjectComponent> _ballsFilter;

    public void Run()
    {
        foreach (var i in _ballsFilter)
        {
            ref var ballComp = ref _ballsFilter.Get1(i);
            ref var sceneObjComp = ref _ballsFilter.Get2(i);

            // Принудительная синхронизация из ECS в объект сцены
            if (sceneObjComp.SceneObject is Ball ball)
            {
                ball.Center.X = ballComp.Center.X;
                ball.Center.Y = ballComp.Center.Y;
            }
        }
    }
}