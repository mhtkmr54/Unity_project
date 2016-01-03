using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public class lightningStorm : MonoBehaviour {


	public float lightningInterval; //For setting range of time delays.This is the minimum seconds between lightning bolts. Max is lightningInterval*2

	public Light lightningBulb; 	//The light in the scene to be ued as lightning(directionaL light works best)
	public float defaultIntensity; 	//the normal intensity of the light. this is essentially the off state of a flash.
	public bool isRaining;          //enebles or disables the lightning storm.
	public AudioClip[] thunderRolls;
	public AudioSource[] thunderSources;

	void Start () {
		//load thunder audio files from Assets/Resources/thunder/
		thunderRolls = Resources.LoadAll<AudioClip>("thunder/");
			if(thunderRolls.Length < 1){Debug.Log ("Missing Sounds"); }
		//get the directional light that will be used as lightning
		if(lightningBulb == null)
			lightningBulb = GameObject.Find("lightninghBulb").GetComponent<Light>();
		//record the start intensity of the light as the default or off state.
		defaultIntensity = lightningBulb.intensity;
		thunderSources = this.GetComponents<AudioSource>();
		

		isRaining = true;
		StartCoroutine(thunderStorm());
	}


	IEnumerator thunderStorm () {
		//if it's raining play lightning.
		while(isRaining){

			yield return new WaitForSeconds(Random.Range(lightningInterval,(lightningInterval*2))); 
			StartCoroutine(playLightning(BoltDefinition()));
			//StartCoroutine(lightningBolt());
		}

	}

	//over-complicated version
	//create a lightning bolt definition..run it through a player
	//BoltOfLightning<interval,intensity>

	public Dictionary<float, float> BoltDefinition(){
		//boltDefinition<interval,intensity>
		Dictionary<float, float> boltDefinition = new Dictionary<float, float>();
		//Random number of flashes that this bolt of lightning displays
		int thisBoltFreq = Random.Range(1, 10);
		//create a set of values for each flash
		for(int i=0;i<thisBoltFreq;i++){
			                              //<interval,intensity>
			boltDefinition.Add(Random.Range (0.001f,0.1f), Random.Range(0, 100));
		}
		//thunderscore of this bolt.
		Debug.Log("ThunderScore = " + boltDefinition.Values.Sum(x => x).ToString());
		//BoltOfLightning<interval,intensity>
		return boltDefinition;
	}


	public IEnumerator playLightning(Dictionary<float,float> thisBoltDef){

		//thunderscore of this bolt.
		//Total all multipliers for the bolt to get a VERY rough score for the intensity of the bolt of lightning
		//use this to choose a thunder audio.clip
		float thisThunderScore = thisBoltDef.Values.Sum(x => x);

		//THE FLASH
		foreach(KeyValuePair<float,float> boltParam in thisBoltDef){
			  //<interval,intensity>
			//set the intensity of the lightningBulb in random flashing pattern.
			lightningBulb.intensity = defaultIntensity * boltParam.Value;
			//wait for random interval
			yield return new WaitForSeconds(boltParam.Key);
			//reset light to default/off
			lightningBulb.intensity = defaultIntensity;
		}

		//TODO: Thunder whould be timed with a delay.
		//The delay should be calculated using the observerDistance/speedOfSound
		//TimeDelay = AudioListener.Vector3.Distance/343.2 meter per sec   

		//speed of sound  
		//343.2 metres per second (1,126 ft/s).
		//1,236 kilometres per hour (667 kn; 768 mph)

		//choose the clip based on the thunderscore.
		//average thunder with a chance of distant rolling thunder added in
		if(thisThunderScore > 100 && thisThunderScore < 399){ 

			thunderSources[1].GetComponent<AudioSource>().PlayOneShot(thunderRolls[2]);
			thunderSources[0].GetComponent<AudioSource>().PlayOneShot(thunderRolls[0],0.5f);
			//random chance is decided by creating a random number...
			float randChance = Random.Range (1, 10);
			//...and checking if it is an even number. Qucik. Dirty. Works.
			if(randChance%2==0){
				//distant thunder
				Debug.Log("added some oompf");
				thunderSources[0].GetComponent<AudioSource>().PlayOneShot(thunderRolls[0],1.5f);

			}
			//average thunder
			
		}
		//LOUD THUNDER
		if(thisThunderScore >= 400){

			thunderSources[1].GetComponent<AudioSource>().PlayOneShot(thunderRolls[1]);
			yield return new WaitForSeconds(0.5f);
			thunderSources[0].GetComponent<AudioSource>().PlayOneShot(thunderRolls[0]);
		}
		//distant rolling thunder
		if(thisThunderScore <= 99 && thisThunderScore > 20) {
			//normal volume
			yield return new WaitForSeconds(1);
			thunderSources[1].GetComponent<AudioSource>().PlayOneShot(thunderRolls[2]);
			thunderSources[0].GetComponent<AudioSource>().PlayOneShot(thunderRolls[0],0.3f);
			}else{
				//low volume, very faint thunder
				yield return new WaitForSeconds(2);
				thunderSources[1].GetComponent<AudioSource>().PlayOneShot(thunderRolls[2],0.5f);
			}


	


	}



	/// <summary>
	/// Lightnings the bolt. 
	/// Quick And Dirty Version.
	/// create a bolt of lighting by flashing the lightningLight x times with y time between fashes.
	/// set a random interval for each flash
	/// set a reandom intensity multiplier for each flash
	/// set the intiensity of the light with intensityMultiplier
	/// setBackTo DefaultIntensity
	/// /// 
	/// </summary>

	public IEnumerator lightningBolt(){
		
		//how many flashes
		int flashCount = Random.Range(1, 10);
		int thisFlash = 0;
		
		while(thisFlash < flashCount){
			
			lightningBulb.intensity = defaultIntensity * Random.Range(0, 100);
			yield return new WaitForSeconds(Random.Range (0.001f,0.1f));
			lightningBulb.intensity = defaultIntensity;
			thisFlash++;
		}

	}

}


