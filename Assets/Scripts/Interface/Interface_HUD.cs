using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class Interface_HUD : MonoBehaviour
{
    private UIDocument Root;
    public Button Menu;
    public Button Sair;
    public VisualElement HUD;
    public VisualElement Confirm;
    public Button Sim;
    public Button Nao;
    

    void Awake()
    {
        var Root = GetComponent<UIDocument>();

        Menu = Root.rootVisualElement.Q<Button>("MainMenu");
        Sair = Root.rootVisualElement.Q<Button>("Sair");
        Sim = Root.rootVisualElement.Q<Button>("Sim");
        Nao = Root.rootVisualElement.Q<Button>("Nao");
        HUD = Root.rootVisualElement.Q<VisualElement>("Back");
        Confirm = Root.rootVisualElement.Q<VisualElement>("Confirma");

        Menu.clicked += MenuPressed;
        Sair.clicked += SairPressed;
        Sim.clicked += SimPressed;
        Nao.clicked += NaoPressed;
    }

    void MenuPressed()
    {
        SceneManager.LoadScene("Pedro_Scene");
    }

    void SairPressed()
    {
        Confirm.style.display = DisplayStyle.Flex;
    }

    void SimPressed()
    {
        Application.Quit();
        Debug.Log("Quitou");
    }
    
    void NaoPressed()
    {
        Confirm.style.display = DisplayStyle.None;
    }

    private void Update() 
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            HUD.style.display = DisplayStyle.Flex;
        }
        else if(Input.GetKeyUp(KeyCode.E))
        {
            HUD.style.display = DisplayStyle.None;
            Confirm.style.display = DisplayStyle.None;
        }
    }
    
}
