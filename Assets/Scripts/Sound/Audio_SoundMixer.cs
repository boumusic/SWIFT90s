using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Audio_SoundMixer : MonoBehaviour
{

    [Header("MIX")]
    [Range(0.0f, 1.0f)]
    public float AnnouncerVolume, AnnouncerBlend;
    [Range(0.0f, 500.0f)]
    public float AnnouncerReverb,AnnouncerReverbDensity;
    [Range(0.0f, 1.0f)]
    public float MovementVolume, MovementBlend;
    [Range(0.0f, 500.0f)]
    public float MovementReverb,MovementReverbDensity;
    [Range(0.0f, 1.0f)]
    public float FightVolume, FightBlend;
    [Range(0.0f, 500.0f)]
    public float FightReverb, FightReverbDensity;
    [Range(0.0f, 1.0f)]
    public float FeedbackVolume, FeedbackBlend;
    [Range(0.0f, 500.0f)]
    public float FeedbackReverb, FeedbackReverbDensity;
    [Range(0.0f, 1.0f)]
    public float MusicVolume, MusicBlend;

    [Header("REFS")]
    public AudioSource Announcer, Movement, Fight, Feedback, Music;
    public AudioReverbFilter AnnouncerReverbF, MovementReverbF, FightReverbF, FeedbackReverbF;

    // Start is called before the first frame update
    void Awake()
    {
        AnnouncerVolume = Announcer.volume;
        AnnouncerBlend = Announcer.spatialBlend;
        AnnouncerReverb = AnnouncerReverbF.reverbLevel;
        AnnouncerReverbDensity = AnnouncerReverbF.density;

        MovementVolume = Movement.volume;
        MovementBlend = Movement.spatialBlend;
        MovementReverb = MovementReverbF.reverbLevel;
        MovementReverbDensity = MovementReverbF.density;

        FightVolume = Fight.volume;
        FightBlend = Fight.spatialBlend;
        FightReverb = FightReverbF.reverbLevel;
        FightReverbDensity = FightReverbF.density;


        FeedbackVolume = Feedback.volume;
        FeedbackBlend = Feedback.spatialBlend;
        FeedbackReverb = FeedbackReverbF.reverbLevel;
        FeedbackReverbDensity = FeedbackReverbF.density;

        MusicVolume = Music.volume;
        MusicBlend = Music.spatialBlend;
    }

    public void AddReverb(float ReverbAdded , AudioReverbFilter reverbfilter)
    {
        reverbfilter.reverbLevel += ReverbAdded;

    }

    public void ResetReverb(AudioReverbFilter reverbFilter)
    {
        reverbFilter.reverbLevel = 0f;
    }

    public void ResetVolume(AudioSource audioSource)
    {
        audioSource.volume = 1;

    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
