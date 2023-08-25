using UnityEngine;
using TMPro;
using DG.Tweening;
public class ZoomAnimation : MonoBehaviour
{
    public float zoomDuration = 2.0f;
    public float zoomScale = 1.5f;
    
    public void OnEnable()
    {
        
        this.transform.localScale = Vector3.zero;

        this.transform.DOScale(Vector3.one * zoomScale, zoomDuration)
            .SetEase(Ease.OutBack);
    }
    
}
