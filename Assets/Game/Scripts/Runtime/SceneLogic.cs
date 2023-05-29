namespace AillieoTech.Game
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using Random = UnityEngine.Random;

    public class SceneLogic : MonoBehaviour
    {
        private Dictionary<int, float> timers = new Dictionary<int, float>();

        [SerializeField]
        private Bounds spawnArea;

        [SerializeField]
        private SpawnTask[] tasks;

        private HashSet<GameObject> managedObjects = new HashSet<GameObject>();
        private List<GameObject> removeBuffer = new List<GameObject>();

        private void Update()
        {
            this.ExecuteSpawnTasks();
            this.CheckDeadObjects();
        }

        private void ExecuteSpawnTasks()
        {
            if (this.managedObjects.Count > 50)
            {
                return;
            }

            for (var i = 0; i < this.tasks.Length; ++i)
            {
                SpawnTask task = this.tasks[i];

                if (!this.timers.TryGetValue(i, out var timer))
                {
                    timer = -task.delay;
                }
                else
                {
                    timer += Time.deltaTime;
                    if (timer > task.interval)
                    {
                        timer -= task.interval;

                        Vector3 center = this.spawnArea.center;
                        var dx = this.spawnArea.extents.x;
                        var dy = this.spawnArea.extents.y;
                        var dz = this.spawnArea.extents.z;

                        GameObject go = Instantiate(
                            task.prefab,
                            center + new Vector3(Random.Range(-dx, dx), Random.Range(-dy, dy), Random.Range(-dz, dz)),
                            Random.rotation,
                            this.transform);
                        this.managedObjects.Add(go);
                    }
                }

                this.timers[i] = timer;
            }
        }

        private void CheckDeadObjects()
        {
            foreach (var go in this.managedObjects)
            {
                if (go.transform.position.y < -10000)
                {
                    this.removeBuffer.Add(go);
                }
            }

            if (this.removeBuffer.Count > 0)
            {
                foreach (var go in this.removeBuffer)
                {
                    this.managedObjects.Remove(go);
                    Destroy(go);
                }

                this.removeBuffer.Clear();
            }
        }

        [Serializable]
        public struct SpawnTask
        {
            public float delay;
            public float interval;
            public GameObject prefab;
        }
    }
}
