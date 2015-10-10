using UnityEngine;
using System.Collections;

public class SCR_characterControl : MonoBehaviour {
	
	public bool standAttack;
	public float invulnerableTime;
	//the amount of time the player is invulnerable after being hit.
	
	float invulnerableCounter=0f;
	
	SCR_character character;
	SCR_characterHealth characterHealth;
	SCR_reticle reticle;
	
	[HideInInspector]
	public bool controlActive;
	
	//CLICK
	bool rightClickHeld;
	int clickCurrent=0;
	float doubleClickTimer=0f;
	float doubleClickLimit=0.15f;
	
	
	
	public void StartUp () {
		character=GetComponent<SCR_character>();
		characterHealth=GetComponent<SCR_characterHealth>();
		
		GetComponent<SCR_characterHealth>().StartUp(1000);
		
		if(GameObject.Find("MAIN").GetComponent<SCR_main>().reticleOn){
			GameObject reticleInst=Instantiate(Resources.Load ("Objects/Gui/OBJ_reticle",typeof(GameObject)),Vector3.zero,Quaternion.identity) as GameObject;
			reticle=reticleInst.GetComponent<SCR_reticle>();
		}
	}
	
	
	void Update () {

		if(SCR_input.playerControlActive&&SCR_stage.paused==false){
			UpdateControl();
		}
		
		if(invulnerableCounter>0f){
			invulnerableCounter=Mathf.MoveTowards(invulnerableCounter,0f,Time.deltaTime);
			
			if(invulnerableCounter==0f){
				character.StopFlash();
			}
		}
	}
	
	void UpdateControl(){
		bool canMove=true;
		bool dirPressed=false;
		bool planeHit=false;
		Vector3 destination=Vector3.zero;
		
		if(character.attacking){
			//prevent movement if the character is performing a standing attack.
			if(character.melee){
				if(character.characterMelee.attackAnimSlot==5||character.characterMelee.attackAnimSlot==6){
					canMove=false;
				}
			}	else {
				if(character.characterRanged.attackAnimSlot==5||character.characterRanged.attackAnimSlot==6){
					canMove=false;
				}
			}
		}

		if(standAttack==true&&character.attacking){
			//prevent movement if the character is performing a standing attack.
			if(character.melee){
				if(character.characterMelee.attackAnimSlot==7||character.characterMelee.attackAnimSlot==8){
					canMove=false;
				    character.speed[1]=Vector3.zero;
				    character.running=false;
				}
			}	else {
				if(character.characterRanged.attackAnimSlot==7||character.characterRanged.attackAnimSlot==8){
					canMove=false;
				    character.speed[1]=Vector3.zero;
				    character.running=false;
				}
			}
		}
		
		//Click or Double Click
		int newClick=0;
		
		if(SCR_input.cType==0){
			if(Input.GetMouseButton(1)){
				if(rightClickHeld==false){
					rightClickHeld=true;
					
					if(clickCurrent==0){
						clickCurrent=1;
						newClick=1;
					}	else {
						if(clickCurrent==1&&doubleClickTimer>0f){
							clickCurrent=0;
							doubleClickTimer=0f;
							newClick=2;
						}
					}
				}
			}	else {
				if(rightClickHeld){
					rightClickHeld=false;
					
					if(clickCurrent==1){
						doubleClickTimer=doubleClickLimit;
					}
				}
			}
			
			if(doubleClickTimer>0f){
				doubleClickTimer=Mathf.MoveTowards(doubleClickTimer,0f,Time.deltaTime);
				
				if(doubleClickTimer==0f){
					clickCurrent=0;	
				}
			}
		}	else {
			if(SCR_input.cType==1){
				if(Input.GetMouseButtonDown(1)){
					newClick=2;	
				}
			}
		}
		
		Plane plane=new Plane(Vector3.up,Vector3.zero);
		float hitDist=0f;
		Ray ray;
		
		ray=Camera.main.ScreenPointToRay(Input.mousePosition);
		
		if (plane.Raycast(ray,out hitDist)) {
			destination=ray.GetPoint(hitDist);
			planeHit=true;
			
			if(reticle&&Time.timeScale==1f){
				reticle.SetPos(new Vector2(destination.x,destination.z));	
			}
		}
		
		
		if(SCR_input.cType==0){
			
			if (planeHit) {
				character.SetRotAngle(destination);
			}
			
			if(Input.GetMouseButton(1)){
				dirPressed=true;
			}
			
		}	else {
			
			float inputX=Input.GetAxis("Horizontal");
			float inputY=Input.GetAxis("Vertical");
			
			if(inputX!=0f||inputY!=0f){
				dirPressed=true;
				
				Vector3 inputDir=new Vector3(inputX,0f,inputY).normalized;
				character.SetRotAngle(transform.position+inputDir);
				
			}
			
		}
		
		
		if(character.rolling==0&&character.stunned==0){
			//Move to destination
			if(dirPressed){
				if(canMove){
					character.speed[1]=(character.dir*character.runSpeed);
				
					character.running=true;
					
					if(character.attacking==false){
						character.PlayAnim(1);
					}
				}
				bool canRotate=true;
				
				if(character.attacking){
					canRotate=false;	
				}
				
				if(character.melee){
					if(character.characterMelee.lockActive){
						canRotate=false;
					}
				}
				
				if(canRotate){
					character.SetRotTarget();
				}
				
			}	else {
				
				character.speed[1]=Vector3.zero;
				character.running=false;
				
				if(character.attacking==false){
					character.PlayAnim(0);
				}	else {
					
					//if character is performing a running attack and stops moving, the attack is cancelled.
					if(character.melee){
						if(character.characterMelee.attackAnimSlot==7||character.characterMelee.attackAnimSlot==8){
							character.PlayAnim(0);
							character.AttackCancel();
						}
					}	else {
						if(character.characterRanged.attackAnimSlot==7||character.characterRanged.attackAnimSlot==8){
							character.PlayAnim(0);
							character.AttackCancel();
						}
					}
				}
			}
			
			//Attack
			
			if(Input.GetMouseButtonDown (0)){
				
				if(character.rolling==0&&character.stunned==0){
					character.SetRotAngle(destination);
					character.SetRotTarget();
				}
				
				character.attackPoint=destination;
				character.AttackStart();
			}
		}
			
		if(newClick==2){
			//Roll
			character.RollStart();
		}
	}
	
	public bool StartInvulnerability(){
		bool invulnerableSuccess=false;
		
		if(invulnerableTime>0f){
			invulnerableCounter=invulnerableTime;
			character.StartFlash(1,10000);
			invulnerableSuccess=true;
		}
		
		return invulnerableSuccess;
	}
	
	void OnTriggerEnter(Collider col){
		if(character.stunned<2){
			if(col.gameObject.tag=="Collectable"){
				SCR_collectable c=col.gameObject.GetComponent<SCR_collectable>();
				
				if(c.isActive){
					c.Collect();
					
					if(c.healthRecovery>0){
						characterHealth.AddHealth(c.healthRecovery);
					}
					
					if(c.powerBoost>0){
						character.PowerBoostStart(c.powerBoost,c.powerBoostDuration);	
					}
				}
			}
		}
	}
	
	void OnTriggerStay(Collider por){
		if(character.stunned<2){
			if(por.gameObject.tag=="Portal"){
				SCR_portal p=por.gameObject.GetComponent<SCR_portal>();
				
				if(p.isActive){
					p.Collect();
				}
			}
		}
	}	
	
	public void ReticleOn(bool com){
		if(reticle){
			reticle.Appear(com);	
		}
	}
}
