using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace File_manager
{
    class Command
    {
        public void Delete(string dir_or_folder, string dirName)
        {
            string name = Path.Combine(dirName, dir_or_folder);
            FileInfo fileInf = new FileInfo(name);
            DirectoryInfo dirInf = new DirectoryInfo(name);

            if (fileInf.Exists)       // существует ли файл
            {
                fileInf.Delete();
                Console.WriteLine("\nФайл успешно удален.");
            }
            else if (dirInf.Exists)   // тогда существует ли каталог
            {
                DirDel(name);
                dirInf.Delete();
                Console.WriteLine("\nКаталог успешно удален.");
            }
            else
                Console.WriteLine("\nНе найдено.");
            Console.WriteLine("\nНажмите любую клавишу ...");
        }
        public void Copy(string dir_or_folder, string dirName)
        {
            string[] names = dir_or_folder.Split('>'); 
            string name = Path.Combine(dirName, names[0]);  // полный путь того, что копируем
            string dir;                                     // полный путь того, куда копируем
            if (names[1].Contains('\\'))                    // если пользователь ввел полный адрес
                dir = names[1];
            else                                            // если ввел неполный адрес
                dir = Path.Combine(dirName, names[1]);
            string to = dir + "\\" + names[0];              // новый адрес файла после копирования
            
            if (File.Exists(name) && Directory.Exists(dir)) // если существует копируемый файл и директория, в которую нужно скопировать
            {
                if(!File.Exists(to))                        // если в этой директории нет файла с таким именем - копируем
                    File.Copy(name, to);
                else
                {
                    string[] parts = to.Split('.');
                    string part = "";                       // полный адрес файла без расширения 
                    for (int i = 0; i < parts.Length - 1; i++)
                        part += parts[i];

                    string[] files = Directory.GetFiles(dir);
                    Stack<int> Dirs = new Stack<int>();     // сюда будем складывать номера файлов (к примеру, у нас есть файлы text.txt, text(1).txt, text(2).txt)
                    string[] ends;                          // промежуточные переменные для нахождения числа в скобочках
                    string end;
                    int num;                                // порядковый номер файла (каталога)

                    foreach (string s in files)
                    {
                        if (s.Contains(part + "(") && s.Contains(")."))    // Ищем последнюю цифру в скобочках во всех файлах папки и складываем цифру в стек
                        {
                            ends = s.Split('(');                           // text    1).txt
                            ends = ends[ends.Length - 1].Split('.');       // 1)      txt
                            end = ends[0];                                 // 1
                            end = end.Substring(0, end.Length - 1);        // если число двузначное
                            if (Int32.TryParse(end, out num))
                                Dirs.Push(num);
                        }
                    }
                    if(Dirs.Count != 0)                          // если в стеке есть найденные цифры, то создаем имя с цифрой на 1 больше последней цифры
                    {
                        num = Dirs.Peek();
                        to = part + $"({num + 1})." + parts[parts.Length - 1];
                        File.Copy(name, to);
                    }
                    else
                    {
                        to = part + "(1)." + parts[parts.Length - 1];
                        File.Copy(name, to);
                    }
                }
                Console.WriteLine("\nФайл успешно скопирован.");
            }
            else if (Directory.Exists(name) && Directory.Exists(dir))   // если существует копируемая директория и директория, куда нужно скопировать
            {                                                           // делаем аналогично копированию файлов
                if (!Directory.Exists(to))
                    Directory.CreateDirectory(to);
                else
                {
                    string[] dirs = Directory.GetDirectories(dir);
                    Stack<int> Dirs = new Stack<int>();
                    string[] ends;                       
                    string end;
                    int num;                               

                    foreach (string s in dirs)
                    {
                        if (s.Contains(to + "(") && s.Contains(")."))
                        {
                            ends = s.Split('(');
                            end = ends[ends.Length - 1];
                            end = end.Substring(0, end.Length - 1);
                            if (Int32.TryParse(end, out num))
                                Dirs.Push(num);
                        }
                    }
                    if(Dirs.Count != 0)
                    {
                        num = Dirs.Peek();
                        to += $"({num + 1})";
                        Directory.CreateDirectory(to);
                    }
                    else
                    {
                        to += "(1)";
                        Directory.CreateDirectory(to);
                    }
                }
                DirCopy(name, to); 
                Console.WriteLine("\nКаталог успешно скопирован.");
            }
            else
                Console.WriteLine("\nНе найдено.");
            Console.WriteLine("\nНажмите любую клавишу ...");
        }
        public string LastDir(string dirName)
        {
            DirectoryInfo dir = new DirectoryInfo(dirName);
            if (dir.Parent != null)
                return dir.Parent.FullName;
            else
                return dirName;
        }
        public void Move(string dir_or_folder, string dirName)
        {
            string[] names = dir_or_folder.Split('>');
            string name = Path.Combine(dirName, names[0]);
            string dir;
            if (names[1].Contains('\\'))
                dir = names[1];
            else
                dir = Path.Combine(dirName, names[1]);
            string to = dir + "\\" + names[0];

            if (File.Exists(name) && Directory.Exists(dir))
            {
                File.Move(name, to);
                Console.WriteLine("\nФайл успешно перемещен.");
            }
            else if (Directory.Exists(name) && Directory.Exists(dir))
            {
                Directory.Move(name, to);
                Console.WriteLine("\nКаталог успешно перемещен.");
            }
            else
                Console.WriteLine("\nНе найдено.");
            
            Console.WriteLine("\nНажмите любую клавишу ...");
        }
        public void Information(string user_name, string dirName)
        {
            void MyConsoleColor(string attr, string result = "")
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write(attr);
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(result);
            }
            string name = Path.Combine(dirName, user_name);
            FileInfo fileInf = new FileInfo(name);
            DirectoryInfo dirInf = new DirectoryInfo(name); 
            FileAttributes attributes = File.GetAttributes(name);

            if (fileInf.Exists)
            {
                MyConsoleColor("\nПолное имя файла: ", fileInf.FullName);
                MyConsoleColor("Дата и время создания: ", $"{fileInf.CreationTime}");
                MyConsoleColor("Дата и время последнего изменения: ", $"{fileInf.LastWriteTime}");
                MyConsoleColor("Размер: ", $"{fileInf.Length} байт");
                MyConsoleColor("Аттрибуты:");

                if ((attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                    Console.WriteLine("Доступен только для чтения.");
                else
                    Console.WriteLine("Доступен для чтения и редактирования.");
                if ((attributes & FileAttributes.Hidden) == FileAttributes.Hidden)
                    Console.WriteLine("Скрытый файл.");
                if ((attributes & FileAttributes.System) == FileAttributes.System)
                    Console.WriteLine("Системный файл.");
                if ((attributes & FileAttributes.Temporary) == FileAttributes.Temporary)
                    Console.WriteLine("Временный файл.");
            }
            else if (dirInf.Exists)
            {
                MyConsoleColor("\nПолное имя каталога: ", dirInf.FullName);
                MyConsoleColor("Дата и время создания: ", $"{dirInf.CreationTime}");
                MyConsoleColor("Дата и время последнего изменения: ", $"{dirInf.LastWriteTime}");
                double size = 0;
                size = DirSize(name, ref size);
                if (size != -1)
                    MyConsoleColor("Размер: ", $"{size} байт");
                else
                    Console.WriteLine("Размер каталога неизвестен из-за ограниченного доступа.");
                MyConsoleColor("Аттрибуты:");

                if ((attributes & FileAttributes.Hidden) == FileAttributes.Hidden)
                    Console.WriteLine("Скрытый каталог.");
                else
                    Console.WriteLine("Доступный каталог.");
            }
        }
        public double DirSize(string dirName, ref double size)
        {
            try
            {
                DirectoryInfo dir = new DirectoryInfo(dirName);
                DirectoryInfo[] dirs = dir.GetDirectories();
                FileInfo[] files = dir.GetFiles();
                foreach (FileInfo f in files)
                {
                    size += f.Length;
                }
                foreach (DirectoryInfo d in dirs)
                {
                    DirSize(d.FullName, ref size);
                }
                return Math.Round((double)size);
            }
            catch(UnauthorizedAccessException)
            {
                return -1;
            }
        }
        public void DirDel(string dirName)
        {
            DirectoryInfo dir = new DirectoryInfo(dirName);
            DirectoryInfo[] dirs = dir.GetDirectories();
            FileInfo[] files = dir.GetFiles();

            foreach (FileInfo f in files)
                f.Delete();

            foreach (DirectoryInfo d in dirs)
            {
                DirDel(d.FullName);
                if (d.GetDirectories().Length == 0 && d.GetFiles().Length == 0) 
                    d.Delete();
            }
        }
        public void DirCopy(string dirName, string to)
        {
            string[] files = Directory.GetFiles(dirName);
            string[] folders = Directory.GetDirectories(dirName);
            foreach (string f in files)
            {
                string[] shortName = f.Split('\\');
                string adress = to + "\\" + shortName[shortName.Length - 1];
                File.Copy(f, adress);
            }
            foreach (string f in folders)
            {
                string[] shortName = f.Split('\\');
                string adress = to + "\\" + shortName[shortName.Length - 1];
                Directory.CreateDirectory(adress);
                DirCopy(f, adress);
            }
        }
    }
}
