using System;
using System.Collections.Generic;
using System.Linq;

namespace Infrastructure
{
    public static class Translit
    {
        public static string[] lat_up =  { "Zh", "Kh", "Ts", "Ch", "Sh", "Sh", "Yu", "Ya", "A", "B", "V", "G", "D", "E", "E", "Z", "I", "Y", "K", "L", "M", "N", "O", "P", "R", "S", "T", "U", "F",  " ", "Y", " ", "E"};
        public static string[] lat_low = { "zh", "kh", "ts", "ch", "sh", "sh", "yu", "ya", "a", "b", "v", "g", "d", "e", "e", "z", "i", "y", "k", "l", "m", "n", "o", "p", "r", "s", "t", "u", "f",  " ", "y", " ", "e"};
        public static string[] rus_up =  { "�",  "�",  "�",  "�",  "�",  "�",  "�",  "�", "�", "�", "�", "�", "�", "�", "�", "�", "�", "�", "�", "�", "�", "�", "�", "�", "�", "�", "�", "�", "�", "�", "�", "�", "�"};
        public static string[] rus_low = { "�", "�", "�", "�", "�", "�", "�", "�", "�", "�", "�", "�", "�", "�", "�", "�", "�", "�", "�", "�", "�", "�", "�", "�", "�", "�", "�", "�", "�", "�", "�", "�", "�" };

        public static string Code(string str)
        {
            for (int i = 0; i <= 32; i++)
            {
                str = str.Replace(rus_up[i], lat_up[i]);
                str = str.Replace(rus_low[i], lat_low[i]);
            }
            return str;
        }

        public static string Decode(string str)
        {
            for (int i = 0; i <= 32; i++)
            {
                str = str.Replace(lat_up[i], rus_up[i]);
                str = str.Replace(lat_low[i], rus_low[i]);
            }
            return str;
        }
    }
}