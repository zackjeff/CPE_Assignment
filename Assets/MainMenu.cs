   using UnityEngine;
   public class MainMenu : MonoBehaviour
   {
       public void PlayGame() { SceneTransition.Instance.GoTo("Game"); }
   }