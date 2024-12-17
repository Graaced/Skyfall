using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpawnManager : MonoBehaviour
{
    //PLANET
    [SerializeField] private Transform planet;

    // ASTEROID
    [SerializeField] private Asteroid asteroidPrefab;
    [SerializeField] private GameObject craterPrefab;
    [SerializeField] private float spawnAsteroidTime = 1.5f;
    [SerializeField] private float destroyCreaterTime = 1.5f;
    [SerializeField] private int asteroidsAmount = 20;

    private float spawnTimer;
    private int currentPoolIndex = 0;
    private bool isActive;

    // SCORE
    [SerializeField] private ScoreManager scoreManager;

    // UI
    [SerializeField] private Image questProgressBar;


    // LIST
    private List<Asteroid> asteroidsList = new List<Asteroid>();
    private List<Asteroid> asteroidsPool = new List<Asteroid>();

    private List<GameObject> activeCraters = new List<GameObject>();
    private List<float> craterTimers = new List<float>();


    // QUEST
    [SerializeField] private int objectsToAvoidForQuest = 10; // total by astoerids and crater
    private int avoidedObjectsCount = 0;
    private bool questCompleted = false;

    // Start is called before the first frame update
    void Start()
    {
        // SET POOL
        for (int i = 0; i < asteroidsAmount; i++)
        {
            Asteroid asteroid = Instantiate(asteroidPrefab);
            asteroid.gameObject.SetActive(false);
            asteroidsPool.Add(asteroid);
        }

        spawnTimer = spawnAsteroidTime;

    }

    // Update is called once per frame
    void Update()
    {

        if (GameManager.Instance.isPlayerDead) 
        {
            return; 
        }

        // ASTEROIDS

        spawnTimer -= Time.deltaTime;

       if (spawnTimer <= 0)
       {
           SpawnAsteroids();
           spawnTimer = spawnAsteroidTime;
       }

       if (scoreManager.isShrinking)
       {
           DestroyCraters();
       }
       else
       {
           for (int i = activeCraters.Count - 1; i >= 0; i--)
           {
               craterTimers[i] -= Time.deltaTime;

               if (craterTimers[i] <= 0)
               {
                   AvoidCrater();
                   Destroy(activeCraters[i]);
                   activeCraters.RemoveAt(i);
                   craterTimers.RemoveAt(i);
               }
           }
       }     
    }



 
    // ASTEROID
    private void SpawnAsteroids()
    {
        if (scoreManager.isShrinking) 
        {
            return;
        }

        if (asteroidsList.Count >= asteroidsAmount) return;

       // Spawn an asteroid from the pool
       Asteroid asteroid = GetAsteroidFromPool();

       if (asteroid != null)
       {
          Vector3 spawnPosition = GetRandomSpawnPosition();
          asteroid.transform.position = spawnPosition;
         
          float randomSize = Random.Range(1f, 2f);
          asteroid.transform.localScale = Vector3.one * randomSize;
         
          asteroid.SetTarget(planet.position);
          asteroid.gameObject.SetActive(true);
          asteroidsList.Add(asteroid);

          // AvoidAsteroid();
       }
       
        
    }
  
    private void AvoidAsteroid()
    {
        if (!questCompleted)
        {
            avoidedObjectsCount++;
            UpdateQuest();

            if (avoidedObjectsCount >= objectsToAvoidForQuest)
            {
                questCompleted = true;
            }
        }
    }

    private Asteroid GetAsteroidFromPool()
    {
        // cycle to find the inactive asteroid
        for (int i = 0; i < asteroidsPool.Count; i++)
        {
            
            if (!asteroidsPool[currentPoolIndex].isActive)
            {
                //Update index for next access
                Asteroid asteroid = asteroidsPool[currentPoolIndex];
                currentPoolIndex = (currentPoolIndex + 1) % asteroidsPool.Count;
                return asteroid;
            }
            currentPoolIndex = (currentPoolIndex + 1) % asteroidsPool.Count;
        }

        return null;
    }

    private Vector3 GetRandomSpawnPosition()
    {
        float spawnDistance = 100f;
        Vector3 direction = Random.onUnitSphere * spawnDistance;
        return planet.position + direction;
    }


    // CRATER
    public void OnAsteroidHit(Asteroid asteroid, Collision collision)
    {
        // Position of the asteroid at the time of impact
        Vector3 hitPosition = asteroid.transform.position;

        // use GetContact to get the surface normal
        Vector3 surfaceNormal = collision.GetContact(0).normal; // need to get the first contact normal

        // use a raycast to get the exact point on the surface
        RaycastHit hit;
        if (Physics.Raycast(hitPosition, -surfaceNormal, out hit))
        {
            // Impact point on the surface
            Vector3 craterPosition = hit.point;

            Quaternion craterRotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
            GameObject crater = Instantiate(craterPrefab, craterPosition, craterRotation);          
            crater.transform.localScale = new Vector3(10, 10, 10); // to modify the crater's size
            activeCraters.Add(crater);
            craterTimers.Add(destroyCreaterTime);

            // disable asteroid
            asteroid.gameObject.SetActive(false);
            asteroid.isActive = false;
            asteroidsList.Remove(asteroid);
            asteroidsPool.Add(asteroid);

            AvoidAsteroid();
        }

    }
  
    private void DestroyCraters()
    {
        for (int i = activeCraters.Count - 1; i >= 0; i--)
        {
            Destroy(activeCraters[i]);
        }       
        activeCraters.Clear();
        craterTimers.Clear();
    }
    private void AvoidCrater()
    {
        if (!questCompleted) 
        {
            avoidedObjectsCount++;
            UpdateQuest();

            if(avoidedObjectsCount >= objectsToAvoidForQuest) 
            {
                questCompleted = true;
            }
        }
    }

    //QUEST
    private void UpdateQuest()
    {
        float progress = (float)avoidedObjectsCount / objectsToAvoidForQuest;
        questProgressBar.fillAmount = progress;
    }

   


}
