using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Serialization;

public class GameOver : MonoBehaviour
{
    [SerializeField] GameObject gameOverPanel;
    public static GameOver Instance;
    public static bool gameEnded = false;
    
    private void Awake()
    {
        // Implement singleton pattern
        if (Instance == null)
        {
            Instance = this;
            gameEnded = false;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    // Trigger the end game sequence with different messages based on the state and points scored
    public void GameEnd(int state, int pointsScored)
    {
        if (!gameEnded)
        {
            // State 0: Successful completion with varying ending messages
            if (state == 0)
            {
                gameOverPanel.SetActive(true);
                // Display points collected
                gameOverPanel.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text =
                    "You collected " + pointsScored + " out of 5 cubes.";

                TextMeshProUGUI ending = gameOverPanel.transform.GetChild(1).GetComponent<TextMeshProUGUI>();

                if (pointsScored == 5)
                {
                    ending.text = "You Win!.";
                }
                else
                {
                    ending.text = "Game over!.";
                }

                gameEnded = true;
            }
            // State 1: Suit out of signal
            else if (state == 1)
            {
                gameOverPanel.SetActive(true);
                gameOverPanel.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text =
                    "Game over!.";
                gameOverPanel.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text =
                    "You Lose!.";
                gameEnded = true;
            }
        }
    }
}
