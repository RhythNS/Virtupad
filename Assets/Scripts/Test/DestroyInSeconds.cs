using System.Collections;
using UnityEngine;

namespace Virtupad
{
    public class DestroyInSeconds : MonoBehaviour
    {
        [SerializeField] private bool affectGameobject = true;
        [SerializeField] private bool justDisable = false;
        [SerializeField] private float seconds;

        IEnumerator Start()
        {
            yield return new WaitForSeconds(seconds);

            if (affectGameobject)
            {
                if (justDisable)
                    gameObject.SetActive(false);
                else
                    Destroy(gameObject);
            }
            else
            {
                if (justDisable)
                    enabled = false;
                else
                    Destroy(this);
            }
        }
    }
}
