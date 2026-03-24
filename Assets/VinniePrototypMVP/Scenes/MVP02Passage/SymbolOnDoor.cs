using Justin;
using UnityEngine;


namespace Justin
{
    public class SymbolOnDoor : MonoBehaviour
    {
        [SerializeField] SpriteRenderer symbolRenderer;
        [SerializeField] DialogVariablesSO dialogVariables;
        [SerializeField] ParticleSystem colorChangeEffect;

        void Start()
        {

            ParseColorName();
        }

        public void ParseColorName()
        {
            if (dialogVariables.dic.TryGetValue("_favoriteColor", out string colorName) && !colorName.Contains("null"))
            {
                if (ColorUtility.TryParseHtmlString(colorName, out Color newColor))
                    symbolRenderer.color = newColor;
            }
            else
            {
                dialogVariables.dic["_favoriteColor"] = "#FFFFFF";
            }
        }
        public void StartParticleEffect()
        {
            var main = colorChangeEffect.main;
            main.startColor = symbolRenderer.color;
            colorChangeEffect.Play();
        }
    }
}
