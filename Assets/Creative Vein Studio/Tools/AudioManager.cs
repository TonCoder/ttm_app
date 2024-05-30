using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _MainApp.Scripts.Brokers;
using Creative_Vein_Studio.Tools.Interfaces;
using Creative_Vein_Studio.Tools.Scriptable_Objects.Audio;
using CreativeVeinStudio.Simple_Dialogue_System.Attributes;
using MAIN_PROJECT._Scripts.Enums;
using MAIN_PROJECT._Scripts.Tools.Scriptable_Objects;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace Creative_Vein_Studio.Tools
{
    public class AudioManager : MonoBehaviour, IAudioActions
    {
        public SoAudioBroker broker;

        public bool makeSingleton = false;
        public static AudioManager Instance;

        [FieldTitle("Button Audio Clips")] [SerializeField]
        private AudioClip buttonClick_success;

        [SerializeField] private AudioClip buttonClick_Cancel;

        [FieldTitle("Audio List")] [SerializeField, ExposeSo]
        private SoSceneMusicList sceneMusicList;

        [FieldTitle("Audio Setup")] [SerializeField]
        private AudioMixer mixer;

        [SerializeField] private AudioMixerGroup musicGroup;
        [SerializeField] private AudioMixerGroup sfxGroup;

        [FormerlySerializedAs("OneShotsfxGroup")] [SerializeField]
        private AudioMixerGroup oneShotsfxGroup;

        [SerializeField] private AudioMixerGroup UiGroup;

        [FieldTitle("Audio Settings")] [SerializeField]
        private float transitionTime = 0.5f;

        [FieldTitle("Audio GameObjects")] [SerializeField]
        private AudioSource asMusic;

        [SerializeField] private AudioSource asSfx;
        [SerializeField] private AudioSource asSfxOneShot;
        [SerializeField] private AudioSource asUi;

        private MusicInfo _transitionToClip;
        private WaitForSeconds _waitForSeconds;

        private void Awake()
        {
            if (makeSingleton) Singleton();

            SceneManager.sceneLoaded += SetMusicByScene;
            Assert.IsNotNull(asMusic, "Audio Manager missing Audio Source Music");
            Assert.IsNotNull(asMusic, "Audio Manager missing Audio Source Sfx");
            Assert.IsNotNull(asMusic, "Audio Manager missing Audio Source OneShot");
            Assert.IsNotNull(asMusic, "Audio Manager missing Audio Source Ui");

            //Init
            asMusic.outputAudioMixerGroup = musicGroup;
            asSfx.outputAudioMixerGroup = sfxGroup;
            asSfxOneShot.outputAudioMixerGroup = oneShotsfxGroup;
            asUi.outputAudioMixerGroup = UiGroup;

            asMusic.loop = true;

            Assert.IsNotNull(sceneMusicList, "Missing Scene Music List");
            Assert.IsNotNull(broker, "The Audio Manager requires the AudioBroker to process requests");
        }

        private void OnEnable()
        {
            if (broker != null)
            {
                broker.onVolumeUpdate += UpdateVolume;
                broker.onPlayButtonCancel += PlayButtonCancel;
                broker.onPlayButtonSuccess += PlayButtonSuccess;
                broker.onPlayClipOnChannel += PlayAudioClipOnChannel;
            }
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= SetMusicByScene;

            if (broker != null) return;
            broker.onVolumeUpdate -= UpdateVolume;
            broker.onPlayButtonCancel -= PlayButtonCancel;
            broker.onPlayButtonSuccess -= PlayButtonSuccess;
            broker.onPlayClipOnChannel -= PlayAudioClipOnChannel;
        }

        private void SetMusicByScene(Scene scene, LoadSceneMode mode)
        {
            if (!sceneMusicList) return;

            _transitionToClip = sceneMusicList.collection?
                .FirstOrDefault(x => x._sceneName == scene.name);

            if (_transitionToClip == null || _transitionToClip._audioClip.Length <= 0)
            {
                return;
            }
#if UNITY_EDITOR
            Debug.LogWarning("Set Music by scene will always play the first audio ONLY");
#endif
            if (!asMusic) return;

            asMusic.clip = _transitionToClip._audioClip?.First();
            asMusic.volume = _transitionToClip._volume;
            asMusic.Play();
        }

        public void UpdateVolume(EAudioChannelVariableName channelVariableName, float volLevel = 1,
            float fadeDuration = 1)
        {
            // Gradually lower the volume over time
            StartCoroutine(FadeVolume(channelVariableName, volLevel, fadeDuration));
        }

        private IEnumerator FadeVolume(EAudioChannelVariableName channelVariableNameName, float targetVolume,
            float fadeDuration)
        {
            float startTime = Time.time;
            mixer.GetFloat(channelVariableNameName.ToString(), out float startVolume);
            float elapsedTime = 0;

            while (elapsedTime < fadeDuration)
            {
                elapsedTime = Time.time - startTime;
                float t = elapsedTime / fadeDuration;
                float newVolume = Mathf.Lerp(startVolume, targetVolume, t);
                mixer.SetFloat(channelVariableNameName.ToString(), newVolume);
                yield return null;
            }

            // Set the final volume
            mixer.SetFloat(channelVariableNameName.ToString(), targetVolume);
        }

        public void PlayButtonSuccess()
        {
            if (buttonClick_success == null)
            {
                Debug.LogWarning("Needs Button success audio clip");
                return;
            }

            asUi.clip = buttonClick_success;
            asUi.Play();
        }

        public void PlayButtonCancel()
        {
            if (buttonClick_Cancel == null)
            {
                Debug.LogWarning("Needs Button cancel audio clip");
                return;
            }

            asUi.clip = buttonClick_Cancel;
            asUi.Play();
        }

        public void PlaySfx(SoAudioClipInfo soAudioClipInfo)
        {
            if (soAudioClipInfo != null)
            {
                asSfx.clip = soAudioClipInfo.Clips[Random.Range(0, soAudioClipInfo.Clips.Count)];
                asSfx.volume = soAudioClipInfo.Volume;
                asSfx.pitch = Random.Range(soAudioClipInfo.Pitch.x, soAudioClipInfo.Pitch.y);
                asSfx.volume = soAudioClipInfo.Volume;
                asSfx.Play();
            }
        }

        public void PlayAudioClipOnChannel(List<AudioClip> clip, EAudioChannelVariableName channelVariableName)
        {
            switch (channelVariableName)
            {
                case EAudioChannelVariableName.MusicVolume:
                    asMusic.clip = clip[Random.Range(0, clip.Count)];
                    asMusic.Play();
                    break;
                case EAudioChannelVariableName.SfxVolume:
                    asSfx.clip = clip[Random.Range(0, clip.Count)];
                    asSfx.Play();
                    break;
                case EAudioChannelVariableName.OneShotSfxVolume:
                    asSfxOneShot.clip = clip[Random.Range(0, clip.Count)];
                    asSfxOneShot.Play();
                    break;
                case EAudioChannelVariableName.UiVolume:
                    asUi.clip = clip[Random.Range(0, clip.Count)];
                    asUi.Play();
                    break;
            }
        }

        private void Singleton()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
        }
    }
}