﻿using UnityEngine;
using System.Collections;

public class Avatar : MonoBehaviour {

	public float DeathCountdown = -1f;
	public ParticleSystem Trail, Burst;
	public Transform Mesh, Camera;
	public Player Player;
	public Light Light;

	public void SetRandomColor ()
	{
		Color color = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));

		Material material = Mesh.GetComponent<MeshRenderer>().material;
		material.color = color;
		material.SetColor("_EmissionColor", color);
		Trail.startColor = color * 1.5f;

		Player.SetColor(color);
	}

	void Update()
	{
		if (DeathCountdown >= 0f)
		{
			DeathCountdown -= Time.deltaTime;
			if (DeathCountdown <= 0f)
			{
				Trail.enableEmission = true;
				DeathCountdown = -1f;
				Player.Die();
			}
			Mesh.localScale = Vector3.one * 0.1f * DeathCountdown;
		}
	}

	void OnTriggerEnter (Collider collider) {
		if (DeathCountdown < 0f)
		{
			if(collider.transform.tag == "Obstacle")
			{
				Trail.enableEmission = false;
				Burst.Emit(Burst.maxParticles);
				DeathCountdown = Burst.startLifetime;
				StartCoroutine(Shake(DeathCountdown));
			}
			else if (collider.transform.tag == "Bonus")
			{
				GameObjectPool.AddObjectIntoPool(collider.transform.parent.parent.gameObject);
				Player.AddBonus();
			}
		}
	}

	IEnumerator Shake (float duration)
	{
		Vector3 initialPos = Camera.localPosition;
		while(duration >= 0.0f)
		{
			duration -= Time.deltaTime;
			Camera.localPosition = initialPos + Random.insideUnitSphere * 0.05f;
			yield return null;
		}
		Camera.localPosition = initialPos;
	}
}