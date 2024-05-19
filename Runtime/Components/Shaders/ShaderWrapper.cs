using UnityEngine;

namespace EwigeDreamer.Additional.Components.Shaders
{
    [RequireComponent(typeof(Renderer))]
    public abstract class ShaderWrapper : MonoBehaviour
    {
        private new Renderer renderer;
        protected Renderer Renderer => renderer != null ? renderer : (renderer = GetComponent<Renderer>());

        protected Material Material => Renderer != null ? Renderer.material : null;
    }
}