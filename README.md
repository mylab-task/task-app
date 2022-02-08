# MyLab.TaskApp
[![NuGet Version and Downloads count](https://buildstats.info/nuget/MyLab.TaskApp)](https://www.nuget.org/packages/MyLab.TaskApp)

```
Поддерживаемые платформы: .NET Core 3.1+
```
Ознакомьтесь с последними изменениями в [журнале изменений](/changelog.md).

## Обзор

Каркас приложения, которое выполняет заданную логику по внешнему `http`-запросу или расписанию. 

Для реализации `Task` необходимо: 

* создать проект веб-приложения;
* реализовать логику задачи;
* интегрировать `MyLab.TaskApp` в сервисы приложения;
* интегрировать `MyLab.TaskApp` в пайплайн обработки `http` запросов.

## Логика

Логикой задачи является класс, реализующий интерфейс [ITaskLogic](./src/MyLab.TaskApp/ITaskLogic.cs)

```C#
/// <summary>
/// Represent a primary task-application logic
/// </summary>
public interface ITaskLogic
{
    /// <summary>
    /// Performs a task logic
    /// </summary>
    Task Perform(CancellationToken cancellationToken);
}
```

## Интеграция

Интеграция в сервисы приложения осуществляется одним из следующих способов:

* с указанием типа логики

  ```C#
  services.AddTaskLogic<MyLogic>();
  ```

* с указанием объекта логики

  ```C#
  var logic = new MyLogic();
  services.AddTaskLogic(logic);
  ```

Интеграция  в пайплайн обработки `http` запросов:

```C#
app.UseTaskApi();
```

## Периодический запуск

### Интеграция исполнителя

Для запуска логики периодического запуска по инициативе самого приложения, необходимо интегрировать механизм циклического запуска:

```C#
IConfiguration _config; 

//...

services.AddTaskCirclePerformer(_config);
```

### Конфигурация периодического запуска

Параметры периодического запуска:

* `IdlePeriod` - период между итерациями в формате [TimeSpan](https://docs.microsoft.com/ru-ru/dotnet/api/system.timespan.parse?view=net-6.0). `1 сек` - по умолчанию.

## Конфигурация

Конфигурация сервисов `Task` по умолчанию берётся из одноимённого узла `Task`. Например:

```json
{
    "Task":{
        "IdlePeriod": "0:00:05"
    }
}
```

 ## Статус

Поддерживает интеграцию с [MyLab.StatusProvider](https://github.com/mylab-tools/status-provider).

Добавляет объект статуса задачи:

```c#
/// <summary>
/// Task application specific status
/// </summary>
public class TaskAppStatus : ICloneable
{
    /// <summary>
    /// Last time when an application logic was started
    /// </summary>
    public DateTime? LastTimeStart { get; set; }
    /// <summary>
    /// Duration of application task logic performing
    /// </summary>
    public TimeSpan? LastTimeDuration { get; set; }
    /// <summary>
    /// Error description which occured at last logic performing
    /// </summary>
    public StatusError LastTimeError { get; set; }

    /// <summary>
    /// Determines that task perform itself logic at this time
    /// </summary>
    public bool Processing { get; set; }
}
```
