﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIManager : MonoBehaviour {

	private static UIManager Instance;

	public Text m_hitCounter;
	public Text m_postGameHitCount;
	public Text m_postGameTimeStat;
	public Text m_title;

	public Button m_startGameButton;
	public Button m_pauseGameButton;

	public GameObject m_postCourseStats;
	public GameObject m_cameraNavigatorAnchor;
	public GameObject m_pauseMenu;
	
	void Start () {
		if (Instance == null) {
			Instance = this;
		}
	}

	public static void UpdateHits(int newHitCount) {
		Instance.m_hitCounter.text = "Hits: " + newHitCount;
	}

	public static void SetupPregame() {
		Instance.m_hitCounter.gameObject.SetActive (false);
		Instance.m_pauseGameButton.gameObject.SetActive (false);
		Instance.m_cameraNavigatorAnchor.gameObject.SetActive (false);
		Instance.m_postCourseStats.gameObject.SetActive (false);
		Instance.m_pauseMenu.gameObject.SetActive (false);
		Instance.m_startGameButton.gameObject.SetActive (true);
		Instance.m_title.gameObject.SetActive (true);
	}

	public static void OnStart() {
		Instance.m_startGameButton.gameObject.SetActive (false);
		Instance.m_title.gameObject.SetActive (false);
		Instance.m_cameraNavigatorAnchor.gameObject.SetActive (true);
		Instance.m_hitCounter.gameObject.SetActive (true);
		Instance.m_pauseGameButton.gameObject.SetActive (true);
	}

	public static void SetupPostGame(GameManager.GameStats gameStats) {
		Instance.m_hitCounter.gameObject.SetActive (false);
		Instance.m_cameraNavigatorAnchor.gameObject.SetActive (false);
		Instance.m_pauseGameButton.gameObject.SetActive (false);
		Instance.m_postCourseStats.gameObject.SetActive (true);

		//set stats
		Instance.m_postGameHitCount.text = "Hits: " + gameStats.GetNumHits ();
		Instance.m_postGameTimeStat.text = "Time: " + gameStats.GetTimePlayed ();
	}

	public static void OnReset() {
		Instance.m_pauseMenu.gameObject.SetActive (false);
		Instance.m_postCourseStats.gameObject.SetActive (false);
		Instance.m_pauseGameButton.gameObject.SetActive (true);
		Instance.m_hitCounter.gameObject.SetActive (true);
		Instance.m_cameraNavigatorAnchor.gameObject.SetActive (true);
	}

	public static void OnPause() {
		Instance.m_cameraNavigatorAnchor.gameObject.SetActive (false);
		Instance.m_hitCounter.gameObject.SetActive (false);
		Instance.m_pauseGameButton.gameObject.SetActive (false);
		Instance.m_pauseMenu.gameObject.SetActive (true);
	}

	public static void OnUnpause() {
		Instance.m_pauseMenu.gameObject.SetActive (false);
		Instance.m_cameraNavigatorAnchor.gameObject.SetActive (true);
		Instance.m_hitCounter.gameObject.SetActive (true);
		Instance.m_pauseGameButton.gameObject.SetActive (true);
	}
}
