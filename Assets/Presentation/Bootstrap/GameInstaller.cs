using UnityEngine;
using VectorArcade.Application.Ports;
using VectorArcade.Application.UseCases;
using VectorArcade.Domain.Services;
using VectorArcade.Infrastructure.Input;
using VectorArcade.Infrastructure.Rendering;
using VectorArcade.Infrastructure.Time;

namespace VectorArcade.Presentation.Bootstrap
{
    public sealed class GameInstaller : MonoBehaviour
    {
        [Header("Scene References")]
        public UnityTimeAdapter timeAdapter;
        public KeyboardMouseInputAdapter inputAdapter;
        public LineMeshBatchRenderer lineRenderer;
        public SystemRandomAdapter randomAdapter;

        [Header("Configs")]
        public SpawnerRules spawnerRules = new();
        public WeaponRules weaponRules = new();
        public ItemRules itemRules = new();

        [HideInInspector] public GameState gameState;
        [HideInInspector] public TickUseCase tickUC;
        [HideInInspector] public ShootUseCase shootUC;
        [HideInInspector] public PlayerControlUseCase playerCtrlUC;
        [HideInInspector] public AsteroidFieldUseCase fieldUC;
        [HideInInspector] public ItemUseCase itemUC;

        void Awake()
        {
            gameState = new GameState();
            gameState.Player.Forward = new Domain.Core.Vec3(0, 0, 1);

            tickUC = new TickUseCase(timeAdapter, weaponRules);
            shootUC = new ShootUseCase(inputAdapter, weaponRules);
            playerCtrlUC = new PlayerControlUseCase(inputAdapter, timeAdapter);
            fieldUC = new AsteroidFieldUseCase(randomAdapter, spawnerRules);
            itemUC = new ItemUseCase(timeAdapter, randomAdapter, itemRules);
        }

        void Start()
        {
            fieldUC.InitializeField(gameState);
        }
    }
}
