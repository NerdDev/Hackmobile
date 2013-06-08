// /////////////////////////////////////////////////////////////////////////////////////////////////////////
//
// Audio Manager.
//
// This code is release under the MIT licence. It is provided as-is and without any warranty.
//
// Developed by Daniel Rodríguez (Seth Illgard) in April 2010
// http://www.silentkraken.com
//
// /////////////////////////////////////////////////////////////////////////////////////////////////////////
using UnityEngine;
using System.Collections;
public class AudioManager : MonoBehaviour
{
    /* Modification to the original code from Silent kraken is very simple.  Only the methods were duplicated and renamed for the simple purpose of allowing
     * the audiomanager to have more control over music that is played.  It also allows for more control and clarification on the current music that is playing
     * in order to have more of a 'jukebox' style feel and control.
     * 	
     * 	
     * 	*******                  Naming convention for all Audioclips is as follows:          **************
     * 	soundEffectSFX;
     *  musicBGM;
     
     PLEASE NOTE:  iTWEEN needs access to it's target first frame.  if currentMusic is null, then adjusting volume on it will break and throw an exception.  Just use caution.
     
     * */
	
	public bool crossfadeInProgress;
	public AudioSource currentPlayingMusic;
	private float musicVolume = 1;
	//Making a property for music volume so it can do all the work of finding objects and adjusting in the setter: 
	public float MusicVolume
	{
	get
		{
			return musicVolume;
		}
		set
		{
			musicVolume = value;
			currentPlayingMusic.volume = value;
		}
		
	}
	private float sfxVolume = 1;//Making this into a property will be fun since gui sfx are audiosources in the scene...lawlz
	public float SFXVolume
	{
	get
		{
			return sfxVolume;
		}
		set
		{
			sfxVolume = value;//This is our general sfx volume line for anything played with PlaySFX()
			//The following is mainly for EZGUI fx audio, as the sources for EZGUI audio effects are AudioSources that pre-exist in the scene
			GameObject[] audioGOs = GameObject.FindGameObjectsWithTag("Audio");
			foreach (GameObject go in audioGOs) 
			{
				AudioSource source = go.GetComponent("AudioSource") as AudioSource;
				source.volume = value;
			}
		}
		
	}
	public float masterVolume = 1;//Empty variable for the time being: Watch for future mods in the Play() code.
	
	//Sound Effects:
	public AudioClip DebugClickSFX;
	public AudioClip otherSFX;
	//Music Tracks:
	public AudioClip MenuBGM;
	public AudioClip PlayingBGM;
	
	
	//Music Options for our jukebox Coroutine:
	public bool allowMusic;
	//public bool shuffleSongs;
	
	
	
	void Start () 
	{
		StartCoroutine("JukeBox");		                          
	}
	void Update () 
	{
		AudioListener.volume = masterVolume;//NOTE: Move this calculation to a a GameState that has access to the actual volume control slider                       
	}
	
	AudioClip DecideWhatMusicToPlay ()
	{
		Debug.Log("Deciding what music to play...");
		//Use this criteria to decide:
		string levelName = Application.loadedLevelName;
		//string currentState = Managers.GameStateManager.State.name;
		
		if (levelName == "MainMenu")
		{		
		Debug.Log("Decided that we'll play " + MenuBGM);
		return MenuBGM;
		}
		else
		{
		Debug.Log("Decided that we'll play " + PlayingBGM);
		return PlayingBGM;	
		}
	}

	public IEnumerator JukeBox()
	{
	
		if (allowMusic == true)//THIS IS NOT NOT NOT A MUTE CHECK!!!
		{
		//Deciding is important here because we may be fading out music from another scene:
			currentPlayingMusic = PlayBGM(DecideWhatMusicToPlay()); 
			
			yield return new WaitForSeconds (currentPlayingMusic.clip.length);	
		}
		else if (allowMusic == false)
		{
			yield return new WaitForSeconds(2f);
		}
		//Looping with StartCoroutine() to avoid inconvenience of while()
				StartCoroutine("JukeBox");
	}
	
	public void AdjustMusicVolume(float targetVolume,float seconds)//Other scripts can use this arbitrarily!
	{
		Debug.Log("Adjusting volume to " + targetVolume + " over a time of " + seconds);
		float timeRemaining = currentPlayingMusic.clip.length - currentPlayingMusic.time;
		if (currentPlayingMusic	 != null && timeRemaining > seconds	)//iTween failsafe:
		{
		Hashtable ht = new Hashtable();	
		ht.Add("volume",targetVolume);
		ht.Add("audiosource",currentPlayingMusic);
		ht.Add("time",seconds);
		iTween.AudioTo(gameObject,ht);
		}
			
	}
	//Chuck Norris says switch this to private:
	public IEnumerator CrossFadeSongs(float time)//This will be called from within audiomanager itself!  All it does is fade out and stop the jukebox coroutine.
	{
		if (crossfadeInProgress	 == false)
		{
Debug.Log("Crossfade Started...");	
		crossfadeInProgress = true;
		StopCoroutine("JukeBox");
    //Check criteria to determine new song:  WIP - changes per game
		
		float fadeTime = time;
		float startDifference = 2;//this is the time before the end of 'fadeTime' that the new song will play
		float timeRemaining = currentPlayingMusic.clip.length - currentPlayingMusic.time;
		//Adjust volume on old song:
		if (currentPlayingMusic	 != null && timeRemaining > fadeTime)//This is mainly a failsafe for iTween
		{
			//Fade Out:
		Hashtable ht = new Hashtable();	
		ht.Add("volume",0);
		ht.Add("audiosource",currentPlayingMusic);
		ht.Add("time",time);
		iTween.AudioTo(gameObject,ht);
		}
		//Saving current music into a GO so we can destroy it:
		GameObject oldMusic = currentPlayingMusic.gameObject;
		//Delay til new songs:
    yield return new WaitForSeconds(time-startDifference);//Playing music early according to hardcoded variable
		StartCoroutine("JukeBox");//Jukebox will decide what song to play now
	yield return new WaitForSeconds(startDifference	+ 1);//This is here so iTween doesn't throw exception:
		//Destroying old one:
			Destroy(oldMusic);
		crossfadeInProgress = false;
Debug.Log("Crossfade Ended.");	
		}
		else if(crossfadeInProgress == true)
			Debug.Log("Crossfade called, but one is already in progress.  Skipping...");

	}
	
	#region Original Kraken Code
//	public AudioSource Play(AudioClip clip, Transform emitter)
//    {
//        return Play(clip, emitter, 1f, 1f);
//    }
// 
//    public AudioSource Play(AudioClip clip, Transform emitter, float volume)
//    {
//        return Play(clip, emitter, volume, 1f);
//    }
// 
//    /// <summary>
//    /// Plays a sound by creating an empty game object with an AudioSource
//    /// and attaching it to the given transform (so it moves with the transform). Destroys it after it finished playing.
//    /// </summary>
//    /// <param name="clip"></param>
//    /// <param name="emitter"></param>
//    /// <param name="volume"></param>
//    /// <param name="pitch"></param>
//    /// <returns></returns>
//    public AudioSource Play(AudioClip clip, Transform emitter, float volume, float pitch)
//    {
//        //Create an empty game object
//        GameObject go = new GameObject ("Audio: " +  clip.name);
//        go.transform.position = emitter.position;
//        go.transform.parent = emitter;
// 
//        //Create the source
//        AudioSource source = go.AddComponent<AudioSource>();
//        source.clip = clip;
//        source.volume = volume;
//        source.pitch = pitch;
//        source.Play ();
//        Destroy (go, clip.length);
//        return source;
//    }
// 
//    public AudioSource Play(AudioClip clip, Vector3 point)
//    {
//        return Play(clip, point, 1f, 1f);
//    }
// 
//    public AudioSource Play(AudioClip clip, Vector3 point, float volume)
//    {
//        return Play(clip, point, volume, 1f);
//    }
// 
//    /// <summary>
//    /// Plays a sound at the given point in space by creating an empty game object with an AudioSource
//    /// in that place and destroys it after it finished playing.
//    /// </summary>
//    /// <param name="clip"></param>
//    /// <param name="point"></param>
//    /// <param name="volume"></param>
//    /// <param name="pitch"></param>
//    /// <returns></returns>
//    public AudioSource Play(AudioClip clip, Vector3 point, float volume, float pitch)
//    {
//        //Create an empty game object
//        GameObject go = new GameObject("Audio: " + clip.name);
//        go.transform.position = point;
// 
//        //Create the source
//        AudioSource source = go.AddComponent<AudioSource>();
//        source.clip = clip;
//        source.volume = volume;
//        source.pitch = pitch;
//        source.Play();
//        Destroy(go, clip.length);
//        return source;
//    }
	#endregion
	
	#region My Shot At Custom SFX Code
	public AudioSource PlaySFX(AudioClip clip)
    {
        return PlayBGM(clip, new Vector3(0,0,0), 1f);
    }
	public AudioSource PlaySFX(AudioClip clip, Transform emitter)
    {
        return PlaySFX(clip, emitter, 1f, sfxVolume);
    }
 
    public AudioSource PlaySFX(AudioClip clip, Transform emitter, float volume)
    {
        return PlaySFX(clip, emitter, volume, 1f);
    }
 
    /// <summary>
    /// Plays a sound by creating an empty game object with an AudioSource
    /// and attaching it to the given transform (so it moves with the transform). Destroys it after it finished playing.
    /// </summary>
    /// <param name="clip"></param>
    /// <param name="emitter"></param>
    /// <param name="volume"></param>
    /// <param name="pitch"></param>
    /// <returns></returns>
    public AudioSource PlaySFX(AudioClip clip, Transform emitter, float volume, float pitch)
    {
        //Create an empty game object
        GameObject go = new GameObject ("Audio: " +  clip.name);
        go.transform.position = emitter.position;
        go.transform.parent = emitter;
 
        //Create the source
        AudioSource source = go.AddComponent<AudioSource>();
        source.clip = clip;
        source.volume = sfxVolume;
        source.pitch = pitch;
        source.Play ();
        Destroy (go, clip.length);
        return source;
    }
 
    public AudioSource PlaySFX(AudioClip clip, Vector3 point)
    {
        return PlaySFX(clip, point, sfxVolume, 1f);
    }
 
    public AudioSource PlaySFX(AudioClip clip, Vector3 point, float volume)
    {
        return PlaySFX(clip, point, volume, 1f);
    }
 
    /// <summary>
    /// Plays a sound at the given point in space by creating an empty game object with an AudioSource
    /// in that place and destroys it after it finished playing.
    /// </summary>
    /// <param name="clip"></param>
    /// <param name="point"></param>
    /// <param name="volume"></param>
    /// <param name="pitch"></param>
    /// <returns></returns>
    public AudioSource PlaySFX(AudioClip clip, Vector3 point, float volume, float pitch)
    {
        //Create an empty game object
        GameObject go = new GameObject("Audio: " + clip.name);
        go.transform.position = point;
 
        //Create the source
        AudioSource source = go.AddComponent<AudioSource>();
        source.clip = clip;
        source.volume = sfxVolume;
        source.pitch = pitch;
        source.Play();
        Destroy(go, clip.length);
        return source;
    }
	#endregion
		
	#region My Shot At Custom BGM Code
	public AudioSource PlayBGM(AudioClip clip)
    {
        return PlayBGM(clip, new Vector3(0,0,0), musicVolume);
    }
	public AudioSource PlayBGM(AudioClip clip, Transform emitter)
    {
        return PlayBGM(clip, emitter, musicVolume, 1f);
    }
 
    public AudioSource PlayBGM(AudioClip clip, Transform emitter, float volume)
    {
        return PlayBGM(clip, emitter, volume, 1f);
    }
 
    /// <summary>
    /// Plays a sound by creating an empty game object with an AudioSource
    /// and attaching it to the given transform (so it moves with the transform). Destroys it after it finished playing.
    /// </summary>
    /// <param name="clip"></param>
    /// <param name="emitter"></param>
    /// <param name="volume"></param>
    /// <param name="pitch"></param>
    /// <returns></returns>
    public AudioSource PlayBGM(AudioClip clip, Transform emitter, float volume, float pitch)
    {
        //Create an empty game object
        GameObject go = new GameObject ("Audio: " +  clip.name);
        go.transform.position = emitter.position;
        go.transform.parent = emitter;
 
        //Create the source
        AudioSource source = go.AddComponent<AudioSource>();
        source.clip = clip;
        source.volume = musicVolume;
        source.pitch = pitch;
        source.Play ();
        Destroy (go, clip.length);
        return source;
    }
 
    public AudioSource PlayBGM(AudioClip clip, Vector3 point)
    {
        return PlayBGM(clip, point, musicVolume, 1f);
    }
 
    public AudioSource PlayBGM(AudioClip clip, Vector3 point, float volume)
    {
        return PlayBGM(clip, point, volume, 1f);
    }
 
    /// <summary>
    /// Plays a sound at the given point in space by creating an empty game object with an AudioSource
    /// in that place and destroys it after it finished playing.
    /// </summary>
    /// <param name="clip"></param>
    /// <param name="point"></param>
    /// <param name="volume"></param>
    /// <param name="pitch"></param>
    /// <returns></returns>
    public AudioSource PlayBGM(AudioClip clip, Vector3 point, float volume, float pitch)
    {
        //Create an empty game object
        GameObject go = new GameObject("Audio: " + clip.name);
        go.transform.position = point;
 
        //Create the source
        AudioSource source = go.AddComponent<AudioSource>();
        source.clip = clip;
        source.volume = volume;
        source.pitch = pitch;
        source.Play();
        Destroy(go, clip.length);
        return source;
    }
	#endregion
	
}