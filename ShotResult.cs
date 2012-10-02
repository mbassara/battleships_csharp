using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Battleships
{
    class ShotResult
    {

        private bool[,] matrix;
        private bool hit;
        private bool sunk;
        private bool gameEnded;
        private Coordinates coordinates;

        public ShotResult(bool isHit, bool isSunk, bool isGameEnded, Coordinates coordinates, bool[,] matrix)
        {
            this.hit = isHit;
            this.sunk = isSunk;
            this.gameEnded = isGameEnded;
            this.coordinates = coordinates;
            this.matrix = matrix;
        }

        public void setCoordinates(Coordinates coordinates)
        {
            this.coordinates = coordinates;
        }

        public bool isHit()
        {
            return hit;
        }

        public bool isSunk()
        {
            return sunk;
        }

        public bool isGameEnded()
        {
            return gameEnded;
        }

        public bool[,] getMatrix()
        {
            return matrix;
        }

        public Coordinates getCoordinates()
        {
            return coordinates;
        }

        public static String ToXMLString(ShotResult shotResult)
        {
            String result = "<shotResult isNull=\"";
            if (shotResult == null)
                result += "true\"";
            else
            {
                result += "false\"";
                result += " isHit=\"" + (shotResult.isHit() ? "true" : "false") + "\"";
                result += " isSunk=\"" + (shotResult.isSunk() ? "true" : "false") + "\"";
                result += " isGameEnded=\"" + (shotResult.isGameEnded() ? "true" : "false") + "\"";
                result += " matrix=\"";
                result += ((shotResult.getMatrix() == null) ? "null" : Global.boolArrayToString(shotResult.getMatrix())) + "\"";
            }

            result += ">" + Coordinates.ToXMLString(shotResult.getCoordinates()) + "</shotResult>";
            return result;
        }
    }
}
