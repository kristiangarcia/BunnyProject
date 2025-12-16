using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using BunnyGame.Core;
using BunnyGame.Managers;
using BunnyGame.Data;

namespace BunnyGame.UI
{
    /// <summary>
    /// Panel para visualizar el ranking global del leaderboard
    /// </summary>
    public class LeaderboardPanel : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject panel;
        [SerializeField] private Transform contentContainer; // Container del ScrollView
        [SerializeField] private GameObject entryPrefab; // Prefab para cada entry del leaderboard
        [SerializeField] private Button refreshButton;
        [SerializeField] private Button cerrarButton;
        [SerializeField] private GameObject loadingIndicator;
        [SerializeField] private TextMeshProUGUI errorTexto;

        private List<GameObject> currentEntries = new List<GameObject>();

        private void Start()
        {
            // Ocultar panel al inicio
            if (panel != null)
                panel.SetActive(false);

            // Configurar botones
            if (refreshButton != null)
                refreshButton.onClick.AddListener(RefreshLeaderboard);

            if (cerrarButton != null)
                cerrarButton.onClick.AddListener(HideLeaderboard);

            // Suscribirse a eventos
            LeaderboardManager.Instance.OnLeaderboardLoaded += OnLeaderboardLoaded;
            LeaderboardManager.Instance.OnLeaderboardError += OnLeaderboardError;
        }

        private void OnDestroy()
        {
            if (LeaderboardManager.Instance != null)
            {
                LeaderboardManager.Instance.OnLeaderboardLoaded -= OnLeaderboardLoaded;
                LeaderboardManager.Instance.OnLeaderboardError -= OnLeaderboardError;
            }
        }

        /// <summary>
        /// Muestra el panel y carga el leaderboard
        /// </summary>
        public void ShowLeaderboard()
        {
            if (panel != null)
                panel.SetActive(true);

            RefreshLeaderboard();
        }

        /// <summary>
        /// Oculta el panel
        /// </summary>
        public void HideLeaderboard()
        {
            if (panel != null)
                panel.SetActive(false);
        }

        /// <summary>
        /// Recarga el leaderboard desde la API
        /// </summary>
        public void RefreshLeaderboard()
        {
            // Limpiar entries anteriores
            ClearEntries();

            // Mostrar loading
            ShowLoading(true);
            ShowError("");

            // Obtener leaderboard
            LeaderboardManager.Instance.GetLeaderboard(GameConstants.LEADERBOARD_MAX_ENTRIES);
        }

        private void OnLeaderboardLoaded(List<LeaderboardEntry> entries)
        {
            ShowLoading(false);

            Debug.Log($"OnLeaderboardLoaded llamado. Entries: {(entries == null ? "null" : entries.Count.ToString())}");

            if (entries == null || entries.Count == 0)
            {
                ShowError("No hay puntuaciones registradas todavia.\nSe el primero en el ranking!");
                return;
            }

            PopulateLeaderboard(entries);
        }

        private void OnLeaderboardError(string error)
        {
            ShowLoading(false);
            ShowError($"Error al cargar ranking:\n{error}");
        }

        /// <summary>
        /// Pobla el leaderboard con las entries
        /// </summary>
        private void PopulateLeaderboard(List<LeaderboardEntry> entries)
        {
            Debug.Log($"PopulateLeaderboard llamado con {entries.Count} entries");

            if (contentContainer == null)
            {
                Debug.LogError("ContentContainer no esta asignado en LeaderboardPanel");
                return;
            }

            if (entryPrefab == null)
            {
                Debug.LogError("EntryPrefab no esta asignado en LeaderboardPanel");
                ShowError("Error: Prefab no configurado");
                return;
            }

            Debug.Log($"ContentContainer: {contentContainer.name}, EntryPrefab: {entryPrefab.name}");

            for (int i = 0; i < entries.Count; i++)
            {
                LeaderboardEntry entry = entries[i];
                Debug.Log($"Creando entrada {i + 1}: {entry.playerName} - {entry.score}");

                // Instanciar prefab
                GameObject entryObj = Instantiate(entryPrefab, contentContainer);
                currentEntries.Add(entryObj);
                entryObj.SetActive(true);

                // Buscar componentes de texto en el prefab
                TextMeshProUGUI[] texts = entryObj.GetComponentsInChildren<TextMeshProUGUI>();

                Debug.Log($"Entry {i + 1} tiene {texts.Length} TextMeshProUGUI");

                if (texts.Length >= 3)
                {
                    // Asumiendo que los textos estan en orden: Rank, Name, Score
                    texts[0].text = $"#{i + 1}";
                    texts[1].text = entry.playerName;
                    texts[2].text = entry.score.ToString();
                    Debug.Log($"Textos asignados: {texts[0].text}, {texts[1].text}, {texts[2].text}");
                }
                else
                {
                    Debug.LogWarning($"Entry prefab no tiene suficientes TextMeshProUGUI. Tiene {texts.Length}, necesita 3");
                }
            }

            Debug.Log($"Leaderboard poblado con {entries.Count} entries");
        }

        /// <summary>
        /// Limpia todas las entries del leaderboard
        /// </summary>
        private void ClearEntries()
        {
            foreach (GameObject entry in currentEntries)
            {
                if (entry != null)
                    Destroy(entry);
            }

            currentEntries.Clear();
        }

        /// <summary>
        /// Muestra u oculta el indicador de carga
        /// </summary>
        private void ShowLoading(bool show)
        {
            if (loadingIndicator != null)
                loadingIndicator.SetActive(show);
        }

        /// <summary>
        /// Muestra un mensaje de error
        /// </summary>
        private void ShowError(string message)
        {
            if (errorTexto != null)
            {
                errorTexto.text = message;
                errorTexto.gameObject.SetActive(!string.IsNullOrEmpty(message));
            }
        }
    }
}
