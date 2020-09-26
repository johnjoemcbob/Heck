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

	private PhotonView PhotonView;

	[HideInInspector]
	public Transform LastFollower;
	[HideInInspector]
	public BordFollower Followee;

	void Awake()
    {
		PhotonView = GetComponent<PhotonView>();

		LastFollower = transform;
	}

	public void OnJoined()
	{
		// Request names from other players
		PhotonView.RPC( "RequestName", PhotonTargets.Others, photonView.viewID );
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
	void RequestName( int viewID )
	{
		// Send my name directly back to the player who requested the info
		LocalPlayer.Instance.Player.photonView.RPC( "SendName", PhotonPlayer.Find( photonView.viewID ), LocalPlayer.Instance.Player.Name );
	}

	void UpdateUI()
	{
		//NameTag.text = Name;
	}

	public void ChirpLocal()
	{
		// Sound
		StaticHelpers.SpawnResourceAudioSource( "chirp" + Random.Range( 3, 5 ), transform.position, Random.Range( 0.8f, 1.2f ) );
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

			PhotonView.RPC( "SendSpawnFollower", PhotonTargets.All, pos );
		}
		Chirp( obj != null );
	}

	[PunRPC]
	public void SendSpawnFollower( Vector3 pos )
	{
		GameObject par = StaticHelpers.EmitParticleDust( pos );
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
		StaticHelpers.SpawnResourceAudioSource( "chirp" + Random.Range( 1, 5 ), transform.position, Random.Range( 0.8f, 1.2f ) );
		//if ( Followee != null )
		//{
		//	Followee.Chirp();
		//}

		GetComponentInChildren<Punchable>().Punch();
	}

	#region Animation Events
	public void Footstep()
	{
		StaticHelpers.SpawnResourceAudioSource( "footstep", transform.position, Random.Range( 0.8f, 1.2f ), 0.2f );
	}

	public void Flap()
	{
		StaticHelpers.SpawnResourceAudioSource( "flap" + Random.Range( 1, 4 ), transform.position, Random.Range( 0.8f, 1.2f ), 0.5f );
	}
	#endregion
}
