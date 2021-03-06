using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooting : MonoBehaviour 
{

	public int gunDamage = 1;		//amount of damage dealt to an object
	public float fireRate = .05f;	//how fast the gun can be shot
	public float weaponRange = 50f;	//the max range that the "bullet" can travel
	public float hitForce = 10000f;	//the amount of force applied to a rigid body
	public Transform gunEnd;		//marks the posistion at end of gun where laser line will begin
	public ParticleSystem muzzleFlash;


	private Camera fpsCam;
	private WaitForSeconds shotDuration = new WaitForSeconds (.07f);	//determine 
	private AudioSource gunAudio;	//gun sounds audio
	private LineRenderer laserLine;	//takes two or more points in 3D space and draws a straight line between the two
	private float nextFire;			//will hold time at which player will be allowed to fire again after firing



	void Start () 
	{
		laserLine = GetComponent<LineRenderer> ();
		gunAudio = GetComponent<AudioSource> ();
		fpsCam = GetComponentInParent<Camera> (); //search gameObject attached to it, then the parents


	}
	
	void Update () 
	{
		if (Input.GetButtonDown ("Fire1") && Time.time > nextFire) 
		{
			nextFire = Time.time + fireRate;

			StartCoroutine (ShotEffect ());

			Vector3 rayOrigin = fpsCam.ViewportToWorldPoint (new Vector3 (.5f, .5f, 0));	//takes a position relative to the camera and converts it to a point in world space
			RaycastHit hit;	//gets the information of what was hit

			laserLine.SetPosition (0, gunEnd.position);

			if (Physics.Raycast (rayOrigin, fpsCam.transform.forward, out hit, weaponRange)) 
			{
				laserLine.SetPosition (1, hit.point);

				ZombieHealth health = hit.collider.GetComponent<ZombieHealth> ();

				if (health != null) 
				{
					health.Damage (gunDamage);
				}
				if (hit.rigidbody != null) 
				{
					hit.rigidbody.AddForce (-hit.normal * hitForce);
				}
			} 
			else 
			{
				laserLine.SetPosition (1, rayOrigin + (fpsCam.transform.forward * weaponRange));
					
			}
		}
	}

	private IEnumerator ShotEffect()	//co-routine
	{
		gunAudio.Play ();
		muzzleFlash = GetComponentInChildren<ParticleSystem>();
		muzzleFlash.Play();
		yield return shotDuration;	//wait .07s before it turns laser back off. Causes co-routine to wait .07s
	}
		

}
