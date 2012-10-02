using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Battleships
{
    class GameResult
    {

        private bool result;

        public GameResult(bool result)
        {
            this.result = result;
        }

        public bool isWinner()
        {
            return result == Global.GAME_RESULT_WINNER;
        }

        public static String ToXMLString(GameResult gameResult)
        {
            String result = "<gameResult isNull=\"";
            if (gameResult == null)
                result += "true\"";
            else
            {
                result += "false\"";
                result += " result=\"" + (gameResult.isWinner() ? "winner" : "looser") + "\"";
            }

            result += "/>";
            return result;
        }

    }
}
