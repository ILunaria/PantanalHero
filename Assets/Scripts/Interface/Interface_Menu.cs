using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class Interface_Menu : MonoBehaviour
{
    public Button NovoJogo;
    public Button Continuar;
    public Button Sair;
    public Button Config;
    public Button Back;
    

    // Start is called before the first frame update
    void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        NovoJogo = root.Q<Button>("Novojogo");
        Continuar = root.Q<Button>("Continuar");
        Config = root.Q<Button>("Configuracao");
        Sair = root.Q<Button>("Sair");
        Back = root.Q<Button>("Back");

        NovoJogo.clicked += NovoJogoPressed;
        Continuar.clicked += ContinuarPressed;
        Sair.clicked += SairPressed;
        
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




}
