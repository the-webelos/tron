using UnityEngine;
using UnityEngine.UI;

public class VolumeController : MonoBehaviour
{
    public Sprite volumeOnImage;
    public Sprite volumeMutedImage;

    Slider volumeSlider;

    public void Awake()
    {
        volumeSlider = gameObject.GetComponent<Slider>();
    }

    public void ToggleSlider(Slider slider) {
        slider.gameObject.SetActive(!slider.IsActive());
    }

    public void SetVolume(Slider slider) {
        AudioListener.volume = slider.value;
    }

    public void OnVolumeChanged(GameObject volumeToggle) {
        Image image = volumeToggle.GetComponent<Image>();

        if (volumeSlider.value <= 0) {
            image.sprite = volumeMutedImage;
        } else {
            image.sprite = volumeOnImage;
        }
    }
}
