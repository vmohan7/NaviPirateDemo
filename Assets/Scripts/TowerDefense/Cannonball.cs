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
///  This class manages what happens to the bomb depending on what it hits and which way it is going
/// </summary>
public class Cannonball : MonoBehaviour {

	[HideInInspector]
	public Vector3 source; //set by the ship so the bomb knows where to return to

	//the direction the bomb is heading (either towards the player or the ship
	private bool OnBallReturning = false;

	/// <summary>
	///  Method that is called when the bomb collides with another object
	/// </summary>
	/// <param name="other">The collider object that hit the player</param>
	void OnTriggerEnter(Collider other){
		if (other.CompareTag("Bomb")) {
			return; //if we collide with a cannonball, ignore it
		}

		if (other.CompareTag("Ship") && OnBallReturning) {
			other.GetComponent<Ship>().OnShipHit(this);
			Destroy(this.gameObject); //remove cannonball
			return;
		}


		if (other.CompareTag("Player")) {
			other.GetComponent<Player>().OnPlayerHit(this);
			Destroy(this.gameObject); //remove cannonball
			return;
		}

		if (other.CompareTag("Tablet")) {
			OnBallReturning = true;

			Vector3 midPoint = (transform.position + source) / 2f;
			midPoint.y += Ship.CannonArcHeight;

			//send back to source
			iTween.MoveTo (this.gameObject, iTween.Hash ("position", source, "easeType", "spring", "speed", Ship.CannonFireSpeed, "path", new Vector3[] {
				transform.position,
				midPoint,
				source
			}, "oncomplete", "OnBombMissed", "oncompletetarget", gameObject));


		}
	}

	/// <summary>
	///  Method that is called after a set time if the bomb has not collided with anything
	/// </summary>
	private void OnBombMissed(){
		Destroy(this.gameObject); //remove cannonball
	}
}