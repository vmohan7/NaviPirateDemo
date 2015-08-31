/*
 * This file is part of NaviPirateDemo.
 * Copyright 2015 Vasanth Mohan. All Rights Reserved.
 * 
 * NaviPirateDemo is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * NaviPirateDemo is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with NaviPirateDemo.  If not, see <http://www.gnu.org/licenses/>.
 */

using UnityEngine;
using System.Collections;

/// <summary>
/// This class manages the life span of the ship. This includes shooting bombs and getting destroyed.
/// </summary>
public class Ship : MonoBehaviour {

	public Cannonball cannonBallPrefab; //ball to attack with
	public Detonator explosionPrefab; //explosion effect that fires when ship is hit
	public Detonator cannonFirePrefab; //explosion effect that fires when ball is shot
	public Transform cannonLocation; //the location on the prefab where the cannon should fire from
	public AudioSource cannonFireSound; //the sound clip to play when a ball is shot

	public AudioSource shipExplosion; //the sound clip to play when the ship is destroyed

	public static float CannonArcHeight = 10f; //the peak of the cannon ball's trajectory
	public static float CannonFireSpeed = 20f; //the speed at which the cannon ball travels

	private static float SinkTime = 5f; //the time it takes for the ship to sink in seconds
	private static float SinkDist = 10f; //the amount of distance the ships sinks when destroyed

	/// <summary>
	/// Method that is called to init everything about the ship
	/// </summary>
	void Start () {
		this.transform.LookAt (Camera.main.transform.position);
		StartCoroutine (ShootCannonBall ());

		ScoreManager.OnGameEnd += OnGameEnds;
	}

	/// <summary>
	/// Method that is called when the ship is destroyed
	/// </summary>
	void OnDestroy(){
		ScoreManager.OnGameEnd -= OnGameEnds;
	}

	/// <summary>
	/// Callback to destroy this ship when the game ends
	/// </summary>
	private void OnGameEnds(){
		Destroy (this.gameObject);
	}

	/// <summary>
	/// Coroutine to fire cannonballs. It waits for the ship to arrive at its destination and then shoots every 2 seconds
	/// </summary>
	IEnumerator ShootCannonBall(){
		yield return new WaitForSeconds(ShipManager.TravelTime);
		while (true) {
			yield return new WaitForSeconds(2f); //delay between shots
			FireBall();
		}
	}

	/// <summary>
	/// Method to control the effect of the cannonball firing and moving
	/// </summary>
	private void FireBall(){
		cannonFireSound.Play (); //play sound clip
		Instantiate (cannonFirePrefab.gameObject, cannonLocation.position, new Quaternion ()); //create smoke effect
		GameObject cannonBall = Instantiate(cannonBallPrefab.gameObject, cannonLocation.position, new Quaternion() ) as GameObject;
		cannonBall.GetComponent<Cannonball>().source = transform.position;//set return location
		Vector3 dest = Camera.main.transform.position; //TODO use a class as oppsed to Camera.main
		
		Vector3 midPoint = (transform.position + dest) /2f;
		midPoint.y += CannonArcHeight;
		
		iTween.MoveTo(cannonBall, iTween.Hash("position", dest, "easeType", "spring", "speed", CannonFireSpeed, "path", new Vector3[] {cannonLocation.position, midPoint, dest} ));
	}

	/// <summary>
	/// Method callback for when the cannonball hits the ship. This method adds some randomness to the effect of the ship sinking.
	/// </summary>
	public void OnShipHit(Cannonball ball){
		ScoreManager.Instance.AddScore (ScoreManager.ShipHitIncrease);

		StopAllCoroutines ();

		shipExplosion.Play ();

		Instantiate (explosionPrefab.gameObject, transform.position, new Quaternion ()); //explosion
		Vector3 dirEuler = (new Vector3 (Random.Range (0f, 1f), Random.Range (0f, 1f), Random.Range (0f, 1f))).normalized*90f;

		Vector3 dest = transform.position;
		dest.y -= SinkDist;

		iTween.MoveTo(gameObject, iTween.Hash("position", dest, "easeType", "linear", "time", SinkTime, "oncomplete", "OnSinkComplete", "oncompletetarget", gameObject));
		iTween.RotateTo(gameObject, iTween.Hash("rotation", dirEuler, "easeType", "linear", "time", SinkTime));
	}

	/// <summary>
	/// When the ship is sunk, we destroy the gameobject to clear space. 
	/// </summary>
	private void OnSinkComplete(){
		Destroy (this.gameObject);
	}
}