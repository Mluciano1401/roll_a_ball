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
        // Guarda el desplazamiento entre c�mara y jugador
        offset = transform.position - jugador.transform.position;

        // Guarda la rotaci�n inicial de la c�mara
        rotacionInicial = transform.rotation;
    }

    void LateUpdate()
    {
        // Actualiza solo la posici�n (sigue al jugador)
        transform.position = jugador.transform.position + offset;

        // Mantiene fija la rotaci�n (sin girar con el jugador)
        transform.rotation = rotacionInicial;
    }
}
