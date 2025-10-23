using UnityEngine;

public enum TechSelectPackType
{
    Company1,
    Company2,
    Company3,
    Company4,
    Company5,
}

public class TechSelectPack : MonoBehaviour
{
    [SerializeField] TechSelectPackInteractor _interactor;
    [SerializeField] TechSelectPackType _packType;

    public void Initialize(TechSelectPackType packType)
    {
        _packType = packType;
    }
}
