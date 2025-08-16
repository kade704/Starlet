using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CraftIngredientPanel : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private Text title;
    [SerializeField] private Text amount;

    private Coroutine animationCoroutine;

    public void Initialize(ItemAsset item, int craftAmount, int myAmount)
    {
        title.text = item.title;
        icon.sprite = item.icon;

        var myAmountText = myAmount < craftAmount ? $"<color=red>{myAmount}</color>" : $"{myAmount}";

        amount.text = $"{craftAmount}/{myAmountText}";
    }

    public void PlayAnimation()
    {
        if (animationCoroutine != null)
        {
            StopCoroutine(animationCoroutine);
        }
        animationCoroutine = StartCoroutine(PlayAnimationCoroutine());
    }

    private IEnumerator PlayAnimationCoroutine(float duration = 0.2f)
    {
        Color originalTextColor = amount.color;
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            amount.transform.localScale = Vector3.Lerp(Vector3.one * 1.1f, Vector3.one, elapsedTime / duration);
            amount.color = Color.Lerp(Color.white, originalTextColor, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
}
