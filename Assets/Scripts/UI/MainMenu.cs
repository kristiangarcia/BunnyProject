using UnityEngine;
using UnityEngine.SceneManagement;
using BunnyGame.Core;

namespace BunnyGame.UI
{
    /// <summary>
    /// Controlador del menú principal
    /// </summary>
    public class MainMenu : MonoBehaviour
    {
        [Header("Referencias de UI")]
        [SerializeField] private GameObject menuPrincipalPanel;
        [SerializeField] private GameObject opcionesPanel;
        [SerializeField] private GameObject leaderboardPanel;

        private void Start()
        {
            // Asegurarse de que el menú principal está activo y opciones desactivado
            if (menuPrincipalPanel != null)
                menuPrincipalPanel.SetActive(true);

            if (opcionesPanel != null)
                opcionesPanel.SetActive(false);

            if (leaderboardPanel != null)
                leaderboardPanel.SetActive(false);

            // Inicializar SettingsManager para aplicar configuración guardada
            var settings = SettingsManager.Instance;
        }

        public void Jugar()
        {
            // Cargar la siguiente escena en el build index
            int siguienteEscena = SceneManager.GetActiveScene().buildIndex + 1;

            if (siguienteEscena < SceneManager.sceneCountInBuildSettings)
            {
                SceneManager.LoadScene(siguienteEscena);
            }
            else
            {
                Debug.LogWarning("No hay más escenas en Build Settings");
            }
        }

        public void CargarEscena(string nombreEscena)
        {
            SceneManager.LoadScene(nombreEscena);
        }

        public void AbrirOpciones()
        {
            if (menuPrincipalPanel != null)
                menuPrincipalPanel.SetActive(false);

            if (opcionesPanel != null)
                opcionesPanel.SetActive(true);
        }

        public void CerrarOpciones()
        {
            if (opcionesPanel != null)
                opcionesPanel.SetActive(false);

            if (menuPrincipalPanel != null)
                menuPrincipalPanel.SetActive(true);
        }

        public void AbrirLeaderboard()
        {
            if (menuPrincipalPanel != null)
                menuPrincipalPanel.SetActive(false);

            if (leaderboardPanel != null)
                leaderboardPanel.SetActive(true);

            // Intentar cargar el leaderboard automáticamente
            LeaderboardPanel panel = FindObjectOfType<LeaderboardPanel>();
            if (panel != null)
            {
                panel.ShowLeaderboard();
            }
        }

        public void CerrarLeaderboard()
        {
            if (leaderboardPanel != null)
                leaderboardPanel.SetActive(false);

            if (menuPrincipalPanel != null)
                menuPrincipalPanel.SetActive(true);
        }

        public void Salir()
        {
            Debug.Log("Saliendo del juego...");

            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }
    }
}
