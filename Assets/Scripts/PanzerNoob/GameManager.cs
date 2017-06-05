namespace PanzerNoob {
	using Tools;
	
	public class GameManager : GenericSingleton<GameManager> {

		protected virtual void OnActorStart () {
			Managers.ScenesManager.CreateInstance();
			Managers.TimeManager.CreateInstance();
		}
	}
}
