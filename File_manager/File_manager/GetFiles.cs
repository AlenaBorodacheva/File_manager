using System;
using System.Collections.Generic;
using System.IO;

namespace File_manager
{
    class GetFiles
    {
        public int depth;   // глубина вывода каталога

        public GetFiles(int depth)
        {
            this.depth = depth;
        }

        public void PrintList(List<string> ls, string dirName, int start, int finish)
        {
            if (finish > ls.Count)
                finish = ls.Count;
            
            if (start < 0)
                start = 0;
            
            Console.ForegroundColor = ConsoleColor.Black; // цвет текущего каталога
            Console.WriteLine(dirName);
            Console.ForegroundColor = ConsoleColor.White;

            if (finish != 0)
            {
                for (int i = start; i < finish; i++)
                    Console.WriteLine(ls[i]);
            }
            else                           //Пользователь захотел просмотреть весь список на одном листе
            {
                for (int i = 0; i < ls.Count; i++)
                    Console.WriteLine(ls[i]);
            }
        }
                
        public List<string> GetRecursFiles(string start_path, int level = 0, string start = "")
        {
            if (level == 0)      // для определения заданной глубины вывода каталога
                level = depth;
            
            List<string> ls = new List<string>();
            string[] folders = Directory.GetDirectories(start_path);

            if(level < depth)
                start = "│   " + start;
            else
                start = "├───" + start;

            for (int i = 0; i < folders.Length; i++)
            {
                DirectoryInfo dirinf = new DirectoryInfo(folders[i]);
                ls.Add(start + dirinf.Name);

                level -= 1;

                if (level > 0)
                    ls.AddRange(GetRecursFiles(folders[i], level, start));
                level += 1;
            }
            string[] files = Directory.GetFiles(start_path);
            foreach (string s in files)
            {
                FileInfo fileinf = new FileInfo(s);
                ls.Add(start + fileinf.Name);
            }
            return ls;
        }
    }
}
