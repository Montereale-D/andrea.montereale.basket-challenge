using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace UI.Gameplay
{
    /// <summary>
    /// Controller class that manage the reward panel visibility and its texts
    /// </summary>
    public class RecapUIController : MonoBehaviour
    {
        [Header("Recap UI")]
        [SerializeField] private GameObject recapPanel;

        [Header("Subpanel UI")] 
        [SerializeField] private GameObject trainingUI;
        [SerializeField] private GameObject duelUI;
        [SerializeField] private GameObject duelReward;
        [SerializeField] private Text duelScoreTitle;

        private const string PlayerWinTitle = "NICE JOB !";
        private const string PlayerLostTitle = "YOU LOST";
        private const string PlayerTieTitle = "TIE";

        private void Awake()
        {
            #if UNITY_EDITOR
            Assert.IsNotNull(recapPanel, $"{nameof(GameObject)} reference is missing on '{gameObject.name}'");
            Assert.IsNotNull(trainingUI, $"{nameof(GameObject)} reference is missing on '{gameObject.name}'");
            Assert.IsNotNull(duelUI, $"{nameof(GameObject)} reference is missing on '{gameObject.name}'");
            Assert.IsNotNull(duelReward, $"{nameof(GameObject)} reference is missing on '{gameObject.name}'");
            Assert.IsNotNull(duelScoreTitle, $"{nameof(Text)} reference is missing on '{gameObject.name}'");
            #endif
        }

        public void Hide()
        {
            recapPanel.SetActive(false);
            trainingUI.SetActive(false);
            duelUI.SetActive(false);
        }
        
        public void ShowSoloRecap(int argsPlayer1Score)
        {
            recapPanel.SetActive(true);
            trainingUI.SetActive(true);
            duelUI.SetActive(false);
        }

        public void ShowDuelRecap(int argsPlayer1Score, int argsPlayer2Score)
        {
            recapPanel.SetActive(true);
            trainingUI.SetActive(false);
            duelUI.SetActive(true);

            if (argsPlayer1Score == argsPlayer2Score)
            {
                duelScoreTitle.text = PlayerTieTitle;
            }
            else
            {
                bool player1Win = argsPlayer1Score > argsPlayer2Score;
                duelScoreTitle.text = player1Win ? PlayerWinTitle : PlayerLostTitle;
                duelReward.SetActive(player1Win);
            }
        }
    }
}