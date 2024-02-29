using LOONACIA.Unity.Managers;
using UnityEngine;

public class TestSound : MonoBehaviour
{
    public AudioClip audioClip;

    private void OnTriggerEnter2D(Collider2D other)
    {
        //ManagerRoot.Sound.Play(audioClip, SoundType.Effect);
        //ManagerRoot.Sound.Play("Simple Click Sound 1", 1f, SoundType.Effect);
    }
}
