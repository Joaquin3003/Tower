using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.EventSystems;

public class Spawner : MonoBehaviour
{
    public GameObject[] ingredientPrefabs;
    private float zOffset = 0f;
    private Ingredient currentIngredient;
    public float minX = -2f;
    public float maxX = 2f;

    public void SpawnIngredient()
    {
        GameObject ingredient = ingredientPrefabs[Random.Range(0, ingredientPrefabs.Length)];
        GameObject ingrediente = Instantiate(ingredient);
        zOffset -= 0.1f;

        // Elegir posición X según si la habilidad está activa o no
        float spawnX = GameplayController.instance.isFrozen
            ? GameplayController.instance.cervezaFrozenX
            : Random.Range(minX, maxX);

        ingrediente.transform.position = new Vector3(spawnX, transform.position.y, transform.position.z + zOffset);
        currentIngredient = ingrediente.GetComponent<Ingredient>();
    }
}
