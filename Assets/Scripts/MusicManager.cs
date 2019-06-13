using System.Collections;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager instance = null;
    public AudioSource[] sequentialClips;
    public AudioSource loopClip;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            if (loopClip.clip.name == instance.loopClip.clip.name)
            {
                Destroy(gameObject);
                return;
            }
            else
            {
                Destroy(instance.gameObject);
                instance = this;
            }
        }

        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        instance.loopClip.loop = true;
    }

    public void PlayClips()
    {
        StartCoroutine(playClips());
    }

    private IEnumerator playClips()
    {
        Debug.Log("Starting audio clips");
        foreach (AudioSource sequentialClip in instance.sequentialClips)
        {
            sequentialClip.Play();
            yield return new WaitForSeconds(sequentialClip.clip.length);
        }

        PlayLoopClip(instance.loopClip);
    }

    public void PlayLoopClip(AudioSource clip)
    {
        if (clip != null)
        {
            clip.Play();
        }
    }

    public void StopLoopClip()
    {
        if (instance.loopClip != null && instance.loopClip.isPlaying)
        {
            instance.loopClip.Stop();
        }
    }
}
