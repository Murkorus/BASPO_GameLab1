using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIButtons : MonoBehaviour
{
    public GameObject tutorialBackground;
    public GameObject tutorialLeftArrow;
    public GameObject tutorialRightArrow;
    public GameObject tutorialFirstPicture;
    public GameObject tutorialSecondPicture;
    public GameObject tutorialThirdPicture;
    private bool tutorialActive = false;
    private int rightarrowActive = 1;

    public void ReloadCurrentScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name.ToString());
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void LoadGame()
    {
        SceneManager.LoadScene(1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void TutorialPopUp()
    {
        if (tutorialActive == false)
        {
            tutorialBackground.SetActive(true);
            tutorialRightArrow.SetActive(true);
            tutorialFirstPicture.SetActive(true);
            tutorialActive = true;
        }
        else
        {
            tutorialBackground.SetActive(false);
            tutorialFirstPicture.SetActive(false);
            tutorialRightArrow.SetActive(false);
            tutorialThirdPicture.SetActive(false);
            tutorialSecondPicture.SetActive(false);
            tutorialActive = false;
            rightarrowActive = 1;
        }


    }

    public void RightArrowTutorial()
    {
        if (rightarrowActive == 1)
        {
            tutorialSecondPicture.SetActive(true);
            tutorialFirstPicture.SetActive(false);
            rightarrowActive = 2;
        }
        else if (rightarrowActive == 2)
        {
            tutorialThirdPicture.SetActive(true);
            tutorialSecondPicture.SetActive(false);
            rightarrowActive = 3;
        }
        else if (rightarrowActive == 3)
        {
            tutorialThirdPicture.SetActive(false);
            tutorialFirstPicture.SetActive(true);
            rightarrowActive = 1;
        }


    }
}
