using UnityEngine;

public class PipeSystem : PanzerNoob.Tools.GenericSingleton<PipeSystem> 
{
	public Pipe PipePrefab;
	[Range(6,18)]
	public int PipeCount;
	[Range(3,6)]
	public int TutorialPipes;

	private Pipe[] _pipes;

	public void StartSystem () 
	{
		_pipes = new Pipe[PipeCount];
		for (int i = 0; i < _pipes.Length; i++) 
		{
			Pipe pipe = _pipes[i] = Instantiate<Pipe>(PipePrefab);
			pipe.transform.parent = transform;
		}
	}

	public Pipe SetupFirstPipe (bool isOnMenu) 
	{
		int pipeToGenerate = Mathf.FloorToInt(_pipes.Length * 0.5f);
		
		for (int i = 0; i < pipeToGenerate; ++i) 
		{
			_pipes[i].Generate(false);
			if (i > 0) 
			{
				_pipes[i].AlignWith(_pipes[i - 1]);
			}
		}
		AlignNextPipeWithOrigin();
		transform.localPosition = new Vector3(0f, -_pipes[1].CurveRadius);
		
		for(int i = TutorialPipes; i < _pipes.Length; ++i)
		{
			_pipes[i].Generate(!isOnMenu);
			_pipes[i].AlignWith(_pipes[i - 1]);
		}
		
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

	private void ShiftPipes () 
	{
		Pipe temp = _pipes[0];
		for (int i = 1; i < _pipes.Length; i++) 
		{
			_pipes[i - 1] = _pipes[i];
		}
		_pipes[_pipes.Length - 1] = temp;
	}

	private void AlignNextPipeWithOrigin () 
	{
		Transform transformToAlign = _pipes[1].transform;
		for (int i = 0; i < _pipes.Length; i++) 
		{
			if (i != 1) 
			{
				_pipes[i].transform.SetParent(transformToAlign);
			}
		}
		
		transformToAlign.localPosition = Vector3.zero;
		transformToAlign.localRotation = Quaternion.identity;
		
		for (int i = 0; i < _pipes.Length; i++) 
		{
			if (i != 1) 
			{
				_pipes[i].transform.SetParent(transform);
			}
		}
	}

	public void SetColor(Color color)
	{
		for (int i = 0; i < _pipes.Length; ++i)
		{
			_pipes[i].SetColor(color);
		}
	}
}