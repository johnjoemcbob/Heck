using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Heck
{
	public class UnlockManager : MonoBehaviour
	{
		public enum ClipboardState
		{
			Idle,
			Opening,
			Pause,
			Closing,
		}

		public static UnlockManager Instance;

		[Header( "References" )]
		public Transform Clipboard;
		public Transform ClipboardParent;

		[Header( "Assets" )]
		public GameObject TextPrefab;

		public Dictionary<BaseUnlockable, BaseChallenge> Unlocks = new Dictionary<BaseUnlockable, BaseChallenge>();
		public bool[] UnlockState;

		private Text[] ClipboardItems;
		private Vector3 TargetAngle;
		private bool UIOpen = false;
		private ClipboardState CurrentState;
		private float StartStateTime = 0;
		private int JustUnlocked = 0;

		private void Awake()
		{
			Instance = this;

			Unlocks.Add( new UnlockableGlide(), new ChallengeHighestPoint() );
			Unlocks.Add( new UnlockableCharacter( 1 ), new ChallengeFarGlide() );

			UnlockState = new bool[Unlocks.Count];

			// Create UI
			ClipboardItems = new Text[Unlocks.Count];
			var index = 0;
			foreach ( var unlock in Unlocks )
			{
				GameObject obj = Instantiate( TextPrefab, ClipboardParent );
				ClipboardItems[index] = obj.GetComponentInChildren<Text>();
				ClipboardItems[index].text = "- " + unlock.Value.GetText();
				var strikethrough = "";
				{
					for ( int i = 0; i < ClipboardItems[index].text.Length; i++ )
					{
						strikethrough += "-";
					}
				}
				ClipboardItems[index].transform.GetChild( 0 ).GetComponent<Text>().text = strikethrough;
				index++;
			}
			TargetAngle = Clipboard.localEulerAngles;
			Clipboard.localEulerAngles += new Vector3( 0, 0, -110 );
		}

		void Update()
		{
			// States
			switch ( CurrentState )
			{
				case ClipboardState.Idle:
					// Input
					if ( Input.GetKeyDown( KeyCode.Tab ) )
					{
						UIOpen = !UIOpen;
					}

					break;
				case ClipboardState.Opening:
					if ( ( Time.time - StartStateTime ) > 1 )
					{
						SwitchState( ClipboardState.Pause );
						StartCoroutine( Co_Scoreout() );
					}
					break;
				case ClipboardState.Pause:
					if ( ( Time.time - StartStateTime ) > 1 )
					{
						SwitchState( ClipboardState.Closing );
					}
					break;
				case ClipboardState.Closing:
					if ( ( Time.time - StartStateTime ) > 1 )
					{
						SwitchState( ClipboardState.Idle );
					}
					break;
				default:
					break;
			}

			// UI
			var target = TargetAngle;
			{
				if ( CurrentState == ClipboardState.Idle )
				{
					if ( !UIOpen )
					{
						target += new Vector3( 0, 0, -110 );
					}
				}
				else
				{
					if ( CurrentState == ClipboardState.Closing )
					{
						target += new Vector3( 0, 0, -110 );
					}
				}
			}
			Clipboard.localRotation = Quaternion.Lerp( Clipboard.localRotation, Quaternion.Euler( target ), Time.deltaTime * 15 );

			// Check for unlocks
			int index = 0;
			foreach ( var unlock in Unlocks )
			{
				if ( !UnlockState[index] )
				{
					if ( unlock.Value.Check() )
					{
						UnlockState[index] = true;
						unlock.Key.OnUnlock();
						JustUnlocked = index;
						SwitchState( ClipboardState.Opening );
					}
				}
				index++;
			}
		}

		public bool GetUnlocked( System.Type type )
		{
			var index = 0;
			foreach ( var unlock in Unlocks )
			{
				if ( unlock.Key.GetType() == type )
				{
					break;
				}
				index++;
			}
			return UnlockState[index];
		}

		public static BaseUnlockable Get( System.Type type )
		{
			foreach ( var unlock in Instance.Unlocks )
			{
				if ( unlock.Key.GetType() == type )
				{
					return unlock.Key;
				}
			}
			return null;
		}

		IEnumerator Co_Scoreout()
		{
			StaticHelpers.GetOrCreateCachedAudioSource( "scoreout", LocalPlayer.Instance.Player.transform, 1, 1, 0, true );

			// UI
			var score = ClipboardItems[JustUnlocked].transform.GetChild( 0 );
			score.gameObject.SetActive( true );
			var text = score.GetComponent<Text>();
			var length = text.text.Length;
			text.text = "";
			var between = 0.2f / length;
			for ( int i = 0; i < length; i++ )
			{
				text.text += "-";
				yield return new WaitForSeconds( between );
			}
		}

		#region States
		public void SwitchState( ClipboardState state )
		{
			StartStateTime = Time.time;

			CurrentState = state;
		}
		#endregion
	}
}
