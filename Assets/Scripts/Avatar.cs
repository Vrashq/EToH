using UnityEngine;
using System.Collections;

public class Avatar : MonoBehaviour {

	public ParticleSystem trail, Burst;
	public float DeathCountdown = -1f;
	public Transform Mesh, Camera;
	public Player Player;

	void OnTriggerEnter (Collider collider) {
		if (DeathCountdown < 0f) {
			ParticleSystem.EmissionModule emissionModule = trail.emission;
			emissionModule.enabled = false;

			Burst.Emit(Burst.maxParticles);
			DeathCountdown = Burst.startLifetime;
			StartCoroutine(Shake(DeathCountdown));
		}
	}
	
	void Update () {
		if (DeathCountdown >= 0f) {
			DeathCountdown -= Time.deltaTime;
			if (DeathCountdown <= 0f) {
				ParticleSystem.EmissionModule emissionModule = trail.emission;
				emissionModule.enabled = true;

				DeathCountdown = -1f;
				Player.Die();
			}
			Mesh.localScale = Vector3.one * 0.1f * DeathCountdown;
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