using UnityEngine;

public class GameAssets : MonoBehaviour {

    public const uint unitLayer = 1u << 6;

    public static GameAssets Instance { get; private set; }

    private void Awake() {
        Instance = this;
    }

}
