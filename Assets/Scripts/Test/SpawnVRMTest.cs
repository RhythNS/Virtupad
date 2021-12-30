using System.Collections;
using UnityEngine;

namespace Virtupad
{
    public class SpawnVRMTest : MonoBehaviour
    {
        [System.Serializable]
        public struct Collection
        {
            public int index;
            public Vector3 position;
            public Quaternion rotation;
        }

        [SerializeField] private Collection[] collections;
        [SerializeField] private float timeBetween = 0.5f;

        private bool alreadySpawned = false;

        public void SpawnAll()
        {
            if (alreadySpawned == true)
                return;

            alreadySpawned = true;

            StartCoroutine(InnerSpawnAll());
        }

        private IEnumerator InnerSpawnAll()
        {
            for (int i = 0; i < collections.Length; i++)
            {
                VRMLoader.Instance.DebugSpawnModel(collections[i].index, collections[i].position, collections[i].rotation);
                yield return new WaitForSeconds(timeBetween);
            }
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.C))
                SpawnAll();
        }

        private void OnDrawGizmosSelected()
        {
            if (collections == null)
                return;

            Gizmos.color = Color.red;
            for (int i = 0; i < collections.Length; i++)
            {
                Gizmos.DrawWireSphere(collections[i].position, 0.25f);
            }
        }
    }
}
