using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Battleships
{
    public partial class MainWindow : Form
    {
        
        public const int MARGIN = 13;

        private GameBoard mainBoard;
        private GameBoard prevBoard;
        private WiFiService wifiService = null;

        ImageButton hostButton = null;
        ImageButton joinButton = null;
        ImageButton exitButton = null;
        ImageButton randomButton = null;
        ImageButton backButton = null;
        ImageButton nextButton = null;
        ImageButton endGameButton = null;

        public MainWindow()
        {
            System.Diagnostics.Debug.Listeners.Add(new System.Diagnostics.TextWriterTraceListener(System.Console.Out));
            InitializeComponent();
            myInitialization();
            FormClosing += delegate(Object o, FormClosingEventArgs e)
            {
                if (wifiService != null)
                    wifiService.Stop();
            };
        }

        private void Form1_Load(object sender, System.EventArgs e)
        {
        }

        private void myInitialization()
        {
            this.ClientSize = new System.Drawing.Size(350, 550);
            int topMargin = MARGIN + 20;
            int width = this.ClientSize.Width;
            int height = this.ClientSize.Height;

            hostButton = new ImageButton("Host Game");
            hostButton.Location = new Point(width / 2 - hostButton.Width / 2, height / 2 - 2 * hostButton.Height);
            hostButton.Click += delegate(object sender, EventArgs e) { creatingShipsLayout(GAME_MODE.HOST); };
            this.Controls.Add(hostButton);

            joinButton = new ImageButton("Join Game");
            joinButton.Location = new Point(width / 2 - joinButton.Width / 2, height / 2 - joinButton.Height + 5);
            joinButton.Click += delegate(object sender, EventArgs e) { creatingShipsLayout(GAME_MODE.CLIENT); };
            this.Controls.Add(joinButton);

            exitButton = new ImageButton("Exit");
            exitButton.Location = new Point(width / 2 - exitButton.Width / 2, height / 2 + joinButton.Height + 5);
            exitButton.Click += delegate(object sender, EventArgs e) { Close(); };
            this.Controls.Add(exitButton);

            backButton = new ImageButton("Back");
            backButton.Location = new Point(width / 4 - backButton.Width / 2, topMargin);
            backButton.Click += delegate(object sender, EventArgs e) { mainMenuLayout(); };

            nextButton = new ImageButton("Next");
            nextButton.Location = new Point(width / 2 + width / 4 - nextButton.Width / 2, topMargin);
            nextButton.Click += StartGame;

            randomButton = new ImageButton("Random");
            randomButton.Location = new Point(width / 2 - randomButton.Width / 2, topMargin + randomButton.Height + 10);
            randomButton.Click += delegate(object sender, EventArgs e) { mainBoard.setShips(GameBoard.generateMatrix()); };
                
            mainBoard = new GameBoard(GameBoard.BoardType.MAIN_BOARD, GameBoard.empty);
            mainBoard.Enabled = false;
            mainBoard.addToWindow(this);
            mainBoard.Visible = false;

            prevBoard = new GameBoard(GameBoard.BoardType.PREVIEW_BOARD, GameBoard.empty);
            prevBoard.Enabled = false;
            prevBoard.addToWindow(this);
            prevBoard.Visible = false;
        }

        private void mainMenuLayout()
        {
            Global.GameMode = GAME_MODE.UNDEFINED;

            this.Controls.Remove(randomButton);
            this.Controls.Remove(backButton);
            this.Controls.Remove(nextButton);
            this.Controls.Remove(endGameButton);

            this.Controls.Add(hostButton);
            this.Controls.Add(joinButton);
            this.Controls.Add(exitButton);
            this.Controls.Add(logoPictureBox);

            mainBoard.Visible = false;
            prevBoard.Visible = false;
            mainBoard.setShips(GameBoard.empty);
        }

        private void creatingShipsLayout(GAME_MODE mode)
        {
            Global.GameMode = mode;

            this.Controls.Remove(hostButton);
            this.Controls.Remove(joinButton);
            this.Controls.Remove(exitButton);
            this.Controls.Remove(logoPictureBox);

            this.Controls.Add(randomButton);
            this.Controls.Add(backButton);
            this.Controls.Add(nextButton);

            mainBoard.setShips(GameBoard.generateMatrix());
            mainBoard.Visible = true;
        }
    }

    class ConnectDialog : Form
    {
        private Label label;
        private ImageButton btn;
        private TextBox input;
        private WiFiService wifiService;
        private bool dialogAlive = true;
        private String partialIPAdress;

        public ConnectDialog(WiFiService wifiService)
        {
            Text = "Get connect";
            this.wifiService = wifiService;
            this.FormClosing += delegate(Object o, FormClosingEventArgs e) { dialogAlive = false; };

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

            btn = new ImageButton(ImageButton.SCALE.SCALE_MID, "Start");
            btn.Location = new Point(ClientSize.Width / 2 - btn.Width / 2, ClientSize.Height - btn.Height - 5);
            this.Controls.Add(btn);

            partialIPAdress = WiFiService.getLocalIP();

            if (Global.GameMode == GAME_MODE.HOST)
            {
                label.TextAlign = ContentAlignment.MiddleCenter;
                label.Text = "Your IP number is " + partialIPAdress + Environment.NewLine + "Waiting for client to connect...";
                btn.Text = "Cancel";

                label.Height = ClientSize.Height - btn.Height - 5;

                btn.Click += button_onCancel;
                Connecting();
            }
            else
            {
                label.TextAlign = ContentAlignment.TopCenter;
                label.Text = "Type here IP of host device:";
                label.TextAlign = ContentAlignment.MiddleCenter;

                input = new TextBox();
                partialIPAdress = partialIPAdress.Substring(0, partialIPAdress.LastIndexOf('.') + 1);
                input.Text = partialIPAdress;
                input.Width = 200;
                input.Font = new Font("Arial", 10, FontStyle.Bold);
                input.ForeColor = Color.Black;

                input.Location = new Point(ClientSize.Width / 2 - input.Width / 2, ClientSize.Height - btn.Height - 10 - input.Height);
                this.Controls.Add(input);

                label.Height = ClientSize.Height - btn.Height - 10 - input.Height;

                btn.Click += button_onStart;
            }
        }

        public void button_onCancel(object o, EventArgs e)
        {
            dialogAlive = false;
            wifiService.Stop();

            System.Threading.Thread.Sleep(500);
            Close();
        }

        public void button_onStart(object o, EventArgs e)
        {
            if (partialIPAdress.Equals(input.Text))     // if user hasn't changed IP in TextBox
                return;

            ((ClientWiFiService)wifiService).HostIP = input.Text;
            btn.Text = "Cancel";
            btn.Click -= button_onStart;
            btn.Click += button_onCancel;
            Connecting();
        }

        public void Connecting()
        {
            System.Threading.Thread thread = new System.Threading.Thread(new System.Threading.ThreadStart(
            delegate {
                wifiService.connect();
                if (Global.GameMode == GAME_MODE.CLIENT)
                    BeginInvoke(new MethodInvoker(delegate() { label.Text = "Connecting... Please wait."; }));
                
                while (!wifiService.isConnected() && dialogAlive && !wifiService.ConnectionTimeout)
                    System.Threading.Thread.Sleep(200);

                if (wifiService.ConnectionTimeout)
                    BeginInvoke(new MethodInvoker(delegate()
                    {
                        label.Text = "Connection timeout, is IP number correct?";
                        btn.Text = "Start";
                        btn.Click += button_onStart;
                        btn.Click -= button_onCancel;
                    }));
                else
                    BeginInvoke(new MethodInvoker(delegate() { Close(); }));
            }));
            thread.Name = "dialogThread";
            thread.Start();
        }
    }
}
