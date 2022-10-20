using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UISettingElement : MonoBehaviour
{
    [System.Serializable]
    public class UIElement
    {
        public bool roundValue = false;
        public TextMeshProUGUI UIText;
        public TextMeshProUGUI DisplayResult;
        public Slider Slide;
        public float min;
        public float max;
        public TMP_Dropdown List;
    }

    public UIElement Element;

    private void Start()
    {
        UIManager.Instance.RegisterUIElement(gameObject);

        UpdateVisual();
    }

    public void UpdateVisual()
    {
        if(Element.Slide != null)
        {
            Element.Slide.minValue = Element.min;
            Element.Slide.maxValue = Element.max;

            if(Element.roundValue)
            {
                Element.DisplayResult.text = (Mathf.Round(Element.Slide.value)).ToString();
            }
            else
            {
                Element.DisplayResult.text = (Element.Slide.value).ToString();
            }
            
        }
    }
}
