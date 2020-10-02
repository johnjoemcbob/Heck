using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Heck
{
	public class ChallengeConversation : BaseChallenge
	{
		public const float Goal = 20;
		public const float Decay = 0.5f;

		private List<float> Chirps = new List<float>();
		List<float> toremove = new List<float>();

		public override bool Check()
		{
			foreach ( var chirp in Chirps )
			{
				if ( chirp + Decay <= Time.time )
				{
					toremove.Add( chirp );
				}
			}
			foreach ( var remove in toremove )
			{
				Chirps.Remove( remove );
			}
			toremove.Clear();

			if ( LocalPlayer.Instance != null && LocalPlayer.Instance.Player != null )
			{
				if ( Chirps.Count >= Goal )
				{
					return true;
				}
			}
			return false;
		}

		// TODO check for other player and this player chirping close proximity a few times
		public void OnChirp( Player chirper )
		{
			if ( LocalPlayer.Instance != null && LocalPlayer.Instance.Player != null )
			{
				if ( LocalPlayer.Instance.Player == chirper )
				{
					Chirps.Add( Time.time );
				}
			}
		}

		public override string GetText()
		{
			return "Make a racket!";
		}
	}
}
