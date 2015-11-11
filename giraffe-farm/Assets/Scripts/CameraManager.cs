﻿using UnityEngine;
using System.Collections;

public class CameraManager : MonoBehaviour {

    private Transform m_ball;

	void Start () {
	
	}
	
	void Update () {
		if (GameManager.CurrentGameState == GameManager.GameState.Unstarted) return;
        
		if (m_ball == null)
        {
            m_ball = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        }
        else
        {
            Camera.main.transform.position = new Vector3(m_ball.position.x, m_ball.position.y + 2.0f, m_ball.position.z - 5.0f);
            Camera.main.transform.LookAt(m_ball);
        }
    }
}