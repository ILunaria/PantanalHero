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
    public Button Config;
    public Button Back;  
    public Button Cred;
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
        Back = rot.rootVisualElement.Q<Button>("Back");
        Vol = rot.rootVisualElement.Q<Button>("Vol");
        Cred = rot.rootVisualElement.Q<Button>("Credito");
        configu = rot.rootVisualElement.Q<VisualElement>("Configurar");
        Credi = rot.rootVisualElement.Q<VisualElement>("Cred");
        Menu = rot.rootVisualElement.Q<VisualElement>("MainMenu");
 

        NovoJogo.clicked += NovoJogoPressed;
        Config.clicked += ConfigPressed;
        Sair.clicked += SairPressed;
        Back.clicked += BackPressed;
        Vol.clicked += VolPressed;
        Cred.clicked += CredPressed;
        
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

   void VolPressed()
   {
       Credi.style.display = DisplayStyle.None;
       configu.style.display = DisplayStyle.Flex;
   }

   void CredPressed()
   {
       Credi.style.display = DisplayStyle.Flex;
       configu.style.display = DisplayStyle.None;
   }




}
