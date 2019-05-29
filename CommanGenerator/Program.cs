using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandGenerator
{
    class Generate
    {
        string src;
        string buffor;
        public Generate()
        {
            src = "";
            buffor = "";
        }

        public Generate(string url)
        {
            src = url;
            buffor = "";
        }

        private ulong LongRandom(ulong min, ulong max, Random rand)
        {
            long result = rand.Next((Int32)(min >> 32), (Int32)(max >> 32));
            result = (result << 32);
            result = result | (long)rand.Next((Int32)min, (Int32)max);
            return (ulong)result;
        }

        private char CharRandom(Random rand)
        {
            int x = rand.Next(0,100);
            if (x < 60)             return 'W';
            if (x >= 60 && x < 70)  return 'L';
            if (x >= 70 && x < 85)  return 'S';
            if (x >= 85 && x < 100) return 'U';
            else return 'x';
        }

        public void Save() => File.WriteAllLines(src, buffor.Split('\n'));

        public void Save(string url)
        {
            File.WriteAllLines(url, buffor.Split('\n'));
        }
        public void Randomize(int count, ulong max)
        {
            Random rng = new Random();
            buffor = "";
            buffor += count + "\n";
            ulong intPart, doublePart;
            char c;


            while (count-- > 0)
            {
                c = CharRandom(rng);
                intPart = LongRandom(0, max, rng);

                buffor += c + " " + intPart;
                if(c != 'L')
                {
                    doublePart = LongRandom(0, max, rng);
                    string s = doublePart.ToString();
                    string dP = ",";
                    for (int i = 15 - s.Length - 1; i > 0; i--)
                    {
                        dP += "0";
                    }
                    dP += s;
                    buffor += dP + "\n";
                } else
                {
                    buffor += "\n";
                }
            }
            Console.WriteLine(buffor);
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            string src = "gen.txt";
            Generate generator = new Generate(src);
            generator.Randomize(14,100000000000000);
            generator.Save();

            Console.WriteLine("Done");
            Console.ReadKey();
        }
    }
}
