using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class Interface_Menu : MonoBehaviour
{
    private UIDocument rot;
    public Button NovoJogo;
    public Button Sair;
    public Button cred;
    public Button Config;
    public Button Back;
    public Button Vol;
    public VisualElement configu;
    public VisualElement Menu;
    public VisualElement Credi;
    

    // Start is called before the first frame update
    void Awake()
    {
        var rot = GetComponent<UIDocument>();

        NovoJogo = rot.rootVisualElement.Q<Button>("Novojogo");
        Config = rot.rootVisualElement.Q<Button>("Configuracao");
        Sair = rot.rootVisualElement.Q<Button>("Sair");
        cred = rot.rootVisualElement.Q<Button>("Credits");
        Vol = rot.rootVisualElement.Q<Button>("Vol");
        Back = rot.rootVisualElement.Q<Button>("Back");

        configu = rot.rootVisualElement.Q<VisualElement>("Configurar");
        Credi = rot.rootVisualElement.Q<VisualElement>("Cred");
        Menu = rot.rootVisualElement.Q<VisualElement>("MainMenu");


        NovoJogo.clicked += NovoJogoPressed;
        Config.clicked += ConfigPressed;
        cred.clicked += Credpressed;
        Sair.clicked += SairPressed;
        Back.clicked += BackPressed;
        Vol.clicked += VolPressed;


    }

   void NovoJogoPressed() 
   {
       SceneManager.LoadScene(1,LoadSceneMode.Single);
   }

   void SairPressed()
   {
       Application.Quit();
       Debug.Log("Saiu");
   }

   void ConfigPressed()
   {
       configu.style.display = DisplayStyle.Flex;
       Menu.style.display = DisplayStyle.None;

   }

   void BackPressed()
   {
       configu.style.display = DisplayStyle.None;
       Menu.style.display = DisplayStyle.Flex;
   }

   private void Credpressed()
   {
       Credi.style.display = DisplayStyle.Flex;
       Menu.style.display = DisplayStyle.None;
   }
    
   void VolPressed()
   {
       Credi.style.display = DisplayStyle.None;
       Menu.style.display = DisplayStyle.Flex;
   }
}
