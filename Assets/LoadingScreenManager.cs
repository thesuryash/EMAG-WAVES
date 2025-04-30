//using UnityEngine;
//using UnityEngine.SceneManagement;
//using UnityEngine.UI;

//public class LoadingScreenManager : MonoBehaviour
//{
//    [SerializeField] private GameObject loadingScreen; // Reference to the loading screen UI
//    [SerializeField] private Slider progressBar; // Reference to the progress bar

//    public void LoadScene(string sceneName)
//    {
//        StartCoroutine(LoadSceneAsync(sceneName));
//    }

//    private IEnumerator LoadSceneAsync(string sceneName)
//    {
//        loadingScreen.SetActive(true); // Show the loading screen

//        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
//        operation.allowSceneActivation = false; // Prevent automatic activation

//        while (!operation.isDone)
//        {
//            // Update progress bar
//            progressBar.value = Mathf.Clamp01(operation.progress / 0.9f);

//            // Activate the scene when loading is complete
//            if (operation.progress >= 0.9f)
//            {
//                operation.allowSceneActivation = true;
//            }

//            yield return null;
//        }

//        loadingScreen.SetActive(false); // Hide the loading screen
//    }
//}