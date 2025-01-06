using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Manages all sound-related functionality in the game, including playing sound effects,
/// managing sound queues, and categorizing positive and negative audio cues.
/// Implements a Singleton pattern for global access.
/// </summary>
public class SoundManager : MonoBehaviour
{
    #region Singleton Implementation

    /// <summary>
    /// Singleton instance of the SoundManager for global access.
    /// </summary>
    public static SoundManager Instance;

    private void Awake()
    {
        // Enforce Singleton pattern to ensure only one instance exists.
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Destroy duplicate instances of SoundManager.
        }
        else
        {
            Instance = this;
        }

        // Initialize audio cue lists.
        InitializeCueLists();
    }

    /// <summary>
    /// Initializes the positive and negative audio cue lists.
    /// </summary>
    private void InitializeCueLists()
    {
        negativeCues = new List<AudioClip>
        {
            losingGround,
            underAttack
        };

        positiveCues = new List<AudioClip>
        {
            goodWork,
            tangoDown,
            targetNeutralized
        };
    }

    #endregion

    #region Serialized Fields (Unity Inspector)

    [Header("General AudioClips")] [Tooltip("AudioClip played for 'Hostiles Inbound'.")] [SerializeField]
    public AudioClip hostilesInbound;

    [Tooltip("AudioClip played when a 'Tango' is taken down.")] [SerializeField]
    public AudioClip tangoDown;

    [Tooltip("AudioClip played when a target is neutralized.")] [SerializeField]
    public AudioClip targetNeutralized;

    [Tooltip("AudioClip for artillery strike.")] [SerializeField]
    public AudioClip artilleryStrike;

    [Tooltip("AudioClip for congratulatory feedback.")] [SerializeField]
    public AudioClip goodWork;

    [Tooltip("AudioClip for 'Losing Ground' warnings.")] [SerializeField]
    public AudioClip losingGround;

    [Tooltip("AudioClip for enemy movement detected.")] [SerializeField]
    public AudioClip enemyMove;

    [Tooltip("AudioClip for when objectives are secured.")] [SerializeField]
    public AudioClip objectiveSecured;

    [Tooltip("AudioClip for when objectives are lost.")] [SerializeField]
    public AudioClip objectiveLost;

    [Tooltip("AudioClip for mission accomplishment.")] [SerializeField]
    public AudioClip missionAccomplished;

    [Tooltip("AudioClip for airstrike confirmation.")] [SerializeField]
    public AudioClip airStrike;

    [Tooltip("AudioClip for 'Under Attack' notifications.")] [SerializeField]
    public AudioClip underAttack;

    [Tooltip("AudioClip for EMP activation.")] [SerializeField]
    public AudioClip empSound;

    [Header("Explosion Sound Effects")] [Tooltip("AudioClips for various explosion sound effects.")] [SerializeField]
    private AudioClip[] explosions;

    [Header("Audio Sources")] [Tooltip("AudioSource used for one-shot sound effects.")] [SerializeField]
    private AudioSource audioSourceOneShot;

    [Tooltip("AudioSource used for scheduled or queued sound effects.")] [SerializeField]
    private AudioSource audioSource;

    #endregion

    #region Private Fields

    private Queue<AudioClip> soundQueue = new Queue<AudioClip>(); // Queue to store AudioClips to be played in sequence.
    private List<AudioClip> negativeCues; // List of negative feedback audio cues.
    private List<AudioClip> positiveCues; // List of positive feedback audio cues.

    #endregion

    #region Public Sound Methods

    /// <summary>
    /// Plays the given AudioClip immediately using the one-shot AudioSource.
    /// </summary>
    /// <param name="clip">The AudioClip to be played.</param>
    public void PlaySoundOneShot(AudioClip clip)
    {
        audioSourceOneShot.PlayOneShot(clip);
    }

    /// <summary>
    /// Schedules or plays the given AudioClip using the main AudioSource.
    /// If another AudioClip is already playing, the new clip is queued.
    /// </summary>
    /// <param name="clip">The AudioClip to be played or queued.</param>
    public void PlaySoundScheduled(AudioClip clip)
    {
        if (audioSource.isPlaying)
        {
            // Enqueue the clip if AudioSource is already playing a sound.
            soundQueue.Enqueue(clip);
        }
        else
        {
            // Play the clip immediately if no sound is currently playing.
            audioSource.clip = clip;
            audioSource.Play();
            StartCoroutine(CheckAndPlayNextClip());
        }
    }

    #endregion

    #region Explosion and Feedback Cues

    /// <summary>
    /// Plays a random explosion sound effect from the explosion list.
    /// </summary>
    public void ExplosionSfx()
    {
        PlaySoundOneShot(explosions[Random.Range(0, explosions.Length)]);
    }

    /// <summary>
    /// Plays a random negative feedback cue from the negative audio cue list.
    /// </summary>
    public void NegativeCue()
    {
        PlaySoundOneShot(negativeCues[Random.Range(0, negativeCues.Count)]);
    }

    /// <summary>
    /// Plays a random positive feedback cue from the positive audio cue list.
    /// </summary>
    public void PositiveCue()
    {
        PlaySoundOneShot(positiveCues[Random.Range(0, positiveCues.Count)]);
    }

    #endregion

    #region Private Utility Methods

    /// <summary>
    /// Checks the sound queue and plays the next AudioClip if the main AudioSource is idle.
    /// </summary>
    private IEnumerator CheckAndPlayNextClip()
    {
        // Wait until the currently playing AudioClip finishes.
        while (audioSource.isPlaying)
        {
            yield return null;
        }

        // If there are clips in the queue, dequeue and play the next one.
        if (soundQueue.Count > 0)
        {
            AudioClip nextClip = soundQueue.Dequeue();
            audioSource.clip = nextClip;
            audioSource.Play();
            StartCoroutine(CheckAndPlayNextClip());
        }
    }

    #endregion
}