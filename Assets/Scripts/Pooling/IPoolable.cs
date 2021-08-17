namespace Quests.ObjectPool
{
    public interface IPoolable
    {
        /// <summary>
        /// The system expects the IPoolable implementation to know which pool spawned it, so cache the reference to it.
        /// </summary>
        void SetParentPool(PrefabPool pool);
        /// <summary>
        /// Prepare the entire game object for being pooled - stop animations, coroutines, disable physics, etc.
        /// </summary>
        void PrepareForPooling();
        /// <summary>
        /// Prepare the game object for spawn, in essence just reset it into an initial state.
        /// </summary>
        void HandleSpawn();
    }
}
