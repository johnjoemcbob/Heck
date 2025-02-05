﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LocalPlayer : MonoBehaviour
{
	public static LocalPlayer Instance;

	public GameObject Camera;

	public Player Player;

	[HideInInspector]
	public Vector3 Direction;
	private Vector3 LastPos;
	private bool grounded = true;

	private void Awake()
	{
		Instance = this;
	}

	void Start()
    {
		UpdateText();

		LastPos = transform.position;
	}

	public void OnSpawn()
	{
		Player.SetAnimal( Player.Animal.Chick );
	}

	void Update()
    {
		if ( Player == null ) return;

		var current =  GetComponent<NaughtyCharacter.Character>().IsGrounded;
		if ( !grounded && current )
		{
			StaticHelpers.EmitParticleDust( transform.position );
		}
		grounded = current;

		if ( LastPos != transform.position )
		{
			Direction = LastPos - transform.position;
			LastPos = transform.position;
		}

		if ( Input.GetKeyDown( KeyCode.F2 ) )
		{
			Player.NextAnimal();
		}
		//if ( Input.GetKeyDown( KeyCode.Escape ) )
		//{
		//	if ( Cursor.visible )
		//	{
		//		Cursor.visible = false;
		//		Cursor.lockState = CursorLockMode.Locked;
		//	}
		//	else
		//	{
		//		Cursor.visible = true;
		//		Cursor.lockState = CursorLockMode.None;
		//	}
		//}
		if ( Input.GetMouseButtonDown( 0 ) )
		{
			Player.ChirpLocal();
		}
    }

	void UpdateText()
	{
		//var count = GameObject.FindGameObjectsWithTag( "Enemy" ).Length;
		//FindObjectsOfType<Text>()[1].text = count + " cars remaining";

		//if ( count <= 4 )
		//{
		//	// TODO WIN
		//	Cursor.visible = true;
		//	Cursor.lockState = CursorLockMode.None;
		//	Win.SetActive( true );
		//}
	}
}
