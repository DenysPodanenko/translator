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
        public static List<string> dataList = new List<string>();
        public static List<string> codeSection = new List<string>();
        public static List<string> loopSection = new List<string>();
        public static List<string> classParser(string[] massFile)
        {
            List<string> classSection = new List<string>();
            string com = "";
            int kl_class = 0;
            for (int i=0; i<massFile.Length; i++)
            { if (Regex.Match(massFile[i], @"^\$c\s+([a-zA-z0-9]+)\s*(\;*(.*))$").Length > 0) kl_class++; }

            //class regocnition
            if(kl_class > 0)
            {
                for (int i = 0; i < massFile.Length; i++)
                {
                    if (Regex.Match(massFile[i], @"^\$c\s+([a-zA-z0-9]+)\s*(\;*(.*))$").Length > 0)
                    {
                        //Match match = Regex.Match(massFile[i], @"^\$c\s+(.*)$");
                        Match match = Regex.Match(massFile[i], @"^\$c\s+([a-zA-z0-9]+)\s*(\;*(.*))$");
                        string comment = ""; if (match.Groups[2].Length != 0) comment = match.Groups[2].Value.ToString();
                        if(comment.Length == 0)com = "struct " + match.Groups[1].Value.ToString();
                        else com = "struct " + match.Groups[1].Value.ToString() + comment;
                        classSection.Add(com);classSection.Add("{");
                        List<string> classOpNames = new List<string>();
                        for(i+=2; i<massFile.Length && Regex.Match(massFile[i], @"^\}+(.*)$").Length == 0; i++)
                        {
                            if (Regex.Match(massFile[i], @"^[\s\t]*\$v\s+([a-zA-Z0-9]+)\s*\=*\s*([0-9]*)\s*(\;*(.*))$").Length > 0)
                            {
                                Match match1 = Regex.Match(massFile[i], @"^[\s\t]*\$v\s+([a-zA-Z0-9]+)\s*\=*\s*([0-9]*)\s*(\;*(.*))$");
                                string name_pr = "";
                                classOpNames.Add(match1.Groups[1].Value.ToString());
                                name_pr = '.' + match1.Groups[1].Value.ToString() + " dd ";
                                if (match1.Groups[2].Length > 0) name_pr += match1.Groups[2].Value.ToString();
                                else name_pr += '?';
                                if (match1.Groups[3].Length > 0) name_pr += match1.Groups[3].Value.ToString();
                                classSection.Add(name_pr);
                            }
                            else if (Regex.Match(massFile[i], @"^[\s\t]*\$m\s+([a-zA-Z0-9]+)\([a-zA-Z\.]+(,[a-zA-Z])*\)\s*(\;*.*)$").Length > 0)
                            {
                                Match match1 = Regex.Match(massFile[i], @"^[\s\t]*\$m\s+([a-zA-Z0-9]+)\(([a-zA-Z\.]+)*\)\s*(\;*.*)$");
                                string name_meth = "", name_per = "";
                                name_meth = '.' + match1.Groups[1].Value.ToString() + ":";
                                if (match1.Groups[3].Length > 0) name_meth += match1.Groups[3].Value.ToString();
                                if (match1.Groups[2].Length > 0) name_per += match1.Groups[2].Value.ToString();
                                classSection.Add(name_meth);
                                classSection.Add("push ebp");
                                classSection.Add("mov ebp, esp");
                                if (name_per.Length > 0){classSection.Add("mov eax,[ebp+8]");}
                                for(i+=2; i < massFile.Length && Regex.Match(massFile[i], @"^\s*\t*\}+(.*)$").Length == 0; i++)
                                {
                                    if (Regex.Match(massFile[i], @"^[\s\t]*\$v\s+([a-zA-Z0-9]+)\s*\=*\s*([0-9]*)\s*(\;*(.*))$").Length > 0)
                                    {
                                        Match match2 = Regex.Match(massFile[i], @"^[\s\t]*\$v\s+([a-zA-Z0-9]+)\s*\=*\s*([0-9]*)\s*(\;*(.*))$");
                                        string name_pr1 = "";
                                        name_pr1 = match2.Groups[1].Value.ToString() + " dd ";
                                        if (match2.Groups[2].Length > 0) name_pr1 += match2.Groups[2].Value.ToString();
                                        else name_pr1 += '?';
                                        if (match2.Groups[3].Length > 0) name_pr1 += match2.Groups[3].Value.ToString();
                                        classSection.Add(name_pr1);
                                    }
                                    else if (Regex.Match(massFile[i], @"^[\s\t]*([a-zA-Z0-9]+)\s*\=+\s*([a-zA-Z0-9]+)\s*(\;*.*)$").Length > 0)
                                    {
                                        string name1 = "", name2 = "", str = massFile[i];
                                        int j = 0;
                                        for (; j < str.Length && (str[j] == ' ' || str[j] == '\t'); j++) ;
                                        for (; j < str.Length && char.IsLetterOrDigit(str[j]); j++) name1+=str[j];
                                        for (; j < str.Length && (str[j] == ' ' || str[j] == '\t'); j++) ;j++;for (; j < str.Length && str[j] == ' '; j++) ;
                                        for (; j < str.Length && char.IsLetterOrDigit(str[j]); j++) name2 += str[j];
                                        string stroka = "mov ";
                                        int naid = 0;
                                        foreach(string n in classOpNames) { if (n == name1) naid++; }
                                        if (name_per == name1) stroka += "eax, ";
                                        else if (naid > 0) stroka = stroka + "[." + name1 + "], ";
                                        else stroka = stroka + "[" + name1 + "], ";
                                        naid = 0;
                                        foreach (string n in classOpNames) { if (n == name2) naid++; }
                                        if (name_per == name2) stroka += "eax";
                                        else if (naid > 0) stroka = stroka + "[." + name2 + "]";
                                        else stroka = stroka + "[" + name2 + "]";
                                        classSection.Add(stroka);
                                    }
                                }
                                classSection.Add("pop ebp");
                                classSection.Add("ret 12");
                            }
                        }
                        classSection.Add("}");
                        classOpNames.Clear();
                    }
                }
            }

            // .data section recognition
            for (int i = 0; i < massFile.Length; i++)
            {
                if (Regex.Match(massFile[i], @"^\$c\s+([a-zA-z0-9]+)\s*(\;*(.*))$").Length > 0)
                {
                    massFile[i] = ""; massFile[i+1] = "";
                    for (i += 2; i < massFile.Length && Regex.Match(massFile[i], @"^\}+(.*)$").Length == 0; i++){massFile[i] = "";}
                    massFile[i] = "";
                }
                if (Regex.Match(massFile[i], @"^[\s\t]*\$v\s+([a-zA-Z0-9]+)\s*\=*\s*([0-9]*)\s*(\;*(.*))$").Length > 0)
                {
                    Match match1 = Regex.Match(massFile[i], @"^[\s\t]*\$v\s+([a-zA-Z0-9]+)\s*\=*\s*([0-9]*)\s*(\;*(.*))$");
                    string name_pr = "";
                    name_pr = '.' + match1.Groups[1].Value.ToString() + " dd ";
                    if (match1.Groups[2].Length > 0) name_pr += match1.Groups[2].Value.ToString();
                    else name_pr += '?';
                    if (match1.Groups[3].Length > 0) name_pr += match1.Groups[3].Value.ToString();
                    massFile[i] = "";
                    dataList.Add(name_pr);
                }
                if (Regex.Match(massFile[i], @"^[\s\t]*([a-zA-Z0-9]+)\s*([a-zA-Z0-9]+)\s*\=+\s*\$n\s*([a-zA-Z0-9]+)\s*(\;*(.*))$").Length > 0)
                {
                    Match match1 = Regex.Match(massFile[i], @"^[\s\t]*([a-zA-Z0-9]+)\s*([a-zA-Z0-9]+)\s*\=+\s*\$n\s*([a-zA-Z0-9]+)\s*(\;*(.*))$");
                    string name_pr = "";
                    name_pr += match1.Groups[2].Value.ToString();
                    name_pr = name_pr + ' ' + match1.Groups[1].Value.ToString();
                    if (match1.Groups[4].Length > 0) name_pr += match1.Groups[4].Value.ToString();
                    massFile[i] = "";
                    dataList.Add(name_pr);
                }
            }

            //.code section recognition
            for(int i=0; i<massFile.Length; i++)
            {
                if (Regex.Match(massFile[i], @"^[\s\t]*([a-zA-Z0-9\.]+)\(+\s*([a-zA-Z0-9]*)\s*\)+\s*(\;*(.*))$").Length > 0 &&
                    Regex.Match(massFile[i], @"^\s*\t*([a-zA-Z0-9]+)\.+([a-zA-Z0-9]+)\(+(.*)$").Length > 0)
                {
                    Match match1 = Regex.Match(massFile[i], @"^[\s\t]*([a-zA-Z0-9\.]+)\(+\s*([a-zA-Z0-9]*)\s*\)+\s*(\;*(.*))$");
                    string zn = "", stroka = "";
                    if(match1.Groups[2].Length > 0) { zn = "push " + match1.Groups[2].Value.ToString(); codeSection.Add(zn); }
                    stroka = "call " + match1.Groups[1].Value.ToString();
                    if(match1.Groups[3].Length > 0) { stroka += match1.Groups[3].Value.ToString(); }
                    codeSection.Add(stroka);
                }
                if(Regex.Match(massFile[i], @"^[\s\t]*([a-zA-Z0-9]+)\(+\s*([a-zA-Z0-9]+)\s*\)+\s*(\;*(.*))$").Length > 0)
                {
                    Match match1 = Regex.Match(massFile[i], @"^[\s\t]*([a-zA-Z0-9]+)\(+\s*([a-zA-Z0-9]+)\s*\)+\s*(\;*(.*))$");
                    string nazv = match1.Groups[1].Value.ToString();
                    string zn = match1.Groups[2].Value.ToString();
                    string comm = "";
                    if (match1.Groups[3].Length > 0) comm += match1.Groups[3].Value.ToString();
                    string code_sec = "if " + zn + " eq 1"; codeSection.Add(code_sec);
                    code_sec = "goto " + nazv; codeSection.Add(code_sec);
                    code_sec = "end if"; codeSection.Add(code_sec);

                    code_sec = nazv + ":"; loopSection.Add(code_sec);
                    for (i += 2; i < massFile.Length && Regex.Match(massFile[i], @"^\s*\t*\}+(.*)$").Length == 0; i++)
                    {
                        if (Regex.Match(massFile[i], @"^[\s\t]*([a-zA-Z0-9\.]+)\s*\=+\s*([0-9]+)\s*(\;*(.*))$").Length > 0)
                        {
                            Match match2 = Regex.Match(massFile[i], @"^[\s\t]*([a-zA-Z0-9\.]+)\s*\=+\s*([0-9]+)\s*(\;*(.*))$");
                            string stroka1 = "mov [" + match2.Groups[1].Value.ToString() + "], " + match2.Groups[2].Value.ToString();
                            if (match2.Groups[3].Length > 0) stroka1 += match2.Groups[3].Value.ToString();
                            loopSection.Add(stroka1);
                        }
                        else if (Regex.Match(massFile[i], @"^[\s\t]*([a-zA-Z0-9\.]+)\s*\=+\s*([a-zA-Z0-9\.]+)\s*(\;*(.*))$").Length > 0)
                        {
                            Match match2 = Regex.Match(massFile[i], @"^[\s\t]*([a-zA-Z0-9\.]+)\s*\=+\s*([a-zA-Z0-9\.]+)\s*(\;*(.*))$");
                            string stroka1 = "mov [" + match2.Groups[1].Value.ToString() + "], [" + match2.Groups[2].Value.ToString() + "]";
                            if (match2.Groups[3].Length > 0) stroka1 += match2.Groups[3].Value.ToString();
                            loopSection.Add(stroka1);
                        }
                    }
                    code_sec = "mov ecx, [" + zn + "]";loopSection.Add(code_sec);
                    code_sec = "if ecx eq 1";loopSection.Add(code_sec);
                    code_sec = "loop " + nazv;loopSection.Add(code_sec);
                    code_sec = "end if";loopSection.Add(code_sec);
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
            string directory = "test";//Console.ReadLine();
            namefile = namefile + directory + '\\';
            Console.Write("Please, input file name: ");
            directory = "code.txt";//Console.ReadLine();
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
                    asmFile.Add("");
                    asmFile.Add("section '.data' data readable writeable ");
                    foreach (string n in dataList) asmFile.Add(n);
                    asmFile.Add("");
                    asmFile.Add("section '.code' code readable executable");
                    foreach (string n in codeSection) asmFile.Add(n);
                    foreach (string n in loopSection) asmFile.Add(n);
                    //asmFile list is ready to be written to a file
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


