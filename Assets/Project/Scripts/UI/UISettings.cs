using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VContainer;

public class UISettings : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Button openBtn;
    [SerializeField] private Button closeBtn;
    [Space(5)]
    [SerializeField] private GameObject settingPanel;
    [Space(5)]
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Button resetBtn;
    [SerializeField] private TextMeshProUGUI resetBtnTxt;
    [Header("Other")]
    [SerializeField] private Color resetBtnDefaultClr;
    [SerializeField] private Color resetBtnWarningClr;
    private SaveService saveService;
    private MusicController musicController;

    private bool isReadyForReset = false;

    [Inject]
    public void Construct(SaveService saveService, MusicController musicController)
    {
        this.saveService = saveService;
        this.musicController = musicController;
    }

    void Awake()
    {
        if (!ValidateSerializedReferences(nameof(Awake)))
            return;

        openBtn.onClick.AddListener(() => OpenSettings());
        closeBtn.onClick.AddListener(() => CloseSettings());

        float defaultVolume = PlayerPrefs.GetFloat(nameof(PlayerPrefsEnums.MusicVolume), 0.5f);

        musicVolumeSlider.value = defaultVolume;
        musicVolumeSlider.onValueChanged.AddListener((float f) => SetMusicVolume(f));

        resetBtn.onClick.AddListener(async () => await ResetProgress());

        CloseSettings();
    }
    private void OpenSettings()
    {
        settingPanel.SetActive(true);
    }
    private void CloseSettings()
    {
        settingPanel.SetActive(false);

        isReadyForReset = false;
        resetBtn.interactable = true;
        resetBtnTxt.SetText("Reset progress");
        resetBtn.image.color = resetBtnDefaultClr;
    }
    private async Task ResetProgress()
    {
        if (saveService == null)
        {
            Debug.LogError($"{nameof(UISettings)}.{nameof(ResetProgress)}: {nameof(saveService)} is null on {name}.");
            return;
        }

        if (!isReadyForReset)
        {
            isReadyForReset = true;
            resetBtnTxt.SetText("Are you sure?");
            resetBtn.image.color = resetBtnWarningClr;
            return;
        }

        resetBtn.interactable = false;

        await Task.Delay(1000);

        EventBus.ClearAll();

        await saveService.DeleteSaveAsync();

        PlayerPrefs.Save();

        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex);
    }
    private void SetMusicVolume(float f)
    {
        PlayerPrefs.SetFloat(nameof(PlayerPrefsEnums.MusicVolume), f);

        if (musicController == null)
        {
            Debug.LogError($"{nameof(UISettings)}.{nameof(SetMusicVolume)}: {nameof(musicController)} is null on {name}.");
            return;
        }

        musicController.ChangeVolume(f);
    }

    private bool ValidateSerializedReferences(string caller)
    {
        if (openBtn == null) return LogMissingReference(nameof(openBtn), caller);
        if (closeBtn == null) return LogMissingReference(nameof(closeBtn), caller);
        if (settingPanel == null) return LogMissingReference(nameof(settingPanel), caller);
        if (musicVolumeSlider == null) return LogMissingReference(nameof(musicVolumeSlider), caller);
        if (resetBtn == null) return LogMissingReference(nameof(resetBtn), caller);
        if (resetBtnTxt == null) return LogMissingReference(nameof(resetBtnTxt), caller);
        return true;
    }

    private bool LogMissingReference(string referenceName, string caller)
    {
        Debug.LogError($"{nameof(UISettings)}.{caller}: {referenceName} is null on {name}.");
        return false;
    }
}
