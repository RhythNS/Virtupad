using UnityEngine;

namespace VRKeys
{
    public class KeyStabCollider : MonoBehaviour
    {
        [SerializeField] private KeyStab keyStab;

        public void OnKeyHit()
        {
            keyStab.OnKeyHit();
        }
    }
}
