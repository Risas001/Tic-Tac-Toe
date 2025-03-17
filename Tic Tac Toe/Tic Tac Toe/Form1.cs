using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Tic_Tac_Toe
{
    public partial class Form1 : Form
    {
        private Button[,] tictactoeFeld = new Button[3, 3];
        private string aktuellesSymbol = "X";
        private Label spielstandLabel;
        private ComboBox spielerComboBox;
        private ComboBox schwierigkeitsComboBox;
        private Button resetButton;
        private Random random = new Random(); // Zufallsgenerator nur einmal erstellen

        public Form1()
        {
            InitializeComponent();
            this.Text = "Tic Tac Toe";
            this.ClientSize = new Size(400, 450);
            ErstelleTicTacToeFeld();
            ErstelleSpielstandLabel();
            ErstelleSpielerComboBox();
            ErstelleSchwierigkeitsComboBox();
            ErstelleResetButton();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void ErstelleTicTacToeFeld()
        {
            int buttonBreite = 100;
            int buttonHoehe = 100;
            int buttonAbstand = 10;
            int startX = 20;
            int startY = 80;

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    tictactoeFeld[i, j] = new Button();
                    tictactoeFeld[i, j].Width = buttonBreite;
                    tictactoeFeld[i, j].Height = buttonHoehe;
                    tictactoeFeld[i, j].Left = startX + (buttonBreite + buttonAbstand) * i;
                    tictactoeFeld[i, j].Top = startY + (buttonHoehe + buttonAbstand) * j;
                    tictactoeFeld[i, j].Tag = new int[] { i, j };
                    tictactoeFeld[i, j].Click += ButtonClick;
                    this.Controls.Add(tictactoeFeld[i, j]);
                }
            }
        }

        private void ErstelleSpielstandLabel()
        {
            spielstandLabel = new Label();
            spielstandLabel.Text = "Aktueller Spieler: " + aktuellesSymbol;
            spielstandLabel.AutoSize = true;
            spielstandLabel.Location = new Point(20, 20);
            this.Controls.Add(spielstandLabel);
        }

        private void ErstelleSpielerComboBox()
        {
            spielerComboBox = new ComboBox();
            spielerComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            spielerComboBox.Items.Add("Spieler");
            spielerComboBox.Items.Add("Bot");
            spielerComboBox.SelectedIndex = 0;
            spielerComboBox.Location = new Point(20, 50);
            spielerComboBox.SelectedIndexChanged += SpielerComboBoxSelectedIndexChanged;
            this.Controls.Add(spielerComboBox);
        }

        private void ErstelleSchwierigkeitsComboBox()
        {
            schwierigkeitsComboBox = new ComboBox();
            schwierigkeitsComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            schwierigkeitsComboBox.Items.Add("Leicht");
            schwierigkeitsComboBox.Items.Add("Mittel");
            schwierigkeitsComboBox.Items.Add("Schwer");
            schwierigkeitsComboBox.SelectedIndex = 0;
            schwierigkeitsComboBox.Location = new Point(220, 50);
            this.Controls.Add(schwierigkeitsComboBox);
        }

        private void ErstelleResetButton()
        {
            resetButton = new Button();
            resetButton.Text = "Reset";
            resetButton.Size = new Size(80, 30);
            resetButton.Location = new Point(130, 420);
            resetButton.Click += ResetButtonClick;
            this.Controls.Add(resetButton);
        }

        private void ButtonClick(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            if (button.Text == "")
            {
                button.Text = aktuellesSymbol;
                WechsleSpieler();

                // Bot soll erst nach dem Zug des Spielers ziehen
                if (spielerComboBox.SelectedIndex == 1 && !PruefeSpielende())
                {
                    BotZiehen();
                }

                if (PruefeSpielende())
                {
                    MessageBox.Show("Spiel beendet!");
                    NeuesSpiel();
                }
            }
        }

        private void WechsleSpieler()
        {
            aktuellesSymbol = (aktuellesSymbol == "X") ? "O" : "X";
            spielstandLabel.Text = "Aktueller Spieler: " + aktuellesSymbol;
        }

        private void BotZiehen()
        {
            if (PruefeSpielende()) return; // Nicht ziehen, wenn das Spiel schon vorbei ist

            switch (schwierigkeitsComboBox.SelectedIndex)
            {
                case 0:
                    BotLeichtZiehen();
                    break;
                case 1:
                    BotMittelZiehen();
                    break;
                case 2:
                    BotSchwerZiehen();
                    break;
            }

            if (PruefeSpielende())
            {
                MessageBox.Show("Spiel beendet!");
                NeuesSpiel();
            }
        }

        private void SpielerComboBoxSelectedIndexChanged(object sender, EventArgs e)
        {
            aktuellesSymbol = "X"; // Spieler beginnt immer mit X
            spielstandLabel.Text = "Aktueller Spieler: " + aktuellesSymbol;

        }

        private void BotLeichtZiehen()
        {
            List<Button> leereSchaltflaechen = tictactoeFeld.Cast<Button>().Where(button => button.Text == "").ToList();
            if (leereSchaltflaechen.Count > 0)
            {
                Button zufaelligeSchaltflaeche = leereSchaltflaechen[random.Next(leereSchaltflaechen.Count)];
                zufaelligeSchaltflaeche.Text = aktuellesSymbol;
                WechsleSpieler();
            }
        }

        private void BotMittelZiehen()
        {
            //1. Gewinnen, wenn möglich
            Button gewinnZug = FindeGewinnZug();
            if (gewinnZug != null)
            {
                gewinnZug.Text = aktuellesSymbol;
                WechsleSpieler();
                return;
            }

            //2. Blockieren, wenn der Gegner gewinnen könnte
            Button blockZug = FindeBlockZug();
            if (blockZug != null)
            {
                blockZug.Text = aktuellesSymbol;
                WechsleSpieler();
                return;
            }

            //3. Ansonsten zufälligen Zug
            BotLeichtZiehen();
        }

        private void BotSchwerZiehen()
        {
            //1. Gewinnen, wenn möglich
            Button gewinnZug = FindeGewinnZug();
            if (gewinnZug != null)
            {
                gewinnZug.Text = aktuellesSymbol;
                WechsleSpieler();
                return;
            }

            //2. Blockieren, wenn der Gegner gewinnen könnte
            Button blockZug = FindeBlockZug();
            if (blockZug != null)
            {
                blockZug.Text = aktuellesSymbol;
                WechsleSpieler();
                return;
            }

            //3. Zentrale auswählen, falls frei
            if (tictactoeFeld[1, 1].Text == "")
            {
                tictactoeFeld[1, 1].Text = aktuellesSymbol;
                WechsleSpieler();
                return;
            }

            //4. Ecke auswählen, falls frei
            Button eckenZug = FindeGuteEcke();
            if (eckenZug != null)
            {
                eckenZug.Text = aktuellesSymbol;
                WechsleSpieler();
                return;
            }

            //5. Ansonsten zufälligen Zug
            BotLeichtZiehen();
        }

        private Button FindeGewinnZug()
        {
            return FindeBesterZug(aktuellesSymbol);
        }

        private Button FindeBlockZug()
        {
            string gegnerSymbol = (aktuellesSymbol == "X") ? "O" : "X";
            return FindeBesterZug(gegnerSymbol);
        }

        private Button FindeBesterZug(string symbol)
        {
            foreach (Button button in tictactoeFeld)
            {
                if (button.Text == "")
                {
                    button.Text = symbol;
                    if (PruefeSpielendeOhneNeuesSpiel())
                    {
                        button.Text = ""; // Zug zurücknehmen
                        return button;
                    }
                    button.Text = ""; // Zug zurücknehmen
                }
            }
            return null;
        }

        private Button FindeGuteEcke()
        {
            List<Button> ecken = new List<Button>
            {
                tictactoeFeld[0, 0],
                tictactoeFeld[0, 2],
                tictactoeFeld[2, 0],
                tictactoeFeld[2, 2]
            };

            //Ecken priorisieren, die diagonal gegenüber vom Gegner liegen
            string gegnerSymbol = (aktuellesSymbol == "X") ? "O" : "X";
            foreach (Button ecke in ecken)
            {
                if (ecke.Text == "")
                {
                    if (ecke == tictactoeFeld[0, 0] && tictactoeFeld[2, 2].Text == gegnerSymbol) return ecke;
                    if (ecke == tictactoeFeld[0, 2] && tictactoeFeld[2, 0].Text == gegnerSymbol) return ecke;
                    if (ecke == tictactoeFeld[2, 0] && tictactoeFeld[0, 2].Text == gegnerSymbol) return ecke;
                    if (ecke == tictactoeFeld[2, 2] && tictactoeFeld[0, 0].Text == gegnerSymbol) return ecke;
                }
            }

            //Ansonsten erste freie Ecke
            return ecken.FirstOrDefault(ecke => ecke.Text == "");
        }

        // Prüft das Spielende, ohne ein neues Spiel zu starten
        private bool PruefeSpielendeOhneNeuesSpiel()
        {
            // Überprüfen, ob es eine vollständige Zeile gibt
            for (int i = 0; i < 3; i++)
            {
                if (tictactoeFeld[i, 0].Text == tictactoeFeld[i, 1].Text &&
                    tictactoeFeld[i, 0].Text == tictactoeFeld[i, 2].Text && tictactoeFeld[i, 0].Text != "")
                {
                    return true;
                }
            }

            // Überprüfen, ob es eine vollständige Spalte gibt
            for (int i = 0; i < 3; i++)
            {
                if (tictactoeFeld[0, i].Text == tictactoeFeld[1, i].Text &&
                    tictactoeFeld[0, i].Text == tictactoeFeld[2, i].Text &&
                    tictactoeFeld[0, i].Text != "")
                {
                    return true;
                }
            }

            // Überprüfen, ob es eine vollständige Diagonale gibt
            if (tictactoeFeld[0, 0].Text == tictactoeFeld[1, 1].Text &&
                tictactoeFeld[0, 0].Text == tictactoeFeld[2, 2].Text &&
                tictactoeFeld[0, 0].Text != "")
            {
                return true;
            }

            // Überprüfen, ob es eine vollständige Diagonale gibt
            if (tictactoeFeld[2, 0].Text == tictactoeFeld[1, 1].Text &&
                tictactoeFeld[2, 0].Text == tictactoeFeld[0, 2].Text &&
                tictactoeFeld[2, 0].Text != "")
            {
                return true;
            }

            // Überprüfen, ob das Spiel unentschieden ist
            bool unentschieden = true;
            foreach (Button button in tictactoeFeld)
            {
                if (button.Text == "")
                {
                    unentschieden = false;
                    break;
                }
            }

            // Wenn das Spiel unentschieden ist, wird eine Nachricht angezeigt und ein neues Spiel gestartet
            if (unentschieden)
            {
                MessageBox.Show("Unentschieden!");
                NeuesSpiel();
                return true;
            }

            return false;
        }

        private bool PruefeSpielende()
        {
            bool spielende = false;

            // Überprüfen, ob es eine vollständige Zeile gibt
            for (int i = 0; i < 3; i++)
            {
                if (tictactoeFeld[i, 0].Text == tictactoeFeld[i, 1].Text &&
                    tictactoeFeld[i, 0].Text == tictactoeFeld[i, 2].Text && tictactoeFeld[i, 0].Text != "")
                {
                    spielende = true;
                }
            }

            // Überprüfen, ob es eine vollständige Spalte gibt
            for (int i = 0; i < 3; i++)
            {
                if (tictactoeFeld[0, i].Text == tictactoeFeld[1, i].Text &&
                    tictactoeFeld[0, i].Text == tictactoeFeld[2, i].Text &&
                    tictactoeFeld[0, i].Text != "")
                {
                    spielende = true;
                }
            }

            // Überprüfen, ob es eine vollständige Diagonale gibt
            if (tictactoeFeld[0, 0].Text == tictactoeFeld[1, 1].Text &&
                tictactoeFeld[0, 0].Text == tictactoeFeld[2, 2].Text &&
                tictactoeFeld[0, 0].Text != "")
            {
                spielende = true;
            }

            // Überprüfen, ob es eine vollständige Diagonale gibt
            if (tictactoeFeld[2, 0].Text == tictactoeFeld[1, 1].Text &&
                tictactoeFeld[2, 0].Text == tictactoeFeld[0, 2].Text &&
                tictactoeFeld[2, 0].Text != "")
            {
                spielende = true;
            }

            // Überprüfen, ob das Spiel unentschieden ist
            bool unentschieden = true;
            foreach (Button button in tictactoeFeld)
            {
                if (button.Text == "")
                {
                    unentschieden = false;
                    break;
                }
            }

            // Wenn das Spiel unentschieden ist, wird eine Nachricht angezeigt und ein neues Spiel gestartet
            if (unentschieden && !spielende)
            {
                MessageBox.Show("Unentschieden!");
                NeuesSpiel();
                return true;
            }

            // gibt an, ob das Spiel beendet ist oder nicht
            return spielende;
        }

        private void Reset()
        {
            foreach (Button button in tictactoeFeld)
            {
                button.Text = "";
                button.Enabled = true;
            }
            aktuellesSymbol = "X";
            spielstandLabel.Text = "Aktueller Spieler: " + aktuellesSymbol;
        }

        private void ResetButtonClick(object sender, EventArgs e)
        {
            Reset();
        }

        private void NeuesSpiel()
        {
            foreach (Button button in tictactoeFeld)
            {
                button.Text = "";
            }
            aktuellesSymbol = "X";
            spielstandLabel.Text = "Aktueller Spieler: " + aktuellesSymbol;
        }
    }
}