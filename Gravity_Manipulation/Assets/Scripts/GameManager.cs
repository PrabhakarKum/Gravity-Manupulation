using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField] private float startDelay = 3f;
    [SerializeField] private float totalTime = 120f;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI gameClockText;
    [SerializeField] private TextMeshProUGUI objectiveText;
    [SerializeField] private CanvasGroup fadeGroup;
    [SerializeField] private PlayerManager playerManager;
    [SerializeField] private InteractController interactController;

    private float timeElapsed;
    private float gameClock;
    
    private void Start()
    {
        gameClock = totalTime;
        timeElapsed = startDelay;
        gameClockText.text = totalTime.ToString();
    }
    
    private void Update()
    {
        
        if (timeElapsed > 0)
        {
            timeElapsed -= Time.deltaTime;
            timerText.text = Mathf.Ceil(timeElapsed).ToString();
            fadeGroup.alpha = Mathf.Sqrt(Mathf.Sqrt(timeElapsed / startDelay));
            playerManager.canMove = false;
        }
        else
        {
            timerText.gameObject.SetActive(false);
            objectiveText.gameObject.SetActive(false);
            fadeGroup.gameObject.SetActive(false);
            playerManager.canMove = true;
            
            if (gameClock > 0)
            {
                gameClock -= Time.deltaTime;
                gameClockText.text = Mathf.Ceil(gameClock).ToString();
            }
            
            if (gameClock <= 15)
            {
                gameClockText.color = Color.red;
            }
            
            if (gameClock <= 0)
            {
                interactController.canInteract = false;
                GameOver.Instance.GameEnd(0, interactController.points);
            }
        }
    }
    
    
}
