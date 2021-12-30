using UnityEngine;

namespace Virtupad
{
    public abstract class CameraMover : MonoBehaviour
    {
        [System.Serializable]
        public enum Type
        {
            Static, Selfie, Rail, Follow, Tracking
        }

        public static System.Type GetSystemTypeForType(Type type)
        {
            switch (type)
            {
                case Type.Static:
                    return typeof(StaticCameraMover);
                case Type.Selfie:
                    break;
                case Type.Rail:
                    break;
                case Type.Follow:
                    break;
                case Type.Tracking:
                    break;
            }

            return null;
        }

        public StudioCamera OnCamera { get; protected set; }

        public void Init(StudioCamera onCamera)
        {
            OnCamera = onCamera;
        }

        public virtual void OnInit() { }

        public abstract void Play();

        public abstract void Restart();
    }
}
