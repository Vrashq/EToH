using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PanzerNoob.Managers 
{
	using Tools;

	public class ScenesManager : GenericSingleton<ScenesManager> 
	{
		private Dictionary<string, Scene> _scenesDic = new Dictionary<string, Scene>();
		public Dictionary<string, Scene> ScenesDic {
			get {
				return _scenesDic;
			}
		}
		private Scene _rootScene;

		protected virtual void OnActorAwake () {
			_rootScene = SceneManager.GetSceneByBuildIndex(0);
			_scenesDic.Add(_rootScene.name, _rootScene);
		}

		public AsyncOperation LoadScene (string name) {
			if(!_scenesDic.ContainsKey(name)) {
				_scenesDic[name] = SceneManager.GetSceneByName(name);
				return SceneManager.LoadSceneAsync(name, LoadSceneMode.Additive);
			}
			return null;
		}

		public AsyncOperation LoadScene (int id) {
			Scene scene = SceneManager.GetSceneByBuildIndex(id);
			return LoadScene(scene.name);
		}

		public bool IsSceneLoaded (string name) {
			return _scenesDic[name] != null;
		}

		public bool IsSceneLoaded (int id) {
			Scene scene = SceneManager.GetSceneByBuildIndex(id);
			return IsSceneLoaded(scene.name);
		}

		public bool IsSceneActive (string name) {
			Scene scene = SceneManager.GetSceneByName(name);
			return IsSceneActive_Implementation(scene);
		}

		public bool IsSceneActive (int id) {
			Scene scene = SceneManager.GetSceneByBuildIndex(id);
			return IsSceneActive_Implementation(scene);
		}

		private bool IsSceneActive_Implementation (Scene scene) {
			return scene.name.Equals(SceneManager.GetActiveScene().name);
		}

		public AsyncOperation UnloadScene (string name) {
			if(_scenesDic.ContainsKey(name) && !_rootScene.name.Equals(name)) {
				_scenesDic.Remove(name);
				return SceneManager.UnloadSceneAsync(name);
			}
			return null;
		}

		public AsyncOperation UnloadScene (int id) {
			Scene scene = SceneManager.GetSceneByBuildIndex(id);
			return UnloadScene(scene.name);
		}
	}
}
