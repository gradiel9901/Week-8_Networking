using TMPro;
using UnityEngine;
using static DogApiManager;

public class DogBreedEntry : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text nameText;
    public TMP_Text descriptionText;
    public TMP_Text statsText;

    public void Populate(BreedAttributes attrs)
    {
        if (nameText != null)
            nameText.text = attrs.name ?? "Unknown";

        if (descriptionText != null)
            descriptionText.text = attrs.description ?? string.Empty;

        if (statsText != null)
        {
            string hypo = attrs.hypoallergenic ? "Yes" : "No";
            statsText.text =
                $"Life: {attrs.life.min}-{attrs.life.max} yrs  |  " +
                $"Weight (M): {attrs.male_weight.min}-{attrs.male_weight.max} kg  |  " +
                $"Weight (F): {attrs.female_weight.min}-{attrs.female_weight.max} kg  |  " +
                $"Hypoallergenic: {hypo}";
        }
    }
}
