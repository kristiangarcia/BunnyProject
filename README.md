# BunnyProject

Un juego platformer 2D desarrollado en Unity con mecánicas clásicas de plataformas y enemigos con inteligencia artificial.

## Características

### Mecánicas de Juego
- **Sistema de Movimiento Avanzado**: Control preciso con física personalizada y jump buffering
- **Sistema de Combate**: Elimina enemigos saltando sobre ellos
- **Coleccionables**: Recoge estrellas para aumentar tu puntuación
- **Powerups**: Potenciadores que aumentan velocidad y capacidad de salto
- **Sistema de Vidas**: Gestión de salud del jugador con respawn
- **Límite de Tiempo**: Completa el nivel antes de que se acabe el tiempo

### Enemigos con IA
- **Abejas**: Patrullan por el aire
- **Babosas**: Se mueven por el suelo
- **Plantas Piraña**: Enemigos estáticos peligrosos
- **Zanahorias**: Enemigos con comportamiento único
- **NavMesh AI**: Inteligencia artificial basada en NavMesh para pathfinding 2D

### Sistemas del Juego
- **Leaderboard Online**: Tabla de clasificación global usando DreamLo API
- **Sistema de Audio**: Música 8-bit y efectos de sonido
- **Gestión de Configuración**: Opciones de volumen y configuración
- **HUD Dinámico**: Muestra puntuación, vidas y tiempo restante
- **Efectos Visuales**: Parallax scrolling y screen shake
- **Sistema de Respawn**: Reaparición inteligente del jugador

## Controles

| Acción | Tecla/Control |
|--------|---------------|
| Mover | A/D o Flechas ← → |
| Saltar | Espacio o W |
| Agacharse | S o Flecha ↓ |
| Pausar | ESC |

## Tecnologías Utilizadas

- **Unity 2022+** - Motor de juego
- **Unity Input System** - Sistema moderno de input
- **NavMesh Components** - Navegación 2D para enemigos
- **Universal Render Pipeline (URP)** - Pipeline de renderizado
- **TextMesh Pro** - Sistema de texto
- **DreamLo API** - Leaderboard online

## Estructura del Proyecto

```
Assets/
├── Animations/          # Animaciones de personajes y objetos
├── Prefabs/            # Prefabs de enemigos, coleccionables y UI
├── Scenes/             # Escenas del juego
│   ├── MenuInicial     # Menú principal
│   └── Juego           # Nivel principal
├── Scripts/
│   ├── Core/           # Constantes y lógica central
│   ├── Player/         # Movimiento, combate, salud del jugador
│   ├── Enemies/        # Sistema de enemigos y IA
│   ├── Managers/       # GameManager, AudioManager, etc.
│   ├── UI/             # Controladores de interfaz
│   ├── Effects/        # Efectos visuales (parallax, camera shake)
│   └── Interfaces/     # Interfaces (IDamageable, IRespawnable)
├── Settings/           # Configuración de URP y build profiles
├── Sound/              # Música y efectos de sonido
└── PhysicsMaterials/   # Materiales de física 2D

```

## Arquitectura del Código

El proyecto sigue una arquitectura modular con separación de responsabilidades:

- **Namespaces organizados**: `BunnyGame.Player`, `BunnyGame.Enemies`, `BunnyGame.Managers`, etc.
- **Patrón Singleton**: Para managers globales (AudioManager, GameManager)
- **Interfaces**: Sistema de interfaces para comportamientos reutilizables
- **Constantes centralizadas**: `GameConstants.cs` para evitar valores mágicos
- **Componentes desacoplados**: Cada script tiene una responsabilidad única

## Sistema de Puntuación

- **Estrellas**: +100 puntos por estrella
- **Tiempo restante**: +10 puntos por segundo sobrante
- **Eliminación de enemigos**: Puntos bonus

## Instalación y Uso

1. Clona el repositorio:
```bash
git clone https://github.com/kristiangarcia/BunnyProject.git
```

2. Abre el proyecto en Unity 2022 o superior

3. Asegúrate de tener instalado:
   - Universal Render Pipeline (URP)
   - Input System Package
   - TextMesh Pro

4. Abre la escena `MenuInicial` y presiona Play

## Build

El proyecto incluye perfiles de build para:
- macOS (configurado)
- Windows (pendiente configuración)
- WebGL (pendiente configuración)

## Créditos

### Assets
- **Música**: 8Bit Music Pack (062022)
- **Efectos de Sonido**: Game Sound Solutions - 8 bits Elements
- **NavMesh 2D**: NavMeshPlus (modificado)

## Licencia

Este proyecto es de código abierto y está disponible para fines educativos.

---

Desarrollado como proyecto de aprendizaje de desarrollo de videojuegos en Unity.
