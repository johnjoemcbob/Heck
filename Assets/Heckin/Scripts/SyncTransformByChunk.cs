using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;

public class SyncTransformByChunk : Photon.MonoBehaviour
{
	string parentName = "";
	Vector3 realPosition = Vector3.zero;
	Quaternion realRotation = Quaternion.identity;

	void Update()
	{
		if ( photonView.isMine )
		{

		}
		else
		{
			transform.localPosition = Vector3.Lerp( transform.localPosition, this.realPosition, 0.1f );
			transform.localRotation = Quaternion.Lerp( transform.localRotation, this.realRotation, 0.1f );
		}
	}

	void OnPhotonSerializeView( PhotonStream stream, PhotonMessageInfo info )
	{
		if ( stream.isWriting )
		{
			stream.SendNext( transform.parent ? transform.parent.name : "Park" );
			stream.SendNext( transform.localPosition );
			stream.SendNext( transform.localRotation );
		}
		else
		{
			this.parentName = (string) stream.ReceiveNext();
			this.realPosition = (Vector3) stream.ReceiveNext();
			this.realRotation = (Quaternion) stream.ReceiveNext();

			// Parent here
			Debug.Log( "Receive parent of other; " + this.parentName );
			transform.parent = GameObject.Find( this.parentName ).transform;
		}
	}
}
