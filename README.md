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

### Реализация логики

Логикой задачи является класс, реализующий интерфейс [ITaskLogic](./src/MyLab.TaskApp/ITaskLogic.cs). При разработке приложения потребуется реализовать эту логику. 

```C#
/// <summary>
/// Represent a primary task-application logic
/// </summary>
public interface ITaskLogic
{
    /// <summary>
    /// Performs a task logic
    /// </summary>
    Task Perform(TaskIterationContext iterationContext, CancellationToken cancellationToken);
}
```

### Контекст итерации таска

Среди прочего, в главный метод логики передаётся контекст итерации таска. Этот объект содержит контекстную информацию об итерации:

* `Id` - идентификатор итерации, соответствующей текущей трассировке;

* `StartAt` - дата и время начала выполнения итерации;

* `Report` - объект отчёта, который будет использован для формирования протокола:

    	* `CorrelationId` - некий идентификатор (например, из предметной области), ассоциированный с текущей итерацией

    	* `Workload` -  признак полезности итерации: `Idle` - бесполезная, `Useful` - полезная;
    	* `SubjectId` - идентификатор субъекта из предметной области, ассоциированный с итерацией;
    	* `Metrics` - произвольный словарь численных значений `string / double`.

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

Конфигурация сервисов `Task` по умолчанию берётся из одноимённого узла `Task`. Параметры:

* `IdlePeriod` - временная задержка между итерациями логики. 1 сек - по умолчанию. Значение должно соответствовать [формату TimeSpan](https://learn.microsoft.com/ru-ru/dotnet/api/system.timespan.parse?view=netcore-3.1);
* `ProtocolId` - идентификатор протокола, в который будут писать записи об итерациях. `tasks` - по умолчанию.

Кроме того, при использовании хранилища протоколов будет необходимо указать адрес хранилища в соответствии с правилами [MyLab.ApiClient](https://github.com/mylab-tools/apiclient#%D0%BA%D0%BE%D0%BD%D1%84%D0%B8%D0%B3%D1%83%D1%80%D0%B8%D1%80%D0%BE%D0%B2%D0%B0%D0%BD%D0%B8%D0%B5) для API с ключом `protocol-storage`:

```json
{
  "Api": {
    "List": {
      "protocol-storage": { "Url": "http://..." }
    }
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

## Протокол

Из коробки поддерживается ведение протокола итераций логики таска в хранилище протоколов [MyLab.ProtocolStorage](https://github.com/mylab-search-fx/protocol-storage).

Для активации ведения протокола, необходимо в конфигурации указать адрес API хранилища протоколов:

```json
{
  "Api": {
    "List": {
      "protocol-storage": { "Url": "http://..." }
    }
  }
}
```

Опционально можно указать идентификатор протокола (`tasks` - по умолчанию):

```json
{
  "Task": {
    "ProtocolId": "tasks"
  }
}
```

Объект события протокола имеет [базовый набор служебных полей](https://github.com/mylab-search-fx/protocol-storage#%D1%81%D0%BE%D0%B1%D1%8B%D1%82%D0%B8%D0%B5-%D0%BF%D1%80%D0%BE%D1%82%D0%BE%D0%BA%D0%BE%D0%BB%D0%B0) и дополнительные поля, специфичные для выполнения таска:

* `id` - идентификатор события. Заполняется из `CorrelationId` отчёта итерации;
* `trace_id` - идентификатор трассировки. Заполняется из `Id` контекста итерации;
* `subject` - субъект события. Заполняется из `Subject` отчёта итерации;
* `datetime` - дата и время итерации. Заполняется из `StartAt` контекста итерации;
* `type` - константа `task-iteration`;
* `workload` - признак полезности итерации: `undefined` - не установлено, `idle` - бесполезная (никакая полезная работа не выполнялась), `useful` - полезная работа выполнялась. Заполняется из `Workload` отчёта итерации;
* `metrics` - объект, поля которого имеют дробные численные значения, отражающие метрики предметной области. Заполняется из `Metrics` отчёта итерации;
* `kicker` - инициатор выполнения итерации: `undefined` - не установлен, `api` - внешний http запрос, `scheduler` - внутренний инициатор (например, циклиический);
* `duration` - длительность выполнения таска в миллисекундах;
* `error` - [объект, описывающий необработанное исключение](https://github.com/mylab-log/log#exceptiondto), возникшее в процессе выполнения итерации.

См: [Контекст итерации таска](#Контекст-итерации-таска)
