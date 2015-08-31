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
///  This class manages events that occur on the player object. Specifically subtracting points and causing the explosion when hit
/// </summary>
[RequireComponent (typeof(Collider))]
public class Player : MonoBehaviour {

	//the type of explosion that happens on the player
	public Detonator explosionPrefab; 
	//whether there is a current explosion; since the explosion is deleted after a set time, it will automatically turn to null when the explosion is deleted
	private GameObject currExplosion; 

	/// <summary>
	///  Event that is called when a cannon hits the player.
	/// </summary>
	/// <param name="ball">The cannonball that hit the player</param>
	public void OnPlayerHit(Cannonball ball) {
		ScoreManager.Instance.SubtractScore (ScoreManager.PlayerHitReduction);

		if (currExplosion == null)
			currExplosion = Instantiate (explosionPrefab.gameObject, transform.position, new Quaternion ()) as GameObject;

	}
}