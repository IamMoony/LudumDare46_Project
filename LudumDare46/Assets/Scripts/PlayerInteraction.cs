﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Food Throwing")]
    public GameObject foodPrefab;
    public int throwForce;
    public int foodCount;
    public Text UI_FoodCount;

    [Header("Hiding")]
    public GameObject hideVFX;
    public AudioClip[] hideSFX;
    public bool isHidden;

    private bool canHide;
    private AudioSource audioSource;
    private SpriteRenderer sr;
    private Animator animator;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        audioSource = GetComponent<AudioSource>();
        sr = GetComponentInChildren<SpriteRenderer>();
        UI_FoodCount.text = foodCount + "";
    }

    private void Update()
    {
        if (foodCount > 0)
        {
            if (Input.GetButtonDown("Throw"))
            {
                Vector3 mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10f));
                Vector2 direction = (mousePosition - transform.position).normalized;
                GameObject food = Instantiate(foodPrefab, (Vector2)transform.position + Vector2.up * 0.25f, Quaternion.identity);
                food.GetComponent<Rigidbody2D>().AddForce(direction * throwForce);
                foodCount--;
                UI_FoodCount.text = foodCount + "";
                animator.SetTrigger("Throw");
                if (isHidden)
                    Hide(false);
            }
        }
        if (canHide)
        {
            if (Input.GetAxisRaw("Vertical") > 0.1f)
            {
                if (!isHidden)
                {
                    Hide(true);
                }
            }
            if (Input.GetAxisRaw("Vertical") < -0.1f)
            {
                if (isHidden)
                {
                    Hide(false);
                }
            }
        }
    }

    void Hide(bool state)
    {
        Color newColor = sr.color;
        newColor.a = state ? 0.5f : 1f;
        sr.color = newColor;
        Destroy(Instantiate(hideVFX, transform.position, Quaternion.identity), 1f);
        audioSource.PlayOneShot(hideSFX[Random.Range(0, hideSFX.Length - 1)], 0.25f);
        isHidden = state;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "HidingSpot")
            canHide = true;
        if (collision.tag == "Food")
        {
            if (collision.GetComponent<Rigidbody2D>().gravityScale == 0)
            {
                Destroy(collision.gameObject);
                foodCount++;
                UI_FoodCount.text = foodCount + "";
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "HidingSpot")
        {
            if (isHidden)
            {
                Hide(false);
            }
            canHide = false;
        }
    }
}
