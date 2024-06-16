using UnityEngine;
using UnityEngine.UI;

namespace ED.Additional.Components.Shaders
{
    [RequireComponent(typeof(Image))]
    public abstract class ShaderWrapperUI : MonoBehaviour
    {
        private Image image;

        protected virtual void Awake()
        {
            image = GetComponent<Image>();
        }
		
        private Material material = null;

        protected Material Material
        {
            get
            {
                if (material == null)
                {
                    material = new Material(image.material);
                    image.material = material;
                }

                return image.materialForRendering;
            }
        }

        protected virtual void OnDestroy() {
            if (material != null) {
                DestroyImmediate(material);
            }
        }
    }
}