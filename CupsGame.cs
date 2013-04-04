using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Potiria_net
{
    class CupsGame
    {
        public static int PLAYER_A = 0;
        public static int PLAYER_B = 1;
        public static int HUMAN_vs_HUMAN = 0;
        public static int HUMAN_vs_AI = 1;
        public string [] PLAYER_STRING = new string[2] { "ΠΑΙΚΤΗΣ Α", "ΠΑΙΚΤΗΣ Β"};

        private int StartingCups;
        private int CurrentCups;
        private int StartingPlayer; // 0: A, 1: B
        private int GameMode; // 0: Human vs Human, 1: Human vs AI
        private List<int> movesA = new List<int>();
        private List<int> movesB = new List<int>();

    	public CupsGame(int player, int cups)
        {
    	    StartingPlayer = player;
	        StartingCups = cups;
	        CurrentCups = cups;
            GameMode = HUMAN_vs_HUMAN;
        }

        public CupsGame(int player, int cups, int mode)
        {
            StartingPlayer = player;
            StartingCups = cups;
            CurrentCups = cups;
            GameMode = mode;
            if (GameMode == HUMAN_vs_AI)
                PLAYER_STRING[HUMAN_vs_AI] = "Η/Υ";
        }

	    public bool move(int player, int cups)
        {
	        if (cups > CurrentCups || cups > 3)
		        return false;

	        CurrentCups -= cups;
	        addMove(player, cups);
            return true;
        }

        public bool isTerminal()
        {
	        if (CurrentCups == 0)
		        return true;
	        else
		        return false;
        }

	    public int reward(int player)
        {
	        if (movesA.Count() == movesB.Count())
		        return 1; // first player wins
	        else
		        return -1; // second player wins
        }

	    public void printState()
        {
	        Console.WriteLine("Cups: " + CurrentCups); 
        }

	    List<int> findMoves()
        {
	        List<int> moves = new List<int>();
	        int m; 
	        if (CurrentCups > 3)
		        m = 3;
	        else
		        m = CurrentCups;

	        for (int i = 1; i <= m; i++)
		        moves.Add(i);

	        return moves;
        }

	    private void addMove(int player, int cups)
        {
	        if (player == PLAYER_A)
		        movesA.Add(cups);
	        else
		        movesB.Add(cups);
        }

        public int getStartingCups()
        {
            return StartingCups;
        }

        public int getCurrentCups()
        {
            return CurrentCups;
        }

        public int bestMove()
        {
            if (CurrentCups == 1)
                return 1;

            int mod = CurrentCups % 4;
            if (mod == 0)
                return 3;
            else if (mod == 3)
                return 2;
            else
                return 1;
        }

        public bool FirstTurnDecision() // False: Do not play first, True: play first
        {
            if (CurrentCups % 4 == 1)
                return true;
            else
                return false;
        }

        public int getGameMode()
        {
            return GameMode;
        }
    };
}
