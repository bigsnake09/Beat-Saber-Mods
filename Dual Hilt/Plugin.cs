using System.Linq;
using IllusionPlugin;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR;

namespace Dual_Hilt
{
    public class Plugin : IPlugin
    {
        public string Name { get { return "Dual Hilt Plugin"; } }
        public string Version { get { return "1.0"; } }

        public void OnApplicationStart()
        {
            SceneManager.activeSceneChanged += OnActiveSceneChanged;
        }

        public void OnApplicationQuit()
        {
            SceneManager.activeSceneChanged -= OnActiveSceneChanged;
        }

        private static void OnActiveSceneChanged(Scene preScene, Scene newScene)
        {
            if (newScene.name == "SongLevel") InitDualSaber();
        }

        private static void InitDualSaber()
        {
            PlayerController controller = Object.FindObjectOfType<PlayerController>();
            if (!controller) return;

            VRController[] controllers = Object.FindObjectsOfType<VRController>();
            VRController leftHand = controllers.FirstOrDefault(c => c.node == XRNode.LeftHand);
            VRController rightHand = controllers.FirstOrDefault(c => c.node == XRNode.RightHand);

            if (leftHand == null || rightHand == null) return;
            if (leftHand.triggerValue < 0.5f && rightHand.triggerValue < 0.5f) return;

            XRNode targetHand = rightHand.triggerValue >= 0.5f ? XRNode.RightHand : XRNode.LeftHand;
            XRNode opositeHand = targetHand == XRNode.RightHand ? XRNode.LeftHand : XRNode.RightHand;

            for (int i = 0; i < controllers.Length; ++i)
            {
                if (controllers[i].node == opositeHand) Object.Destroy(controllers[i]);
            }

            Saber attachSaber = opositeHand == XRNode.RightHand ? controller.rightSaber : controller.leftSaber;
            Saber targetSaber = targetHand == XRNode.LeftHand ? controller.leftSaber : controller.rightSaber;

            targetSaber.transform.localRotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);

            attachSaber.transform.SetParent(targetSaber.transform.parent);
            attachSaber.transform.localPosition = Vector3.zero;

            attachSaber.transform.position += attachSaber.transform.forward * 0.15f;
            targetSaber.transform.position += targetSaber.transform.forward * 0.15f;
        }

        public void OnLevelWasLoaded(int level) { }
        public void OnLevelWasInitialized(int level) { }
        public void OnUpdate() { }
        public void OnFixedUpdate() { }
    }
}
