using DoctorAppointmentDemo.Domain.Entities;
using DoctorAppointmentDemo.Domain.Enums;
using DoctorAppointmentDemo.Service.Interfaces;
using DoctorAppointmentDemo.Service.Services;
using DoctorAppointmentDemo.UI.Helpers;
using Spectre.Console;

namespace DoctorAppointmentDemo.UI
{
    public class DoctorAppointment
    {
        private readonly IDoctorService _doctorService;
        private Doctor BackButton = new Doctor();

        public DoctorAppointment()
        {
            _doctorService = new DoctorService();
            BackButton = new Doctor
            {
                Id = -1,
                Name = "<< Назад >>",
                Surname = string.Empty
            };
        }

        public void Menu()
        {
            var exit = false;

            while (!exit)
            {
                AnsiConsole.Clear();
                AnsiConsole.MarkupLine("[bold cyan]Doctor management menu[/]");
                AnsiConsole.WriteLine();

                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<MenuItemEnum>()
                        .Title("Выберите [green]действие[/]:")
                        .UseConverter(item => item switch
                        {
                            MenuItemEnum.Create => "Создать доктора",
                            MenuItemEnum.Update => "Обновить доктора",
                            MenuItemEnum.Delete => "Удалить доктора",
                            MenuItemEnum.GetById => "Просмотреть доктора по Id",
                            MenuItemEnum.ListAll => "Показать список всех докторов",
                            MenuItemEnum.BackToMain => "Назад в главное меню",
                            _ => item.ToString()
                        })
                        .AddChoices(Enum.GetValues<MenuItemEnum>()));

                switch (choice)
                {
                    case MenuItemEnum.Create:
                        CreateDoctor();
                        break;

                    case MenuItemEnum.Update:
                        UpdateDoctor();
                        break;

                    case MenuItemEnum.Delete:
                        DeleteDoctor();
                        break;

                    case MenuItemEnum.GetById:
                        ShowDoctorById();
                        break;

                    case MenuItemEnum.ListAll:
                        ListDoctors();
                        break;

                    case MenuItemEnum.BackToMain:
                        exit = true;
                        break;
                }

                if (!exit)
                {
                    AnsiConsole.WriteLine();
                    AnsiConsole.MarkupLine("[grey]Нажмите любую клавишу, чтобы продолжить...[/]");
                    Console.ReadKey(true);
                }
            }
        }

        private void CreateDoctor()
        {
            AnsiConsole.Clear();
            AnsiConsole.MarkupLine("[bold green]Создание доктора[/]");
            AnsiConsole.WriteLine();

            var doctor = new Doctor
            {
                Name = ConsoleInputHelpers.AskRequired("Введите [green]имя[/]:"),
                Surname = ConsoleInputHelpers.AskRequired("Введите [green]фамилию[/]:"),
                Phone = ConsoleInputHelpers.AskRequired("Введите [green]телефон[/]:"),
                Email = ConsoleInputHelpers.AskOptional("Введите [green]email[/] (или Enter — пропустить):"),
                DoctorType = AskDoctorType(),
                Experience = ConsoleInputHelpers.AskByte("Введите [green]стаж[/] (в годах):"),
                Salary = ConsoleInputHelpers.AskDecimal("Введите [green]зарплату[/]:")
            };

            var created = _doctorService.Upsert(doctor);

            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine($"[green]Доктор создан с Id = {created.Id}[/]");
        }

        private void UpdateDoctor()
        {
            AnsiConsole.Clear();
            AnsiConsole.MarkupLine("[bold yellow]Обновление доктора[/]");
            AnsiConsole.WriteLine();

            var doctors = _doctorService.GetAll().ToList();
            if (!doctors.Any())
            {
                AnsiConsole.MarkupLine("[yellow]Список докторов пуст. Обновлять нечего.[/]");
                return;
            }

            var existing = AnsiConsole.Prompt(
                new SelectionPrompt<Doctor>()
                    .Title("Выберите [green]доктора для обновления[/]:")
                    .UseConverter(d =>
                        $"{d.Id}. {d.Name} {d.Surname} ({d.DoctorType})")
                    .AddChoices(doctors)
                    .AddChoices(BackButton)
            );

            if (existing.Id == -1)
                return;

            AnsiConsole.MarkupLine($"Редактирование доктора: [bold]{existing.Name} {existing.Surname}[/]");
            AnsiConsole.WriteLine();

            existing.Name = ConsoleInputHelpers.AskWithDefault("Имя", existing.Name);
            existing.Surname = ConsoleInputHelpers.AskWithDefault("Фамилия", existing.Surname);
            existing.Phone = ConsoleInputHelpers.AskWithDefault("Телефон", existing.Phone);
            existing.Email = ConsoleInputHelpers.AskWithDefault("Email", existing.Email);

            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine($"Текущий тип врача: [blue]{existing.DoctorType}[/]");
            if (AnsiConsole.Confirm("Изменить тип врача?"))
            {
                existing.DoctorType = AskDoctorType();
            }

            existing.Experience = ConsoleInputHelpers.AskByteWithDefault("Стаж (годы)", existing.Experience);
            existing.Salary = ConsoleInputHelpers.AskDecimalWithDefault("Зарплата", existing.Salary);

            var updated = _doctorService.Update(existing.Id, existing);

            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine($"[green]Доктор с Id = {updated.Id} обновлён.[/]");
        }


        private void DeleteDoctor()
        {
            AnsiConsole.Clear();
            AnsiConsole.MarkupLine("[bold red]Удаление доктора[/]");
            AnsiConsole.WriteLine();

            var doctors = _doctorService.GetAll().ToList();
            if (!doctors.Any())
            {
                AnsiConsole.MarkupLine("[yellow]Список докторов пуст. Удалять нечего.[/]");
                return;
            }

            var doctor = AnsiConsole.Prompt(
                new SelectionPrompt<Doctor>()
                    .Title("Выберите [green]доктора для удаления[/]:")
                    .UseConverter(d =>
                        $"{d.Id}. {d.Name} {d.Surname} ({d.DoctorType})")
                    .AddChoices(doctors)
                    .AddChoices(BackButton)
            );

            if (doctor.Id == -1)
                return;

            if (!AnsiConsole.Confirm($"Вы уверены, что хотите удалить доктора [red]{doctor.Name} {doctor.Surname}[/] (Id = {doctor.Id})?"))
                return;

            var result = _doctorService.Delete(doctor.Id);

            if (result)
                AnsiConsole.MarkupLine($"[green]Доктор с Id = {doctor.Id} удалён.[/]");
            else
                AnsiConsole.MarkupLine($"[red]Доктор с Id = {doctor.Id} не найден.[/]");
        }


        private void ShowDoctorById()
        {
            AnsiConsole.Clear();
            AnsiConsole.MarkupLine("[bold cyan]Просмотр доктора по Id и вывод в таблице[/]");
            AnsiConsole.WriteLine();

            var id = ConsoleInputHelpers.AskInt("Введите [green]Id доктора[/]:");

            var doctor = _doctorService.Get(id);
            if (doctor is null)
            {
                AnsiConsole.MarkupLine($"[red]Доктор с Id = {id} не найден.[/]");
                return;
            }

            PrintDoctorTable(new[] { doctor });
        }

        private void ListDoctors()
        {
            AnsiConsole.Clear();
            AnsiConsole.MarkupLine("[bold cyan]Список докторов[/]");
            AnsiConsole.WriteLine();

            var doctors = _doctorService.GetAll().ToList();

            if (!doctors.Any())
            {
                AnsiConsole.MarkupLine("[yellow]Список пуст.[/]");
                return;
            }

            PrintDoctorTable(doctors);
        }

        private void PrintDoctorTable(IEnumerable<Doctor> doctors)
        {
            var table = new Table();
            table.Border = TableBorder.Rounded;

            table.AddColumn("Id");
            table.AddColumn("Имя");
            table.AddColumn("Фамилия");
            table.AddColumn("Тип врача");
            table.AddColumn("Стаж");
            table.AddColumn("Зарплата");
            table.AddColumn("Телефон");
            table.AddColumn("Email");

            foreach (var d in doctors)
            {
                table.AddRow(
                    d.Id.ToString(),
                    d.Name,
                    d.Surname,
                    d.DoctorType.ToString(),
                    d.Experience.ToString(),
                    d.Salary.ToString("0.00"),
                    d.Phone ?? "",
                    d.Email ?? ""
                );
            }

            AnsiConsole.Write(table);
        }

        //------------------------------------------------------------
        // выбор типа врача по енам
        public static DoctorTypes AskDoctorType()
        {
            return AnsiConsole.Prompt(
                new SelectionPrompt<DoctorTypes>()
                    .Title("Выберите [green]тип врача[/]:")
                    .UseConverter(t => t switch
                    {
                        DoctorTypes.Dentist => "Dentist (Стоматолог)",
                        DoctorTypes.Dermatologist => "Dermatologist (Дерматолог)",
                        DoctorTypes.FamilyDoctor => "FamilyDoctor (Семейный врач)",
                        DoctorTypes.Paramedic => "Paramedic (Фельдшер)",
                        _ => t.ToString()
                    })
                    .AddChoices(Enum.GetValues<DoctorTypes>())
            );
        }
    }
}
