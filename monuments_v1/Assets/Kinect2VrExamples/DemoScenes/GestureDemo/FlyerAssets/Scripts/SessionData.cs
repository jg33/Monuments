using UnityEngine;

namespace VRStandardAssets.Flyer
{
    // This class is used to keep score during a game and save
    // the highscores to PlayerPrefs.
    public static class SessionData
    {
        private static int s_HighScore = 0;                             // Used to store the highscore for the current game type.
        private static int s_Score = 0;                                 // Used to store the current game's score.
		private static string s_CurrentGame = "flyerData";                        // The name of the current game type.


        public static int HighScore { get { return s_HighScore; } }
        public static int Score { get { return s_Score; } }


        public static void Restart()
        {
            // Reset the current score and get the highscore from player prefs.
            s_Score = 0;
            s_HighScore = GetHighScore();
        }


        public static void AddScore(int score)
        {
            // Add to the current score and check if the high score needs to be set.
            s_Score += score;
            CheckHighScore();
        }


        public static int GetHighScore()
        {
            // Get the value of the highscore from the game name.
            return PlayerPrefs.GetInt(s_CurrentGame, 0);
        }


        private static void CheckHighScore()
        {
            // If the current score is greater than the high score then set the high score.
            if (s_Score > s_HighScore)
                SetHighScore();
        }


        private static void SetHighScore()
        {
            // Make sure the name of the current game has been set.
            if (string.IsNullOrEmpty(s_CurrentGame))
                Debug.LogError("m_CurrentGame not set");

            // The high score is now equal to the current score.
            s_HighScore = s_Score;

            // Set the high score for the current game's name and save it.
            PlayerPrefs.SetInt(s_CurrentGame, s_Score);
            PlayerPrefs.Save();
        }
    }
}