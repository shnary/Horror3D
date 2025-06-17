namespace Core.Systems {
    public interface ISavable {
        
        string SaveKey { get; }
        
        object OnSerialize();

        void OnDeserialize(object serializedData);
    }
}