using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;

namespace Battleships
{
    abstract class GamePacketSerialization
    {
        public static String serialize(GamePacket packet)
        {
            return "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"no\"?>" + packet.getXMLString();
        }

        public static GamePacket deserialize(String packetString)
        {
            GamePacket packet = null;
            GamePacket.TYPE type = GamePacket.TYPE.UNDEFINED;
            String message = null;
            Coordinates coordinates = null;
            bool whoStarts = false;
            ShotResult shotResult = null;
            GameResult gameResult = null;
            bool[,] matrix = null;
            bool isHit = false;
            bool isSunk = false;
            bool isGameEnded = false;

            XmlTextReader textReader = new XmlTextReader(new StringReader(packetString));
            textReader.Read();
            //Console.WriteLine("deserialization is ready to start:\n" + packetString);
            while (textReader.Read())
            {
                if (textReader.NodeType == XmlNodeType.Element)
                {
                    if (textReader.Name.Equals("gamePacket"))
                    {
                        String typeString = textReader.GetAttribute("type");
                        if (typeString.Equals("whoStarts"))
                        {
                            type = GamePacket.TYPE.WHO_STARTS;
                            whoStarts = textReader.GetAttribute("whoStarts").Equals("host") ? Global.HOST_FIRST : Global.CLIENT_FIRST;
                            packet = new GamePacket(whoStarts);
                        }
                        else if (typeString.Equals("textMessage"))
                        {
                            type = GamePacket.TYPE.TEXT_MESSAGE;
                            message = textReader.GetAttribute("message");
                            packet = new GamePacket(message);
                        }
                        else if (typeString.Equals("shot"))
                        {
                            type = GamePacket.TYPE.SHOT;
                        }
                        else if (typeString.Equals("result"))
                        {
                            type = GamePacket.TYPE.RESULT;
                        }
                        else if (typeString.Equals("gameResult"))
                        {
                            type = GamePacket.TYPE.GAME_RESULT;
                        }
                    }
                    else if (textReader.Name.Equals("shotResult") && textReader.GetAttribute("isNull").Equals("false"))
                    {
                        isHit = Boolean.Parse(textReader.GetAttribute("isHit"));
                        isSunk = Boolean.Parse(textReader.GetAttribute("isSunk"));
                        isGameEnded = Boolean.Parse(textReader.GetAttribute("isGameEnded"));
                        String matrixStr = textReader.GetAttribute("matrix");
                        if (!matrixStr.Equals("null"))
                            matrix = Global.stringToBoolArray(matrixStr);
                    }
                    else if (textReader.Name.Equals("coordinates") && textReader.GetAttribute("isNull").Equals("false"))
                    {
                        int x = Int32.Parse(textReader.GetAttribute("x"));
                        int y = Int32.Parse(textReader.GetAttribute("y"));
                        coordinates = Coordinates.Get(x, y);
                    }
                    else if (textReader.Name.Equals("gameResult") && textReader.GetAttribute("isNull").Equals("false"))
                    {
                        gameResult = new GameResult(textReader.GetAttribute("result").Equals("winner") ? Global.GAME_RESULT_WINNER : Global.GAME_RESULT_LOOSER);
                    }
                }
                else if (textReader.NodeType == XmlNodeType.EndElement)
                {
                    if (textReader.Name.Equals("gamePacket"))
                    {
                        switch (type)
                        {
                            case GamePacket.TYPE.GAME_RESULT:
                                packet = new GamePacket(gameResult);
                                break;
                            case GamePacket.TYPE.RESULT:
                                packet = new GamePacket(shotResult);
                                break;
                            case GamePacket.TYPE.SHOT:
                                packet = new GamePacket(coordinates);
                                break;
                            case GamePacket.TYPE.TEXT_MESSAGE:
                                packet = new GamePacket(message);
                                break;
                            case GamePacket.TYPE.WHO_STARTS:
                                packet = new GamePacket(whoStarts);
                                break;
                            default:
                                break;
                        }
                    }
                    else if (textReader.Name.Equals("shotResult"))
                    {
                        shotResult = new ShotResult(isHit, isSunk, isGameEnded, coordinates, matrix);
                    }
                }
            }

            return packet;
        }
    }
}
