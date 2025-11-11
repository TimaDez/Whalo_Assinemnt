using Cysharp.Threading.Tasks;
using Models;
using UnityEngine;

namespace Infrastructure
{
    public interface IInteractable
    {
        UniTask Interact();
    }
}