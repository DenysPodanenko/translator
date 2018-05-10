using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace ookpExamTranslator
{
    class ReadFromFile
    {
        int size = 0;
        string NameFile;
        public ReadFromFile(string filename)
        {
            NameFile = filename;
        }

        public int sizemass()
        {
            try
            {
                int n = 0;
                StreamReader ReadFile = File.OpenText(NameFile);
                string Input = null;
                while ((Input = ReadFile.ReadLine()) != null) { n++; }
                ReadFile.Close();
                size = n;
                return n;
            }
            catch
            {
                return -1;
            }
        }

        public string[] ReturnMass()
        {
            string[] mass = new string[size];
            StreamReader ReadFile = File.OpenText(NameFile);
            string Input = null; int i = 0;
            while ((Input = ReadFile.ReadLine()) != null) { mass[i] = Input; i++; }
            ReadFile.Close();
            return mass;
        }
    }

    class Program
    {
        public static List<string> classParser(string[] massFile)
        {
            List<string> classSection = new List<string>();
            string com = "";
            for (int i=0;i<massFile.Length;i++)
            {
                if (Regex.Match(massFile[i],@"^\$c\s+(.*)$").Length > 0)
                {
                    //Match match = Regex.Match(massFile[i], @"^\$c\s+(.*)$");
                    //var com1 = match.Groups[1].Value;
                    com = "struct "+ Regex.Match(massFile[i], @"^\$c\s+(.*)$").Groups[1].Value.ToString();
                }
                if (Regex.Match(massFile[i], @"^[\s\t]*\$m\s+([a-zA-Z0-9]+)\([a-zA-Z\.]+(,[a-zA-Z])*\)$").Length > 0)
                {
                    Match match = Regex.Match(massFile[i], @"^[\s\t]*\$m\s+([a-zA-Z0-9])\([a-zA-Z\.]+(,[a-zA-Z])*\)$");
                    var com1 = match.Groups[1].Value;
                    com = Regex.Match(massFile[i], @"^\$c\s+(.*)$").Groups[1].Value.ToString();
                }

            }

            return classSection;
        }

        static void Main(string[] args)
        {
            Console.WriteLine(' ');
            string namefile = Directory.GetCurrentDirectory() + '\\';
            //Console.WriteLine(namefile);
            Console.Write("Please, input directory name: ");
            string directory = Console.ReadLine();
            namefile = namefile + directory + '\\';
            Console.Write("Please, input file name: ");
            directory = Console.ReadLine();
            namefile = namefile + directory;
            //Console.WriteLine(namefile);
            try
            {
                    ReadFromFile reader = new ReadFromFile(namefile);
                    int size = reader.sizemass();
                    if (size != -1)
                    {
                        string[] massFile = new string[size];
                        massFile = reader.ReturnMass();

                        List<string> asmFile = new List<string>();
                        asmFile.Add("format PE GUI");
                        asmFile.Add("entry start");
                        asmFile.AddRange(classParser(massFile));
                }
                
            }
            catch
            {
                Console.WriteLine("ERROR");
            }
            Console.ReadKey();
        }
    }
}


