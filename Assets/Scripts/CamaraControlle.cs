using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamaraController : MonoBehaviour
{
    public GameObject jugador;
    private Vector3 offset;
    private Quaternion rotacionInicial;

    void Start()
    {
        // Guarda el desplazamiento entre cámara y jugador
        offset = transform.position - jugador.transform.position;

        // Guarda la rotación inicial de la cámara
        rotacionInicial = transform.rotation;
    }

    void LateUpdate()
    {
        // Actualiza solo la posición (sigue al jugador)
        transform.position = jugador.transform.position + offset;

        // Mantiene fija la rotación (sin girar con el jugador)
        transform.rotation = rotacionInicial;
    }
}
