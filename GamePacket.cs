using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Battleships
{
    class GamePacket
    {
        public enum TYPE { UNDEFINED, WHO_STARTS, SHOT, RESULT,
                         TEXT_MESSAGE, GAME_RESULT, USER_NAME };

        private TYPE type;
        public TYPE Type
        {
            get { return type; }
        }
        private String message = null;
        private Coordinates coordinates = null;
        private bool whoStarts = true;
        private ShotResult shotResult = null;
        private GameResult gameResult = null;


        public String getXMLString()
        {
            String result = "<gamePacket type=\"" + type;

            switch (type)
            {
                case TYPE.GAME_RESULT:
                    result += "\">" + GameResult.ToXMLString(gameResult);
                    break;
                case TYPE.RESULT:
                    result += "\">" + ShotResult.ToXMLString(shotResult);
                    break;
                case TYPE.SHOT:
                    result += "\">" + Coordinates.ToXMLString(coordinates);
                    break;
                case TYPE.TEXT_MESSAGE:
                    result += "\" message=\"" + message + "\"/>";
                    return result;
                case TYPE.WHO_STARTS:
                    result += "\" whoStarts=\"" + (whoStarts == Global.HOST_FIRST ? "host" : "client") + "\"/>";
                    return result;
                default:
                    result += "\"/>";
                    return result;
            }

            result += "</gamePacket>";

            return result;
        }

        public GamePacket(bool whoStarts)
        {
            this.whoStarts = whoStarts;
            type = TYPE.WHO_STARTS;
        }

        public GamePacket(ShotResult result)
        {
            this.shotResult = result;
            type = TYPE.RESULT;
        }

        public GamePacket(GameResult result)
        {
            this.gameResult = result;
            type = TYPE.GAME_RESULT;
        }

        public GamePacket(Coordinates coordinates)
        {
            this.coordinates = coordinates;
            type = TYPE.SHOT;
        }

        public GamePacket(int x, int y)
        {
            coordinates = Coordinates.Get(x, y);
            type = TYPE.SHOT;
        }

        public GamePacket(String message)
        {
            this.message = message;
            type = TYPE.TEXT_MESSAGE;
        }

        public bool getWhoStarts()
        {
            return whoStarts;
        }

        public String getMessage()
        {
            return message;
        }

        public ShotResult getShotResult()
        {
            return shotResult;
        }

        public GameResult getGameResult()
        {
            return gameResult;
        }

        public void setMessage(String message)
        {
            this.message = message;
        }

        public Coordinates getCoordinates()
        {
            return coordinates;
        }

    }
}
