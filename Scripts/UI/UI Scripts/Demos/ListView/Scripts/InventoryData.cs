    using UnityEngine;

    public enum EInventoryClass
    {
        Seeds, Plants, Tools, Decor
    }

    [CreateAssetMenu]
    public class InventoryData : ScriptableObject
    {
        public string m_InventoryItemName;
        public EInventoryClass m_Class;
        public Sprite m_ItemImage;
    }