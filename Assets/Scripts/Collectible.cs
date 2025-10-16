using UnityEngine;

public class Collectible : MonoBehaviour
{
    public float rotationSpeed = 100f;
    public AudioClip collectSound;

    void Update()
    {
        // Rotar el coleccionable para hacerlo más visible
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Reproducir sonido si existe
            if (collectSound != null)
            {
                AudioSource.PlayClipAtPoint(collectSound, transform.position);
            }

            // Notificar al GameManager
            if (GameManager.Instance != null)
            {
                GameManager.Instance.CollectItem();
            }

            // Destruir el objeto
            Destroy(gameObject);
        }
    }
}