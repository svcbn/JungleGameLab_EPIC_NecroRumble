using UnityEngine;
using DG.Tweening;

public interface IShakable
{
    public void Shake(float power = 1f, float duration = 0.5f, int vibrato = 20, float randomness = 0f);
}