using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LocalPlayer : MonoBehaviour
{
	public static LocalPlayer Instance;

	public Transform LastFollower;
	public GameObject Win;

	public Player Player;

	[HideInInspector]
	public BordFollower Followee;
	[HideInInspector]
	public Vector3 Direction;
	private Vector3 LastPos;
	private bool grounded = true;
	private NaughtyCharacter.Character Character;

	private void Awake()
	{
		Instance = this;
	}

	void Start()
    {
		LastFollower = transform;
		UpdateText();

		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;

		LastPos = transform.position;
	}

	public void OnSpawn()
	{
		Character = FindObjectOfType<NaughtyCharacter.Character>();
	}

	void Update()
    {
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

		if ( Input.GetKeyDown( KeyCode.Escape ) )
		{
			if ( Cursor.visible )
			{
				Cursor.visible = false;
				Cursor.lockState = CursorLockMode.Locked;
			}
			else
			{
				Cursor.visible = true;
				Cursor.lockState = CursorLockMode.None;
			}
		}
		if ( Input.GetMouseButtonDown( 0 ) )
		{
			// Sound
			StaticHelpers.SpawnResourceAudioSource( "chirp" + Random.Range( 1, 5 ), transform.position, Random.Range( 0.8f, 1.2f ) );
			if ( Followee != null )
			{
				Followee.Chirp();
			}

			Player.GetComponentInChildren<Punchable>().Punch();

			// Check for close objects with tags
			GameObject obj = FindClosestEnemy();
			if ( obj != null )
			{
				// Delete object,
				Vector3 pos = obj.transform.position;
				Destroy( obj );

				GameObject par = StaticHelpers.EmitParticleDust( obj.transform.position );
				par.transform.localScale *= 2;

				// Spawn follower at object, and follow last
				GameObject follower = Instantiate( Resources.Load( "Prefabs/BordFollower" ) ) as GameObject;
				follower.transform.position = pos;
				follower.GetComponent<BordFollower>().ToFollow = LastFollower;
				if ( Followee != null && LastFollower != null && LastFollower.GetComponent<BordFollower>() != null )
				{
					LastFollower.GetComponent<BordFollower>().Followee = follower.GetComponent<BordFollower>();
				}
				LastFollower = follower.transform;
				if ( Followee == null )
				{
					Followee = LastFollower.GetComponent<BordFollower>();
				}

				UpdateText();
			}
		}
    }

	public GameObject FindClosestEnemy()
	{
		GameObject[] gos;
		gos = GameObject.FindGameObjectsWithTag( "Enemy" );
		GameObject closest = null;
		float distance = Mathf.Infinity;
		Vector3 position = transform.position;
		foreach ( GameObject go in gos )
		{
			Vector3 diff = go.transform.position - position;
			float curDistance = diff.sqrMagnitude;
			if ( curDistance < distance && curDistance < 15 )
			{
				closest = go;
				distance = curDistance;
			}
		}
		return closest;
	}

	void UpdateText()
	{
		var count = GameObject.FindGameObjectsWithTag( "Enemy" ).Length;
		FindObjectsOfType<Text>()[1].text = count + " cars remaining";

		if ( count <= 4 )
		{
			// TODO WIN
			Cursor.visible = true;
			Cursor.lockState = CursorLockMode.None;
			Win.SetActive( true );
		}
	}
}
