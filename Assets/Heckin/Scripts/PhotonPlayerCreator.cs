using UnityEngine;
using System.Collections;

public class PhotonPlayerCreator : MonoBehaviour
{
	public static Vector3 Spawn = new Vector3( 0, 0, 0 );

	void OnJoinedRoom()
    {
        CreatePlayerObject();
	}

    void CreatePlayerObject()
    {

        GameObject newPlayerObject = PhotonNetwork.Instantiate( "BordPlayer", Spawn, Quaternion.identity, 0 );
		if ( LocalPlayer.Instance.Player == null )
		{
			LocalPlayer.Instance.Player = newPlayerObject.GetComponentInChildren<Player>();
			LocalPlayer.Instance.Player.OnJoined();
			LocalPlayer.Instance.OnSpawn();
		}

		//Camera.Target = newPlayerObject.transform;
		var character = FindObjectOfType<NaughtyCharacter.Character>( true );
		character._characterController = newPlayerObject.GetComponentInChildren<CharacterController>( true );
		character._characterAnimator = newPlayerObject.GetComponentInChildren<NaughtyCharacter.CharacterAnimator>( true );
		newPlayerObject.GetComponentInChildren<NaughtyCharacter.CharacterAnimator>( true )._character = character;
		character.transform.SetParent( newPlayerObject.transform );
		character.transform.localPosition = Vector3.zero;
	}
}
