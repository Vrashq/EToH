using UnityEngine;
using System.Collections;

public class PauseMenu : MonoBehaviour
{
	private Animator _animator;

	public void OnEnable ()
	{
		_animator.SetTrigger("Animate");
	}
}
