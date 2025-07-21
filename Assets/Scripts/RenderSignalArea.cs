using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class RenderSignalArea : MonoBehaviour, IInteractionArea, IEventSystemHandler
{
	public SpriteRenderer render;

	public Color colorRender;

	private Color startColor = new Color(1f, 1f, 1f, 1f);

	private float timeSignal;

	private void Start()
	{
		startColor = render.color;
	}

	private void Update()
	{
		if (render != null && render.isVisible && render.gameObject.activeInHierarchy)
		{
			timeSignal += Time.deltaTime * 135f;
			timeSignal %= 360f;
			render.color = Color.Lerp(colorRender, startColor, Mathf.Abs(Mathf.Sin(timeSignal * ((float)Math.PI / 180f))));
		}
	}

	private void OnDisable()
	{
		render.color = colorRender;
		timeSignal = 0f;
	}

	public void OnActiveArea()
	{
		if (render != null)
		{
			render.gameObject.SetActive(value: false);
		}
	}

	public void OnInactiveArea()
	{
		if (render != null)
		{
			render.gameObject.SetActive(value: true);
		}
	}

	public void OnActivationArea()
	{
	}
}
