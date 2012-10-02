using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Battleships
{

    public enum GAME_MODE { UNDEFINED, WIFI, SINGLE, HOST, CLIENT };

    static class Global
    {
        public const bool LOGS_ENABLED = true;

        public const bool HOST_FIRST = false;
        public const bool CLIENT_FIRST = true;

        public const byte DEVICE_TYPE_ANDROID = 0;
        public const byte DEVICE_TYPE_WINDOWS = 1;
        public static byte OPPONENT_DEVICE_TYPE = DEVICE_TYPE_WINDOWS;

        public const String END_OF_PACKET = "end_168321_end";

        public const bool GAME_RESULT_WINNER = true;
        public const bool GAME_RESULT_LOOSER = false;

        public static int[] SHIPS_COUNER = { 0, 0, 2, 2, 2, 1 };

        public const int WIFI_PORT = 57419;

        public static bool SHOOTING_TIPS_ENABLED = true;

        public static GAME_MODE GameMode = GAME_MODE.UNDEFINED;

        public const String UUID = "76b4c611-da5a-4672-af97-7eb2fb71597e";


        public static String boolArrayToString(bool[,] array)
        {
            String result = "";
            for (int i = 0; i < array.GetLength(0); i++)
            {
                for (int j = 0; j < array.GetLength(1); j++)
                    result += array[i, j] ? "1" : "0";
                result += ",";
            }

            return result.Substring(0, result.Length - 1);
        }

        public static bool[,] stringToBoolArray(String str)
        {
            char[] delim = { ',' };
            String[] strArray = str.Split(delim);
            int size = strArray.Length;
            bool[,] result = new bool[size, size];

            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                    result[i, j] = strArray[i][j] == '1';

            return result;
        }
    }
}
