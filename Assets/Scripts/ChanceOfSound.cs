using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChanceOfSound : MonoBehaviour
{
    [SerializeField] private float chanceOfSound;
    [SerializeField] private AudioSource m_MyAudioSource;

    // Start is called before the first frame update
    void Start()
    {
        m_MyAudioSource.enabled = false;

        if (Random.value < chanceOfSound)
        {
            m_MyAudioSource.enabled = true;
        }
        else
        {
            m_MyAudioSource.enabled = false;
        }
    }
}
