using UnityEngine;
using System.Collections;

public class MovimentoJogador : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private CharacterController Controller;
    private Transform myCamera;
    private Animator Animator;
    private bool chao = true;
    private float forcay;
    public dano dano;
    private bool dash = false;
    private bool espera = true;
    [SerializeField] private Transform pe;
    [SerializeField] private LayerMask colisaol;
    public bool estaNoChao = true;
    public Transform feetPosition; // Objeto vazio no pé do personagem
    public LayerMask groundLayer;  // Camada do chão
    public float rayDistance = 0.3f;
    Vector3 movimentoo = new Vector3(0, 0, 2);

    bool esspera = false;
    void Start()
    {
        
        Controller = GetComponent<CharacterController>();
        myCamera = Camera.main.transform;
        Animator = GetComponent<Animator>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 movimento = movimentoo;


        if (dash == false)
        {


            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");
            movimento = new Vector3(horizontal, 0, vertical);
        }
        else
        {
            if(movimento.z > 0)
            {

                movimentoo.z -= 0.03f;
                movimentoo.y = 0;
            }
            
        }
            estaNoChao = Physics.Raycast(feetPosition.position, Vector3.down, rayDistance, groundLayer);


        movimento = myCamera.TransformDirection(movimento);

        movimento.y = 0;
        
        if (espera || esspera)
        {
            Controller.Move(movimento * Time.deltaTime * 5);
        }




        if (movimento != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(movimento), Time.deltaTime * 10);
        }

        Animator.SetBool("Movendo", movimento != Vector3.zero);
        
        Animator.SetBool("Chao", estaNoChao);
        if (Input.GetKeyDown(KeyCode.Space) && estaNoChao && espera)
        {
            forcay = 9f;
            Animator.SetTrigger("Pulo");
        }
        if (forcay > -9.81f)
        {
            forcay += -9.81f * Time.deltaTime;
        }
        Controller.Move(new Vector3(0, forcay, 0) * Time.deltaTime);
        
        if (Input.GetMouseButtonDown(0) && estaNoChao && espera)
        {
            Animator.SetTrigger("Soco");
            

        }
        if (Input.GetKeyDown(KeyCode.F) && espera && estaNoChao)
        {
            Animator.SetBool("Defesa", true);
            espera = false;
        }
        if (Input.GetKeyUp(KeyCode.F))
        {
            Animator.SetBool("Defesa", false);

            espera = true;
        }
        if (Input.GetKeyDown(KeyCode.Q) && espera && esspera == false)
        {
            
            esspera = true;
            movimentoo = new Vector3(0, 0, 4);
            Animator.SetTrigger("Dash");
            dash = true;
            StartCoroutine(Dado());

        }
    }
    IEnumerator Dado()
    {
        yield return new WaitForSeconds(0.6f);
        dash = false;
        
        movimentoo = new Vector3(0, 0, 4);
        Animator.speed = 2.0f;
    }
    void Soco()
    {
        
        esspera = false;
        Animator.speed = 1.0f;
        dano.recebi();
    }
   
}
