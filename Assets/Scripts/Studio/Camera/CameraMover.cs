using UnityEngine;

namespace Virtupad
{
    public abstract class CameraMover : MonoBehaviour
    {
        [System.Serializable]
        public enum Type
        {
            Default, Static, Selfie, Rail, Follow, Tracking
        }

        [System.Serializable]
        public enum TrackingSpace
        {
            Playspace = 0,
            ModelSpace = 1
        }

        public static System.Type GetSystemTypeForType(Type type)
        {
            switch (type)
            {
                case Type.Default:
                    return typeof(DefaultCameraMover);
                case Type.Static:
                    return typeof(StaticCameraMover);
                case Type.Selfie:
                    return typeof(SelfieCameraMover);
                case Type.Rail:
                    //return typeof(StaticCameraMover);
                    break;
                case Type.Follow:
                    return typeof(FollowCameraMover);
                case Type.Tracking:
                    return typeof(TrackingCameraMover);
            }

            return null;
        }

        public static Type GetType(CameraMover cameraMover)
        {
            if (cameraMover is DefaultCameraMover)
                return Type.Default;
            if (cameraMover is StaticCameraMover)
                return Type.Static;
            if (cameraMover is SelfieCameraMover)
                return Type.Selfie;
            if (cameraMover is FollowCameraMover)
                return Type.Follow;
            if (cameraMover is TrackingCameraMover)
                return Type.Tracking;

            Debug.LogError("Could not get type for: " + cameraMover.name + " " + cameraMover.GetType());
            return default;
        }

        public StudioCamera OnCamera { get; protected set; }

        public void Init(StudioCamera onCamera)
        {
            onCamera.SetDefaultValues();
            OnCamera = onCamera;
        }

        public virtual void OnInit() { }

        public virtual void Play() { }

        public virtual void Stop() { }

        public virtual void Restart() { }

        public virtual void OnRemove() { }

        public virtual void OnFollowTypeChanged() { }
    }
}
