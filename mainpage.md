/** 
@mainpage Scene Visualizer - Документация проекта

@section intro Введение

Scene Visualizer - это 2D физический симулятор, реализующий следующие технологии:

- **LeoECS** - Entity Component System для управления логикой
- **Avalonia UI** - кроссплатформенный UI фреймворк

@section architecture Архитектура

@dot
digraph architecture {
    rankdir=TB;
    node [shape=rectangle];
    
    MainWindow -> CustomCanvas;
    MainWindow -> EcsSceneManager;
    EcsSceneManager -> Scene;
    EcsSceneManager -> Systems;
    CustomCanvas -> Scene;
    Systems -> Components;
}
@enddot

@section components Основные компоненты

### ECS Компоненты
- SceneObjectComponent - связь с объектом сцены
- WallComponent - данные стены
- BallComponent - данные шара
- MovementDirection - направление движения

### ECS Системы
- WallInitSystem - инициализация стен
- BallInitSystem - инициализация шаров
- MovementSystem - система движения
- CollisionSystem - система столкновений
- SceneSyncSystem - синхронизация сцены

### UI Компоненты
- CustomCanvas - кастомный канвас для отрисовки
- MainWindow - главное окно приложения

@section usage Использование

1. Приложение создает сцену с объектами
2. ECS система управляет физикой и движением
3. CustomCanvas визуализирует состояние сцены
4. Таймер обновляет симуляцию каждые 16мс

@author Богомолов Артём
@date 2025
*/