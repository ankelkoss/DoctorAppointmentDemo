# DoctorAppointmentDemo

# Описание

.NET 8 консольное приложение с объектно-ориентированной архитектурой: DoctorAppointmentDemo и консольное меню.

## Модификации по сравнению с классическим решением

1. В Constants добавлены механизмы для автоматического определения путей к решению и подстановки относительных путей проектов

2. В IGenericRepository добавлен и корректно реализован в наследниках через рефлексию (ReflectionSerch.cs)

   ```
   TSource Upsert(TSource source);
   ```

3. В GenericRepository и других

   - Заменены поля

     ```
     public override string Path { get; set; }
     public override int LastId { get; set; }
     ```

     на

     ```
     public override JsonConfig JsonConfig { get; set; }
     ```

   - Обновлен GetAll(), добавлена авто проверка\авто фикс последнего корректного id записи

   - Реализация метода перенесена из наследников

     ```
     protected abstract void SaveLastId();
     ```

     на

     ```
     public void SaveLastId(JsonConfig jsonConfig)
     ```

   - Добавлены два метода для обработки относительных путей json файлов
     ```
     protected AppDbConfig AddGlobalPathAppSettings(AppDbConfig appDbConfig)
     protected AppDbConfig MoveGlobalPathAppSettings(AppDbConfig appDbConfig)
     ```

# Требования

SDK .NET 8 или новее: https://dotnet.microsoft.com/download
ОС: Windows / Linux / macOS
Желательно: Git 2.4+ для работы с ветками и PR

# Быстрый старт

dotnet build

# Запустить консольное приложение

dotnet run --project ./DoctorAppointmentDemo
