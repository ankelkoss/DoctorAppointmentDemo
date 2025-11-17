using DoctorAppointmentDemo.Domain.Entities;
using DoctorAppointmentDemo.Domain.Enums;
using DoctorAppointmentDemo.Service.Interfaces;
using DoctorAppointmentDemo.Service.Services;
using DoctorAppointmentDemo.UI.Helpers;
using Spectre.Console;
using System.Numerics;

namespace DoctorAppointmentDemo.UI
{
    public class PatientAppointment
    {
        private readonly IPatientService _patientService;
        private Patient BackButton = new Patient();

        public PatientAppointment()
        {
            _patientService = new PatientService();
            BackButton = new Patient
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
                AnsiConsole.MarkupLine("[bold cyan]Patient management menu[/]");
                AnsiConsole.WriteLine();

                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<MenuItemEnum>()
                        .Title("Выберите [green]действие[/]:")
                        .UseConverter(item => item switch
                        {
                            MenuItemEnum.Create => "Создать пациента",
                            MenuItemEnum.Update => "Обновить пациента",
                            MenuItemEnum.Delete => "Удалить пациента",
                            MenuItemEnum.GetById => "Просмотреть пациента по Id",
                            MenuItemEnum.ListAll => "Показать список всех пациентов",
                            MenuItemEnum.BackToMain => "Назад в главное меню",
                            _ => item.ToString()
                        })
                        .AddChoices(Enum.GetValues<MenuItemEnum>()));

                switch (choice)
                {
                    case MenuItemEnum.Create:
                        CreatePatient();
                        break;

                    case MenuItemEnum.Update:
                        UpdatePatient();
                        break;

                    case MenuItemEnum.Delete:
                        DeletePatient();
                        break;

                    case MenuItemEnum.GetById:
                        ShowPatientById();
                        break;

                    case MenuItemEnum.ListAll:
                        ListPatients();
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

        private void CreatePatient()
        {
            AnsiConsole.Clear();
            AnsiConsole.MarkupLine("[bold green]Создание пациента[/]");
            AnsiConsole.WriteLine();

            var patient = new Patient
            {
                Name = ConsoleInputHelpers.AskRequired("Введите [green]имя[/]:"),
                Surname = ConsoleInputHelpers.AskRequired("Введите [green]фамилию[/]:"),
                Phone = ConsoleInputHelpers.AskRequired("Введите [green]телефон[/]:"),
                Email = ConsoleInputHelpers.AskOptional("Введите [green]email[/] (или Enter — пропустить):"),
                IllnessType = ConsoleInputHelpers.AskIllnessType(),
                AdditionalInfo = ConsoleInputHelpers.AskOptional("Дополнительная информация (или Enter — пропустить):"),
                Address = ConsoleInputHelpers.AskOptional("Адрес (или Enter — пропустить):")
            };

            // по твоей просьбе — используем Upsert вместо Create
            var created = _patientService.Upsert(patient);

            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine($"[green]Пациент создан с Id = {created.Id}[/]");
        }

        private void UpdatePatient()
        {
            AnsiConsole.Clear();
            AnsiConsole.MarkupLine("[bold yellow]Обновление пациента[/]");
            AnsiConsole.WriteLine();

            var patients = _patientService.GetAll().ToList();
            if (!patients.Any())
            {
                AnsiConsole.MarkupLine("[yellow]Список пациентов пуст. Обновлять нечего.[/]");
                return;
            }

            var existing = AnsiConsole.Prompt(
                new SelectionPrompt<Patient>()
                    .Title("Выберите [green]пациента для обновления[/]:")
                    .UseConverter(p =>
                        $"{p.Id}. {p.Name} {p.Surname} ({p.IllnessType})")
                    .AddChoices(patients)
                    .AddChoices(BackButton)
            );

            if (existing.Id == -1)
                return;

            AnsiConsole.MarkupLine($"Редактирование пациента: [bold]{existing.Name} {existing.Surname}[/]");
            AnsiConsole.WriteLine();

            existing.Name = ConsoleInputHelpers.AskWithDefault("Имя", existing.Name);
            existing.Surname = ConsoleInputHelpers.AskWithDefault("Фамилия", existing.Surname);
            existing.Phone = ConsoleInputHelpers.AskWithDefault("Телефон", existing.Phone);
            existing.Email = ConsoleInputHelpers.AskWithDefault("Email", existing.Email);

            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine($"Текущий тип болезни: [blue]{existing.IllnessType}[/]");
            if (AnsiConsole.Confirm("Изменить тип болезни?"))
            {
                existing.IllnessType = ConsoleInputHelpers.AskIllnessType();
                // или просто AskIllnessType(), если метод не static / в этом же классе
            }

            existing.AdditionalInfo = ConsoleInputHelpers.AskWithDefault("Доп. информация", existing.AdditionalInfo);
            existing.Address = ConsoleInputHelpers.AskWithDefault("Адрес", existing.Address);

            var updated = _patientService.Update(existing.Id, existing);

            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine($"[green]Пациент с Id = {updated.Id} обновлён.[/]");
        }


        private void DeletePatient()
        {
            AnsiConsole.Clear();
            AnsiConsole.MarkupLine("[bold red]Удаление пациента[/]");
            AnsiConsole.WriteLine();

            var patients = _patientService.GetAll().ToList();
            if (!patients.Any())
            {
                AnsiConsole.MarkupLine("[yellow]Список пациентов пуст. Удалять нечего.[/]");
                return;
            }

            var patient = AnsiConsole.Prompt(
                new SelectionPrompt<Patient>()
                    .Title("Выберите [green]пациента для удаления[/]:")
                    .UseConverter(p =>
                        $"{p.Id}. {p.Name} {p.Surname} ({p.IllnessType})")
                    .AddChoices(patients)
                    .AddChoices(BackButton)
            );

            if (patient.Id == -1)
                return;

            if (!AnsiConsole.Confirm($"Вы уверены, что хотите удалить пациента [red]{patient.Name} {patient.Surname}[/] (Id = {patient.Id})?"))
                return;

            var result = _patientService.Delete(patient.Id);

            if (result)
                AnsiConsole.MarkupLine($"[green]Пациент с Id = {patient.Id} удалён.[/]");
            else
                AnsiConsole.MarkupLine($"[red]Пациент с Id = {patient.Id} не найден.[/]");
        }

        private void ShowPatientById()
        {
            AnsiConsole.Clear();
            AnsiConsole.MarkupLine("[bold cyan]Просмотр пациента по Id и вывод в таблице[/]");
            AnsiConsole.WriteLine();

            var id = ConsoleInputHelpers.AskInt("Введите [green]Id пациента[/]:");

            var patient = _patientService.Get(id);
            if (patient is null)
            {
                AnsiConsole.MarkupLine($"[red]Пациент с Id = {id} не найден.[/]");
                return;
            }

            PrintPatientTable(new[] { patient });
        }

        private void ListPatients()
        {
            AnsiConsole.Clear();
            AnsiConsole.MarkupLine("[bold cyan]Список пациентов[/]");
            AnsiConsole.WriteLine();

            var patients = _patientService.GetAll().ToList();

            if (!patients.Any())
            {
                AnsiConsole.MarkupLine("[yellow]Список пуст.[/]");
                return;
            }

            PrintPatientTable(patients);
        }

        private void PrintPatientTable(IEnumerable<Patient> patients)
        {
            var table = new Table();
            table.Border = TableBorder.Rounded;

            table.AddColumn("Id");
            table.AddColumn("Имя");
            table.AddColumn("Фамилия");
            table.AddColumn("Тип болезни");
            table.AddColumn("Телефон");
            table.AddColumn("Email");
            table.AddColumn("Доп. информация");
            table.AddColumn("Адрес");

            foreach (var p in patients)
            {
                table.AddRow(
                    p.Id.ToString(),
                    p.Name,
                    p.Surname,
                    p.IllnessType.ToString(),
                    p.Phone ?? "",
                    p.Email ?? "",
                    p.AdditionalInfo ?? "",
                    p.Address ?? ""
                );
            }

            AnsiConsole.Write(table);
        }
    }
}
