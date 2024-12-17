using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class Player : MonoBehaviour
{
    // CONTROLLER
    private PlayerController playerController;

    // RIGIDBODY
    private Rigidbody rb;

    // MOVEMENT
    [SerializeField] private float speed = 10f;
    [SerializeField] private float rotationSpeed = 3f; // forward rotate speed

    // PLANET
    //[SerializeField] private Transform planet; 
    //[SerializeField] private float surfaceDistance = 1.5f;
    [SerializeField] private FauxGravityAttractor attractor;


    // HEALTH
    [SerializeField] private float maxhealth = 100f;
    public float currentHealth = 50f;
    public float bonusHealth = 50f;

    private float healthBonus = 50f;  // Amount of health to add on quest completion

    // SCORE MANAGER 
    [SerializeField] private ScoreManager scoreManager;

    // UI
    [SerializeField] private TextMeshProUGUI pressText;
    [SerializeField] private TextMeshProUGUI spaceText;
    [SerializeField] private TextMeshProUGUI torestartText;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private Image questProgressBar;
    [SerializeField] private Image questProgressBar2;
    [SerializeField] private Image timerBanner;


    // AUDIO
    [SerializeField] private AudioSource music;


    private void Start()
    {
        // RB
        rb = GetComponent<Rigidbody>();
        //rb.useGravity = false;

        
        // CONTROLLER
        playerController = GetComponent<PlayerController>();

        // TEXT
        pressText.gameObject.SetActive(false);
        spaceText.gameObject.SetActive(false);
        torestartText.gameObject.SetActive(false);

        if (attractor != null)
        {
            // Usa PlaceOnSurface per mettere il player sulla superficie del pianeta
            attractor.PlaceOnSurface(rb);
        }
    }

    
    private void Update()
    {
        if (currentHealth <= 0)
        {
            scoreManager.PlayerDied();
        }
    }

    private void FixedUpdate()
    {
        // attractor.Attract(rb);
        //AlignWithPlanetSurface();
        Move(); 
    }


    // MOVEMENT

    //private void Move()
    //{
    //    Vector2 input = playerController.GetMovement();


    //    Vector3 movementDirection = (transform.right * input.x) + (transform.forward * input.y);

    //    // if there is input, rotate the object in the direction it is moving.
    //    if (movementDirection.magnitude > 0.1f) // avoid rotating if there is no input
    //    {

    //        Quaternion targetRotation = Quaternion.LookRotation(movementDirection.normalized, transform.up);

    //        rb.rotation = Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
    //    }


    //    Vector3 forwardMovement = transform.forward * speed * Time.fixedDeltaTime;


    //    rb.MovePosition(rb.position + forwardMovement);


    //}

    //private void AlignWithPlanetSurface()
    //{
    //    Vector3 directionToPlanet = (transform.position - planet.position).normalized;

    //    // Raycast to find the surface of the planet
    //    RaycastHit hit;
    //    if (Physics.Raycast(transform.position, -directionToPlanet, out hit))
    //    {
    //        // Set the player's position slightly above the surface
    //        Vector3 targetPosition = hit.point + directionToPlanet * surfaceDistance;
    //        rb.MovePosition(targetPosition);

    //        // Align the car's up direction with the surface normal
    //        Vector3 surfaceNormal = hit.normal;

    //        // Adjust the car's forward direction to stay parallel with the surface
    //        Vector3 forwardAlongSurface = Vector3.Cross(transform.right, surfaceNormal).normalized;

    //        // Create a rotation that aligns the car's up direction with the surface normal
    //        // and keeps the car's forward direction parallel to the ground
    //        Quaternion targetRotation = Quaternion.LookRotation(forwardAlongSurface, surfaceNormal);

    //        rb.rotation = Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);

    //    }
    //}


    private void Move() // quello giusto
    {
        Vector2 input = playerController.GetMovement();
        Vector3 movementDirection = (transform.right * input.x) + (transform.forward * input.y);

        if (movementDirection.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(movementDirection.normalized, transform.up);
            rb.rotation = Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
        }

        Vector3 forwardMovement = transform.forward * speed * Time.fixedDeltaTime;


        rb.MovePosition(rb.position + forwardMovement);

    }


    private void OnEnable()
    {
        DamageEvent.OnDamage += TakeDamage;
        ScoreManager.OnQuestCompleted += AddHealthBonus;
    }

    private void OnDisable()
    {
        DamageEvent.OnDamage -= TakeDamage;
        ScoreManager.OnQuestCompleted -= AddHealthBonus;
    }

    private void TakeDamage(float damage)
    {
        currentHealth -= damage;
        Debug.Log("danno ricevuto : " + damage + ", salute attuale : " + currentHealth);

        if (currentHealth <= 0)
        {
            scoreManager.PlayerDied();
            GameManager.Instance.PlayerDied();
            Destroy(gameObject);

            // UI
            pressText.gameObject.SetActive(true);
            spaceText.gameObject.SetActive(true);
            torestartText.gameObject.SetActive(true);
            scoreManager.gameObject.SetActive(false);
            questProgressBar.gameObject.SetActive(false);
            questProgressBar2.gameObject.SetActive(false);
            timerText.gameObject.SetActive(false);
            timerBanner.gameObject.SetActive(false);

            // AUDIO
            music.Stop();

        }
        if (currentHealth <= 50f)
        {
            scoreManager.HealthBonusImage.SetActive(false);
        }

    }


    private void AddHealthBonus()
    {
        currentHealth += healthBonus;
        currentHealth = MathF.Min(currentHealth, maxhealth);

    }

}

