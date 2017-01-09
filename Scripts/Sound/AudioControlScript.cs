using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioControlScript : MonoBehaviour {

    [System.Serializable]
    public class SoundEntry
    {
        public string name;
        public AudioClip startClip;
        public AudioClip loopClip;
    }

    //Array of all sounds
    public SoundEntry[] musicClips;                 //All music for current scene

    //Dictionaries
    Dictionary<string, SoundEntry> musicDict;       //Dictionary for playable music
    Dictionary<string, AudioClip> soundDict;        //Dictionary for playable sound effects

    private AudioSource musicSource;                //Enables music to play in scene

    public string currentMusic = "None";            //Music to play at Start()

	// Use this for initialization
	void Awake () {
        musicSource = GetComponent<AudioSource>();
        ConstructDicts();
	}

    void Start(){
        PlayMusic(currentMusic);    //TEMPORARY Might want a better place to start this
    }

	// Update is called once per frame
	void Update () {
        BeginLoop();
	}

    //Constructs dictionaries
    void ConstructDicts()
    {
        musicDict = new Dictionary<string, SoundEntry>();
        foreach (SoundEntry song in musicClips)
        {
            SoundEntry entry = new SoundEntry();

            entry.startClip = song.startClip;
            entry.loopClip = song.loopClip;
            musicDict[song.name] = entry;
        }
    }

    //Plays music with given name
    public void PlayMusic(string name)
    {
        currentMusic = name;
        if (name == "None")
            musicSource.Stop();
        else
        {
            if (musicDict[currentMusic].startClip == null)
                BeginLoop();
            else {
                musicSource.loop = false;
                musicSource.clip = musicDict[currentMusic].startClip;
                musicSource.Play();
            }
        }
    }

    void BeginLoop()
    {
        if (currentMusic != "None" && !musicSource.isPlaying)
        {
            musicSource.loop = true;
            musicSource.clip = musicDict[currentMusic].loopClip;
            musicSource.Play();
        }
    }
}
