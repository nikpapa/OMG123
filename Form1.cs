using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using System.Windows.Forms;

namespace Potiria_net
{
    public partial class Form1 : Form
    {
        private CupsGame game;
        private int playerToMove;
        private int playerToStart;
        private bool[] Boxes;

        public Form1()
        {
            InitializeComponent();
            InitControls();
            updateBoxes(0);
            edHuman.Checked = true;
        }
               
        protected void updateBoxes(int cups)
        {
            for (int i = 1; i <= 20; i++)
            {
                foreach (Control c in lpBoxes.Controls) //assuming this is a Form
                {
                    if (c is PictureBox && c.Name == "Box" + i.ToString())
                    {
                        if (i <= cups)
                            ((PictureBox)c).Image = global::Potiria_net.Properties.Resources.glass4;
                        else
                            ((PictureBox)c).Image = null;
                    }
                }
            }
        }

        protected void updateBoxes()
        {
            for (int i = 1; i <= 20; i++)
            {
                foreach (Control c in lpBoxes.Controls) //assuming this is a Form
                {
                    if (c is PictureBox && c.Name == "Box" + i.ToString())
                    {
                        if (Boxes[i - 1])
                            ((PictureBox)c).Image = global::Potiria_net.Properties.Resources.glass4;
                        else
                            ((PictureBox)c).Image = null;
                    }
                }
            }

            Application.DoEvents();
        }

        private void updateMessage(string text)
        {
            edMessage.AppendText(text);
            edMessage.SelectionStart = edMessage.Text.Length;
            edMessage.ScrollToCaret();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            StartGame();
        }

        private void StartGame()
        {
            int StartCups = 0;
            if (edCups.Value == 0)
            {
                Random rnd = new Random();
                StartCups = rnd.Next(5, 21);
            }
            else
                StartCups = Convert.ToInt32(edCups.Value);

            game = new CupsGame(CupsGame.PLAYER_A, StartCups, Convert.ToInt32(edAI.Checked));

            Boxes = new bool[20];

            initBoxPlacement(StartCups);

            updateBoxes();
            updateMessage("\nΞεκίνησε νέο παιχνίδι με " + game.getStartingCups() + " ποτήρια!");

            edCups.Enabled = false;
            edHuman.Enabled = false;
            edAI.Enabled = false;
            btnStart.Enabled = false;
            btnStart.BackColor = Form1.DefaultBackColor;

            playerToMove = QuestionStartingTurn(); 
            playerToStart = playerToMove;
            updateMessage("\nΤο παιχνίδι ξεκινά ο " + game.PLAYER_STRING[playerToStart]);
            ReadyForPlayer();
        }

        private void initBoxPlacement(int cups)
        {
            List<int> numbers = new List<int>();
            for (int i = 0; i < 20; i++)
                numbers.Add(i);

            var Shuffled = numbers.OrderBy(a => Guid.NewGuid()).GetEnumerator();

            int count = 0;
            while(Shuffled.MoveNext() && count < cups)
            {
                Boxes[Shuffled.Current] = true;
                count++;
            }
        }

        private void ReadyForPlayer()
        {
            if (game.getGameMode() == CupsGame.HUMAN_vs_AI && playerToMove == CupsGame.PLAYER_B)
                PlayAIMove();
            
            int cups = game.getCurrentCups();


            if (playerToMove == CupsGame.PLAYER_A)
            {
                btn_B_1.Enabled = false;
                btn_B_2.Enabled = false;
                btn_B_3.Enabled = false;
                btn_B_resign.Enabled = false;

                btn_A_1.Enabled = (cups >= 1);
                btn_A_2.Enabled = (cups >= 2);
                btn_A_3.Enabled = (cups >= 3);
                btn_A_resign.Enabled = true;
            }
            else
            {
                btn_A_1.Enabled = false;
                btn_A_2.Enabled = false;
                btn_A_3.Enabled = false;
                btn_A_resign.Enabled = false;                

                btn_B_1.Enabled = (cups >= 1);
                btn_B_2.Enabled = (cups >= 2);
                btn_B_3.Enabled = (cups >= 3);
                btn_B_resign.Enabled = true;
            }
        }

        private void InitControls()
        {
            btn_A_1.Enabled = false;
            btn_A_2.Enabled = false;
            btn_A_3.Enabled = false;
            btn_A_resign.Enabled = false; 
            btn_B_1.Enabled = false;
            btn_B_2.Enabled = false;
            btn_B_3.Enabled = false;
            btn_B_resign.Enabled = false;

            edCups.Enabled = true;
            edHuman.Enabled = true;
            edAI.Enabled = true;
            btnStart.Enabled = true;
            btnStart.BackColor = Color.GreenYellow;
        }

        private void makeMove(int cups, int player)
        {
            if (game.move(player, cups))
            {
                moveUI(cups);
                updateBoxes();
                updateMessage("\nΟ " + game.PLAYER_STRING[playerToMove] + " αφαίρεσε " + cups.ToString() + " ποτήρια." +
                              " Απέμειναν " + game.getCurrentCups() + " ποτήρια.");

                if (checkTerminal())
                {
                    return;
                }

                playerToMove = (playerToMove + 1) % 2;
                ReadyForPlayer();
            }
        }

        private int QuestionStartingTurn()
        {
            DialogResult dialogResult = MessageBox.Show(
            "ΠΑΙΚΤΗ Α, θέλεις να παίξεις πρώτος;", "OMG123",
            MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);

            if (dialogResult == DialogResult.Yes)
                return CupsGame.PLAYER_A;

            if (game.getGameMode() == CupsGame.HUMAN_vs_HUMAN)
            {
                DialogResult dialogResult2 = MessageBox.Show(
                "ΠΑΙΚΤΗ B, ο ΠΑΙΚΤΗΣ Α αρνήθηκε να παίξει πρώτος. Μήπως θέλεις να παίξεις εσύ πρώτος;", "OMG123",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);

                if (dialogResult2 == DialogResult.Yes)
                    return CupsGame.PLAYER_B;
                else
                    return CupsGame.PLAYER_A;
            }
            else //if (game.FirstTurnDecision())
                return CupsGame.PLAYER_B;
            //else
            //    return CupsGame.PLAYER_A;
        }

        private void btn_A_1_Click(object sender, EventArgs e)
        {
            makeMove(1, CupsGame.PLAYER_A);
        }

        private void btn_A_2_Click(object sender, EventArgs e)
        {
            makeMove(2, CupsGame.PLAYER_A);
        }

        private void btn_A_3_Click(object sender, EventArgs e)
        {
            makeMove(3, CupsGame.PLAYER_A);
        }

        private void btn_B_1_Click(object sender, EventArgs e)
        {
            makeMove(1, CupsGame.PLAYER_B);
        }

        private void btn_B_2_Click(object sender, EventArgs e)
        {
            makeMove(2, CupsGame.PLAYER_B);
        }

        private void btn_B_3_Click(object sender, EventArgs e)
        {
            makeMove(3, CupsGame.PLAYER_B);
        }

        private void btnAbout_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show(
            "Συντελεστές\n----------------\nΣύλληψη, λογική: Όμιλος Μαθηματικών Γυμν. Σχολής Μωραΐτη" +
            "\n     Υπεύθυνοι Καθηγητές: Δρ.Κωστής Ανδριόπουλος, Στέλλα Κίτσου MSc" +
            "\n     Μέλη (μαθητές) του Ομίλου: Γιώργος Βούζας, " +
            "\n     Οδυσσέας Ζαχαριάδης, Κίμων Καπετανάκης, Γιώργος Μακράκης, " +
            "\n     Πέτρος Νομικός, Τζώρτζης Παναγόπουλος, Κίμων Τρούκης, " +
            "\n     Δανάη Πετροπούλου, Βασιλική Πουλά, Ιωάννα Σαραντοπούλου, " +
            "\n     Σοφοκλής Στρόμπολας, Χρήστος Παπαδημούλης" +
            "\nΠρογραμματισμός εφαρμογής: Νίκος Παπαχρήστου" + 
            "\n\nΟδηγίες\n----------------\nΤο παιχνίδι παίζεται από δύο παίκτες (ο ένας παίκτης μπορεί να είναι ο υπολογιστής). Οι παίκτες επιλέγουν το πλήθος των ποτηριών τού παιχνιδιού και παίζουν εναλλάξ. Το ποιός θα παίξει πρώτος καθορίζεται από τους παίκτες με μια αυτόματη ερώτηση. Οι μόνες επιτρεπτές κινήσεις σε οποιαδήποτε φάση του παιχνιδιού είναι ο παίκτης να " +
            "πάρει από το πλήθος των εναπομείναντων ποτηριών 1 ή 2 ή 3 ποτήρια. Σκοπός τους είναι να προχωρήσουν σε μία κίνηση έτσι ώστε να αφήσουν ένα μόνο ποτήρι για τον αντίπαλο. Με άλλα λόγια, αυτός που θα αναγκαστεί να πάρει το τελευταίο ποτήρι χάνει.",             
            "OMG123: Οδηγίες",
            MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void PlayAIMove()
        {
            int cupsAI = game.bestMove();
            game.move(CupsGame.PLAYER_B, cupsAI);
            updateMessage("\nΟ " + game.PLAYER_STRING[playerToMove] + " αφαίρεσε " + cupsAI.ToString() + " ποτήρια." +
                          " Απέμειναν " + game.getCurrentCups() + " ποτήρια.");

            moveUI(cupsAI);
            updateBoxes();
            if (checkTerminal())
                return;

            playerToMove = (playerToMove + 1) % 2;
            ReadyForPlayer();
        }

        private bool checkTerminal()
        {
            if (game.isTerminal())
            {
                int reward = game.reward(playerToStart);
                int playerWin;
                if (playerToStart == CupsGame.PLAYER_A)
                    if (reward == 1)
                        playerWin = CupsGame.PLAYER_A;
                    else
                        playerWin = CupsGame.PLAYER_B;
                else
                    if (reward == 1)
                        playerWin = CupsGame.PLAYER_B;
                    else
                        playerWin = CupsGame.PLAYER_A;

                updateMessage("\nΟ " + game.PLAYER_STRING[playerWin] + " νίκησε!");

                string FinalMessage = "Ο " + game.PLAYER_STRING[playerWin] + " κέρδισε το παιχνίδι. ";
                if (playerWin == CupsGame.PLAYER_B && game.getGameMode() == CupsGame.HUMAN_vs_AI)
                    FinalMessage += "Προσπάθησε ξανά!";
                else
                    FinalMessage += "Συγχαρητήρια!";

                DialogResult dialogResult = MessageBox.Show(FinalMessage, "OMG123",
                                            MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                updateMessage("\nΠατήστε το κουμπί Εκκίνηση για να παίξετε καινούργιο παιχνίδι.");

                InitControls();
                return true;
            }

            return false;
        }

        private void moveUI(int cups)
        {
            List<int> posOccupied = new List<int>();
            List<int> posRemoved = new List<int>();

            for (int i = 0; i < 20; i++)
                if (Boxes[i])
                    posOccupied.Add(i);

            int count = posOccupied.Count();
            int pos;
            for (int i = 0; i < cups; i++)
            {
                Random rnd = new Random();
                pos = rnd.Next(0, count - i);
                posRemoved.Add(posOccupied[pos]);
                posOccupied.RemoveAt(pos);
            }

            for (int i = 0; i < posRemoved.Count; i++)
                Boxes[posRemoved[i]] = false;

            for (int j = 0; j < 3; j++)
            {
                System.Threading.Thread.Sleep(200);
                updateBoxes();
                Application.DoEvents();
                for (int i = 0; i < posRemoved.Count; i++)
                    Boxes[posRemoved[i]] = true;

                System.Threading.Thread.Sleep(200);
                updateBoxes();
                Application.DoEvents();
                for (int i = 0; i < posRemoved.Count; i++)
                    Boxes[posRemoved[i]] = false;

            }
 //           System.Threading.Thread.Sleep(500);
            updateBoxes();
            Application.DoEvents();
        }

        private void btn_A_resign_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show(
            "ΠΑΙΚΤΗ Α, είσαι σίγουρος ότι θέλεις να εγκαταλείψεις το τρέχον παιχνίδι;", "OMG123",
            MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);

            if (dialogResult == DialogResult.Yes)
            {
                updateMessage("\nΟ Παίκτης Α εγκατέλειψε το τρέχον παιχνίδι. \nΟ " + game.PLAYER_STRING[1] + " είναι ο νικήτης!");

                string FinalMessage = "Ο " + game.PLAYER_STRING[1] + " κέρδισε το παιχνίδι. ";
                if (game.getGameMode() == CupsGame.HUMAN_vs_AI)
                    FinalMessage += "Προσπάθησε ξανά!";
                else
                    FinalMessage += "Συγχαρητήρια!";

                DialogResult dialogResult2 = MessageBox.Show(FinalMessage, "OMG123",
                                            MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                updateMessage("\nΠατήστε το κουμπί Εκκίνηση για να παίξετε καινούργιο παιχνίδι.");

                InitControls();
            }
        }

        private void btn_B_resign_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show(
            "ΠΑΙΚΤΗ B, είσαι σίγουρος ότι θέλεις να εγκαταλείψεις το τρέχον παιχνίδι;", "OMG123",
            MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);

            if (dialogResult == DialogResult.Yes)
            {
                updateMessage("\nΟ Παίκτης B εγκατέλειψε το τρέχον παιχνίδι. \nΟ Παίκτης A είναι ο νικήτης!");

                string FinalMessage = "Ο " + game.PLAYER_STRING[0] + " κέρδισε το παιχνίδι. ";
                if (game.getGameMode() == CupsGame.HUMAN_vs_AI)
                    FinalMessage += "Προσπάθησε ξανά!";
                else
                    FinalMessage += "Συγχαρητήρια!";

                DialogResult dialogResult2 = MessageBox.Show(FinalMessage, "OMG123",
                                            MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                updateMessage("\nΠατήστε το κουμπί Εκκίνηση για να παίξετε καινούργιο παιχνίδι.");

                InitControls();
            }
        }

    }
}
