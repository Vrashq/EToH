using UnityEngine;

public class PipeSystem : MonoBehaviour {

	public Pipe PipePrefab;
	[Range(3,64)]
	public int PipeCount;

	private Pipe[] _pipes;

	public void Start () {
		_pipes = new Pipe[PipeCount];
		for (int i = 0; i < _pipes.Length; i++) {
			Pipe pipe = _pipes[i] = Instantiate<Pipe>(PipePrefab);
			pipe.transform.parent = transform;
		}
	}

	public Pipe SetupFirstPipe () {
		for (int i = 0; i < _pipes.Length; i++) {
			Pipe pipe = _pipes[i];
			pipe.Generate(false);
			if (i > 0) {
				pipe.AlignWith(_pipes[i - 1]);
			}
		}
		AlignNextPipeWithOrigin();
		transform.localPosition = new Vector3(0f, -_pipes[1].CurveRadius);
		return _pipes[1];
	}

	public Pipe SetupNextPipe (bool generate = true)
	{
		ShiftPipes();
		AlignNextPipeWithOrigin();
		if(generate)
		{
			_pipes[_pipes.Length - 1].Generate();
		}
		_pipes[_pipes.Length - 1].AlignWith(_pipes[_pipes.Length - 2]);
		transform.localPosition = new Vector3(0f, -_pipes[1].CurveRadius);
		return _pipes[1];
	}

	private void ShiftPipes () {
		Pipe temp = _pipes[0];
		for (int i = 1; i < _pipes.Length; i++) {
			_pipes[i - 1] = _pipes[i];
		}
		_pipes[_pipes.Length - 1] = temp;
	}

	private void AlignNextPipeWithOrigin () {
		Transform transformToAlign = _pipes[1].transform;
		for (int i = 0; i < _pipes.Length; i++) {
			if (i != 1) {
				_pipes[i].transform.SetParent(transformToAlign);
			}
		}
		
		transformToAlign.localPosition = Vector3.zero;
		transformToAlign.localRotation = Quaternion.identity;
		
		for (int i = 0; i < _pipes.Length; i++) {
			if (i != 1) {
				_pipes[i].transform.SetParent(transform);
			}
		}
	}
}