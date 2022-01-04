//-----------------------------------------------------------------------
// <copyright file="CameraPointer.cs" company="Google LLC">
// Copyright 2020 Google LLC
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Sends messages to gazed GameObject.
/// </summary>
public class CameraPointer : MonoBehaviour
{
    public Material inactiveMaterial;
    public Material activeMaterial;
    public List<GameObject> items;
    public List<GameObject> ignore;
    public List<GameObject> volumeBars;
    public List<GameObject> sfxBars;
    public GameObject level1;
    public GameObject level2;
    public GameObject level3;
    public GameObject level4;
    public GameObject level5;
    public GameObject level6;
    // public GameObject loadingScreen;
    
    private const float _maxDistance = 50;
    private const float _capSizeGrowing = 1;
    private const float _growRate = .5f;
    private float _grownBy;
    private GameObject _gazedAtObject = null;
    private Renderer _myRenderer;
    private bool _options = false;
    private bool _archiv = false;
    private bool _level1 = false;
    private bool _level2 = false;
    private bool _level3 = false;
    private bool _level4;
    private bool _level5;
    private bool _level6;
    private int _volume = -1;
    private int _sfx = -1;
    private LoadLevel _levelLoader;
    
    private AudioManager _audioManager;
    
    private static int _sceneIndex;
    
    public void Start()
    {
        _myRenderer = GetComponent<Renderer>();
        _audioManager = GetComponent<AudioManager>();
        _levelLoader = GetComponent<LoadLevel>();
        SetMaterial(false);
    }

    /// <summary>
    /// Update is called once per frame.
    /// </summary>
    public void Update()
    {
        // Casts ray towards camera's forward direction, to detect if a GameObject is being gazed
        // at.
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, _maxDistance))
        {
            // GameObject detected in front of the camera.
            if (_gazedAtObject != hit.transform.gameObject)
            {
                // New GameObject.
                _gazedAtObject?.SendMessage("OnPointerExit");
                _gazedAtObject = hit.transform.gameObject;
                _gazedAtObject.SendMessage("OnPointerEnter");
            }

            foreach (GameObject g in ignore)
            {
                if (g == _gazedAtObject)
                {
                    _gazedAtObject.SendMessage("OnPointerExit");
                    Reset();
                    SetMaterial(false);
                    return;
                }
            }
            SetMaterial(true);
            ChangeSize(_growRate * Time.deltaTime, _capSizeGrowing);
            _grownBy += _growRate * Time.deltaTime;
            if (_grownBy >= _capSizeGrowing)
            {
                MenuItemPressed(_gazedAtObject);
            }
        }
        else
        {
            // No GameObject detected in front of the camera.
            SetMaterial(false);
            _gazedAtObject?.SendMessage("OnPointerExit");
        }


        // Checks for screen touches.
        if (Google.XR.Cardboard.Api.IsTriggerPressed)
        {
            _gazedAtObject?.SendMessage("OnPointerClick");
        }
    }

    private void MenuItemPressed(GameObject gazedAtObject)
    {
        switch (gazedAtObject.tag)
        {
            case "play":
                /* open level scene 1 */
                _levelLoader.LevelLoad(3);
                break;
            case "archiv":
                /* load "archiv" scene */
                _archiv = true;
                SceneManager.LoadScene(1);
                break;
            case "pillar": /*zoom to item on pillar*/ break;
            case "exit":
                /* quit game */
                Application.Quit();
                break;
            case "options":
                /* load "options" scene */
                _options = true;
                SceneManager.LoadScene(2);
                break;
            case "volume+":
                /* make next bar visible */
                if (_volume < 5)
                {
                    _volume++;
                    volumeBars[_volume].SetActive(true);
                }
                // _gazedAtObject = null;
                Reset();
                _audioManager.VolumeUp("soundtrack");
                break;
            case "volume-":
                /* make next bar invisible */
                if (_volume >= 0)
                {
                    volumeBars[_volume].SetActive(false);
                    _volume--;
                }
                Reset();
                _audioManager.VolumeUp("soundtrack");
                break;
            case "sfx+": 
                if (_sfx < 5)
                {
                    _sfx++;
                    sfxBars[_sfx].SetActive(true);
                }
                Reset();
                _audioManager.VolumeUp("soundtrack");
                break;
            case "sfx-": 
                if (_sfx >= 0)
                {
                    sfxBars[_sfx].SetActive(false);
                    _sfx--;
                }
                Reset();
                _audioManager.VolumeUp("soundtrack");
                break;
            case "back":
                SceneManager.LoadScene(0);
                if (_options)
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 2);
                if (_archiv)
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
                _options = false;
                _archiv = false;
                break;
            case "level1":
                if (level2.active) level2.SetActive(false);
                if (level3.active) level3.SetActive(false);
                if (level4.active) level4.SetActive(false);
                if (level5.active) level5.SetActive(false);
                if (level6.active) level6.SetActive(false);
                level1.SetActive(true);
                _level1 = true;
                _level2 = false;
                _level3 = false;
                _level4 = false;
                _level5 = false;
                _level6 = false;
                ignore.Remove(GameObject.Find("Start"));
                // Reset();
                break;
            case "level2":
                if (level1.active) level1.SetActive(false);
                if (level3.active) level3.SetActive(false);
                if (level4.active) level4.SetActive(false);
                if (level5.active) level5.SetActive(false);
                if (level6.active) level6.SetActive(false);
                level2.SetActive(true);
                _level1 = false;
                _level2 = true;
                _level3 = false;
                _level4 = false;
                _level5 = false;
                _level6 = false;
                ignore.Remove(GameObject.Find("Start"));
                // Reset();
                break;
            case "level3":
                if (level1.active) level1.SetActive(false);
                if (level2.active) level2.SetActive(false);
                if (level4.active) level4.SetActive(false);
                if (level5.active) level5.SetActive(false);
                if (level6.active) level6.SetActive(false);
                level3.SetActive(true);
                _level1 = false;
                _level2 = false;
                _level3 = true;
                _level4 = false;
                _level5 = false;
                _level6 = false;
                ignore.Remove(GameObject.Find("Start"));
                // Reset();
                break;
            case "level4":
                if (level1.active) level1.SetActive(false);
                if (level2.active) level2.SetActive(false);
                if (level3.active) level3.SetActive(false);
                if (level5.active) level5.SetActive(false);
                if (level6.active) level6.SetActive(false);
                level4.SetActive(true);
                _level1 = false;
                _level2 = false;
                _level3 = false;
                _level4 = true;
                _level5 = false;
                _level6 = false;
                ignore.Remove(GameObject.Find("Start"));
                // Reset();
                break;
            case "level5":
                if (level1.active) level1.SetActive(false);
                if (level2.active) level2.SetActive(false);
                if (level3.active) level3.SetActive(false);
                if (level4.active) level4.SetActive(false);
                if (level6.active) level6.SetActive(false);
                level5.SetActive(true);
                _level1 = false;
                _level2 = false;
                _level3 = false;
                _level4 = false;
                _level5 = true;
                _level6 = false;
                ignore.Remove(GameObject.Find("Start"));
                // Reset();
                break;
            case "level6":
                if (level1.active) level1.SetActive(false);
                if (level2.active) level2.SetActive(false);
                if (level3.active) level3.SetActive(false);
                if (level4.active) level4.SetActive(false);
                if (level5.active) level5.SetActive(false);
                level6.SetActive(true);
                _level1 = false;
                _level2 = false;
                _level3 = false;
                _level4 = false;
                _level5 = false;
                _level6 = true;
                ignore.Remove(GameObject.Find("Start"));
                // Reset();
                break;
            case "startLevel":
                if (_level1)
                {
                    _sceneIndex = 3;
                    SceneManager.LoadScene(9);
                }
                if (_level2)
                {
                    _sceneIndex = 4;
                    SceneManager.LoadScene(9);
                }
                if (_level3)
                {
                    _sceneIndex = 5;
                    SceneManager.LoadScene(9);
                }
                if (_level4)
                {
                    _sceneIndex = 6;
                    SceneManager.LoadScene(9);
                }
                if (_level5)
                {
                    _sceneIndex = 7;
                    SceneManager.LoadScene(9);
                }
                if (_level6)
                {
                    _sceneIndex = 8;
                    SceneManager.LoadScene(9);
                }
                break;
        }
    }

    private void Reset()
    {
        _gazedAtObject = null;
        _grownBy = 0;
        ResetSize(.5f);
    }

    private void ResetSize(float resizeBy)
    {
        transform.localScale = new Vector3(resizeBy, resizeBy, resizeBy);
    }

    private void ChangeSize(float changeBy, float capAt)
    {
        if (transform.localScale.x > capAt) return;
        transform.localScale += new Vector3(changeBy, changeBy, changeBy);
    }

    private void SetMaterial(bool isEntered)
    {
        if (inactiveMaterial != null && activeMaterial != null)
        {
            _myRenderer.material = isEntered ? activeMaterial : inactiveMaterial;

        } 
    }

    public static int GetSceneIndex()
    {
        return _sceneIndex;
    }
}
