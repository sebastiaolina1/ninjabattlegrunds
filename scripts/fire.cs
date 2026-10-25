using UnityEngine;

public class Projetil : MonoBehaviour
{
    public float velocidade = 15f;
    public float crescimento = 1f;
   

    public GameObject blocoPrefab;

    public float distanciaEntreBlocos = 1f;
    
    public float distanciaEntreLinhas = 1f;
    public float aumentoDaDistancia = 0.3f;

    public float tamanhoBloco = 1f;
    public float aumentoDoTamanho = 0.1f;
    private Vector3 ultimaPosicao;
    private void Start()
    {
        ultimaPosicao = transform.position;
    }
    void Update()
    {
            distanciaEntreLinhas += aumentoDaDistancia * Time.deltaTime;

    // Aumenta o tamanho dos próximos blocos
    tamanhoBloco += aumentoDoTamanho * Time.deltaTime;
        // Movimento
        transform.position += transform.forward * velocidade * Time.deltaTime;

        // Aumenta o tamanho
        transform.localScale += Vector3.one * crescimento * Time.deltaTime;
        transform.position += transform.forward * velocidade * Time.deltaTime;

        // Cria novos blocos conforme ele anda
        if (Vector3.Distance(transform.position, ultimaPosicao) >= distanciaEntreBlocos)
        {
            CriarBlocos();
            ultimaPosicao = transform.position;
        }
    }
    void CriarBlocos()
    {
        // Lado esquerdo
        Vector3 esquerda =
            transform.position - transform.right * distanciaEntreLinhas / 2f;

        // Lado direito
        Vector3 direita =
            transform.position + transform.right * distanciaEntreLinhas / 2f;

        CriarBloco(esquerda);
        CriarBloco(direita);
    }
    void CriarBloco(Vector3 posicao)
    {
        if (Physics.Raycast(
            posicao + Vector3.up * 2f,
            Vector3.down,
            out RaycastHit hit,
            5f))
        {
            GameObject bloco = Instantiate(
                blocoPrefab,
                hit.point,
                Quaternion.identity
            );

            // Tamanho base + pequena variação aleatória
            float tamanho = tamanhoBloco * Random.Range(0.8f, 1.2f);

            bloco.transform.localScale = new Vector3(
                tamanho,
                tamanho,
                tamanho
            );

            // Rotação aleatória
            bloco.transform.rotation = Quaternion.Euler(
                Random.Range(-10f, 10f),
                Random.Range(0f, 360f),
                Random.Range(-10f, 10f)
            );
        }
    }

}