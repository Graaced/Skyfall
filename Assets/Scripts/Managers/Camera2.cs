using UnityEngine;

public class Camera2 : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private Transform planet;
    [SerializeField] private Vector3 offset;
    [SerializeField] private Vector3 rotationOffset;
    [SerializeField] private float followSpeed = 0.1f;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private float radius = 10f; // Distance between camera and planet

    private Vector3 velocity = Vector3.zero;
    private Camera cameraComponent;

    void Start()
    {
        if (planet == null)
        {
            GameObject planetObject = GameObject.FindGameObjectWithTag("Planet");
            if (planetObject != null)
            {
                planet = planetObject.transform;
            }
        }

        // Initialize the Camera component
        cameraComponent = GetComponent<Camera>();
    }

    void LateUpdate()
    {
        FollowPlayer();
        AlignCameraWithPlayer();
        AdjustFieldOfView(); // Dynamically adjust FOV based on player's speed
    }

    private void FollowPlayer()
    {
        if (player == null)
        {
            return;
        }

        // Calcola la posizione target della telecamera, applicando l'offset al giocatore
        Vector3 targetPosition = player.position + player.TransformDirection(offset);

        // Usa un Raycast per evitare ostacoli tra la telecamera e il giocatore
        RaycastHit hit;
        if (Physics.Raycast(player.position, transform.position - player.position, out hit, radius))
        {
            // Se c'è un ostacolo, posiziona la telecamera al punto di collisione
            targetPosition = hit.point;
        }

        // SmoothDamp per un movimento fluido della telecamera verso la posizione target
        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
    }

    private void AlignCameraWithPlayer()
    {
        if (player == null)
        {
            return;
        }

        // Calcola la direzione verso il giocatore
        Vector3 directionToPlayer = player.position - transform.position;

        // Calcola la rotazione target
        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer, player.up) * Quaternion.Euler(rotationOffset);

        // Usa Slerp per una rotazione più fluida della telecamera
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    private void AdjustFieldOfView()
    {
        // Calcola la velocità del giocatore (se usa un Rigidbody)
        float playerSpeed = player.GetComponent<Rigidbody>().velocity.magnitude;

        // Modifica il FOV dinamicamente in base alla velocità del giocatore
        cameraComponent.fieldOfView = Mathf.Lerp(60f, 90f, playerSpeed / 10f);
    }
}

