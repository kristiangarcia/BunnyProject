using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using BunnyGame.Core;

namespace BunnyGame.Player
{
    /// <summary>
    /// Maneja las entradas especiales del jugador (menú, pausa, etc.)
    /// </summary>
    public class PlayerInputHandler : MonoBehaviour
    {
        public void OnAbrirMenuInicial(InputAction.CallbackContext contexto)
        {
            if (contexto.performed)
            {
                Debug.Log("<color=#FFFF00>Abriendo menú inicial...</color>");
                SceneManager.LoadScene(GameConstants.SCENE_MAIN_MENU);
            }
        }
    }
}
