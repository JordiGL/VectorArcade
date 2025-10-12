// // Assets/Application/UseCases/SpawnUseCase.cs
// using VectorArcade.Application.Ports;
// using VectorArcade.Domain.Entities;
// using VectorArcade.Domain.Services;
// using VectorArcade.Domain.Core;

// namespace VectorArcade.Application.UseCases
// {
//     public sealed class SpawnUseCase
//     {
//         private readonly ITimeProvider _time;
//         private readonly IRandomProvider _rng;
//         private float _accum;

//         public SpawnerRules Rules { get; }

//         public SpawnUseCase(ITimeProvider time, IRandomProvider rng, SpawnerRules rules)
//         {
//             _time = time; _rng = rng; Rules = rules;
//         }

//         // Spawnea delante del jugador en +Z mundial
//         public void Execute(GameState state)
//         {
//             _accum += _time.DeltaTime;
//             if (_accum < Rules.SpawnEverySeconds) return;
//             _accum = 0f;

//             float dist = _rng.NextFloat(Rules.MinZ, Rules.MaxZ);
//             float lateralX = _rng.NextFloat(-Rules.LateralRange, Rules.LateralRange);
//             float lateralY = _rng.NextFloat(-Rules.LateralRange * 0.6f, Rules.LateralRange * 0.6f);
//             float r = _rng.NextFloat(Rules.MinRadius, Rules.MaxRadius);

//             // Base delante en +Z mundial (no depende de la mirada)
//             var basePos = state.Player.Position + new Vec3(0, 0, 1) * dist;

//             // Ejes laterales fijos (mundo)
//             var right = new Vec3(1, 0, 0);
//             var up    = new Vec3(0, 1, 0);

//             var pos = basePos + right * lateralX + up * lateralY;

//             state.Asteroids.Add(new Asteroid { Position = pos, Radius = r, Alive = true });
//         }

//     }
// }
