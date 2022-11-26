using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Core.SceneManagement;
using GrandmaGreen.Entities;


namespace GrandmaGreen
{
    public interface IGameTile
    {
        void DoTileAction();
        void DoTileAction(EntityController entity);
    }

    [CreateAssetMenu(menuName = "GrandmaGreen/Tiles", fileName = "GamePortalTile")]
    public class GamePortalTile : Tile, IGameTile
    {
        public SCENES toLoad;
        public Direction portalDirection;
        public AreaExitState exitState;
        
        public virtual void DoTileAction()
        {
            exitState.exitSide = portalDirection;
            toLoad.LoadAsync();
        }

        public virtual void DoTileAction(EntityController entity)
        {
            DoTileAction();
        }

    }
}
