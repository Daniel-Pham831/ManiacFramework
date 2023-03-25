#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Maniac.Utils
{
    public class ChangeScreen : MonoBehaviour
    {
        public enum State
        {
            None = 0,
            Previous = -1,
            Next = 1,
        }

        [MenuItem("Maniac/Utils/ChangeScreen/Previous Scene #1")]
        static void LoadPreviousScene()
        {
            LoadScene(State.Previous);
        }

        [MenuItem("Maniac/Utils/ChangeScreen/Next Scene #2")]
        static void LoadNextScene()
        {
            LoadScene(State.Next);
        }

        static void LoadScene(State state)
        {
            int sceneCount = EditorSceneManager.sceneCountInBuildSettings;
            int currentSceneIndex = EditorSceneManager.GetActiveScene().buildIndex;
            int targetSceneIndex = (currentSceneIndex + sceneCount + (int)state) % sceneCount;
            string targetScene = SceneUtility.GetScenePathByBuildIndex(targetSceneIndex);
            EditorSceneManager.OpenScene(targetScene);
        }
    }
}

#endif