using UnityEngine;

public class RandomPlacer : PipeItemGenerator {

	public string[] itemPrefabs;

	public override void GenerateItems (Pipe pipe, Color color) {
		float angleStep = pipe.CurveAngle / pipe.CurveSegmentCount;
		for (int i = 0; i < pipe.CurveSegmentCount; i++)
		{
			string itemName = itemPrefabs[Random.Range(0, itemPrefabs.Length)];
			PipeItem item = GameObjectPool.GetAvailableObject<PipeItem>(itemName);
			item.SetColor(color);
			float pipeRotation = (Random.Range(0, pipe.PipeSegmentCount) + 0.5f) * 360f / pipe.PipeSegmentCount;
			item.Position(pipe, i * angleStep, pipeRotation);
		}
	}
}