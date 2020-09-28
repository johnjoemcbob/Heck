using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

	public GameObject[] StateObjects;
	public RectTransform TitleLerper;
	public Transform SelectableCharacters;

	[HideInInspector]
	public State CurrentState;
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
				foreach ( Transform child in SelectableCharacters )
				{
					child.gameObject.SetActive( false );
				}
				SelectableCharacters.GetChild( 0 ).gameObject.SetActive( true );
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
			case State.TitleToPlay:
				var speed = 2;
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
		SwitchState( State.CharacterSelect );
	}

	public void ButtonPlay()
	{
		SwitchState( State.TitleToPlay );
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
	#endregion

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

	}
}
