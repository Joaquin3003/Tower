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
    
    public void SpawnIngredient()
    {
        GameObject ingredient = ingredientPrefabs[Random.Range(0, ingredientPrefabs.Length)];
        GameObject ingrediente = Instantiate(ingredient);
        zOffset -= 0.1f;
        ingrediente.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + zOffset);
        currentIngredient = ingrediente.GetComponent<Ingredient>();
    }
}
