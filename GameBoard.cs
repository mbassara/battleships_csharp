using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Threading;

namespace Battleships
{
    class GameBoard
    {
        public static bool[,] empty = {
                                          {false,false,false,false,false,false,false,false,false,false},
                                          {false,false,false,false,false,false,false,false,false,false},
                                          {false,false,false,false,false,false,false,false,false,false},
                                          {false,false,false,false,false,false,false,false,false,false},
                                          {false,false,false,false,false,false,false,false,false,false},
                                          {false,false,false,false,false,false,false,false,false,false},
                                          {false,false,false,false,false,false,false,false,false,false},
                                          {false,false,false,false,false,false,false,false,false,false},
                                          {false,false,false,false,false,false,false,false,false,false},
                                          {false,false,false,false,false,false,false,false,false,false}
                                      };

        private Ship[,] ships = null;
        private bool[,] shipsMatrix;
        private static int margin = 15;
        private BoardType boardType;

        public enum BoardType { MAIN_BOARD, PREVIEW_BOARD };

        public GameBoard(BoardType type, bool[,] matrix)
        {
            shipsMatrix = matrix;
            boardType = type;
            this.ships = new Ship[matrix.GetLength(0), matrix.GetLength(1)];
            for (int i = 0; i < this.ships.GetLength(0); i++)
                for (int j = 0; j < this.ships.GetLength(1); j++)
                {
                    ships[i, j] = new Ship(i, j, type, matrix[i, j]);
                    ships[i, j].Name = i + "" + j;
                }
        }

        #region GUI stuff

        public int Margin
        {
            get { return margin; }
        }

        public int Width
        {
            get { return 10 * (new Ship(0, 0, boardType, false).Width + 2) - 2; }
        }

        public int Height
        {
            get { return 10 * (new Ship(0, 0, boardType, false).Height + 2) - 2; }
        }

        private bool visible;
        public bool Visible
        {
            get { return visible; }
            set
            {
                visible = value;
                for (int i = 0; i < 10; i++)
                    for (int j = 0; j < 10; j++)
                        ships[i, j].Visible = visible;
            }
        }

        private bool enabled;
        public bool Enabled
        {
            get { return enabled; }
            set
            {
                enabled = value;
                for (int i = 0; i < 10; i++)
                    for (int j = 0; j < 10; j++)
                        ships[i, j].Enabled = enabled;
            }
        }

        private EventHandler click = delegate(object o, EventArgs e) { };
        public EventHandler Click
        {
            get { return click; }
            set{
                click += value;
                for (int i = 0; i < 10; i++)
                    for (int j = 0; j < 10; j++)
                        ships[i, j].Click += value;
            }
        }

        private EventHandler doubleClick = delegate(object o, EventArgs e) { };
        public EventHandler DoubleClick
        {
            get { return doubleClick; }
            set
            {
                doubleClick += value;
                for (int i = 0; i < 10; i++)
                    for (int j = 0; j < 10; j++)
                        ships[i, j].DoubleClick += value;
            }
        }

        public void setShips(bool[,] matrix)
        {
            shipsMatrix = matrix;
            Thread thread = new Thread(new ThreadStart(delegate()
            {
                for (int i = 0; i < 10; i++)
                    for (int j = 0; j < 10; j++)
                        ships[i, j].State = matrix[i, j] ? Ship.ShipState.SHIP : Ship.ShipState.NOT_SHIP;
            }));
            thread.Start();
        }

        public bool[,] getShips()
        {
            bool[,] matrix = new bool[10, 10];
            for (int i = 0; i < 10; i++)
                for (int j = 0; j < 10; j++)
                    matrix[i, j] = shipsMatrix[i, j];

            return matrix;
        }

        public void addToWindow(Form window)
        {
            for (int i = 0; i < 10; i++)
                for (int j = 0; j < 10; j++)
                {
                    window.Controls.Add(ships[i, j]);
                    ships[i, j].Location = getLocation(j, i, window);
                }
        }

        private System.Drawing.Point getLocation(int x, int y, Form window)
        {
            int width = window.Width;
            int height = window.Height;
            if (boardType == BoardType.MAIN_BOARD)
            {
                margin = width / 2 - 10 * (new Ship(0, 0, BoardType.MAIN_BOARD, false).Width + 2) / 2;
                return new System.Drawing.Point(margin + x * (ships[x, y].Width + 2), height - 25 - (margin + (10 - y) * (ships[x, y].Height + 2)));
            }
            else
                return new System.Drawing.Point(margin + x * (ships[x, y].Width + 2), margin + y * (ships[x, y].Height + 2));
        }

        #endregion

        public void setShipSunk(bool[,] matrix)
        {
            if (matrix != null)
                for (int i = 0; i < 10; i++)
                    for (int j = 0; j < 10; j++)
                        if (matrix[i, j])
                            ships[i, j].State = Ship.ShipState.SUNK;
        }

        public void ShotResult(ShotResult shotResult)
        {
            int x = shotResult.getCoordinates().X;
            int y = shotResult.getCoordinates().Y;
            bool result = shotResult.isHit();

            if (result)
                ships[x, y].State = Ship.ShipState.HIT;
            else
                ships[x, y].State = Ship.ShipState.MISSED;
        }

        public ShotResult Shoot(Coordinates field)
        {
            int x = field.X;
            int y = field.Y;
            if (ships[x, y].Shoot())
                return markShipAsSunkIfReallyIs(x, y);
            else
                return new ShotResult(false, false, isGameEnded(), Coordinates.Get(x, y), (bool[,])null);
        }

        public ShotResult markShipAsSunkIfReallyIs(int x, int y)
        {
            bool[,] matrix = new bool[10, 10];

            for (int i = 0; i < 10; i++)
                for (int j = 0; j < 10; j++)
                    matrix[i, j] = true;

            bool result = isWholeShipSunkRecursive(x, y, ref matrix);
            if (result)
            {
                for (int i = 0; i < 10; i++)
                    for (int j = 0; j < 10; j++)
                    {
                        matrix[i, j] = matrix[i, j] ^ true;		// toggling values
                        if (matrix[i, j])
                            ships[i, j].State = Ship.ShipState.SUNK;
                    }
            }

            return new ShotResult(true, result, isGameEnded(), Coordinates.Get(x, y), result ? matrix : null);
        }

        private bool isWholeShipSunkRecursive(int x, int y, ref bool[,] matrix)
        {

            bool result = true;
            matrix[x, y] = false;

            if ((x + 1) < 10 && matrix[x + 1, y] && shipsMatrix[x + 1, y])
                result = result && ships[x + 1, y].isHit() && isWholeShipSunkRecursive(x + 1, y, ref matrix);
            if (result && (y + 1) < 10 && matrix[x, y + 1] && shipsMatrix[x, y + 1])
                result = result && ships[x, y + 1].isHit() && isWholeShipSunkRecursive(x, y + 1, ref matrix);
            if (result && (x - 1) >= 0 && matrix[x - 1, y] && shipsMatrix[x - 1, y])
                result = result && ships[x - 1, y].isHit() && isWholeShipSunkRecursive(x - 1, y, ref matrix);
            if (result && (y - 1) >= 0 && matrix[x, y - 1] && shipsMatrix[x, y - 1])
                result = result && ships[x, y - 1].isHit() && isWholeShipSunkRecursive(x, y - 1, ref matrix);

            System.Diagnostics.Debug.WriteLine("Matrix for (" + x + "," + y + "), result: " + result);
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                    System.Diagnostics.Debug.Write(" " + (shipsMatrix[i, j] ? "." : "F"));
                System.Diagnostics.Debug.WriteLine("");
            }
            System.Diagnostics.Debug.WriteLine("");

            return result;
        }

        public bool isGameEnded()
        {
            bool ended = true;

            for (int i = 0; i < 10 && ended; i++)
                for (int j = 0; j < 10 && ended; j++)
                    if (!ships[i, j].isFinished())
                        ended = false;

            return ended;
        }


        public bool isNotShip(int x, int y)
        {
            return ships[x, y].isNotShip();
        }

        public void setShipLaF(int x, int y, Ship.LAF laf)
        {
            ships[x, y].setLaF(laf);
        }

        public static bool[,] generateMatrix()
        {
            bool[,] matrix = new bool[10, 10];
            ArrayList fields = new ArrayList();
            ArrayList tmp = new ArrayList();

            for (int i = 0; i < 10; i++)
                for (int j = 0; j < 10; j++)
                {
                    matrix[i, j] = false;
                    fields.Add(Coordinates.Get(i, j));
                }

            Random rand = new Random();

            for (int size = Global.SHIPS_COUNER.Length - 1; size > 1; size--)
            {
                for (int no = 0; no < Global.SHIPS_COUNER[size]; no++)
                {
                    ShuffleInPlace(fields);
                    foreach (Coordinates field in fields)
                    {
                        bool possible = true;
                        bool isHorizontal = rand.Next() % 2 == 0;

                        tmp.Clear();
                        if (isHorizontal)
                        {
                            for (int i = 1; i < size; i++)
                                if (field.X + i < 10 && fields.Contains(Coordinates.Get(field.X + i, field.Y)))
                                    tmp.Add(Coordinates.Get(field.X + i, field.Y));
                                else
                                    possible = false;
                            if (!possible)
                            {
                                tmp.Clear();
                                for (int i = 1; i < size; i++)
                                    if (field.X - i >= 0 && fields.Contains(Coordinates.Get(field.X - i, field.Y)))
                                        tmp.Add(Coordinates.Get(field.X - i, field.Y));
                                    else
                                        possible = false;
                            }
                        }
                        else
                        {
                            for (int i = 1; i < size; i++)
                                if (field.Y + i < 10 && fields.Contains(Coordinates.Get(field.X, field.Y + i)))
                                    tmp.Add(Coordinates.Get(field.X, field.Y + i));
                                else
                                    possible = false;
                            if (!possible)
                            {
                                tmp.Clear();
                                for (int i = 1; i < size; i++)
                                    if (field.Y - i >= 0 && fields.Contains(Coordinates.Get(field.X, field.Y - i)))
                                        tmp.Add(Coordinates.Get(field.X, field.Y - i));
                                    else
                                        possible = false;
                            }
                        }

                        if (possible)
                        {
                            tmp.Add(field);
                            foreach (Coordinates tmpField in tmp)
                            {
                                matrix[tmpField.X, tmpField.Y] = true;
                                fields.Remove(tmpField);
                                fields.Remove(Coordinates.Get(tmpField.X - 1, tmpField.Y - 1));
                                fields.Remove(Coordinates.Get(tmpField.X, tmpField.Y - 1));
                                fields.Remove(Coordinates.Get(tmpField.X + 1, tmpField.Y - 1));
                                fields.Remove(Coordinates.Get(tmpField.X + 1, tmpField.Y));
                                fields.Remove(Coordinates.Get(tmpField.X + 1, tmpField.Y + 1));
                                fields.Remove(Coordinates.Get(tmpField.X, tmpField.Y + 1));
                                fields.Remove(Coordinates.Get(tmpField.X - 1, tmpField.Y + 1));
                                fields.Remove(Coordinates.Get(tmpField.X - 1, tmpField.Y));
                            }
                            break;	// stop searching
                        }
                    }
                }
            }
            return matrix;
        }

        public static void ShuffleInPlace(ArrayList source)
        {
            Random rnd = new Random();
            for (int inx = source.Count - 1; inx > 0; --inx)
            {
                int position = rnd.Next(inx);
                object temp = source[inx];
                source[inx] = source[position];
                source[position] = temp;
            }
        }
	
	    public void setShootable(bool shootable) {
		
		    this.Enabled = shootable;
		    if(shootable) {
			    for(int i = 0; i < 10; i++)
				    for(int j = 0; j < 10; j++)
					    if(ships[i,j].isNotShip()) {
						    if(!Global.SHOOTING_TIPS_ENABLED || isFieldShootable(i, j))
							    ships[i,j].setLaF(Ship.LAF.SHOOTABLE);
						    else
							    ships[i,j].setLaF(Ship.LAF.NOT_SHOOTABLE);
					    }
		    }
		    else {
			    for(int i = 0; i < 10; i++)
				    for(int j = 0; j < 10; j++)
					    if(ships[i,j].isNotShip())
						    ships[i,j].setNotShip();
		    }
	    }
    
        public bool isFieldShootable(int x, int y) {

    	    // CORNER CHECK
    	    if((x+1)<10 && (y+1)<10 && (ships[x+1,y+1].isShip() || ships[x+1,y+1].isSunk()))
    		    return false;
    	    if((x-1)>=0 && (y+1)<10 && (ships[x-1,y+1].isShip() || ships[x-1,y+1].isSunk()))
    		    return false;
    	    if((x+1)<10 && (y-1)>=0 && (ships[x+1,y-1].isShip() || ships[x+1,y-1].isSunk()))
    		    return false;
    	    if((x-1)>=0 && (y-1)>=0 && (ships[x-1,y-1].isShip() || ships[x-1,y-1].isSunk()))
    		    return false;
    	
    	    // SIDES CHECK
    	    if((y+1)<10 && (ships[x,y+1].isShip() || ships[x,y+1].isSunk()))
    		    return false;
    	    if((y-1)>=0 && (ships[x,y-1].isShip() || ships[x,y-1].isSunk()))
    		    return false;
    	    if((x+1)<10 && (ships[x+1,y].isShip() || ships[x+1,y].isSunk()))
    		    return false;
    	    if((x-1)>=0 && (ships[x-1,y].isShip() || ships[x-1,y].isSunk()))
    		    return false;
    	
    	    return true;	
        }
    }
}
