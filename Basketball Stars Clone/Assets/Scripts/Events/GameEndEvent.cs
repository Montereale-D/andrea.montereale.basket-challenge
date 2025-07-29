namespace Events
{
    public class GameEndEvent : EventData
    {
        public int Player1Score { get; set; }
        public int Player2Score { get; set; }

        public GameEndEvent(int score1, int score2)
        {
            Player1Score = score1;
            Player2Score = score2;
        }
    }
}