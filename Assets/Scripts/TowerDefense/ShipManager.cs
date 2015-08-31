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
/// This class manages spawning ships that attack the player. It only spawns ships when the game starts and stop when the game ends. 
/// NOTE: THIST MUST BE ATTACHED TO THE ENVIRONMENT TO GET THE HEIGHT AND CENTER OF THE ISLAND
/// </summary>
public class ShipManager : MonoBehaviour {

	private float outerSpawnRadius = 500f; //the distance at which ships are spwaned from the center of the island
	private float MinStopDist = 30f; // minimum distance the ship stops from the island
	private float MaxStopDist = 50f; //max distance ship stops from the island

	public static float TravelTime = 5f; //Time it takes for ship to travel to island

	public Ship shipPrefab; //the prefab to spawn

	public static ShipManager Instance; //the instance variable to access this manager globally within the game

	/// <summary>
	/// First method that is called an sets up the global variable
	/// </summary>
	void Awake () {
		if (Instance == null)
			Instance = this;
	}

	/// <summary>
	/// Method that is called when fully initialized. This is sets up events to listen for the start and stop of the game
	/// </summary>
	void Start(){
		ScoreManager.OnGameStart += OnGameBegins;
		ScoreManager.OnGameEnd += OnGameEnds;
	}

	/// <summary>
	/// Method that is called when object is destroy. This is stops listening for the start and stop of the game
	/// </summary>
	void OnDestroy(){
		ScoreManager.OnGameStart -= OnGameBegins;
		ScoreManager.OnGameEnd -= OnGameEnds;
	}

	/// <summary>
	/// Starts the spawn ship coroutine that handles spawning ships for the duration of the game
	/// </summary>
	private void OnGameBegins(){
		StartCoroutine (SpawnShips ());
	}

	/// <summary>
	/// Stops the spawn ship coroutine at the end of the game
	/// </summary>
	private void OnGameEnds(){
		StopAllCoroutines ();
	}

	/// <summary>
	/// Coroutine that handles spawning ships at random locations based on the paramters above. 
	/// It also uses iTween to move ships to the proper location and waits for ships to get there before
	/// spawning another ship.
	/// </summary>
	IEnumerator SpawnShips(){
		while (true) {
			float angle = Random.Range(0f, Mathf.PI*2f);
			Vector3 pos = gameObject.transform.position; //height set by position of prefab
			pos.x = Mathf.Cos(angle)*outerSpawnRadius;
			pos.z = Mathf.Sin(angle)*outerSpawnRadius;

			GameObject shipObj = Instantiate(shipPrefab.gameObject, pos, new Quaternion()) as GameObject;
			Vector3 dest = pos - Camera.main.transform.position;
			dest.y = 0f;
			dest.Normalize();
			dest *= Random.Range(MinStopDist, MaxStopDist);
			dest.y = pos.y; //maintain same height
			iTween.MoveTo(shipObj, iTween.Hash("position", dest, "easeType", "easeOutQuad", "time", TravelTime));
			yield return new WaitForSeconds(TravelTime + 5f);
		}
	}
}