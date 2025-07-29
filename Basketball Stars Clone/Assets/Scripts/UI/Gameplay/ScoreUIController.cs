using Enums;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

namespace UI.Gameplay
{
    /// <summary>
    /// Controller class to update the scores values and visibility
    /// </summary>
    public class ScoreUIController : MonoBehaviour
    {
        [Header("UI")] 
        [SerializeField] private TextMeshProUGUI scorePlayer1;
        [SerializeField] private TextMeshProUGUI scorePlayer2;

        private void Awake()
        {
            #if UNITY_EDITOR
            Assert.IsNotNull(scorePlayer1, $"{nameof(TextMeshProUGUI)} reference is missing on '{gameObject.name}'");
            Assert.IsNotNull(scorePlayer2, $"{nameof(TextMeshProUGUI)} reference is missing on '{gameObject.name}'");
            #endif
        }

        public void ResetScore()
        {
            scorePlayer1.text = "0";
            scorePlayer2.text = "0";
        }

        public void UpdateScore(PlayerNumber player, int score)
        {
            if (player == PlayerNumber.Player1)
            {
                scorePlayer1.text = "" + score;
            }
            else
            {
                scorePlayer2.text = "" + score;
            }
        }

        public void ShowScores()
        {
            scorePlayer1.gameObject.SetActive(true);
            scorePlayer2.gameObject.SetActive(true);
        }

        public void HideScores()
        {
            scorePlayer1.gameObject.SetActive(false);
            scorePlayer2.gameObject.SetActive(false);
        }
    }
}