namespace Lib
{
    public class UDouble
    {
        public ulong intPart;
        public ulong doublePart;
        public int E;

        public UDouble(string number)
        {
            Set(number);
        }

        public UDouble(UDouble number)
        {
            this.intPart = number.intPart;
            this.doublePart = number.doublePart;
            this.E = number.E;
        }

        public UDouble(double number)
        {
            UDouble num = UDouble.Parse(number);
            this.intPart = num.intPart;
            this.doublePart = num.doublePart;
            this.E = num.E;
        }

        public UDouble(int number)
        {
            UDouble num = UDouble.Parse(number);
            this.intPart = num.intPart;
            this.doublePart = num.doublePart;
            this.E = num.E;
        }

        public UDouble()
        {
            intPart = 0;
            doublePart = 0;
            E = 0;
        }

        private void Set(string number)
        {
            E = 0;
            string[] parts = number.Replace('.', ',').Split(',');

            if (parts.Length != 1)
            {
                foreach (char c in parts[1])
                {
                    if (c == '0') E += 1;
                    else break;
                }
                if(E != 0 && parts[1] != "0") parts[1] = parts[1].Remove(0, E);
                intPart = ulong.Parse(parts[0]);
                doublePart = ulong.Parse(parts[1]);

                doublePart *= (ulong)System.Math.Pow(
                    10, 
                    15 - (ulong)parts[1].Length
                );
            } else {
                intPart = ulong.Parse(parts[0]);
                doublePart = 0;
            }

        }

        public static UDouble Parse(int number)
        {
            UDouble num = new UDouble
            {
                intPart = (ulong)number,
                doublePart = 0,
                E = 0
            };
            return num;
        }
        public static UDouble Parse(double number)
        {
            UDouble num = new UDouble();
            num.E = 0;
            string s = "";
            s += number;
            string[] parts = s.Split(',');
            if (parts.Length == 1)
            {
                num.E = 0;
                num.intPart = ulong.Parse(parts[0]);
                num.doublePart = 0;
            }
            else
            {
                num.intPart = ulong.Parse(parts[0]);
                foreach (char c in parts[1])
                    if (c == '0') num.E += 1;
                    else break;
                num.doublePart = ulong.Parse(parts[1]);
            }

            return num;
        }

        public int CompareTo(UDouble target)
        {
            if (intPart == target.intPart)
                if (E == target.E)
                    if (doublePart == target.doublePart) return 0;
                    else if (doublePart < target.doublePart) return -1;
                    else return 1;
                else if (E < target.E) return -1;
                else return 1;
            else if (intPart < target.intPart) return -1;
            else return 1;
        }

        public static int Compare(UDouble a, UDouble b)
        {
            if (a.intPart == b.intPart)
                if (a.E == b.E)
                    if (a.doublePart == b.doublePart) return 0;
                    else if (a.doublePart < b.doublePart) return -1;
                    else return 1;
                else if (a.E < b.E) return -1;
                else return 1;
            else if (a.intPart < b.intPart) return -1;
            else return 1;
        }

        override public string ToString()
        {
            string str = "";
            str += intPart;
            str += ",";
            for (int i = E; i > 0; i--)
                str += "0";
            str += doublePart;
            int ptr = str.Length - 1;
            while (str[ptr--] == '0');
            if (ptr != str.Length - 1)
                str = str.Remove(ptr + 2, str.Length - ptr - 2);
            if (doublePart == 0) str += "0";
            return str;
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(UDouble a, UDouble b)
        {
            if (a.intPart == b.intPart &&
                a.doublePart == b.doublePart &&
                a.E == b.E) return true;
            return false;
        }

        public static bool operator !=(UDouble a, UDouble b)
        {
            if (a.intPart == b.intPart)
                if (a.E == b.E)
                    if (a.doublePart == b.doublePart)
                        return false;
                    else return true;
                else return false;
            else return false;
        }

        public static bool operator <(UDouble a, UDouble b)
        {
            if (a.CompareTo(b) == -1) return true;
            return false;
        }
        public static bool operator >(UDouble a, UDouble b)
        {
            if (b.CompareTo(a) == -1) return true;
            return false;
        }
    }
}
