using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections;
using System.Collections.Generic;
using BunnyGame.Core;
using BunnyGame.Data;

namespace BunnyGame.Managers
{
    /// <summary>
    /// Manager singleton para gestionar el leaderboard global usando DreamLo API
    /// </summary>
    public class LeaderboardManager : MonoBehaviour
    {
        private static LeaderboardManager instance;
        public static LeaderboardManager Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject go = new GameObject("LeaderboardManager");
                    instance = go.AddComponent<LeaderboardManager>();
                    DontDestroyOnLoad(go);
                }
                return instance;
            }
        }

        // Eventos para notificar cuando se completan operaciones
        public event Action OnSubmitSuccess;
        public event Action<string> OnSubmitError;
        public event Action<List<LeaderboardEntry>> OnLeaderboardLoaded;
        public event Action<string> OnLeaderboardError;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }
        }

        private void OnDestroy()
        {
            // Limpiar la instancia si este objeto se destruye
            if (instance == this)
            {
                instance = null;
            }
        }

        /// <summary>
        /// Calcula la puntuación basada en estrellas y tiempo restante
        /// </summary>
        public int CalculateScore(int stars, float timeRemaining)
        {
            int baseScore = stars * GameConstants.STARS_SCORE_MULTIPLIER;
            int timeBonus = Mathf.FloorToInt(timeRemaining * GameConstants.TIME_SCORE_MULTIPLIER);
            return baseScore + timeBonus;
        }

        /// <summary>
        /// Verifica si hay conexión a internet
        /// </summary>
        public bool IsOnline()
        {
            return Application.internetReachability != NetworkReachability.NotReachable;
        }

        /// <summary>
        /// Envía un score al leaderboard de DreamLo
        /// </summary>
        public void SubmitScore(string playerName, int stars, float timeRemaining)
        {
            if (!IsOnline())
            {
                OnSubmitError?.Invoke("Sin conexión a internet");
                return;
            }

            // Validar nombre
            if (!ValidatePlayerName(playerName))
            {
                OnSubmitError?.Invoke("Nombre inválido. Usa 3-15 caracteres alfanuméricos.");
                return;
            }

            int score = CalculateScore(stars, timeRemaining);
            int timeInt = Mathf.FloorToInt(timeRemaining);

            // Validar score razonable (máximo 10 estrellas * 100 + 60s * 10 = 1600)
            if (score > 2000 || score < 0)
            {
                OnSubmitError?.Invoke("Puntuación inválida");
                return;
            }

            StartCoroutine(SubmitScoreCoroutine(playerName, score, timeInt));
        }

        /// <summary>
        /// Obtiene el leaderboard de DreamLo
        /// </summary>
        public void GetLeaderboard(int maxEntries = -1)
        {
            if (!IsOnline())
            {
                OnLeaderboardError?.Invoke("Sin conexión a internet");
                return;
            }

            if (maxEntries <= 0)
                maxEntries = GameConstants.LEADERBOARD_MAX_ENTRIES;

            StartCoroutine(GetLeaderboardCoroutine(maxEntries));
        }

        /// <summary>
        /// Valida que el nombre del jugador sea válido
        /// </summary>
        private bool ValidatePlayerName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return false;

            if (name.Length < 3 || name.Length > 15)
                return false;

            // Solo permitir alfanuméricos y guiones bajos
            foreach (char c in name)
            {
                if (!char.IsLetterOrDigit(c) && c != '_')
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Corrutina para enviar score a DreamLo
        /// </summary>
        private IEnumerator SubmitScoreCoroutine(string playerName, int score, int timeRemaining)
        {
            // URL format: http://dreamlo.com/lb/{PRIVATE_KEY}/add/{PLAYER_NAME}/{SCORE}
            string url = $"{GameConstants.DREAMLO_BASE_URL}{GameConstants.DREAMLO_PRIVATE_KEY}/add/{UnityWebRequest.EscapeURL(playerName)}/{score}";

            Debug.Log($"Enviando score a DreamLo: {playerName} - {score} puntos");
            Debug.Log($"URL completa: {url}");

            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                request.timeout = 10;
                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    Debug.Log("Score enviado exitosamente");
                    OnSubmitSuccess?.Invoke();

                    // Guardar nombre del jugador para próxima vez
                    PlayerPrefs.SetString(GameConstants.PREF_PLAYER_NAME, playerName);
                    PlayerPrefs.Save();
                }
                else
                {
                    string error = $"Error al enviar score: {request.error}";
                    Debug.LogError(error);
                    OnSubmitError?.Invoke(error);
                }
            }
        }

        /// <summary>
        /// Corrutina para obtener leaderboard de DreamLo
        /// </summary>
        private IEnumerator GetLeaderboardCoroutine(int maxEntries)
        {
            // URL format: http://dreamlo.com/lb/{PUBLIC_KEY}/pipe/{MAX_ENTRIES}
            string url = $"{GameConstants.DREAMLO_BASE_URL}{GameConstants.DREAMLO_PUBLIC_KEY}/pipe/{maxEntries}";

            Debug.Log($"Obteniendo leaderboard de DreamLo (Top {maxEntries})");

            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                request.timeout = 10;
                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    string response = request.downloadHandler.text;
                    Debug.Log($"Leaderboard obtenido: {response}");

                    List<LeaderboardEntry> entries = LeaderboardEntry.ParseDreamLoResponse(response);
                    OnLeaderboardLoaded?.Invoke(entries);
                }
                else
                {
                    string error = $"Error al obtener leaderboard: {request.error}";
                    Debug.LogError(error);
                    OnLeaderboardError?.Invoke(error);
                }
            }
        }

        /// <summary>
        /// Obtiene el último nombre de jugador guardado
        /// </summary>
        public string GetSavedPlayerName()
        {
            return PlayerPrefs.GetString(GameConstants.PREF_PLAYER_NAME, "");
        }
    }
}
