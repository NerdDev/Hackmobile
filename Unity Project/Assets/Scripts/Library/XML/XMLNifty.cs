using System;
using XML;

namespace XML
{
    public class XMLNifty
    {
        static public int StringToInt(string toParse)
        {
            int temp;
            if (int.TryParse(toParse, out temp))
            {
                return temp;
            }
            else
            {
                throw new ArgumentException("String cannot be parsed to integer!");
            }
        }

        static public double StringToDouble(string toParse)
        {
            double temp;
            if (double.TryParse(toParse, out temp))
            {
                return temp;
            }
            else
            {
                throw new ArgumentException("String cannot be parsed to double!");
            }
        }

        static public bool StringToBool(string toParse)
        {
            bool temp;
            if (bool.TryParse(toParse, out temp))
            {
                return temp;
            }
            else
            {
                throw new ArgumentException("String cannot be parsed to boolean!");
            }
        }

        public static int SelectInt(XMLNode x, string toParse)
        {
            return StringToInt(x.select(toParse).getText());
        }

        public static T SelectEnum<T>(XMLNode x, string toParse)
        {
            return (T)Enum.Parse(typeof(T), x.select(toParse).getText(), true);
        }

        public static string SelectString(XMLNode x, string node)
        {
            return x.select(node).getText();
        }

        public static bool SelectBool(XMLNode x, string toParse)
        {
            return StringToBool(x.select(toParse).getText());
        }
    }
}