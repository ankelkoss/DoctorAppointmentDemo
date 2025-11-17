using Spectre.Console;

namespace DoctorAppointmentDemo.UI
{
    public static class StartEffect
    {
        public static void Banner()
        {
            AnsiConsole.Clear();

            // Баннер приложения
            var figlet = new FigletText("Doctor Appointment App")
                .Centered()
                .Color(Color.Red);

            AnsiConsole.Write(figlet);

            AnsiConsole.MarkupLine("[grey]Нажмите любую клавишу, чтобы открыть меню...[/]");
            Console.ReadKey(true);
        }

        public static void Matrix()
        {
            Console.CursorVisible = false;
            AnsiConsole.Clear();

            var rnd = new Random();

            int width = Console.WindowWidth;
            int height = Console.WindowHeight - 1;

            // для каждой колонки х хранится текущая "голова" дождя
            int[] yPos = new int[width];
            for (int i = 0; i < width; i++)
            {
                yPos[i] = rnd.Next(height);
            }

            int counter = 0;

            while (counter < 50)   // выход по любой клавише
            {
                counter++;
                for (int x = 0; x < width; x++)
                {
                    // случайная цифра
                    char c = (char)('0' + rnd.Next(10));

                    int y = yPos[x];

                    // ставим курсор и рисуем зелёную цифру
                    AnsiConsole.Cursor.SetPosition(x, y);
                    AnsiConsole.Markup("[green]" + c + "[/]");

                    // иногда делаем "пробел" (как затухание)
                    if (rnd.NextDouble() < 0.1)
                    {
                        int yFade = (y - 2 + height) % height;
                        AnsiConsole.Cursor.SetPosition(x, yFade);
                        AnsiConsole.Markup(" ");
                    }

                    // двигаем "голову" вниз
                    yPos[x] = (y + 1) % height;
                }

                Thread.Sleep(30);
            }

            // очистка клавиатуры и возврат нормального курсора
            if (Console.KeyAvailable)
                Console.ReadKey(true);

            Console.CursorVisible = true;
        }
    }
}
