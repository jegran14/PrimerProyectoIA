using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelChanger : MonoBehaviour
{
    public Animator animator;
    public bool victory;
    public int levelSelection;
    private int CurrentLevel;
    private PlayerController player;

    void Start()
    {
        CurrentLevel = SceneManager.GetActiveScene().buildIndex;
        player = GameObject.Find("Player").GetComponent<PlayerController>();
        victory = false;
        levelSelection = 1;
        
    }

    public void FadeToBlack()
    {
        player.isEnabled = false;
        animator.SetTrigger("FadeOut");
    }

    // Tras terminar la animación del Fade Out se ejecutará 
    // el método para cambiar de nivel o repetirlo en caso de derrota

    public void ChangeLevel()
    {
        if (CurrentLevel == 0) // We are in the Main Menu
        {
            SceneManager.LoadScene(levelSelection);
        }
        else if (victory) // Next Level when victory
        {
            victory = false;
            // Si la escena es la última, regresará al menú principal (escena 0)
            if (CurrentLevel >= 3) SceneManager.LoadScene(0); 
            else SceneManager.LoadScene(CurrentLevel+1);
        }
        else // Repeat Level when defeat
            SceneManager.LoadScene(CurrentLevel); 
    }

}
