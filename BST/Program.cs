//#define DEBUG
#undef DEBUG

using Lib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BST
{
    class BST
    {
        class Node
        {
            public UDouble data;
            public int count;
            public Node left;
            public Node right;

            public Node(UDouble data)
            {
                this.data = data;
                count = 1;
                left = null;
                right = null;
            }
        }

        private Node root;

        public BST()
        {
            root = null;
        }

        public void Add(UDouble data)
        {
            Node newNode = new Node(data);
            if (root == null)
            {
                root = newNode;
            }
            else
            {
                Insert(root, newNode);
                decimalParts.Clear();
            }
        }

        private List<ulong> decimalParts = new List<ulong>(1);
        private ulong deletedPart;
        private UDouble targetTemp;
        private UDouble firstToDelete;
        private bool firstFound = false;

        private Node Insert(Node current, Node n)
        {
            if (current == null)
            { current = n; decimalParts.Add(n.data.intPart); return current; }
            else if (n.data < current.data)
                current.left = Insert(current.left, n);
            else if (n.data > current.data)
                current.right = Insert(current.right, n);

            if (decimalParts.Contains(current.data.intPart))
                current.count += 1;

            return current;
        }

        public void Delete(UDouble target) {
            deletedPart = target.intPart;
            targetTemp = target;
            root = Delete(root, target);
            firstFound = false;
        }
        private Node Delete(Node current, UDouble target)
        {
            Node parent;
            if (current == null) return null;
            else
            {
                // lewa strona
                if (target < current.data)
                    current.left = Delete(current.left, target);
                // prawa strona
                else if (target > current.data)
                    current.right = Delete(current.right, target);
                // lewa + prawa + rodzic
                else
                {
                    if (!firstFound) { firstToDelete = current.data; firstFound = true; }
                    if (current.right != null)
                    {
                        if (current.data == targetTemp)
                            targetTemp = current.right.data;
                        parent = current.right;
                        while (parent.left != null)
                            parent = parent.left;
                        current.data = parent.data;
                        current.count = parent.count;
                        current.right = Delete(current.right, parent.data);
                    }
                    else
                    {
                        return current.left;
                    }
                }
            }
            if (current.data.intPart == deletedPart && firstFound != false)
                current.count -= 1;
            return current;
        }

        public int Find(UDouble key)
        {
            try
            {
                UDouble n = Find(key, root).data;
                if (n == key) return 1;
            }
            catch (NullReferenceException)
            {
                return -1;
                throw;
            }

            return -1;
        }

        private Node Find(UDouble target, Node current)
        {
            if (current == null) throw new NullReferenceException();
            if (target < current.data)
                {
                    if (target == current.data) return current;
                else return Find(target, current.left);
            }
            else
            {
                if (target == current.data) return current;
                else return Find(target, current.right);
            }
        }
        private int CountParts(UDouble target, Node current)
        {
            int count = 0;
            if (current == null) return count;
            if (current.data.intPart == target.intPart)
                return current.count;

            if (target < current.data)
                count = CountParts(target, current.left);
            if (target > current.data)
                count = CountParts(target, current.right);

            return count;
        }
        public int CountDecimalParts(UDouble target)
        {
            if (root == null) { Console.WriteLine("Tree is empty"); return -1; }
            return CountParts(target, root);
        }

        public void PrintAsList()
        {
            if (root == null)
            { Console.WriteLine("Tree is empty"); return; }

            LKP(root);
            Console.WriteLine();
        }
        private void LKP(Node current)
        {
            if (current != null)
            {
                LKP(current.left);
                Console.Write("({0}) ", current.data);
                LKP(current.right);
            }
        }

        public void PrintTree() => PKL_Print(root, 0);

        private void PKL_Print(Node current, int spaces)
        {
            if (root == null) {
                Console.WriteLine("Tree is empty");
                return;
            }
            int count = 6;

            spaces += count;
            if (current.right != null)
                PKL_Print(current.right, spaces);

            Console.Write("\n");
            for (int i = count; i < spaces; i++)
                Console.Write(' ');
            Console.Write(current.data.ToString() + '\n');

            if(current.left != null)
                PKL_Print(current.left, spaces);
        }

    }

    class ParseTxt
    {
        private readonly string file = "";
        private string logs = "";

        public ParseTxt(string url)
        {
            file = url;
        }
        public ParseTxt()
        {
            file = "";
        }

        public void SaveLogs(string url) => File.WriteAllLines(url, logs.Split('\n'));

        public void ParseToTree(ref BST tree)
        {
            Console.WriteLine("~    Logs    ~");
            long time = 0;
            decimal ticks = 0;
            decimal nano = 0;

            using (StreamReader sr = File.OpenText(file))
            {
                string s;
                while (!sr.EndOfStream)
                {
                    s = sr.ReadLine(); s.ToUpper();
                    string[] parts = s.Split(' ');
                    if (parts.Length == 1) continue;

                    if (parts[1].Contains('.'))
                        parts[1] = parts[1].Replace('.', ',');

                    UDouble param = new UDouble(parts[1]);

                    Stopwatch sw = Stopwatch.StartNew();
                    switch (parts[0])
                    {
                        case "W":
                            tree.Add(param);
#if DEBUG
                            Console.WriteLine("Dodałem {0}", param);
#endif
                            break;
                        case "S":
                            if (tree.Find(param) > 0)
                            {
                                Console.WriteLine("TAK");
                                logs += "TAK\n";
                            }
                            else
                            {
                                Console.WriteLine("NIE");
                                logs += "NIE\n";
                            }
                            break;
                        case "U":
                            tree.Delete(param);
#if DEBUG
                            Console.WriteLine("Usunąłem {0}", param);
#endif
                            break;
                        case "L":
                            int c = tree.CountDecimalParts(param);
                            Console.WriteLine("{0}", c);
                            logs += c + "\n";
                            break;
                    } // switch
                    sw.Stop(); time += sw.ElapsedMilliseconds; ticks += sw.ElapsedTicks;
                    nano += ((decimal)sw.ElapsedTicks / (decimal)Stopwatch.Frequency) * 1000000;
                } // while
            } // using

            Console.BackgroundColor = ConsoleColor.Blue;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("\nParsing ends succesfully!");
            Console.WriteLine(
                "Parsing time: {0} ms\n{1}Ticks: {2}", 
                time, 
                nano < 3000 ? nano + " ns\n" : "", 
                ticks
            );
            Console.ResetColor();
        }

        private bool autoClear = false;

        public void Cmd(ref BST tree, string str)
        {
            str = str.Trim().ToLower();
            if (str == "exit" || str == "quit" || str == "q") Environment.Exit(0);
            if (str == "clear") { Console.Clear(); return; }
            if (str == "tree" || str == "print")
            { if (autoClear) Console.Clear(); tree.PrintTree(); return; }
            if (str == "print as list" || str == "list") { if (autoClear) Console.Clear(); tree.PrintAsList(); return; }
            if (str == "autoclear") { if (autoClear) Console.Clear(); autoClear ^= true; return; }
            if (str == "help" || str == "?")
            {
                Console.WriteLine("Komendy:\nW x – wstawia x" +
                "\nU x – usuwa x" +
                "\nS x – szuka x" +
                "\nL x – wypisuje, ile liczb posiada część całkowitą równą x" +
                "\ntree | print - wypisuje drzewo na płaszyczyźnie" +
                "\nlist | print as list - wypisuje drzewo w porządku rosnącym jako lisa" +
                "\nclear - czyści ekran" +
                "\nautoclear - automatycznie czyści ekran po wpisaniu komendy" +
                "\nexit | quit | q - wychodzi z programu");
                return;
            }

            if (autoClear) Console.Clear();
            ParseToTree(ref tree, str);
        }
        private void ParseToTree(ref BST tree, string str)
        {
            Stopwatch sw = Stopwatch.StartNew();
            str = str.ToUpper().Trim();
            string[] parts = str.Split(' ');
            UDouble param;


            if (parts.Length != 2)
            {
                Console.BackgroundColor = ConsoleColor.Red;
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("Parsing failure!");
                Console.ResetColor();
                return;
            }

            if (parts[1].Contains('.'))
                parts[1] = parts[1].Replace('.', ',');

            try
            {
                param = new UDouble(parts[1]);
            }
            catch (Exception)
            {
                Console.BackgroundColor = ConsoleColor.Red;
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("Parsing failure!");
                Console.ResetColor();
                return;
                throw;
            }

            switch (parts[0])
            {
                case "W":
                    tree.Add(param);
#if DEBUG
                    Consol.WriteLine("Dodałem {0}", param);
#endif
                    break;
                case "S":
                    if (tree.Find(param) > 0)
                    {
                        Console.BackgroundColor = ConsoleColor.DarkGreen;
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine("Znalazłem {0}", param);
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.BackgroundColor = ConsoleColor.Red;
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine("Nie znaleziono {0}", param);
                        Console.ResetColor();
                    }
                    break;
                case "U":
                    tree.Delete(param);
#if DEBUG
                    Console.WriteLine("Usunąłem {0}", param); 
#endif
                    break;
                case "L":
                    int c = tree.CountDecimalParts(param);
                    if (c == 0) Console.BackgroundColor = ConsoleColor.Red;
                    else        Console.BackgroundColor = ConsoleColor.DarkGreen;
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("Znaleziono {0} wierzchołków o części całkowitej {1}", c, param);
                    Console.ResetColor();
                    break;
                default:
                    Console.BackgroundColor = ConsoleColor.Red;
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("Parsing failure!");
                    Console.ResetColor();
                    return;
            } // switch

            sw.Stop();
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("\nParsing ends succesfully!");
            Console.WriteLine(
                " Parsing time: \n{0} ms; \n{1} ns. \nTicks: {2}", 
                sw.ElapsedMilliseconds, 
                (decimal)sw.ElapsedTicks/(decimal)Stopwatch.Frequency * 1000000, 
                sw.ElapsedTicks
            );
            Console.ResetColor();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            BST tree = new BST();
            ParseTxt parser = new ParseTxt("in1.txt");

            parser.ParseToTree(ref tree);
            parser.SaveLogs("output.txt");

            /////////////////////////////////////////////////
            //               Wiersz poleceń                // 
            /////////////////////////////////////////////////

            string s = "";
            while (true)
            {
                Console.Write("\nWprowadz komendę: ");
                s = Console.ReadLine();
                parser.Cmd(ref tree, s);
            }
        }
    }
}
