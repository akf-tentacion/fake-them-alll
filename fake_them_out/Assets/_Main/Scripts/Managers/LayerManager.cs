namespace AKUtil
{
    public static class GameLayer
    {
        /// <summary>
        /// 全てのレイヤーと衝突するレイヤーマスク
        /// </summary>
        /// <returns>The all collision mask.</returns>
        /// <param name="targetLayers">Target layers.</param>
        public static int GetAllCollisionLayerMask(this Layer[] targetLayers)
        {
            int layerMask = 0;
            foreach (var layer in targetLayers)
            {
                layerMask |= 1 << (int)layer;
            }
            return layerMask;
        }
    }
}

public enum Layer
{
    Player = 8,
    Interactive = 9
}