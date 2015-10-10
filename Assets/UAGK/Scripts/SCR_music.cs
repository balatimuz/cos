using UnityEngine;
using System.Collections;

public class SCR_music : MonoBehaviour {
	
	public float musicVolume=1f;
	//the volume of the music. 1 is maximum volume, 0 is silent.
	
	float fadeSpeed;
	float[] vol=new float[2];
	
	bool isPlaying=false;
	
	void Update () {
		if(vol[0]!=vol[1]){
			vol[0]=Mathf.MoveTowards(vol[0],vol[1],(fadeSpeed*SCR_main.counterMult));
			
			UpdateVolume();
			
			if(vol[0]==0f&&vol[1]==0f){
				GetComponent<AudioSource>().Stop();
				isPlaying=false;
			}
		}
	}
	
	void UpdateVolume(){
		GetComponent<AudioSource>().volume=(vol[0]*musicVolume);
	}
	
	public void PlayMusic(AudioClip mus){
		GetComponent<AudioSource>().clip=mus;
		StartTrack();
	}
	
	public void StartTrack(){
		if(isPlaying){
			GetComponent<AudioSource>().Stop ();
		}
		
		if(GetComponent<AudioSource>().clip){
			if(SCR_main.musOn==1){
				vol=new float[2]{1f,1f};
				UpdateVolume ();
				GetComponent<AudioSource>().Play ();
				isPlaying=true;
			}
		}
	}
	
	public void FadeOut(int com){
		if(isPlaying){
			if(com==0){
				fadeSpeed=0.52f;
			}
			if(com==1){
				fadeSpeed=0.05f;
			}
			vol[1]=0f;
		}
	}
}
