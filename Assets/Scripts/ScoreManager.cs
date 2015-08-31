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
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// This class controls sending events about the follow of the game to the system. It also keeps track of the score.
/// Games are started by tapping with 3 fingers.
/// </summary>
[RequireComponent (typeof(AudioSource))]
public class ScoreManager : MonoBehaviour {

	public delegate void GameEventAction();
	public static event GameEventAction OnGameStart;
	public static event GameEventAction OnGameEnd;
	
	public const int PlayerHitReduction = 50; //score reduction when a player is hit
	public const int ShipHitIncrease = 1000; //points when you take out a ship (a lot I know but I want people to be happy =D)

	private int Score; //keeps track of your score
	private int TimeLeft; //keeps track of time left in game

	private const string scoreString = "Score: "; //string to display to user

	public static ScoreManager Instance; //global variable to access the score manager

	public AudioClip startGameClip; //sound clip to play at the start of the game
	public AudioClip endGameClip; //sound clip to indicate the end of the game

	private AudioSource audioSource; //source to play sound from

	public Text startText; //text to give instructions on how to start the game
	public Text scoreCard; //text to display how many points the user has
	public Text timeCard; //text to display time left

	/// <summary>
	/// First method that is called an sets up the global variable
	/// </summary>
	void Awake () {
		if (Instance == null)
			Instance = this;
	}
		
	/// <summary>
	/// Method used for initalizing the instance 
	/// </summary>
	void Start () {
		Score = 0;
		scoreCard.text = scoreString + Score;

		audioSource = GetComponent<AudioSource> ();

		GestureManager.OnThreeFingerTap += OnThreeFingerBegin;
	}

	/// <summary>
	/// Callback for when the user taps with 3 fingers. This will start the game. 
	/// </summary>
	private void OnThreeFingerBegin(){
		if (OnGameStart != null)
			OnGameStart ();

		Score = 0;
		scoreCard.text = scoreString + Score;

		TimeLeft = 120;

		timeCard.gameObject.SetActive (true);
		startText.gameObject.SetActive (false);
		StartCoroutine (UpdateScoreboard());

		audioSource.clip = startGameClip;
		audioSource.Play ();

		GestureManager.OnThreeFingerTap -= OnThreeFingerBegin; //makes sure we cannot start the game again
	}

	/// <summary>
	/// Couroutine the update how much time is left and dispatch an event when the game is over. 
	/// </summary>
	private IEnumerator UpdateScoreboard(){
		while (TimeLeft >= 0) {

			timeCard.text = "Time: " + TimeLeft;

			TimeLeft--;

			yield return new WaitForSeconds(1);
		}

		OnTimeUp ();
	}

	/// <summary>
	/// Ends the game and resets the board
	/// </summary>
	private void OnTimeUp(){
		if (OnGameEnd != null)
			OnGameEnd ();

		audioSource.clip = endGameClip;
		audioSource.Play ();
		
		startText.gameObject.SetActive (true);
		timeCard.gameObject.SetActive (false);
		
		GestureManager.OnThreeFingerTap += OnThreeFingerBegin;
	}

	/// <summary>
	/// Public method to add score to the total
	/// </summary>
	public void AddScore(int score){
		if (TimeLeft > 0)
			Score += score;
		scoreCard.text = scoreString + Score;
	}

	/// <summary>
	/// Public method to subtract score from the total
	/// </summary>
	public void SubtractScore(int score){
		if (TimeLeft > 0)
			Score -= score;
		scoreCard.text = scoreString + Score;
	}
	
}