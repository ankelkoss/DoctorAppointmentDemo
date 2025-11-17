using DoctorAppointmentDemo.Domain.Entities;
using DoctorAppointmentDemo.Domain.Enums;
using DoctorAppointmentDemo.Service.Interfaces;
using DoctorAppointmentDemo.Service.Services;
using DoctorAppointmentDemo.UI.Helpers;
using Spectre.Console;

namespace DoctorAppointmentDemo.UI
{
    public class AppAppointment
    {
        private readonly IAppointmentService _appointmentService;
        private readonly IDoctorService _doctorService;
        private readonly IPatientService _patientService;
        private Appointment BackButton = new Appointment();

        public AppAppointment()
        {
            _appointmentService = new AppointmentService();
            _doctorService = new DoctorService();
            _patientService = new PatientService();

            BackButton = new Appointment
            {
                Id = -1,
                Description = "<< Назад >>"
            };
        }

        public void Menu()
        {
            var exit = false;

            while (!exit)
            {
                AnsiConsole.Clear();
                AnsiConsole.MarkupLine("[bold cyan]Appointment management menu[/]");
                AnsiConsole.WriteLine();

                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<MenuItemEnum>()
                        .Title("Выберите [green]действие[/]:")
                        .UseConverter(item => item switch
                        {
                            MenuItemEnum.Create => "Создать запись",
                            MenuItemEnum.Update => "Обновить запись",
                            MenuItemEnum.Delete => "Удалить запись",
                            MenuItemEnum.GetById => "Просмотреть запись по Id",
                            MenuItemEnum.ListAll => "Показать список всех записей",
                            MenuItemEnum.BackToMain => "Назад в главное меню",
                            _ => item.ToString()
                        })
                        .AddChoices(Enum.GetValues<MenuItemEnum>()));

                switch (choice)
                {
                    case MenuItemEnum.Create:
                        CreateAppointment();
                        break;

                    case MenuItemEnum.Update:
                        UpdateAppointment();
                        break;

                    case MenuItemEnum.Delete:
                        DeleteAppointment();
                        break;

                    case MenuItemEnum.GetById:
                        ShowAppointmentById();
                        break;

                    case MenuItemEnum.ListAll:
                        ListAppointments();
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

        private void CreateAppointment()
        {
            AnsiConsole.Clear();
            AnsiConsole.MarkupLine("[bold green]Создание записи[/]");
            AnsiConsole.WriteLine();

            var doctor = SelectDoctor();
            if (doctor is null)
            {
                AnsiConsole.MarkupLine("[red]Нет доступных докторов. Сначала создайте доктора.[/]");
                return;
            }

            var patient = SelectOrCreatePatient();
            if (patient is null)
            {
                AnsiConsole.MarkupLine("[red]Пациент не выбран и не создан.[/]");
                return;
            }

            var from = ConsoleInputHelpers.AskDateTime("Введите дату и время [green]начала приёма[/] (формат: yyyy-MM-dd HH:mm):");
            var to = ConsoleInputHelpers.AskDateTime("Введите дату и время [green]окончания приёма[/] (формат: yyyy-MM-dd HH:mm):");

            if (to <= from)
            {
                AnsiConsole.MarkupLine("[red]Время окончания должно быть позже времени начала.[/]");
                return;
            }

            var description = ConsoleInputHelpers.AskOptional("Описание приёма (или Enter — пропустить):");

            var appointment = new Appointment
            {
                Doctor = doctor,
                Patient = patient,
                DateTimeFrom = from,
                DateTimeTo = to,
                Description = description
            };

            var created = _appointmentService.Upsert(appointment);

            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine($"[green]Запись создана с Id = {created.Id}[/]");
        }

        private void UpdateAppointment()
        {
            AnsiConsole.Clear();
            AnsiConsole.MarkupLine("[bold yellow]Обновление записи[/]");
            AnsiConsole.WriteLine();

            var apps = _appointmentService.GetAll().ToList();
            if (!apps.Any())
            {
                AnsiConsole.MarkupLine("[yellow]Список записей пуст. Обновлять нечего.[/]");
                return;
            }

            var existing = AnsiConsole.Prompt(
                new SelectionPrompt<Appointment>()
                    .Title("Выберите [green]запись для обновления[/]:")
                    .UseConverter(a =>
                    {
                        if (a.Id == -1)
                            return "[grey]<< Назад >>[/]";

                        var doctor = a.Doctor is null
                            ? "—"
                            : $"{a.Doctor.Name} {a.Doctor.Surname}";

                        var patient = a.Patient is null
                            ? "—"
                            : $"{a.Patient.Name} {a.Patient.Surname}";

                        return $"{a.Id}. {a.DateTimeFrom:yyyy-MM-dd HH:mm} | Dr: {doctor} | Pat: {patient}";
                    })
                    .AddChoices(apps)
                    .AddChoices(BackButton)
            );

            if (existing.Id == -1)
                return;

            AnsiConsole.MarkupLine("[bold]Текущая запись:[/]");
            PrintAppointmentTable(new[] { existing });
            AnsiConsole.WriteLine();

            // Доктор
            if (existing.Doctor is not null)
                AnsiConsole.MarkupLine($"Текущий доктор: [blue]{existing.Doctor.Id}. {existing.Doctor.Name} {existing.Doctor.Surname}[/]");
            else
                AnsiConsole.MarkupLine("[yellow]Доктор не назначен.[/]");
            if (AnsiConsole.Confirm("Изменить доктора?"))
            {
                var doctor = SelectDoctor();
                if (doctor is not null)
                    existing.Doctor = doctor;
            }

            // Пациент
            if (existing.Patient is not null)
                AnsiConsole.MarkupLine($"Текущий пациент: [blue]{existing.Patient.Id}. {existing.Patient.Name} {existing.Patient.Surname}[/]");
            else
                AnsiConsole.MarkupLine("[yellow]Пациент не назначен.[/]");
            if (AnsiConsole.Confirm("Изменить пациента?"))
            {
                var patient = SelectOrCreatePatient();
                if (patient is not null)
                    existing.Patient = patient;
            }

            // Время
            existing.DateTimeFrom = ConsoleInputHelpers.AskDateTimeWithDefault("Дата/время начала", existing.DateTimeFrom);
            existing.DateTimeTo = ConsoleInputHelpers.AskDateTimeWithDefault("Дата/время окончания", existing.DateTimeTo);

            if (existing.DateTimeTo <= existing.DateTimeFrom)
            {
                AnsiConsole.MarkupLine("[red]Время окончания должно быть позже времени начала. Изменения не сохранены.[/]");
                return;
            }

            // Описание
            existing.Description = ConsoleInputHelpers.AskWithDefault("Описание", existing.Description);

            var updated = _appointmentService.Update(existing.Id, existing);

            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine($"[green]Запись с Id = {updated.Id} обновлена.[/]");
        }


        private void DeleteAppointment()
        {
            AnsiConsole.Clear();
            AnsiConsole.MarkupLine("[bold red]Удаление записи[/]");
            AnsiConsole.WriteLine();

            var apps = _appointmentService.GetAll().ToList();
            if (!apps.Any())
            {
                AnsiConsole.MarkupLine("[yellow]Список записей пуст. Удалять нечего.[/]");
                return;
            }

            var app = AnsiConsole.Prompt(
                new SelectionPrompt<Appointment>()
                    .Title("Выберите [green]запись для удаления[/]:")
                    .UseConverter(a =>
                    {
                        if (a.Id == -1)
                            return "[grey]<< Назад >>[/]";

                        var doctor = a.Doctor is null
                            ? "—"
                            : $"{a.Doctor.Name} {a.Doctor.Surname}";

                        var patient = a.Patient is null
                            ? "—"
                            : $"{a.Patient.Name} {a.Patient.Surname}";

                        return $"{a.Id}. {a.DateTimeFrom:yyyy-MM-dd HH:mm} | Dr: {doctor} | Pat: {patient}";
                    })
                    .AddChoices(apps)
                    .AddChoices(BackButton)
            );

            if (app.Id == -1)
                return;

            if (!AnsiConsole.Confirm(
                    $"Вы уверены, что хотите удалить запись [red]Id = {app.Id}[/] от {app.DateTimeFrom:yyyy-MM-dd HH:mm}?"))
                return;

            var result = _appointmentService.Delete(app.Id);

            if (result)
                AnsiConsole.MarkupLine($"[green]Запись с Id = {app.Id} удалена.[/]");
            else
                AnsiConsole.MarkupLine($"[red]Запись с Id = {app.Id} не найдена.[/]");
        }


        private void ShowAppointmentById()
        {
            AnsiConsole.Clear();
            AnsiConsole.MarkupLine("[bold cyan]Просмотр записи по Id[/]");
            AnsiConsole.WriteLine();

            var id = ConsoleInputHelpers.AskInt("Введите [green]Id записи[/]:");

            var app = _appointmentService.Get(id);
            if (app is null)
            {
                AnsiConsole.MarkupLine($"[red]Запись с Id = {id} не найдена.[/]");
                return;
            }

            PrintAppointmentTable(new[] { app });
        }

        private void ListAppointments()
        {
            AnsiConsole.Clear();
            AnsiConsole.MarkupLine("[bold cyan]Список записей[/]");
            AnsiConsole.WriteLine();

            var apps = _appointmentService.GetAll().ToList();

            if (!apps.Any())
            {
                AnsiConsole.MarkupLine("[yellow]Список пуст.[/]");
                return;
            }

            PrintAppointmentTable(apps);
        }

        //------------------------------------------------------------

        private Doctor? SelectDoctor()
        {
            var doctors = _doctorService.GetAll().ToList();
            if (!doctors.Any())
            {
                return null;
            }

            return AnsiConsole.Prompt(
                new SelectionPrompt<Doctor>()
                    .Title("Выберите [green]доктора[/]:")
                    .UseConverter(d =>
                        $"{d.Id}. {d.Name} {d.Surname} ({d.DoctorType})")
                    .AddChoices(doctors));
        }

        private Patient? SelectOrCreatePatient()
        {
            if (!AnsiConsole.Confirm("Выбрать существующего пациента? (No — создать нового)"))
            {
                return CreateNewPatientInline();
            }

            var patients = _patientService.GetAll().ToList();
            if (!patients.Any())
            {
                AnsiConsole.MarkupLine("[yellow]Существующих пациентов нет. Нужно создать нового.[/]");
                return CreateNewPatientInline();
            }

            return AnsiConsole.Prompt(
                new SelectionPrompt<Patient>()
                    .Title("Выберите [green]пациента[/]:")
                    .UseConverter(p =>
                        $"{p.Id}. {p.Name} {p.Surname} ({p.IllnessType})")
                    .AddChoices(patients));
        }

        private Patient CreateNewPatientInline()
        {
            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine("[bold green]Создание нового пациента[/]");

            var patient = new Patient
            {
                Name = ConsoleInputHelpers.AskRequired("Введите [green]имя пациента[/]:"),
                Surname = ConsoleInputHelpers.AskRequired("Введите [green]фамилию пациента[/]:"),
                Phone = ConsoleInputHelpers.AskRequired("Введите [green]телефон пациента[/]:"),
                Email = ConsoleInputHelpers.AskOptional("Введите [green]email[/] (или Enter — пропустить):"),
                IllnessType = ConsoleInputHelpers.AskIllnessType(),
                AdditionalInfo = ConsoleInputHelpers.AskOptional("Дополнительная информация (или Enter — пропустить):"),
                Address = ConsoleInputHelpers.AskOptional("Адрес (или Enter — пропустить):")
            };

            var created = _patientService.Upsert(patient);

            AnsiConsole.MarkupLine($"[green]Пациент создан с Id = {created.Id}[/]");
            return created;
        }

        private void PrintAppointmentTable(IEnumerable<Appointment> apps)
        {
            var table = new Table();
            table.Border = TableBorder.Rounded;

            table.AddColumn("Id");
            table.AddColumn("Доктор");
            table.AddColumn("Пациент");
            table.AddColumn("Начало");
            table.AddColumn("Окончание");
            table.AddColumn("Описание");

            foreach (var a in apps)
            {
                var doctorText = a.Doctor is null
                    ? "[red]нет[/]"
                    : $"{a.Doctor.Id}. {a.Doctor.Name} {a.Doctor.Surname} ({a.Doctor.DoctorType})";

                var patientText = a.Patient is null
                    ? "[red]нет[/]"
                    : $"{a.Patient.Id}. {a.Patient.Name} {a.Patient.Surname} ({a.Patient.IllnessType})";

                table.AddRow(
                    a.Id.ToString(),
                    doctorText,
                    patientText,
                    a.DateTimeFrom.ToString("yyyy-MM-dd HH:mm"),
                    a.DateTimeTo.ToString("yyyy-MM-dd HH:mm"),
                    a.Description ?? ""
                );
            }

            AnsiConsole.Write(table);
        }
    }
}
