using UnityEngine;

public class InimigoScript : MonoBehaviour
    
{
    private Animator animator;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
  

    // Update is called once per frame
    void Update()
    {
        
    }
    void Start()
    {
        animator = GetComponent<Animator>();    
    }
    private void OnCollisionEnter(Collision collision)
    {
        // Verifica se o objeto tocado tem a tag "Inimigo"
        if (collision.gameObject.CompareTag("Soco"))
        {
            Debug.Log("Bati em um inimigo!");
            animator.SetTrigger("Hit");
        }
    }
}
