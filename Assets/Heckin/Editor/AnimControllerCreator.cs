using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using System.Collections.Generic;

// Create a menu item that causes a new controller and statemachine to be created.
public class AnimControllerCreator : MonoBehaviour
{
	[MenuItem( "MyMenu/Create Controller" )]
	static void CreateController()
	{
		var animals = new Dictionary<string, string>();
		{
			animals.Add( "Pengiun", "Arctic" );
			animals.Add( "Chick", "Farm" );
			animals.Add( "Duck", "Farm" );
			animals.Add( "Hen", "Farm" );
			animals.Add( "Crow", "Forest" );
			animals.Add( "Hornbill", "Forest" );
			animals.Add( "Owl", "Forest" );
			animals.Add( "Parrot", "Pets" );
			animals.Add( "Flamingo", "Safari" );
			animals.Add( "Ostrich", "Safari" );
		}
		foreach ( var animal in animals )
		{
			string Theme = animal.Value;
			string Animal = animal.Key;

			// Creates the controller
			var controller = UnityEditor.Animations.AnimatorController.CreateAnimatorControllerAtPath("Assets/Heckin/Editor/" + Theme + "_" + Animal + ".controller");

			// Add parameters
			controller.AddParameter( "HorizontalSpeed", AnimatorControllerParameterType.Float );
			controller.AddParameter( "VerticalSpeed", AnimatorControllerParameterType.Float );
			controller.AddParameter( "IsGrounded", AnimatorControllerParameterType.Bool );

			// Get motions
			var assetPath = "Assets/Quirky Series/" + Theme + "/" + Animal + "/Animation/" + Animal + "_Animations.fbx";
			object[] a = AssetDatabase.LoadAllAssetsAtPath(assetPath);

			List<AnimationClip> animations = new List<AnimationClip>();
			AnimationClip idle_a = null, run = null, fly = null;
			foreach ( var obj in a )
			{
				AnimationClip anim = obj as AnimationClip;
				if ( anim != null )
				{
					if ( anim.name == "Idle_A" )
					{
						idle_a = anim;
					}
					if ( anim.name == "Run" )
					{
						run = anim;
					}
					if ( anim.name == "Fly" )
					{
						fly = anim;
					}
				}
			}

			// Add States
			var rootStateMachine = controller.layers[0].stateMachine;
			var state_idle = rootStateMachine.AddState( "Idle" );
			state_idle.motion = idle_a;
			var state_run = rootStateMachine.AddState( "Run" );
			state_run.motion = run;
			var state_fly = rootStateMachine.AddState( "Fly" );
			state_fly.motion = fly;

			// Idle <-> Run
			var trans_idle_run = state_idle.AddTransition( state_run );
			trans_idle_run.AddCondition( UnityEditor.Animations.AnimatorConditionMode.Greater, 0, "HorizontalSpeed" );
			trans_idle_run.duration = 0;

			var trans_run_idle = state_run.AddTransition( state_idle );
			trans_run_idle.AddCondition( UnityEditor.Animations.AnimatorConditionMode.Less, 0.01f, "HorizontalSpeed" );
			trans_run_idle.duration = 0;

			// Any -> Fly -> Idle || Run
			var states_any = new AnimatorState[] { state_idle, state_run }; // Any, but not self fly!
			foreach ( var state_any in states_any )
			{
				var trans_any_fly = state_any.AddTransition( state_fly );
				trans_any_fly.AddCondition( UnityEditor.Animations.AnimatorConditionMode.IfNot, 0, "IsGrounded" );
				trans_any_fly.duration = 0;
			}

			var trans_fly_run = state_fly.AddTransition( state_run );
			trans_fly_run.AddCondition( UnityEditor.Animations.AnimatorConditionMode.If, 0, "IsGrounded" );
			trans_fly_run.AddCondition( UnityEditor.Animations.AnimatorConditionMode.Greater, 0, "HorizontalSpeed" );
			trans_fly_run.duration = 0;

			var trans_fly_idle = state_fly.AddTransition( state_idle );
			trans_fly_idle.AddCondition( UnityEditor.Animations.AnimatorConditionMode.If, 0, "IsGrounded" );
			trans_fly_idle.AddCondition( UnityEditor.Animations.AnimatorConditionMode.Less, 0.01f, "HorizontalSpeed" );
			trans_fly_idle.duration = 0;

			// Add Transitions
			//var exitTransition = stateA1.AddExitTransition();
			//exitTransition.AddCondition( UnityEditor.Animations.AnimatorConditionMode.If, 0, "TransitionNow" );
			//exitTransition.duration = 0;

			//var resetTransition = rootStateMachine.AddAnyStateTransition(stateA1);
			//resetTransition.AddCondition( UnityEditor.Animations.AnimatorConditionMode.If, 0, "Reset" );
			//resetTransition.duration = 0;

			//var transitionB1 = stateMachineB.AddEntryTransition(stateB1);
			//transitionB1.AddCondition( UnityEditor.Animations.AnimatorConditionMode.If, 0, "GotoB1" );
			//stateMachineB.AddEntryTransition( stateB2 );
			//stateMachineC.defaultState = stateC2;
			//var exitTransitionC2 = stateC2.AddExitTransition();
			//exitTransitionC2.AddCondition( UnityEditor.Animations.AnimatorConditionMode.If, 0, "TransitionNow" );
			//exitTransitionC2.duration = 0;

			//var stateMachineTransition = rootStateMachine.AddStateMachineTransition(stateMachineA, stateMachineC);
			//stateMachineTransition.AddCondition( UnityEditor.Animations.AnimatorConditionMode.If, 0, "GotoC" );
			//rootStateMachine.AddStateMachineTransition( stateMachineA, stateMachineB );
		}
	}
}