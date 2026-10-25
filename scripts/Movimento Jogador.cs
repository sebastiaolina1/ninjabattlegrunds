
using Photon.Pun;
using System;
using System.Collections;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

using UnityEngine.UI;
using UnityEngine.UIElements;
using UnityEngine.Video;
using static UnityEngine.Analytics.IAnalytic;

public class Player : MonoBehaviourPun
{
    /*=============================================================
        HITBOXES
     ==============================================================*/
    [SerializeField] private SphereCollider Hitbox_LP;
    [SerializeField] private SphereCollider Hitbox_RP;
    [SerializeField] private SphereCollider Hitbox_LF;
    [SerializeField] private SphereCollider Hitbox_RF;
    // ============================================================
    // COMPONENTES
    // ============================================================

    private CharacterController Controller;
    public Transform myCamera;
    private Animator Animator;
    public GameObject hit1;

    // ============================================================
    // FIRE / COOLDOWN
    // ============================================================

    public UnityEngine.UI.Image icone;

    public float cooldownFire = 5f;
    float tempoFire = 0f;

    private bool fireDisponivel = true;

    public UnityEngine.UI.Image rock;

    public float cooldownRock = 5f;
    float tempoRock = 0f;

    private bool rockDisponivel = true;


    // ============================================================
    // VFX
    // ============================================================

    public GameObject vfxPrefab;
    public GameObject vfxPrefab_rock;
    public float velocidadevfx = 15f;
    public Transform boca;
    public float distanciaProjetil = 1.5f;
    public GameObject spam;

    // ============================================================
    // DANO
    // ============================================================

    public dano dano;
    public dano1 danof;
    public float tempoVulto = 0f;


    // ============================================================
    // MOVIMENTO
    // ============================================================

    private bool chao = true;

    public bool estaNoChao = true;

    public Transform feetPosition;

    public LayerMask groundLayer;

    public float rayDistance = 0.3f;

    Vector3 movimentoo = new Vector3(0, 0, 2);

    private float forcay;

    float vertical = 0;
    float horizontal = 0;


    // ============================================================
    // DASH
    // ============================================================

    private bool dash = false;
    private bool dashDisponivel = true;
    private bool fazendoDash = false;


    // ============================================================
    // SOCO
    // ============================================================

    private bool socoDisponivel = true;

    int slep = 1;


    // ============================================================
    // DEFESA
    // ============================================================

    bool defesa = false;


    // ============================================================
    // OUTROS
    // ============================================================

    bool timee = true;

    bool oi = true;

    bool esspera = false;


    // ============================================================
    // ÁUDIO
    // ============================================================

    [SerializeField] private AudioSource step;


    // ============================================================
    // COLISÃO
    // ============================================================

    [SerializeField] private LayerMask colisaol;


    // ============================================================
    // CÂMERA
    // ============================================================

    [SerializeField] MovimentoJogador2 a;

    public Transform vcam;

    public Transform novoInimigo;

    public Transform onbto;


    // ============================================================
    // VIDA
    // ============================================================

    int vida = 100;

    int vidat = 100;

    public UnityEngine.UI.Image barra1;
    public UnityEngine.UI.Image barra2;


    // ============================================================
    // FÍSICA
    // ============================================================

    private Rigidbody rb;

    public float forca = 10f;


    // ============================================================
    // RASTRO
    // ============================================================

    [SerializeField] private TrailRenderer rastro;


    // ============================================================
    // MIRA
    // ============================================================

    public void LiberarMira()
    {
        if (vcam != null)
            vcam.LookAt(onbto);
    }


    public void TravarMira()
    {
        if (vcam != null)
            vcam.LookAt(novoInimigo);
    }


    // ============================================================
    // START
    // ============================================================

    void Start()
    {
        rastro.emitting = false;

        Controller = GetComponent<CharacterController>();

        Animator = GetComponent<Animator>();
        Hitbox_LP.enabled = false;
        Hitbox_RP.enabled = false;
        Hitbox_LF.enabled = false;
        Hitbox_RF.enabled = false;
        if (!photonView.IsMine)
        {
            a.enabled = false;
        }
    }


    // ============================================================
    // UPDATE
    // ============================================================

    void Update()
    {

        if (!photonView.IsMine)
            return;

        // resto do código...

        // ========================================================
        // COOLDOWN DO FIRE
        // ========================================================

        if (tempoFire > 0)
        {
            tempoFire -= Time.deltaTime;

            icone.fillAmount = 1 - (tempoFire / cooldownFire);
        }
        if (tempoRock > 0)
        {
            tempoRock -= Time.deltaTime*0.2f;

            rock.fillAmount = 1 - (tempoRock / cooldownRock);
        }

        Debug.DrawRay(
    feetPosition.position,
    Vector3.down * rayDistance,
    Color.red
);
        // ========================================================
        // ATIVAR FIRE
        // ========================================================

        if (Input.GetKeyDown(KeyCode.Alpha1) && fireDisponivel && tempoFire < 0.05f)
        {
            StartCoroutine(FIRE());
        }
        if (Input.GetKeyDown(KeyCode.Alpha2) && rockDisponivel && tempoRock < 0.05f)
        {
            StartCoroutine(ROCK());
        }


        // ========================================================
        // SAIR DO JOGO
        // ========================================================

        if (Input.GetKeyDown(KeyCode.F11))
        {
            Debug.Log("Saindo do jogo...");

            Application.Quit();
        }


        // ========================================================
        // MIRA
        // ========================================================

        TravarMira();


        // ========================================================
        // MOVIMENTO
        // ========================================================

        Vector3 movimento = movimentoo;


        if (Input.GetKey(KeyCode.W))
        {
            vertical = 1;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            vertical = -1;
        }
        else
        {
            vertical = 0;
        }


        if (Input.GetKey(KeyCode.A))
        {
            horizontal = -1;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            horizontal = 1;
        }
        else
        {
            horizontal = 0;
        }


        // ========================================================
        // MOVIMENTO NORMAL / DASH
        // ========================================================

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


        // ========================================================
        // VERIFICAR CHÃO
        // ========================================================

        estaNoChao = Physics.Raycast(
            feetPosition.position,
            Vector3.down,
            rayDistance,
            groundLayer
        );


        movimento = myCamera.TransformDirection(movimento);

        movimento.y = 0;


        // ========================================================
        // MOVIMENTO
        // ========================================================

        // FIRE impede o movimento durante sua execução.
        // O dash continua funcionando normalmente.

        if (fireDisponivel)
        {
            Controller.Move(movimento * Time.deltaTime * 5);
        }

        if (rockDisponivel)
        {
            Controller.Move(movimento * Time.deltaTime * 5);
        }


        // ========================================================
        // ROTAÇÃO
        // ========================================================

        if (movimento != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(movimento),
                Time.deltaTime * 10
            );
        }


        // ========================================================
        // ANIMAÇÕES
        // ========================================================

        Animator.SetBool("Movendo", movimento != Vector3.zero);

        Animator.SetBool("Chao", estaNoChao);


        // ========================================================
        // PULO
        // ========================================================

        if (Input.GetKeyDown(KeyCode.Space) &&
            estaNoChao &&
            fireDisponivel)
        {
            forcay = 9f;

            Animator.SetTrigger("Pulo");
        }


        if (forcay > -9.81f)
        {
            forcay += -9.81f * Time.deltaTime;
        }


        Controller.Move(
            new Vector3(0, forcay, 0) * Time.deltaTime
        );


        // ========================================================
        // MOUSE
        // ========================================================

        if (Input.GetKeyDown(KeyCode.Z))
        {
            UnityEngine.Cursor.lockState = CursorLockMode.Locked;
            UnityEngine.Cursor.visible = false;
        }


        // ========================================================
        // SOCO
        // ========================================================

        if (Input.GetMouseButtonDown(0) &&
            estaNoChao &&
            socoDisponivel &&
            fireDisponivel)
        {
            Debug.Log(slep);

            socoDisponivel = false;


            if (slep == 1)
            {
                slep += 1;

                Animator.SetTrigger("Soco1");


            }
            else if (slep == 2)
            {
                slep++;

                Animator.SetTrigger("Soco2");
            }
            else if (slep == 3)
            {
                slep++;

                Animator.SetTrigger("Soco3");
            }
            else if (slep == 4)
            {
                slep++;

                Animator.SetTrigger("Soco4");
            }
            else if (slep == 5)
            {
                slep = 1;

                Animator.SetTrigger("Soco5");
            }
        }


        // ========================================================
        // DEFESA
        // ========================================================

        if (Input.GetKeyDown(KeyCode.F) &&
            estaNoChao &&
            fireDisponivel)
        {
            Animator.SetBool("Defesa", true);

            defesa = true;
        }


        if (Input.GetKeyUp(KeyCode.F))
        {
            Animator.SetBool("Defesa", false);

            defesa = false;
        }


        // ========================================================
        // DASH
        // ========================================================

        if (Input.GetKeyDown(KeyCode.Q) &&
            dashDisponivel &&
            esspera == false &&
            fireDisponivel)
        {
            dashDisponivel = false;

            esspera = true;

            movimentoo = new Vector3(0, 0, 4);

            Animator.SetTrigger("Dash");

            dash = true;

            rastro.emitting = true;

            fazendoDash = true;

            StartCoroutine(Dado());
        }


        // ========================================================
        // MORTE
        // ========================================================

        if (vida <= 0 && oi)
        {
            Animator.SetTrigger("Morte");

            Debug.Log("MORREU ");

            vida = 100;

            barra2.fillAmount = (float)vida / vidat;

            StartCoroutine(Daaado());
        }
    }



    // ============================================================
    // DASH
    // ============================================================

    IEnumerator Dado()
    {
        yield return new WaitForSeconds(0.8f);

        dash = false;

        fazendoDash = false;

        rastro.emitting = false;

        movimentoo = new Vector3(0, 0, 4);

        StartCoroutine(Dash());
    }


    IEnumerator Dash()
    {
        yield return new WaitForSeconds(1.2f);

        esspera = false;

        dashDisponivel = true;
    }


    // ============================================================
    // SOCO
    // ============================================================

    void Soco3()
    {
        danof.recebi();

        socoDisponivel = true;

        StartCoroutine(Dassh());
    }


    void Soco()
    {
        esspera = false;

        StartCoroutine(Dassh());

        dano.recebi();
    }


    void Soco0()
    {
        dano.recebi();
    }


    void SOCO1()
    {
        Debug.Log("n deu certo");

        dano.recebi();

        socoDisponivel = true;
        StartCoroutine(Hit_LP_RP());

    }


    void SOCO2()
    {

        dano.recebi();

        socoDisponivel = true;
        StartCoroutine(Hit_LP_RP());
    }


    void SOCO3()
    {
        dano.recebi();

        socoDisponivel = true;
        StartCoroutine(Hit_LP_RP());
    }


    void SOCO4()
    {
        dano.recebi();

        socoDisponivel = true;
        StartCoroutine(Hit_LF_RF());
    }


    void SOCO5()
    {
        StartCoroutine(SOCO5T());

        dano.recebi();
    }


    IEnumerator Dassh()
    {
        yield return new WaitForSeconds(0.6f);

        socoDisponivel = true;
    }
    IEnumerator Hit_LP_RP()
    {
        Hitbox_LP.enabled = true;
        Hitbox_RP.enabled = true;
        yield return new WaitForSeconds(0.5f);
        Hitbox_LP.enabled = false;
        Hitbox_RP.enabled = false;
    }
    IEnumerator Hit_LF_RF()
    {
        Hitbox_LF.enabled = true;
        Hitbox_RF.enabled = true;
        yield return new WaitForSeconds(0.5f);
        Hitbox_LF.enabled = false;
        Hitbox_RF.enabled = false;

    }

    IEnumerator SOCO5T()
    {
        yield return new WaitForSeconds(0.6f);

        socoDisponivel = true;
    }


    // ============================================================
    // PASSOS
    // ============================================================

    void Passos0()
    {
        step.Play();
    }


    void Passos1()
    {
        step.Play();
    }


    // ============================================================
    // DANO
    // ============================================================

    IEnumerator Daado()
    {
        timee = false;

        yield return new WaitForSeconds(0.6f);

        timee = true;
    }


    IEnumerator S()
    {
        yield return new WaitForSeconds(1f);

        socoDisponivel = true;
    }


    IEnumerator Daaaaado()
    {
        a.Stun();

        yield return new WaitForSeconds(2.3f);

        a.Stun0();
    }


    public void Stun()
    {
        socoDisponivel = false;

        dashDisponivel = false;

        fireDisponivel = false;

        rockDisponivel=false;

        Animator.SetTrigger("Hit");
    }


    public void Stun0()
    {
        socoDisponivel = true;

        dashDisponivel = true;

        fireDisponivel = true;

        rockDisponivel = true;
    }


    // ============================================================
    // VIDA
    // ============================================================





    IEnumerator Daaado()
    {
        oi = false;

        yield return new WaitForSeconds(3f);

        oi = true;
    }


    // ============================================================
    // FIRE
    // ============================================================

    IEnumerator FIRE()
    {
        fireDisponivel = false;

        photonView.RPC("AnimacaoFire", RpcTarget.All);

        yield return new WaitForSeconds(5f);

        Usar();

        yield return new WaitForSeconds(1f);

        fireDisponivel = true;
    }
    IEnumerator ROCK()
    {
        rockDisponivel = false;

        photonView.RPC("AnimacaoRock", RpcTarget.All);

        yield return new WaitForSeconds(5f);
        Usar_rock();

        yield return new WaitForSeconds(1f);

        rockDisponivel = true;
    }

    [PunRPC]
    void AnimacaoFire()
    {
        Animator.SetTrigger("Fire");
    }
    [PunRPC]
    void AnimacaoRock()
    {
        Debug.Log("OIA A PEDRA");
        Animator.SetTrigger("Rock");
    }

    void Fire()
    {
        photonView.RPC("CriarFire", RpcTarget.All);
    }
    void Rock()
    {
        Debug.Log("OIA A PEDRA2");
        photonView.RPC("CriarRock", RpcTarget.All);
    }
    [PunRPC]
    void CriarFire()
    {
        GameObject projetil = Instantiate(
            vfxPrefab,
            boca.position + transform.forward * distanciaProjetil,
            transform.rotation
        );

        Destroy(projetil, 15f);
    }


    [PunRPC]
    void CriarRock()
    {
        GameObject projetil = Instantiate(
            vfxPrefab_rock,
            new Vector3(-200, 1950, 0) + transform.forward * distanciaProjetil,
            transform.rotation
        );
        Debug.Log("OIA A PEDRA7");
        Destroy(projetil, 20);
    }
    public void Usar()
    {
        if (tempoFire <= 0)
        {
            tempoFire = cooldownFire;
        }
    }
    public void Usar_rock()
    {
        if (tempoRock <= 0)
        {
            tempoRock = cooldownRock;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.root == transform.root)
        {
            return;
        }
        if (other.CompareTag("HITBOX"))
        {
            Transform inimigo = other.transform.root;
            Vector3 direcao = inimigo.position - transform.position;
            direcao.y = 0;
            if (direcao != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(direcao);
            }
            Animator.SetTrigger("Hit 0");
            vida -= 5;
            barra2.fillAmount = (float)vida / vidat;
            StartCoroutine(barra_2());
        }

        if (other.CompareTag("HITBOX_FIRE"))
        {
            Transform inimigo = other.transform;
            Vector3 direcao = inimigo.position - transform.position;
            direcao.y = 0;
            if (direcao != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(direcao);
            }
            vida -= 35;
            Animator.SetTrigger("Hit 0");
            barra2.fillAmount = (float)vida / vidat;
            StartCoroutine(barra_2());
        }
        if (other.CompareTag("HITBOX_ROCK"))
        {
            Debug.Log("PEDRA");
            Animator.SetTrigger("HIT_ROCK");
            vida -= 75;
            barra2.fillAmount = (float)vida / vidat;
            StartCoroutine(barra_2());
        }

    }
    IEnumerator barra_2()
    {
        yield return new WaitForSeconds(1.5f);
        barra1.fillAmount = (float)vida / vidat;

    }
}
