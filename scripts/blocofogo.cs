using UnityEngine;

public class BlocoFogo : MonoBehaviour
{
    public float tempoParaSumir = 1f;
    public float velocidadeDiminuir = 3.5f;

    private float tempo;

    void Update()
    {
        tempo += Time.deltaTime;

        if (tempo >= tempoParaSumir)
        {
            transform.localScale -= Vector3.one * velocidadeDiminuir * Time.deltaTime;
            if (transform.localScale.x <= 0.05f)
            {
                Destroy(gameObject);
            }
        }
    }
}