using System.Collections;
using UnityEngine;

public class CardAnimation : MonoBehaviour
{
    public IEnumerator RotateCard(float duration, int spins)
    {
        float angle = 360f * spins;
        Quaternion startRotation = transform.rotation;
        Quaternion endRotation = startRotation * Quaternion.Euler(0, 0, angle);
        float elapsed = 0f;

        while (elapsed < duration)
        {
            transform.rotation = Quaternion.Slerp(startRotation, endRotation, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.rotation = endRotation;
    }
}
