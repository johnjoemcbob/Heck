using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
	public const float TRANS_LERP_TIME = 2;

	public enum State
	{
		Title,
		CharacterSelect,
		TitleToPlay,
		Play,
		PlayToTitle,
	}

	public static Game Instance;
	public static Transform RuntimeParent;

	[Header( "Variables" )]
	public string[] CharacterNames;

	[Header( "References" )]
	public GameObject[] StateObjects;
	public RectTransform TitleLerper;
	public Transform SelectableCharacters;
	public GameObject SelectableCharacterLock;
	public Transform SelectedCharacterNameTexts;

	[HideInInspector]
	public State CurrentState;
	[HideInInspector]
	public bool[] UnlockedCharacters;
	private float StateStartTime;
	private Vector2 TitleLerpMin;
	private Vector2 TitleLerpMax;

	private void Awake()
	{
		Instance = this;
		RuntimeParent = transform;

		foreach ( GameObject state in StateObjects )
		{
			state.SetActive( false );
		}
		StartState( State.Title );

		TitleLerpMin = TitleLerper.anchorMin;
		TitleLerpMax = TitleLerper.anchorMax;

		UnlockedCharacters = new bool[SelectableCharacters.childCount];
		UnlockedCharacters[0] = true;
	}

	private void Update()
	{
		UpdateState();

		if ( Input.GetKeyDown( KeyCode.Escape ) && CurrentState == State.Play )
		{
			SwitchState( State.PlayToTitle );
		}
	}

	#region States
	public void SwitchState( State state )
	{
		FinishState( CurrentState );
		CurrentState = state;
		StartState( CurrentState );
	}

	public void StartState( State state )
	{
		StateObjects[(int) state].SetActive( true );

		switch ( state )
		{
			case State.Title:
				Cursor.visible = true;
				Cursor.lockState = CursorLockMode.None;
				break;
			case State.CharacterSelect:
				LoopCharacterSelect( 0 );
				break;
			case State.TitleToPlay:
				TitleCamera.Instance.LerpStartTime = Time.time;
				TitleCamera.Instance.LerpIn = true;
				break;
			case State.Play:
				Cursor.visible = false;
				Cursor.lockState = CursorLockMode.Locked;
				break;
			case State.PlayToTitle:
				TitleCamera.Instance.LerpStartTime = Time.time;
				TitleCamera.Instance.LerpIn = false;
				break;
			default:
				break;
		}

		StateStartTime = Time.time;
	}

	public void UpdateState()
	{
		switch ( CurrentState )
		{
			case State.Title:
				break;
			case State.CharacterSelect:
				var speed = 0.1f;
				SelectableCharacters.localEulerAngles = new Vector3(
					2 * Mathf.Sin( Time.time * speed ),
					180,
					-10 * Mathf.Cos( Time.time * -speed )
				);
				break;
			case State.TitleToPlay:
				speed = 2;
				var progress = ( Time.time - StateStartTime ) / TRANS_LERP_TIME * speed;

				TitleLerper.anchorMin = Vector2.Lerp( TitleLerpMin, Vector2.zero, progress );
				TitleLerper.anchorMax = Vector2.Lerp( TitleLerpMax, Vector2.one, progress );

				if ( progress >= speed )
				{
					SwitchState( State.Play );
				}
				break;
			case State.Play:
				break;
			case State.PlayToTitle:
				speed = 2;
				progress = Mathf.Max( 0, ( Time.time - StateStartTime ) / TRANS_LERP_TIME * speed - 0.5f );

				TitleLerper.anchorMin = Vector2.Lerp( Vector2.zero, TitleLerpMin, progress );
				TitleLerper.anchorMax = Vector2.Lerp( Vector2.one, TitleLerpMax, progress );

				if ( progress >= speed )
				{
					SwitchState( State.Title );
				}
				break;
			default:
				break;
		}
	}

	public void FinishState( State state )
	{
		StateObjects[(int) state].SetActive( false );

		switch ( state )
		{
			case State.Title:
				break;
			case State.TitleToPlay:
				break;
			case State.Play:
				break;
			case State.PlayToTitle:
				break;
			default:
				break;
		}
	}
	#endregion

	#region Buttons
	public void ButtonGo()
	{
		StartCoroutine( Co_JoinGame() );
	}

	public void ButtonBack()
	{
		switch ( CurrentState )
		{
			case State.Title:
				Application.Quit();
				break;
			case State.CharacterSelect:
				SwitchState( State.Title );
				break;
			case State.TitleToPlay:
				break;
			case State.Play:
				break;
			case State.PlayToTitle:
				break;
			default:
				break;
		}
	}

	public void ButtonPlay()
	{
		if ( UnlockedCharacters[(int) LocalPlayer.Instance.Player.CurrentAnimal] )
		{
			SwitchState( State.TitleToPlay );
		}
	}

	public void ButtonLeft()
	{
		LoopCharacterSelect( -1 );
	}

	public void ButtonRight()
	{
		LoopCharacterSelect( 1 );
	}

	public void ButtonJohnjoemcbob()
	{
		Application.OpenURL( "www.johnjoemcbob.com" );
	}

	public void ButtonToggleMusic()
	{
		var src = GameObject.Find( "Music" ).GetComponent<AudioSource>();
		if ( src.isPlaying )
		{
			src.Pause();
		}
		else
		{
			src.Play();
		}
	}
	#endregion

	IEnumerator Co_JoinGame()
	{
		while ( !LocalPlayer.Instance || !LocalPlayer.Instance.Player )
		{
			yield return new WaitForEndOfFrame();
		}

		SwitchState( State.CharacterSelect );
	}

	void LoopCharacterSelect( int dir )
	{
		// Add
		var anim = LocalPlayer.Instance.Player.CurrentAnimal + dir;

		// Loop
		if ( anim >= Player.Animal.Count )
		{
			anim = 0;
		}
		else if ( anim < 0 )
		{
			anim = Player.Animal.Count - 1;
		}

		// Enable/disable
		LocalPlayer.Instance.Player.SetAnimal( anim );
		LocalPlayer.Instance.Player.ChirpLocal( true );
		foreach ( Transform child in SelectableCharacters )
		{
			child.gameObject.SetActive( false );
		}
		SelectableCharacters.GetChild( (int) anim ).gameObject.SetActive( true );
		SelectableCharacters.GetChild( (int) anim ).GetComponentInChildren<Animator>().SetBool( "Spin", true );

		// Update name
		foreach ( var text in SelectedCharacterNameTexts.GetComponentsInChildren<Text>() )
		{
			text.text = CharacterNames[(int) LocalPlayer.Instance.Player.CurrentAnimal];
		}

		// Update locked
		bool unlocked = UnlockedCharacters[(int) LocalPlayer.Instance.Player.CurrentAnimal];
		SelectableCharacterLock.SetActive( !unlocked );
		var button = "meen't :(";
		if ( unlocked )
		{
			button = "me!";
		}
		GameObject.Find( "Me (Button)" ).GetComponentInChildren<Text>().text = button;
	}
}
