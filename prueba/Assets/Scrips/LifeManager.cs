using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LifeManager : MonoBehaviour
{
    public static LifeManager Instance;

    private int ingredientesRestantes = 4; // Comienza en 4 e irá bajando hasta 0
    private bool isCounting = true; // Controla si el contador está activo

    public Text textoIngredientes; // Referencia al texto UI que muestra los ingredientes restantes

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        ReiniciarContador();
    }

    public void PerderIngrediente()
    {
        if (isCounting && ingredientesRestantes > 0)
        {
            ingredientesRestantes--;
            ActualizarTextoIngredientes();
            Debug.Log("Ingrediente perdido. Restantes: " + ingredientesRestantes);
        }

        if (ingredientesRestantes <= 0)
        {
            Debug.Log("No quedan ingredientes restantes.");
        }
    }

    public void ReiniciarContador()
    {
        ingredientesRestantes = 4;
        isCounting = false;  //  Bloquea la resta de vidas justo después del reinicio
        ActualizarTextoIngredientes();
        Debug.Log("Contador de ingredientes reiniciado.");

        StartCoroutine(ActivarConteo());
    }

    private IEnumerator ActivarConteo()
    {
        yield return new WaitForSeconds(0.5f); //  Pequeño delay para evitar pérdida inmediata
        isCounting = true;
    }

    public int GetIngredientesRestantes()
    {
        return ingredientesRestantes;
    }

    private void ActualizarTextoIngredientes()
    {
        if (textoIngredientes != null)
        {
            textoIngredientes.text = ingredientesRestantes.ToString();
        }
    }
}