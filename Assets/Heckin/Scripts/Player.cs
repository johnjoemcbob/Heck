using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon;
using ExitGames.Client.Photon;

public class Player : PunBehaviour
{
	[Header( "Variables" )]
	public string Name;

	[Header( "References" )]
	public Text NameTag;
	public GameObject[] Animals;

	private PhotonView PhotonView;

	[HideInInspector]
	public Transform LastFollower;
	[HideInInspector]
	public BordFollower Followee;

	public enum Animal
	{
		Chick,
		Parrot,
		Duck,
		Hen,
		Crow,
		Hornbill,
		Owl,
		Flamingo,
		Ostrich,
		Count
	}
	Animal CurrentAnimal;

	void Awake()
    {
		PhotonView = GetComponent<PhotonView>();

		LastFollower = transform;

		// Select first animal
		foreach ( var anim in Animals )
		{
			anim.SetActive( false );
		}
	}

	public void OnJoined()
	{
		// Request sync from other players on late join
		PhotonView.RPC( "RequestSync", PhotonTargets.Others, photonView.viewID );
	}

	public void SetName( string name )
	{
		Name = name;
		NameTag.text = "";

		PhotonView.RPC( "SendName", PhotonTargets.Others, name );
	}

	[PunRPC]
	void SendName( string name )
	{
		Name = name;
		UpdateUI();
	}

	[PunRPC]
	void RequestSync( int viewID )
	{
		// Send my name directly back to the player who requested the info
		LocalPlayer.Instance.Player.photonView.RPC( "SendAnimal", PhotonPlayer.Find( photonView.viewID ), LocalPlayer.Instance.Player.CurrentAnimal );
	}

	void UpdateUI()
	{
		//NameTag.text = Name;
	}

	public void ChirpLocal()
	{
		// Sound
		StaticHelpers.GetOrCreateCachedAudioSource( "chirp" + Random.Range( 3, 5 ), transform.position, Random.Range( 0.8f, 1.2f ), 1, 0, true );
		if ( Followee != null )
		{
			Followee.Chirp();
		}

		GetComponentInChildren<Punchable>().Punch();

		// Check for close objects with tags
		GameObject obj = FindClosestEnemy();
		if ( obj != null )
		{
			// Delete object,
			Vector3 pos = obj.transform.position;
			Destroy( obj );

			PhotonView.RPC( "SendSpawnFollower", PhotonTargets.All, pos, CurrentAnimal );
		}
		Chirp( obj != null );
	}

	[PunRPC]
	public void SendSpawnFollower( Vector3 pos, Animal animal )
	{
		GameObject par = StaticHelpers.EmitParticleDust( pos );
		par.transform.localScale *= 2;

		// Spawn follower at object, and follow last
		GameObject follower = Instantiate( Resources.Load( "Prefabs/BordFollower" ) ) as GameObject;
		follower.transform.position = pos;
		follower.GetComponent<BordFollower>().ToFollow = LastFollower;
		follower.GetComponent<BordFollower>().SetAnimal( animal );
		if ( Followee != null && LastFollower != null && LastFollower.GetComponent<BordFollower>() != null )
		{
			LastFollower.GetComponent<BordFollower>().Followee = follower.GetComponent<BordFollower>();
		}
		LastFollower = follower.transform;
		if ( Followee == null )
		{
			Followee = LastFollower.GetComponent<BordFollower>();
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

	public void Chirp( bool addfollower )
	{
		PhotonView.RPC( "SendChirp", PhotonTargets.Others, addfollower );
	}

	[PunRPC]
	void SendChirp( bool addfollower )
	{
		// Sound
		StaticHelpers.GetOrCreateCachedAudioSource( "chirp" + Random.Range( 3, 5 ), transform.position, Random.Range( 0.8f, 1.2f ), 1, 0, true );
		if ( Followee != null )
		{
			Followee.Chirp();
		}

		GetComponentInChildren<Punchable>().Punch();
	}

	public void NextAnimal()
	{
		var anim = CurrentAnimal + 1;
		if ( anim == Animal.Count )
		{
			anim = Animal.Chick;
		}
		SetAnimal( anim );
	}

	public void SetAnimal( Animal anim, bool fromremote = false )
	{
		if ( !fromremote )
		{
			PhotonView.RPC( "SendAnimal", PhotonTargets.Others, anim );
		}

		Animals[(int) CurrentAnimal].SetActive( false );
		CurrentAnimal = anim;
		Animals[(int) CurrentAnimal].SetActive( true );

		GetComponent<NaughtyCharacter.CharacterAnimator>().UpdateAnimal();
	}

	[PunRPC]
	void SendAnimal( Animal anim )
	{
		SetAnimal( anim, true );
	}
}
