using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;

public class Bouncer : Photon.MonoBehaviour
{
	public static float Speed = 20;

	private Punchable Punch;

	private void Start()
	{
		Punch = gameObject.AddComponent( typeof( Punchable ) ) as Punchable;
		gameObject.AddComponent( typeof( PhotonView ) );
		photonView.viewID = Mathf.Abs( Mathf.CeilToInt( transform.position.x + transform.position.y * 10 + transform.position.z * 100 ) );
	}

	private void OnTriggerEnter( Collider collider )
	{
		var ply = collider.transform.GetComponentInParent<Player>();
		if ( LocalPlayer.Instance.Player != null && ply != null && ply == LocalPlayer.Instance.Player )
		{
			FindObjectOfType<NaughtyCharacter.Character>().SetUpVelocity( Speed );
			photonView.RPC( "SendEffect", PhotonTargets.All, ply.transform.position );
		}
	}

	[PunRPC]
	void SendEffect( Vector3 pos )
	{
		// Effects
		StaticHelpers.GetOrCreateCachedAudioSource( "boing", LocalPlayer.Instance.Player.transform, Random.Range( 0.8f, 1.2f ), 1, 0, true );
		GameObject par = StaticHelpers.EmitParticleDust( pos );
		par.transform.localScale *= 2;
		Punch.Punch();
	}
}
