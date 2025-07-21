using UnityEngine;

[CreateAssetMenu(menuName = "Play Sound Area")]
public class PlaySoundArea : ComponentAreaScriptable
{
	public AudioSource source;

	public AudioClip audioClip;

	public AudioClip[] randomClip = new AudioClip[0];

	public override void OnActivation(AreaDamage area)
	{
		int num = Random.Range(0, randomClip.Length + 1);
		if (randomClip.Length == 0 || num == randomClip.Length)
		{
			source.clip = audioClip;
		}
		else
		{
			source.clip = randomClip[num];
		}
		source.Play();
	}
}
