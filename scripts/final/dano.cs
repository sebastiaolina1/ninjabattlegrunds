using System;
using System.Collections;
using System.Xml.Serialization;
using UnityEngine;

public class dano1 : MonoBehaviour
{
    // Arraste o objeto "Destino" para este campo no Inspector
    public Transform destino;
    public GameObject GameObject;

   
    public void recebi()
    {
        GameObject.transform.position = destino.position;
        StartCoroutine(EsperarECONTINUAR());
        
    }
    IEnumerator EsperarECONTINUAR()
    {
        // Pausa a execução aqui por 0.1 segundos
        yield return new WaitForSeconds(0.1f);
        GameObject.transform.position =new Vector3(1000, 1000,1000);

    }
  

   
}
