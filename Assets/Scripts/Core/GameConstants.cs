namespace BunnyGame.Core
{
    /// <summary>
    /// Constantes globales del juego para evitar números y strings mágicos
    /// </summary>
    public static class GameConstants
    {
        // Nombres de Escenas
        public const string SCENE_MAIN_MENU = "MenuInicial";
        public const string SCENE_GAME = "Game";

        // Tags
        public const string TAG_ENEMY = "Enemigo";
        public const string TAG_TRAP = "Trampa";
        public const string TAG_COLLECTIBLE = "Objeto";
        public const string TAG_POWERUP = "Powerup";
        public const string TAG_PLAYER = "Player";

        // Capas
        public const string LAYER_GROUND = "Suelo";

        // Índices de clips de audio
        public const int SFX_COLLECT_STAR = 0;
        public const int SFX_JUMP = 1;
        public const int SFX_POWERUP = 2;
        public const int SFX_ENEMY_KILL = 3;
        public const int SFX_PLAYER_DEATH = 4;
        public const int SFX_COUNTDOWN = 5;
        public const int SFX_TIME_UP = 6;

        // Nombres de GameObjects
        public const string GO_MUSIC_MANAGER = "Música";

        // Leaderboard API - DreamLo (http://dreamlo.com)
        public const string DREAMLO_PUBLIC_KEY = "693b60578f40bb100460dd03";
        public const string DREAMLO_PRIVATE_KEY = "FtCXl91fxUe1pZeqVoYLrgu5ALQ89ZtkKg5e7Vnm2eQg";
        public const string DREAMLO_BASE_URL = "http://dreamlo.com/lb/";
        public const int LEADERBOARD_MAX_ENTRIES = 10;

        // Sistema de Puntuación
        public const int STARS_SCORE_MULTIPLIER = 100;
        public const int TIME_SCORE_MULTIPLIER = 10;

        // PlayerPrefs Keys
        public const string PREF_PLAYER_NAME = "PlayerName";
    }
}
