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

    void Awake()
    {
		PhotonView = GetComponent<PhotonView>();
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
