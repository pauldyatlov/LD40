﻿using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] private InteractionButton _interactionButton;
    [SerializeField] private Text _catCounter;
    [SerializeField] private Slider _slider;

    [SerializeField] private Button _pauseButton;
    [SerializeField] private GameObject _pauseScreen;

    [SerializeField] private Button _helpButton;
    [SerializeField] private GameObject _helpScreen;

    [SerializeField] private Button _restartButton;
    [SerializeField] private Button _exitButton;

    [SerializeField] private CatUi _catUiTemplate;

    [SerializeField] private GameObject _won;
    [SerializeField] private GameObject _lost;

    private Transform _point;
    private Transform _target;

    private float _startDistance;
    private bool _end;

    private void Awake()
    {
        _pauseButton.onClick.AddListener(() =>
        {
            SetPauseStatus(!_pauseScreen.activeSelf);
        });

        _helpButton.onClick.AddListener(() =>
        {
            _helpScreen.SetActive(!_helpScreen.activeSelf);

            Time.timeScale = _helpScreen.activeSelf ? 0.0f : 1.0f;
        });

        _restartButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("Main", LoadSceneMode.Single);
            SceneManager.LoadScene("Level", LoadSceneMode.Additive);
        });

        _exitButton.onClick.AddListener(() =>
        {
            Application.Quit();

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        });
    }

    public void Init(Transform point, Transform target)
    {
        _point = point;
        _target = target;

        _startDistance = Vector3.Distance(_point.position, _target.position);

        _slider.value = 0;
        _slider.maxValue = _startDistance;

        SetPauseStatus(false);

        _won.SetActive(false);
        _lost.SetActive(false);

        _interactionButton.Init(point);
        _end = false;
    }

    public void Won()
    {
        if (!_end)
        {
            _end = true;

            Time.timeScale = 0.6f;

            StartCoroutine(Co_Wait(() =>
            {
                _won.SetActive(true);
                _lost.SetActive(false);

                SetPauseStatus(true);
            }));
        }
    }

    public void Lost()
    {
        if (!_end)
        {
            _end = true;

            Time.timeScale = 0.6f;

            StartCoroutine(Co_Wait(() =>
            {
                _won.SetActive(false);
                _lost.SetActive(true);

                SetPauseStatus(true);
            }));
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (_helpScreen.activeSelf) {
                _helpScreen.SetActive(false);
            }

            SetPauseStatus(!_pauseScreen.activeSelf);
        }

        if (Input.GetKeyDown(KeyCode.F1))
        {
            if (_pauseScreen.activeSelf) {
                _pauseScreen.SetActive(false);
            }

            _helpScreen.SetActive(!_helpScreen.activeSelf);

            Time.timeScale = _helpScreen.activeSelf ? 0.3f : 1.0f;
        }

        var distance = Vector3.Distance(_point.position, _target.position);

        _slider.value = _startDistance - distance;
    }

    private void SetPauseStatus(bool status)
    {
        if (status)
        {
            _pauseScreen.SetActive(true);
            Time.timeScale = 0.3f;
        }
        else
        {
            _pauseScreen.SetActive(false);
            Time.timeScale = 1.0f;
        }
    }

    public void SetCatsCount(int catCount)
    {
        _catCounter.text = catCount.ToString();
    }

    public void StartInteraction(float time, Action callback)
    {
        _interactionButton.StartInteraction(time, callback);
    }

    public void ShowInteractionButton(string text)
    {
        _interactionButton.Show(text);
    }

    public void HideInteractionButton()
    {
        _interactionButton.Hide();
    }

    public void CreateCatUi(Cat cat)
    {
        var catUi = Instantiate(_catUiTemplate, transform);
        catUi.Cat = cat;
        catUi.gameObject.SetActive(true);
    }

    private IEnumerator Co_Wait(Action callback)
    {
        yield return new WaitForSeconds(2);

        callback();
    }
}