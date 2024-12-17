using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ScoreManager : MonoBehaviour
{
    // TRANSFORM
    [SerializeField] private Transform planet;
    [SerializeField] private float scaleDecreaseAmount = 10f;
    [SerializeField] private float minScale = 20f;
    [SerializeField] private float shrinkDuration = 2f;  //how long does the shrinking of the planet last 
    private Vector3 targetScale;
   
    public bool isShrinking = false;
    private float shrinkTimer = 0f;

    // UI 
    [SerializeField] private TextMeshProUGUI timerText;  // Text UI component reference
    [SerializeField] private TextMeshProUGUI healthBonusText; // Text UI component reference
    [SerializeField] private TextMeshProUGUI shrinkText;  
    [SerializeField] private Image questProgressBar;
    [SerializeField] private Image healthBonus;
    [SerializeField] private Image bannerText;
    [SerializeField] private Image shrinkBanner;

    public GameObject HealthBonusImage => healthBonus.gameObject;

    // TIMERS
    private float scoreTime = 12f;
    private float scoreTimer;
    private bool isPlayerDead = false;

    // TIMERS UI 
    private float bonusDisplayTime = 3f; 
    private float bonusTimer; 
    private bool isBonusActive = false; 

    // QUEST 
    private float survivalTime = 0f; // Tracks how long the player has survived
    private bool questCompleted = false;  
    private float questGoalTime = 10f;

    // EVENT
    public static event Action OnQuestCompleted;

    // Start is called before the first frame update
    void Start()
    {
        scoreTimer = scoreTime;
        healthBonus.gameObject.SetActive(false);
        healthBonusText.gameObject.SetActive(false);
        bannerText.gameObject.SetActive(false);
        shrinkText.gameObject.SetActive(true);
        shrinkBanner.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        // to stop the timer when planet reach the minScale
        if (planet.localScale.x <= minScale)
        {
            
            return;
        }

        if (!isShrinking && !isPlayerDead) 
        {
            scoreTimer -= Time.deltaTime;

            UpdateTimerUI(scoreTimer);// update canva timer
            

            if (scoreTimer < 0)
            {               
                //scoreTimer = scoreTime;
                ShrinkPlanet();
                shrinkBanner.gameObject.SetActive(true);
                shrinkText.gameObject.SetActive(false); 

            }

        }

        // Update the player's survival time
        survivalTime += Time.deltaTime;

        // Check if the quest has been completed
        if (!questCompleted)
        {
            UpdateQuest();
        }



        // Update the scale gradually if the planet is shrinking
        if (isShrinking)
        {
            planet.localScale = Vector3.Lerp(planet.localScale, targetScale, Time.deltaTime);

            // scale has been reached, stop reduction
            if (Vector3.Distance(planet.localScale, targetScale) < 0.01f)
            {
                planet.localScale = targetScale;
                isShrinking = false;
                scoreTimer = scoreTime;
                shrinkBanner.gameObject.SetActive(false);
                shrinkText.gameObject.SetActive(true); 
            }
        }

        if (isBonusActive)
        {
            bonusTimer -= Time.deltaTime;

            if (bonusTimer <= 0)
            {             
                healthBonusText.gameObject.SetActive(false);
                bannerText.gameObject.SetActive(false);
                isBonusActive = false; 
            }
        }


    }


    
    private void ShrinkPlanet()
    {
        if (planet != null)
        {
            
            float newScale = Mathf.Max(planet.localScale.x - scaleDecreaseAmount, minScale);

            // Apply the new scale only if it is not already at minimum
            targetScale = new Vector3(newScale, newScale, newScale);
            isShrinking = true;
        }
    }

    //UI
    private void UpdateTimerUI(float time) 
    {
        if (timerText != null)
        {
            int wholeTime = Mathf.Max(0, Mathf.FloorToInt(time));  // Round down to the whole number
            timerText.text = wholeTime.ToString();
        }
    }


    // QUEST
    private void UpdateQuest()
    {
        if (questProgressBar != null)
        {
            questProgressBar.fillAmount = survivalTime / questGoalTime;

            if (survivalTime >= questGoalTime)
            {
                questProgressBar.fillAmount = 1f;
                questCompleted = true;
                healthBonus.gameObject.SetActive(true);
                bannerText.gameObject.SetActive(true);
                healthBonusText.gameObject.SetActive(true);

                //Event
                OnQuestCompleted?.Invoke();

                bonusTimer = bonusDisplayTime;
                isBonusActive = true; // Imposta lo stato del bonus attivo

            }
        }
    }


    public void PlayerDied()  
    {
        isPlayerDead = true;
    }

    
}
