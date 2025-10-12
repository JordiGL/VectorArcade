// Assets/Presentation/Bootstrap/GameController.cs
using UnityEngine;
using VectorArcade.Presentation.HUD;

namespace VectorArcade.Presentation.Bootstrap
{
    public sealed class GameController : MonoBehaviour
    {
        public GameInstaller installer;

        void Update()
        {
            // 1) Mirar y avanzar (recto en +Z mundial)
            installer.playerCtrlUC.Execute(installer.gameState);

            // 2) Mantener el campo de asteroides alrededor del jugador
            installer.fieldUC.UpdateField(installer.gameState);

            // 3) Lógica (balas, colisiones, score)
            installer.tickUC.Execute(installer.gameState);
            installer.shootUC.Execute(installer.gameState);

            // 4) Cámara = posición + forward del player
            var p = installer.gameState.Player;
            var pos = new Vector3(p.Position.x, p.Position.y, p.Position.z);
            var fwd = new Vector3(p.Forward.x, p.Forward.y, p.Forward.z);
            Camera.main.transform.SetPositionAndRotation(pos, Quaternion.LookRotation(fwd, Vector3.up));

            // 5) Dibujar
            VectorHudPresenter.UpdateFps(); // antes de dibujar HUD
            installer.lineRenderer.BeginFrame();
            WireframePresenter.DrawAll(installer.lineRenderer, installer.gameState, Camera.main);
            VectorHudPresenter.DrawScore(installer.lineRenderer, installer.gameState, Camera.main);
            VectorHudPresenter.DrawFps(installer.lineRenderer, Camera.main);
            installer.lineRenderer.EndFrame();
        }
    }
}
