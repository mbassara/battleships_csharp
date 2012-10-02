using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Drawing;
using System.Windows.Forms;

namespace Battleships
{
    public partial class MainWindow : Form
    {
        private delegate void Run();

        private bool meStartFirst;
        private Ship currentTarget = null;
        private bool gameIsEnded = true;

        private void StartGame(object sender, EventArgs e)
        {
            if (Global.GameMode == GAME_MODE.HOST)
                wifiService = new HostWiFiService();
            else
                wifiService = new ClientWiFiService();

            ConnectDialog dialog = new ConnectDialog(wifiService);
            dialog.ShowDialog();

            if (!wifiService.isConnected())
                return;

            this.Controls.Remove(randomButton);
            this.Controls.Remove(backButton);
            this.Controls.Remove(nextButton);

            if (endGameButton == null)
            {
                endGameButton = new ImageButton("End game");
                endGameButton.Click += delegate(object o, EventArgs ev) { EndGame(false); };
                int widthSpaceForEndButton = ClientSize.Width - prevBoard.Margin - prevBoard.Width;
                endGameButton.Location = new System.Drawing.Point(prevBoard.Margin + prevBoard.Width + widthSpaceForEndButton / 2 - endGameButton.Width / 2,
                                                                  prevBoard.Margin + prevBoard.Height / 2 - endGameButton.Height / 2);
            }

            this.Controls.Add(endGameButton);
            bool[,] matrix = mainBoard.getShips();
            mainBoard.setShips(GameBoard.empty);
            prevBoard.setShips(matrix);
            prevBoard.Visible = true;
        
            prevBoard.Enabled = false;
            mainBoard.Enabled = false;
            mainBoard.Click += onShipClick;   // add action to each Ship in mainBoard
            mainBoard.DoubleClick += onShipClick;

            if (Global.GameMode == GAME_MODE.HOST)
            {
                bool whoStarts = (new Random()).Next() % 2 == 0;
                meStartFirst = whoStarts == Global.HOST_FIRST;
                GamePacket packet = new GamePacket(whoStarts);
                wifiService.send(packet);

                mainBoard.setShootable(meStartFirst);
            }
            else
                mainBoard.setShootable(false);

            this.FormClosing += delegate(object s, FormClosingEventArgs ev) { gameIsEnded = true; };

            Thread ReceivingThread = new Thread(new ThreadStart(ReceivingThreadStart));
            ReceivingThread.Name = "Main game loop ReceivingThread";
            ReceivingThread.Start();
        }

        private void onShipClick(object sender, EventArgs e)
        {
            Ship button = (Ship) sender;
            if(!button.IsTarget) {
                int x = button.X;
                int y = button.Y;
                if(button.isNotShip()) {
                    if(currentTarget != null)
                        currentTarget.IsTarget = false;

                    currentTarget = button;
                    button.IsTarget = true;
                }
            }
            else if(currentTarget != null) {	// shot
                button.IsTarget = false;
                button.setLaF(Ship.LAF.SHOT);
                mainBoard.Enabled = false;
                wifiService.send(new GamePacket(currentTarget.X, currentTarget.Y));
            }
        }

        private void EndGame(bool result) {
            gameIsEnded = true;
            GameResultDialog dialog = new GameResultDialog(result);
            dialog.ShowDialog(this);
            mainMenuLayout();
        }

        private void ReceivingThreadStart() {
            System.Diagnostics.Debug.WriteLine("ReceivingThread started");
            GamePacket packet;
            gameIsEnded = false;
            while (!gameIsEnded) {
                Thread.Sleep(250);

                packet = wifiService.receive();
                if (packet != null)
                {
                    System.Diagnostics.Debug.WriteLine(packet.GetType() + " packt received:" + System.Environment.NewLine + GamePacketSerialization.serialize(packet));

                    if (packet.Type == GamePacket.TYPE.WHO_STARTS)
                    {
                        meStartFirst = packet.getWhoStarts() == Global.CLIENT_FIRST;
                        BeginInvoke(new MethodInvoker(delegate() { mainBoard.setShootable(meStartFirst); }));
                    }
                    else if (packet.Type == GamePacket.TYPE.USER_NAME) {
                    }
                    else if (packet.Type == GamePacket.TYPE.SHOT)
                    {
                        BeginInvoke(new MethodInvoker(delegate()
                        {
                            ShotResult result = prevBoard.Shoot(packet.getCoordinates());
                            if (prevBoard.isGameEnded())
                            {
                                wifiService.send(new GamePacket(new GameResult(Global.GAME_RESULT_WINNER)));	// opponent is winner because he sunk all of my ships
                                EndGame(false);
                            }
                            else
                            {
                                wifiService.send(new GamePacket(result));
                                if (!result.isHit() || result.isSunk())
                                    BeginInvoke(new MethodInvoker(delegate() { mainBoard.setShootable(true); }));
                            }
                        }));
                    }
                    else if(packet.Type == GamePacket.TYPE.RESULT) {
                        bool result = packet.getShotResult().isHit();
                        BeginInvoke(new MethodInvoker(delegate()
                        {
                            mainBoard.ShotResult(packet.getShotResult());
                            mainBoard.setShootable(result && !packet.getShotResult().isSunk());
                        }));
				
                        currentTarget = null;
                        if (packet.getShotResult().isSunk())
                            BeginInvoke(new MethodInvoker(delegate() { mainBoard.setShipSunk(packet.getShotResult().getMatrix()); }));
                    }
                    else if (packet.Type == GamePacket.TYPE.GAME_RESULT)
                    {
                        BeginInvoke(new MethodInvoker(delegate() { EndGame(packet.getGameResult().isWinner()); }));
                    }
                }
            }
        }
    }

    class GameResultDialog : Form
    {
        private Label label;
        private ImageButton btn;

        public GameResultDialog(bool gameResult)
        {
            Text = "Game result";

            FormBorderStyle = FormBorderStyle.FixedDialog;
            ControlBox = false;
            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = false;
            BackgroundImage = global::Battleships.Properties.Resources.background_small;
            BackgroundImageLayout = ImageLayout.Center;
            ClientSize = new System.Drawing.Size(300, 120);

            label = new Label();
            this.Controls.Add(label);
            label.Size = this.ClientSize;
            label.BackColor = Color.Transparent;
            label.ForeColor = Color.LightGray;
            label.Font = new Font("Arial", 12, FontStyle.Bold);
            label.TextAlign = ContentAlignment.MiddleCenter;
            if (gameResult == Global.GAME_RESULT_WINNER)
                label.Text = "Congratulations!!! You won!!!";
            else
                label.Text = "Unfortunately, your opponent turned out to be better...";

            btn = new ImageButton(ImageButton.SCALE.SCALE_MID, "OK");
            btn.Location = new Point(ClientSize.Width / 2 - btn.Width / 2, ClientSize.Height - btn.Height - 5);
            btn.Click += delegate(object s, EventArgs e) { this.Close(); };
            this.Controls.Add(btn);

            label.Height = ClientSize.Height - btn.Height - 5;

        }
    }
}
