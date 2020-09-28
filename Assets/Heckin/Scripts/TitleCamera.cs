using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleCamera : MonoBehaviour
{
	public static TitleCamera Instance;

	public float Speed = 0.5f;
	public float Distance = 1;

	[HideInInspector]
	public float LerpStartTime = -1;
	[HideInInspector]
	public bool LerpIn = true;

	private Vector3 BasePos;
	private Vector3 StartLerpPos;
	private Quaternion StartLerpRotation;

	private void Awake()
	{
		Instance = this;
	}

    void Update()
    {
		if ( BasePos == Vector3.zero )
		{
			BasePos = transform.localPosition;
		}

		if ( LerpStartTime == -1 )
		{
			var target = BasePos +
				new Vector3( 1, 0, 0 ) * Mathf.Sin( Time.time * Speed ) * Distance +
				new Vector3( 0, 0, 1 ) * Mathf.Cos( Time.time * -Speed ) * Distance;
			transform.localPosition = Vector3.Lerp( transform.localPosition, target, Time.deltaTime * 5 );
			StartLerpPos = transform.position;
			StartLerpRotation = transform.rotation;
		}
		else
		{
			var progress = ( Time.time - LerpStartTime ) / Game.TRANS_LERP_TIME;
			if ( LerpIn )
			{
				transform.position = Vector3.Lerp( StartLerpPos, LocalPlayer.Instance.Camera.transform.position, progress );
				transform.rotation = Quaternion.Lerp( StartLerpRotation, LocalPlayer.Instance.Camera.transform.rotation, progress );

				if ( progress >= 1 )
				{
					var y = BasePos.y;
					BasePos = LocalPlayer.Instance.transform.position;
					BasePos.y = y;

					StartLerpPos = BasePos;
				}
			}
			else
			{
				transform.position = Vector3.Lerp( LocalPlayer.Instance.Camera.transform.position, StartLerpPos, progress );
				transform.rotation = Quaternion.Lerp( LocalPlayer.Instance.Camera.transform.rotation, StartLerpRotation, progress );

				if ( progress >= 1 )
				{
					LerpStartTime = -1;
				}
			}
		}
	}
}
