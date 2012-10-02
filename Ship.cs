using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Battleships
{
    class Ship : PictureBox
    {
        public enum ShipState { NOT_SHIP, SHIP, HIT, MISSED, SUNK };
        public enum LAF { POSSIBLE, IMPOSSIBLE, NORMAL, SELECTED, HIT, SUNK,
                        MISSED, SHOOTABLE, NOT_SHOOTABLE, TARGET, PREVIOUS, SHOT };

        private LAF LaF = LAF.NORMAL;
        private LAF prevLaF = LAF.NORMAL;
        private GameBoard.BoardType boardType;

        private int SIZE;

        public Ship(int x, int y, GameBoard.BoardType boardType, bool state)
        {
            this.x = x;
            this.y = y;
            this.boardType = boardType;
            this.SIZE = boardType == GameBoard.BoardType.MAIN_BOARD ? 30 : 13;
            this.State = state ? ShipState.SHIP : ShipState.NOT_SHIP;
            Width = SIZE;
            Height = SIZE;
            BackColor = System.Drawing.Color.Transparent;
        }

        private ShipState state;
        public ShipState State
        {
            get { return state; }
            set {
                state = value;
                switch (state)
                {
                    case ShipState.NOT_SHIP:
                        setLaF(LAF.NORMAL);
                        break;
                    case ShipState.SHIP:
                        setLaF(LAF.SELECTED);
                        break;
                    case ShipState.HIT:
                        setLaF(LAF.HIT);
                        break;
                    case ShipState.MISSED:
                        setLaF(LAF.MISSED);
                        break;
                    case ShipState.SUNK:
                        setLaF(LAF.SUNK);
                        break;
                }
            }
        }

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

        private bool isTarget = false;
        public bool IsTarget
        {
            get { return isTarget; }
            set
            {
                this.isTarget = value;

                if (isTarget)
                    this.setLaF(LAF.TARGET);
                else
                    this.setLaF(LAF.PREVIOUS);
            }
        }

        public bool Shoot()
        {
            if (State == ShipState.SHIP)
            {
                State = ShipState.HIT;
                return true;
            }
            else if (State == ShipState.SUNK || State == ShipState.HIT)
                return true;
            else
            {
                State = ShipState.MISSED;
                return false;
            }
        }

        public bool isShip()
        {
            return state == ShipState.SHIP;
        }

        public bool isNotShip()
        {
            return state == ShipState.NOT_SHIP;
        }

        public bool isHit()
        {
            return state == ShipState.SUNK || state == ShipState.HIT;
        }

        public bool isSunk()
        {
            return state == ShipState.SUNK;
        }

        public bool isFinished()
        {	// sunk, missed or not ship
            return state == ShipState.SUNK || state == ShipState.MISSED || state == ShipState.NOT_SHIP;
        }

        public void setNotShip()
        {
            state = ShipState.NOT_SHIP;
            this.setLaF(LAF.NORMAL);
        }

        public void setShip()
        {
            state = ShipState.SHIP;
            this.setLaF(LAF.SELECTED);
        }

        public void setSunk()
        {
            state = ShipState.SUNK;
            this.setLaF(LAF.SUNK);
        }

        public void setHit()
        {
            state = ShipState.HIT;
            this.setLaF(LAF.HIT);
        }

        public void setMissed()
        {
            state = ShipState.MISSED;
            this.setLaF(LAF.MISSED);
        }

        public LAF getLaF()
        {
            return LaF;
        }

        public void setLaF(LAF laf)
        {
            if (laf == LAF.PREVIOUS)
            {
                setLaF(prevLaF);
                return;
            }

            LAF prevPrevLaf = prevLaF;
            prevLaF = LaF;
            LaF = laf;
            switch (laf)
            {
                case LAF.IMPOSSIBLE:
                    BackgroundImage = (boardType == GameBoard.BoardType.MAIN_BOARD)
                                        ? global::Battleships.Properties.Resources.ic_im_ship_impossible
                                        : global::Battleships.Properties.Resources.ic_im_ship_impossible_small;
                    break;
                case LAF.POSSIBLE:
                    BackgroundImage = (boardType == GameBoard.BoardType.MAIN_BOARD)
                                        ? global::Battleships.Properties.Resources.ic_im_ship_possible
                                        : global::Battleships.Properties.Resources.ic_im_ship_possible_small;
                    break;
                case LAF.NORMAL:
                    BackgroundImage = (boardType == GameBoard.BoardType.MAIN_BOARD)
                                        ? global::Battleships.Properties.Resources.ic_im_ship
                                        : global::Battleships.Properties.Resources.ic_im_ship_small;
                    break;
                case LAF.SELECTED:
                    BackgroundImage = (boardType == GameBoard.BoardType.MAIN_BOARD)
                                        ? global::Battleships.Properties.Resources.ic_im_ship_sel
                                        : global::Battleships.Properties.Resources.ic_im_ship_sel_small;
                    break;
                case LAF.HIT:
                    BackgroundImage = (boardType == GameBoard.BoardType.MAIN_BOARD)
                                        ? global::Battleships.Properties.Resources.ic_im_ship_hit
                                        : global::Battleships.Properties.Resources.ic_im_ship_hit_small;
                    break;
                case LAF.SUNK:
                    BackgroundImage = (boardType == GameBoard.BoardType.MAIN_BOARD)
                                        ? global::Battleships.Properties.Resources.ic_im_ship_sunk
                                        : global::Battleships.Properties.Resources.ic_im_ship_sunk_small;
                    break;
                case LAF.MISSED:
                    BackgroundImage = (boardType == GameBoard.BoardType.MAIN_BOARD)
                                        ? global::Battleships.Properties.Resources.ic_im_ship_missed
                                        : global::Battleships.Properties.Resources.ic_im_ship_missed_small;
                    break;
                case LAF.SHOOTABLE:
                    BackgroundImage = (boardType == GameBoard.BoardType.MAIN_BOARD)
                                        ? global::Battleships.Properties.Resources.ic_im_ship_shootable
                                        : global::Battleships.Properties.Resources.ic_im_ship_shootable_small;
                    break;
                case LAF.NOT_SHOOTABLE:
                    BackgroundImage = (boardType == GameBoard.BoardType.MAIN_BOARD)
                                        ? global::Battleships.Properties.Resources.ic_im_ship_not_shootable
                                        : global::Battleships.Properties.Resources.ic_im_ship_not_shootable_small;
                    break;
                case LAF.TARGET:
                    BackgroundImage = (boardType == GameBoard.BoardType.MAIN_BOARD)
                                        ? global::Battleships.Properties.Resources.ic_im_ship_shootable_target
                                        : global::Battleships.Properties.Resources.ic_im_ship_shootable_target_small;
                    break;
                case LAF.SHOT:
                    BackgroundImage = (boardType == GameBoard.BoardType.MAIN_BOARD)
                                        ? global::Battleships.Properties.Resources.ic_im_ship_shootable_shot
                                        : global::Battleships.Properties.Resources.ic_im_ship_shootable_shot_small;
                    break;
                default:
                    LaF = prevLaF;
                    prevLaF = prevPrevLaf;
                    break;
            }
        }

        public new String ToString()
        {
            String result = null;
            switch (state)
            {
                case ShipState.SHIP:
                    result = "S";
                    break;
                case ShipState.HIT:
                    result = "H";
                    break;
                case ShipState.MISSED:
                    result = "M";
                    break;
                case ShipState.SUNK:
                    result = "X";
                    break;
                default:
                    result = ".";
                    break;
            }

            return result;
        }

    }
}
