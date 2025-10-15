using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 상호작용 가능한 오브젝튿들을 관리하는 컨트롤러
/// </summary>
public class InteractionController : MonoBehaviour
{
    [SerializeField] InteractableObject[] _interactors;   // 상호작용 가능한 오브젝트들을 에디터에서 할당받음

    Dictionary<InteractableType, IInteractable> _interactableMap = new();   // 상호작용 가능한 오브젝트들을 타입별로 저장할 딕셔너리

    public event Action<WeaponType> OnWeaponInteract;   // 무기와 상호작용했을 때 발행되는 이벤트

    public void Initialize()
    {
        InitailizeInteractableMap();
    }

    /// <summary>
    /// 상호작용 가능한 오브젝트들을 타입별로 맵핑
    /// </summary>
    void InitailizeInteractableMap()
    {
        foreach(var interactable in _interactors)
        {
            if (!_interactableMap.ContainsKey(interactable.InteractableType))
            {
                _interactableMap.Add(interactable.InteractableType, interactable);
            }

            // 각각의 상호작용 가능한 물체들의 이벤트를 분류하는 함수가 구독하도록 설정
            interactable.OnInteracted += PublishInteractableEventByType;
        }
    }

    /// <summary>
    /// 상호작용 가능한 물체의 상호작용 이벤트를 구독하여
    /// 상호작용 가능한 물체의 타입에 따라 적절한 이벤트를 발행
    /// </summary>
    /// <param name="interactable"></param>
    void PublishInteractableEventByType(IInteractable interactable)
    {
        if(interactable.InteractableType == InteractableType.Weapon)
        {
            var weaponInteractor = interactable as WeaponInteractor;
            if(weaponInteractor != null)
            {
                OnWeaponInteract?.Invoke(weaponInteractor.WeaponType);
            }
        }

    }
}
