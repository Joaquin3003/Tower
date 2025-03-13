using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // El objeto que la cámara sigue (el SpawnPoint)
    public float smoothSpeed = 0.125f; // Velocidad de seguimiento
    public float offsetY = 2f; // Desplazamiento vertical de la cámara respecto al target

    void LateUpdate()
    {
        if (target != null)
        {
            Vector3 newPosition = transform.position;
            newPosition.y = Mathf.Lerp(transform.position.y, target.position.y + offsetY, smoothSpeed);
            transform.position = newPosition;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // Elimina ingredientes que salgan de la pantalla
        if (other.gameObject.CompareTag("ingredientes"))
        {
            Destroy(other.gameObject);
        }
    }
}
