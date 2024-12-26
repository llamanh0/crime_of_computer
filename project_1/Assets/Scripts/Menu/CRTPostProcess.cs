using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CRTPostProcess : MonoBehaviour
{
    private Volume volume;
    private ChromaticAberration chromatic;
    private FilmGrain filmGrain;
    private LensDistortion lensDistortion;
    private Vignette vignette;

    private void Awake()
    {
        volume = GetComponent<Volume>();
        if (volume != null && volume.profile != null)
        {
            // Chromatic Aberration
            if (volume.profile.TryGet(out chromatic))
            {
                chromatic.intensity.Override(0.2f);  // Kenarlardaki renk ayrışma
            }
            // Film Grain
            if (volume.profile.TryGet(out filmGrain))
            {
                filmGrain.type.Override(FilmGrainLookup.Thin1);
                filmGrain.intensity.Override(0.4f);
            }
            // Lens Distortion
            if (volume.profile.TryGet(out lensDistortion))
            {
                lensDistortion.intensity.Override(0.15f);
            }
            // Vignette
            if (volume.profile.TryGet(out vignette))
            {
                vignette.intensity.Override(0.3f);
                vignette.smoothness.Override(0.6f);
            }
        }
    }
}
