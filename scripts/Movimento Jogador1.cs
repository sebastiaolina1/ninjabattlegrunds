using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;




public class MovimentoJogador2 : MonoBehaviour
{

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private CharacterController Controller;
    public Transform myCamera;
    private Animator Animator;
    private bool chao = true;
    private float forcay;
    public dano dano;
    bool cam;
    public Transform onbto;
    public dano1 danof;
    private bool dash = false;
    private bool espera = true;
    [SerializeField] private Transform pe;
    [SerializeField] private LayerMask colisaol;
    public bool estaNoChao = true;
    public Transform feetPosition; // Objeto vazio no pé do personagem
    public LayerMask groundLayer;  // Camada do chão
    public float rayDistance = 0.3f;
    Vector3 movimentoo = new Vector3(0, 0, 2);
    bool timee = true;
    bool defesa = false;
    bool esspera = false;
    int vida = 100;
    int vidat = 100;
    bool oi = true;
    bool w = false;
    float horizontal = 0;
    float vertical = 0;
    public Image barra1;
    public Image barra2;
    private Rigidbody rb;
    public float forca = 10f;
    int slep = 0;
    [SerializeField] Player a;
    public Transform vcam;
    public Transform novoInimigo;

    // Para trocar o foco para o adversário
    public void TravarMira()
    {
        
            vcam.LookAt(novoInimigo); // A propriedade agora é apenas LookAt (sem o m_)
    }

    // Para remover o foco e voltar a olhar para frente
    public void LiberarMira()
    {
        if (vcam != null)
            vcam.LookAt(onbto);
    }
    void Start()
    {
        TravarMira();
        Controller = GetComponent<CharacterController>();
        rb = GetComponent<Rigidbody>();
        Animator = GetComponent<Animator>();
        
    }


   
    // Update is called once per frame
    void Update()
    { 
     

        Vector3 movimento = movimentoo;
        // Percorre todos os botões possíveis do joystick (0 a 19)

        vertical = Input.GetAxis("Lado");
        horizontal = Input.GetAxis("Baixo");

        if (dash == false)
        {




            movimento = new Vector3(horizontal, 0, vertical);
        }
        else
        {
            if (movimento.z > 0)
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


        if (Input.GetButtonDown("Pulo") && estaNoChao && espera)
        {
            forcay = 9f;
            Animator.SetTrigger("Pulo");
        }
        if (forcay > -9.81f)
        {
            forcay += -9.81f * Time.deltaTime;
        }
        Controller.Move(new Vector3(0, forcay, 0) * Time.deltaTime);

        if (Input.GetButtonDown("Soco") && estaNoChao && espera)
        {
            if (slep == 3)
            {
                slep = 0;
                Animator.SetTrigger("Soco3");
                espera = false;

            }
            else
            {
                slep += 1;
                espera = false;
                Animator.SetTrigger("Soco");
            }

        }

        if (Input.GetButtonDown("Defesa") && espera && estaNoChao)
        {
            Animator.SetBool("Defesa", true);
            espera = false;
            defesa = true;
        }
        if (Input.GetButtonUp("Defesa"))
        {
            Animator.SetBool("Defesa", false);
            defesa = false;
            espera = true;
        }
        if (Input.GetButtonDown("Dash") && espera && esspera == false)
        {

            esspera = true;
            movimentoo = new Vector3(0, 0, 4);
            Animator.SetTrigger("Dash");
            dash = true;
            StartCoroutine(Dado());

        }
        if (vida <= 0 && oi)
        {
            Animator.SetTrigger("Morte");
            Debug.Log("MORREU ");
            vida = 100;
            barra1.fillAmount = (float)vida / vidat;
            StartCoroutine(Daaado());
        }
        if (Input.GetButtonDown("Cam"))
        {
            Debug.Log("oiiiiiiiiiiiiiiiiii");
            if (cam)
            {
                cam = false;
                TravarMira();
            }
            else
            {
                LiberarMira();
                cam = true;
            }

        }
        
        
        
    }
    void OnTriggerEnter(Collider hit)
    {


        if (hit.gameObject.CompareTag("Caixa") && timee)
        {
            Debug.Log("Colidiu com um Box Collider: " + hit.name);

            // Descobre qual Player é dono da hitbox que bateu
            

            
            if (defesa == false)
            {
                // Faz o personagem atingido olhar para quem deu o soco
                /*
                Vector3 posicaoAlvo = atacante.transform.position;
                posicaoAlvo.y = transform.position.y;

                transform.LookAt(posicaoAlvo);

                espera = false;
                Animator.SetTrigger("Hit");

                vida -= 4;
                barra1.fillAmount = (float)vida / vidat;

                StartCoroutine(Daaaado());

                Debug.Log("vida " + vida);

                StartCoroutine(S());
                */
            }
            else
            {
                Animator.SetTrigger("Soco");
                StartCoroutine(Daaaaado());
            }
        }

        if (hit.gameObject.CompareTag("Caixa3") && timee)
        {
            Debug.Log("Colidiu com um Box Collider: " + hit.name);

            if (defesa == false)
            {
                espera = false;
                Vector3 posicaoAlvo = a.transform.position;
                posicaoAlvo.y = transform.position.y;
                transform.LookAt(posicaoAlvo);
                Animator.SetTrigger("Hit2");
                vida -= 20;

            }
            else
            {
                Animator.SetTrigger("Soco");
                slep = 1;
                StartCoroutine(Daaaaado());
            }

        }
        
    }

    IEnumerator Dado()
    {
        yield return new WaitForSeconds(0.6f);
        dash = false;

        movimentoo = new Vector3(0, 0, 4);
        StartCoroutine(Dash());

    }
    void Soco0()
    {
        dano.recebi();
    }
    IEnumerator S()
    {

        yield return new WaitForSeconds(1f);
        espera = true;

    }
    IEnumerator Dash()
    {
        yield return new WaitForSeconds(0.6f);

        esspera = false;

    }
    IEnumerator Daaaado()
    {
        yield return new WaitForSeconds(1.3f);
        barra2.fillAmount = (float)vida / vidat;

    }
    IEnumerator Daado()
    {
        timee = false;
        yield return new WaitForSeconds(0.6f);
        timee = true;

    }
    IEnumerator Daaado()
    {
        oi = false;
        yield return new WaitForSeconds(3f);
        oi = true;

    }
    void Soco()
    {

        esspera = false;
        StartCoroutine(Dassh());
        dano.recebi();
    }
    IEnumerator Daaaaado()
    {
        a.Stun();
        yield return new WaitForSeconds(2.3f);
        a.Stun0();

    }
    public void Stun()
    {
        espera = false;
        esspera = true;
        Animator.SetTrigger("Hit");
    }
    public void Stun0()
    {
        espera = true;
        espera = true;
    }

    void Soco3()
    {
        danof.recebi();
        espera = true;
        StartCoroutine(Dassh());
    }
    IEnumerator Dassh()
    {
        yield return new WaitForSeconds(0.6f);

        espera = true;
    }
}
