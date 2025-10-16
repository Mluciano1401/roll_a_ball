using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JugadorController : MonoBehaviour
{
    private Rigidbody rb;
    private int contador;

    public Text textoContador;
    public Text textoGanar;
    public float velocidad = 10f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        contador = 0;

        // Verificar que los textos están asignados
        if (textoContador == null || textoGanar == null)
        {
            Debug.LogError("⚠️ Asigna los textos 'textoContador' y 'textoGanar' en el Inspector.");
            enabled = false;
            return;
        }

        textoGanar.text = "";
        setTextoContador();
    }

    void FixedUpdate()
    {
        float movimientoH = Input.GetAxis("Horizontal");
        float movimientoV = Input.GetAxis("Vertical");

        Vector3 movimiento = new Vector3(movimientoH, 0.0f, movimientoV);
        rb.AddForce(movimiento * velocidad);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Coleccionable"))
        {
            other.gameObject.SetActive(false);
            contador++;
            setTextoContador();
        }
    }

    void Update()
    {
        // Reiniciar posición si cae del mapa
        if (transform.position.y < -10f)
        {
            GameManager.Instance?.RestartLevel();
        }
    }

    void setTextoContador()
    {
        textoContador.text = "Contador: " + contador.ToString();

        if (contador >= 12)
        {
            textoGanar.text = "¡Ganaste!";
        }
    }
}
