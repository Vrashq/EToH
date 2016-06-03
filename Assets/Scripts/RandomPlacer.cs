using UnityEngine;
using System.Collections.Generic;

public class RandomPlacer : PipeItemGenerator {

	public string[] itemPrefabs;

	public override void GenerateItems (Pipe pipe) {
		float angleStep = pipe.CurveAngle / pipe.CurveSegmentCount;
		for (int i = 0; i < pipe.CurveSegmentCount; i++)
		{
			string itemName = itemPrefabs[Random.Range(0, itemPrefabs.Length)];
			PipeItem item = GameObjectPool.GetAvailableObject<PipeItem>(itemName);
			float pipeRotation = (Random.Range(0, pipe.pipeSegmentCount) + 0.5f) * 360f / pipe.pipeSegmentCount;
			item.Position(pipe, i * angleStep, pipeRotation);
		}
	}
}