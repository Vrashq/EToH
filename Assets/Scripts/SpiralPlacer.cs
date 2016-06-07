using UnityEngine;

public class SpiralPlacer : PipeItemGenerator {

	public string[] itemPrefabs;

	public override void GenerateItems (Pipe pipe, Color color) {
		float start = (Random.Range(0, pipe.PipeSegmentCount) + 0.5f);
		float direction = Random.value < 0.5f ? 1f : -1f;

		float angleStep = pipe.CurveAngle / pipe.CurveSegmentCount;
		for (int i = 0; i < pipe.CurveSegmentCount; i++) {
			string itemName = itemPrefabs[Random.Range(0, itemPrefabs.Length)];
			PipeItem item = GameObjectPool.GetAvailableObject<PipeItem>(itemName);
			item.SetColor(color);
			float pipeRotation = (start + i * direction) * 360f / pipe.PipeSegmentCount;
			item.Position(pipe, i * angleStep, pipeRotation);
		}
	}
}