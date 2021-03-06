﻿using UnityEngine;
using UnityStandardAssets.ImageEffects;
using System.Collections;

public class CameraManager : MonoBehaviour {

	private static CameraManager Instance;
	private Transform m_cameraMarker;
	private Vector3 m_courseCenter;
	private float m_radius = 5.0f;
	private float m_radiusMax = 10.0f;
	private float m_radiusMin = 5.0f;
	private float m_radiusSpeed = 0.5f;
	private bool m_initialGameStart;
	private bool m_gameCameraPositioned;
	private bool m_ignoreGameState;
	private float m_targetSaturation;

	public static bool GameCameraPositioned { get { return Instance.m_gameCameraPositioned; } }

	[SerializeField] private UnityStandardAssets.Cameras.AutoCam m_autoCamInstance;

	void Start () {
		if (Instance == null) {
			Instance = this;
		}

		m_targetSaturation = Camera.main.gameObject.GetComponent<ColorCorrectionCurves> ().saturation;
	}
	
	void FixedUpdate () {
		if (GameManager.CurrentGameState == GameManager.GameState.Unstarted || GameManager.CurrentGameState == 
			GameManager.GameState.Paused || (GameManager.CurrentGameState == GameManager.GameState.Ended && !m_ignoreGameState)) {
			m_autoCamInstance.transform.RotateAround (m_courseCenter, Vector3.up, 4.0f * Time.deltaTime);
			Vector3 desiredPosition = (m_autoCamInstance.transform.position - m_courseCenter).normalized * m_radius + m_courseCenter;

			//this is pretty hacky...
			if((GameManager.CurrentGameState == GameManager.GameState.Paused || GameManager.CurrentGameState == GameManager.GameState.Ended) 
			   && m_autoCamInstance.transform.position.y < 2.0f) {
				//Debug.Log("artifically increasing");
				//Instance.m_autoCamInstance.transform.LookAt (m_cameraMarker);

				desiredPosition.y += 0.5f;
			}

			if (Mathf.Approximately (m_autoCamInstance.transform.position.x - desiredPosition.x, 0.0f)) {
				if (m_radius == m_radiusMin) {
					m_radius = m_radiusMax;
				} else {
					m_radius = m_radiusMin;
				}
			}

			Instance.m_autoCamInstance.transform.position = Vector3.MoveTowards (m_autoCamInstance.transform.position, desiredPosition, Time.deltaTime * m_radiusSpeed);   
			//Instance.m_autoCamInstance.transform.ro (m_courseCenter);

			Vector3 targetPoint = m_courseCenter;
			if (PlayerManager.Ball != null) {
				targetPoint = PlayerManager.Ball.transform.position;
			}
			Quaternion targetRotation = Quaternion.LookRotation(targetPoint - Instance.m_autoCamInstance.transform.position, Vector3.up);
			Instance.m_autoCamInstance.transform.rotation = Quaternion.Slerp(Instance.m_autoCamInstance.transform.rotation, targetRotation, Time.deltaTime * 0.15f);  
		} else if (Input.GetKey (KeyCode.Q)) {
			m_autoCamInstance.transform.RotateAround (GameObject.FindGameObjectWithTag("Player").transform.position, Vector3.up, 4.0f * Time.deltaTime);
		} else {
			m_autoCamInstance.ManualUpdate();
		}
    }

	public static void MoveCameraRight() {
		Instance.m_autoCamInstance.transform.RotateAround (GameObject.FindGameObjectWithTag("Player").transform.position, Vector3.down, 50.0f * Time.deltaTime);
		if (!GameCameraPositioned) {
			SetGameCameraAsPositioned ();
		}
	}

	public static void MoveCameraLeft() {
		Instance.m_autoCamInstance.transform.RotateAround (GameObject.FindGameObjectWithTag("Player").transform.position, Vector3.up, 50.0f * Time.deltaTime);
		if (!GameCameraPositioned) {
			SetGameCameraAsPositioned ();
		}
	}

	public static void SetCameraPreGamePosition() {
		Vector3 courseCenter = CourseCreator.Course [CourseCreator.Course.Count / 2].transform.position;
		Instance.m_autoCamInstance.transform.position = new Vector3 (courseCenter.x - 8.0f, courseCenter.y + 6.0f, courseCenter.z);
		Instance.m_autoCamInstance.transform.LookAt (courseCenter);

		Instance.m_courseCenter = courseCenter;
	}

	public static void SetGameCameraAsPositioned() {
		Instance.m_gameCameraPositioned = true;
	}

	public static void Reset() {
		if (PlayerManager.IsRolling) {
			Instance.UnPauseWhileRolling();
			return;
		}

		Instance.m_gameCameraPositioned = false;
	}

	public static void HardReset() {
		Instance.m_autoCamInstance.SetOverrideForHardReset ();
		Instance.m_gameCameraPositioned = false;
	}

	public static void OnPause() {
		/*if (PlayerManager.IsRolling) {
			Instance.OnPauseWhileRolling();
			return;
		}*/

		Instance.m_autoCamInstance.SetSavedDirection(Instance.m_autoCamInstance.transform.forward);
	}

	private void UnPauseWhileRolling() {
		m_autoCamInstance.SetOverrideForMovingPause ();
	}

	public static void SetIgnoreGameState(bool ignore) {
		Instance.m_ignoreGameState = ignore;
	}

	public static void FadeCameraOnLaunch() {
		Instance.StartCoroutine ("CameraFadeInCoroutine");
	}

	public static void FadeCamera() {
		Instance.StartCoroutine ("CameraFadeOutCoroutine");
	}

	public static void BlurBackgroundOnPause() {
		Instance.StartCoroutine ("BackgroundBlurInCoroutine");
	}

	public static void UnblurBackgroundOnPause() {
		Instance.StartCoroutine ("BackgroundBlurOutCoroutine");
	}

	IEnumerator CameraFadeOutCoroutine () {
		UIManager.DisplayCourseControls (false);
		VignetteAndChromaticAberration vignette = Camera.main.gameObject.GetComponent<VignetteAndChromaticAberration> ();
		float t = 0.0f;
		while(t < 1.01f) {
			vignette.intensity = Mathf.Lerp(0.0f, 1.0f, t / 1.0f);
			t += 0.75f * Time.deltaTime;
			yield return null;
		}	

		yield return new WaitForSeconds (0.75f);

		CourseCreator.GenerateNewCourse ();

		StartCoroutine ("CameraFadeInCoroutine");
	}

	IEnumerator CameraFadeInCoroutine () {
		VignetteAndChromaticAberration vignette = Camera.main.gameObject.GetComponent<VignetteAndChromaticAberration> ();
		float t = 0.0f;
		while(t < 1.01f) {
			vignette.intensity = Mathf.Lerp(1.0f, 0.0f, t / 1.0f);
			t += 0.75f * Time.deltaTime;
			yield return null;
		}

		UIManager.DisplayCourseControls (true);
	}

	IEnumerator BackgroundBlurOutCoroutine() {
		BlurOptimized blur = Camera.main.gameObject.GetComponent<BlurOptimized> ();
		ColorCorrectionCurves colorCorrection = Camera.main.gameObject.GetComponent<ColorCorrectionCurves> ();
		VignetteAndChromaticAberration vignette = Camera.main.gameObject.GetComponent<VignetteAndChromaticAberration> ();
		float t = 0.0f;
		while (t < 1.01f) {
			blur.blurSize = Mathf.Lerp(4.0f, 0.0f, t / 1.0f);
			colorCorrection.saturation = Mathf.Lerp (0.0f, m_targetSaturation, t / 1.0f);
			vignette.intensity = Mathf.Lerp(0.3f, 0.0f, t / 1.0f); 
			t += 1.0f * Time.deltaTime;
			yield return null;
		}
		blur.enabled = false;
	}

	IEnumerator BackgroundBlurInCoroutine() {
		BlurOptimized blur = Camera.main.gameObject.GetComponent<BlurOptimized> ();
		ColorCorrectionCurves colorCorrection = Camera.main.gameObject.GetComponent<ColorCorrectionCurves> ();
		VignetteAndChromaticAberration vignette = Camera.main.gameObject.GetComponent<VignetteAndChromaticAberration> ();
		blur.enabled = true;
		float t = 0.0f;
		while (t < 1.01f) {
			blur.blurSize = Mathf.Lerp(0.0f, 4.0f, t / 1.0f);
			colorCorrection.saturation = Mathf.Lerp (m_targetSaturation, 0.0f, t / 1.0f);
			vignette.intensity = Mathf.Lerp(0.0f, 0.3f, t / 1.0f); 
			t += 1.0f * Time.deltaTime;
			yield return null;
		}
	}
}
