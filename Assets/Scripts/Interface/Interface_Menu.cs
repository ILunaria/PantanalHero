using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class Interface_Menu : MonoBehaviour
{
    private UIDocument rot;
    public Button NovoJogo;
    public Button Continuar;
    public Button Sair;
    public Button Config;
    public Button Back;    
    public VisualElement configu;

    public VisualElement Menu;
    

    // Start is called before the first frame update
    void Awake()
    {
        var rot = GetComponent<UIDocument>();

        NovoJogo = rot.rootVisualElement.Q<Button>("Novojogo");
        Continuar = rot.rootVisualElement.Q<Button>("Continuar");
        Config = rot.rootVisualElement.Q<Button>("Configuracao");
        Sair = rot.rootVisualElement.Q<Button>("Sair");
        Back = rot.rootVisualElement.Q<Button>("Back");
        configu = rot.rootVisualElement.Q<VisualElement>("Configurar");
        Menu = rot.rootVisualElement.Q<VisualElement>("MainMenu");


        NovoJogo.clicked += NovoJogoPressed;
        Continuar.clicked += ContinuarPressed;
        Config.clicked += ConfigPressed;
        Sair.clicked += SairPressed;
        Back.clicked += BackPressed;
        
    }

   void NovoJogoPressed() 
   {
       SceneManager.LoadScene("Anderson_Scene");
   }

   void ContinuarPressed() 
   {
       Debug.Log("continua");
   }

   void SairPressed()
   {
       Application.Quit();
       Debug.Log("Saiu");
   }

   void ConfigPressed()
   {
       configu.style.display = DisplayStyle.Flex;
   }

   void BackPressed()
   {
       configu.style.display = DisplayStyle.None;
   }




}
