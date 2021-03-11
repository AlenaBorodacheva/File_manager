using System;
using System.Collections.Generic;
using System.IO;
using System.Globalization;

namespace File_manager
{
    class Program
    {
        static readonly string instr1 = " Используйте стрелки вправо и влево для перемещения по листам." +
            "\n Для перехода в следующий каталог нажмите пробел." +
            "\n Для возвращения в предыдущий каталог нажмите Backspace." +
            "\n Для ввода команды нажмите Enter," +
            "\n Для выхода из программы нажмите Esc.";
        static readonly string instr2 = " inf - получение информации о файле или каталоге,             " +
            "\n del - удаление файла или каталога,              " +
            "\n cp - копирование файла или каталога,                   " +
            "\n mv - перемещение файла или каталога (с перезаписью)." +
            "\n                                       ";
        static readonly int count_instr_str = instr1.Split('\n').Length + 1;                       // количество строчек в инструкциях
        public static int depth = int.Parse(Config.Depth(), CultureInfo.InvariantCulture);         // Глубина дерева
        public static int strCount = int.Parse(Config.StrCount(), CultureInfo.InvariantCulture);   // Кол-во выводимых строк
        public static int maxHor = int.Parse(Config.StrMaxHor(), CultureInfo.InvariantCulture);    // Размер консоли по горизонтали. Рекомендуется не менее 65.

        static readonly int str = strCount - 1;   // Потому что первая строчка - имя директории, в которой находимся
        static void Main(string[] args)
        {
            if (maxHor < 65)
                maxHor = 65;

            Console.BackgroundColor = ConsoleColor.DarkGray;
            char sym = '═';                                   // Символ рамки

            if (str != 0)                                     // Если вывести не весь список целиком
            {
                Console.SetWindowSize(maxHor, str + 23);
                Console.SetBufferSize(maxHor, str + 23); 
            }

            string dirName;    // Директория, в которой находимся
            try
            {
                dirName = File.ReadAllText("Directory.json");
            }
            catch(FileNotFoundException)
            {
                dirName = "C:\\";
            }

            int start = 0;                          // диапазон выводимых строк на одной странице
            int finish = str;  
            int page = 1;                           // начальная страница
            List<string> ls = new List<string>();   // Список папок и файлов

            while (true)
            {
                try
                {
                    if (str != 0)
                        PrintArea(sym);
                    else
                        Console.Clear();
                    ls = GetList(dirName, page, start, finish);
                }
                catch(DirectoryNotFoundException)
                {
                    Console.Clear();
                    Console.WriteLine("\nКаталог не найден.\nНажмите любую клавишу ...");
                    Console.ReadKey(true);
                    Command command = new Command();
                    dirName = command.LastDir(dirName);   // выходим из каталога
                    if (str != 0)
                        PrintArea(sym);
                    else
                        Console.Clear();
                    ls = GetList(dirName, page, start, finish);
                }
                catch(UnauthorizedAccessException)
                {
                    Console.Clear();
                    if(depth != 1)   // Если вложенная папка закрыта - меняем глубину
                    {
                        Console.WriteLine("\nНет доступа к некоторым директориям. Глубина отображения дерева изменена.\nНажмите любую клавишу ...");
                        Console.ReadKey(true);
                        depth = 1;
                        if (str != 0)
                            PrintArea(sym);
                        else
                            Console.Clear();
                        continue;
                    }
                    else          // Значит, закрыт текущий каталог. Нужно менять адрес
                    {
                        Console.WriteLine("\nНет доступа к указанной директории. Измените адрес:   ");
                        dirName = Console.ReadLine();
                        continue;
                    }
                }
                catch
                {
                    Console.Clear();
                    Console.WriteLine("\nНеизвестная ошибка. Обратитесь к разработчику.\nПриложение будет закрыто.");
                    Console.ReadKey(true);
                    break;
                }

                ConsoleKey key = Console.ReadKey(true).Key;
                if (key == ConsoleKey.RightArrow)
                {
                    if (finish != 0 && finish < ls.Count)
                    {
                        start = finish;
                        finish += str;
                        page++;
                    }
                }
                else if(key == ConsoleKey.LeftArrow)
                {
                    if (start != 0)
                    {
                        finish = start;
                        start -= str;
                        page--;
                    }
                }
                else if (key == ConsoleKey.Spacebar)
                {
                    Console.Write("\nВведите имя каталога:  ");
                    dirName = Path.Combine(dirName, Console.ReadLine());
                    start = 0;
                    finish = str;
                    page = 1;
                }
                else if(key == ConsoleKey.Backspace)
                {
                    Command command = new Command();
                    dirName = command.LastDir(dirName);
                    start = 0;
                    finish = str;
                    page = 1;
                }
                else if(key == ConsoleKey.Enter)
                {
                    try
                    {
                        Command command = new Command();
                        if (str != 0)
                            Console.SetCursorPosition(0, str + count_instr_str);
                        Console.WriteLine("\n" + instr2);
                        Console.Write("\nВведите команду:  ");

                        string user_command = Console.ReadLine();
                        string[] user_commands = user_command.Split(' '); // разбиваем на команды
                        string lastStr = "";
                        for (int i = 1; i < user_commands.Length; i++)    // убираем из строки первое слово
                            lastStr += user_commands[i] + " ";
                        lastStr = lastStr.Substring(0, lastStr.Length - 1);

                        if (user_commands[0] == "del")
                            command.Delete(lastStr, dirName);

                        else if (user_commands[0] == "cp")
                            command.Copy(lastStr, dirName);

                        else if (user_commands[0] == "mv")
                            command.Move(lastStr, dirName);

                        else if (user_commands[0] == "inf")
                            command.Information(lastStr, dirName);
                        else
                            throw new ArgumentOutOfRangeException(); 
                    }
                    catch (UnauthorizedAccessException)
                    {
                        Console.WriteLine("\nОтсутствует доступ к файлу. Операция не выполнена.\nНажмите любую клавишу ...");
                    }
                    catch(ArgumentOutOfRangeException)
                    {
                        Console.WriteLine("\nКоманда не найдена.\nНажмите любую клавишу ...");
                    }
                    catch (FileNotFoundException)
                    {
                        Console.WriteLine("\nНе найдено.\nНажмите любую клавишу ...");
                    }
                    catch
                    {
                        Console.WriteLine("\nНеизвестная ошибка. Обратитесь к разработчику\nНажмите любую клавишу ...");
                    }
                    Console.ReadKey(true);
                }
                else if(key == ConsoleKey.Escape)
                {
                    File.WriteAllText("Directory.json", dirName);
                    break;
                }
            }
        }
        static List<string> GetList(string dirName, int page, int start, int finish)
        {
            GetFiles files = new GetFiles(depth);
            List<string> ls = files.GetRecursFiles(dirName);
            int pages;
            if (str == 0)
                pages = 1;
            else if (ls.Count % str != 0)
                pages = ls.Count / str + 1;
            else
                pages = ls.Count / str;
            files.PrintList(ls, dirName, start, finish);
            if(str != 0)
            {
                Console.SetCursorPosition(0, str + 4);
                Console.WriteLine($"\n Страница {page} из {pages}.");
                Console.SetCursorPosition(0, str + count_instr_str);
            }
            Console.WriteLine("\n" + instr1);
            return ls;
        }

        static void PrintArea(char sym)
        {
            Console.Clear();
            int maxVert = str + count_instr_str;
            for (int i = 0; i < maxHor; i++)
                Console.Write(sym);

            Console.SetCursorPosition(0, maxVert);

            for (int i = 0; i < maxHor; i++)
                Console.Write(sym);

            Console.SetCursorPosition(0, maxVert + count_instr_str);

            for (int i = 0; i < maxHor; i++)
                Console.Write(sym);

            Console.SetCursorPosition(0, 1);
        }
    }
}
