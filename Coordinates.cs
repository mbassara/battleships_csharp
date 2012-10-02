using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Battleships
{
    class Coordinates
    {
        private static System.Collections.Generic.Dictionary<String, Coordinates> map = new Dictionary<String, Coordinates>();

        private int x;
        public int X
        {
            get { return x; }
            set { x = value; }
        }
        private int y;
        public int Y
        {
            get { return y; }
            set { y = value; }
        }

        public static Coordinates Get(int x, int y)
        {
            String key = "(" + x + "," + y + ")";
            if (!map.ContainsKey(key))
                map.Add(key, new Coordinates(x, y));

            return map[key];
        }

        private Coordinates(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public static String ToXMLString(Coordinates coordinates)
        {
            String result = "<coordinates isNull=\"";
            if (coordinates == null)
                result += "true\"";
            else
                result += "false\" x=\"" + coordinates.X + "\" y=\"" + coordinates.Y + "\"";

            result += "/>";
            return result;
        }

        public override string ToString()
        {
            return "(" + x + "," + y + ")";
        }
    }
}
