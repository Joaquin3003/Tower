using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ingredient : MonoBehaviour
{
    private float min_x = -1.75f, max_x = 1.75f;
    public bool canMove;
    private float move_Speed = 2f;

    private Rigidbody2D ingrediente;
    private bool gameOver;
    private bool ignoreCollision;
    private bool ignoreTrigger;
    private bool hasLanded = false;
    private void Awake()
    {
        ingrediente = GetComponent<Rigidbody2D>();
        ingrediente.gravityScale = 0f;
    }

    // Start is called before the first frame update
    void Start()
    {
        canMove = true;
        if(Random.Range(0, 2) > 0)
        {
            move_Speed *= -1f;
        }
        GameplayController.instance.currentIngredient = this;
    }

    // Update is called once per frame
    void Update()
    {
        MoveIngredient();
    }
    void MoveIngredient()
    {
        if (canMove && !GameplayController.instance.isFrozen)
        {
            Vector3 temp = transform.position;
            temp.x += move_Speed * Time.deltaTime;

            if (temp.x > max_x)
            {
                temp.x = max_x;
                move_Speed *= -1f;
            }
            else if (temp.x < min_x)
            {
                temp.x = min_x;
                move_Speed *= -1f;
            }

            transform.position = temp;
        }
    }

    public void SoltarIngrediente()
    {
        if (Time.timeScale == 0f) return;

        canMove = false;
        ingrediente.gravityScale = 1f;
    }

    public void OnLanded()
    {
        if (gameOver || hasLanded) return;

        hasLanded = true; //nos aseguramos que solo se ejecute una vez
        ignoreCollision = true;
        ignoreTrigger = true;

        AudioManager.Instance.PlaySound(AudioManager.Instance.fallIngredientSound); // Solo se reproduce una vez

        GameplayController.instance.SpawnNewIngredient();
        GameplayController.instance.CheckTowerHeight();
        GameplayController.instance.AddScore(10);
    }

    void RestartGame()
    {
        GameplayController.instance.RestartGame();
    }

    private void OnCollisionEnter2D(Collision2D target)
    {
        if (ignoreCollision || hasLanded) return;

        if (target.gameObject.CompareTag("Base") || target.gameObject.CompareTag("ingredientes"))
        {
            Invoke("OnLanded", 0.1f);
            ignoreCollision = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D target)
    {
        if (ignoreTrigger) return;

        if(target.tag == "Borde")
        {
            CancelInvoke("OnLanded");
            Destroy(gameObject);
            ignoreTrigger = true;
        }
    }
}
