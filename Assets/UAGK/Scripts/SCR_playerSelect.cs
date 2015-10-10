using UnityEngine;
using System.Collections;

public class SCR_playerSelect : MonoBehaviour {
	
	public AudioClip playerSelectMusic;
	//the music that plays on this screen.
	
	public Material skyBoxMaterial;
	//fill this in if you want to use a skybox for this screen.
	
	int state=10;
	float stateCounter=0f;
	
	void Awake () {
		SCR_gui.CreateScreenCover(0);
		SCR_gui.CreateText("PlayerSelect",Vector3.zero);
		
		if(skyBoxMaterial){
			RenderSettings.skybox=skyBoxMaterial;
		}
		
		GameObject m=GameObject.Find ("MAIN");
		SCR_main main=m.GetComponent<SCR_main>();
		
		if(playerSelectMusic&&SCR_main.hMusic){
			m.GetComponent<SCR_music>().PlayMusic(playerSelectMusic);	
		}
		
		SCR_main.level=1;

	}
}
