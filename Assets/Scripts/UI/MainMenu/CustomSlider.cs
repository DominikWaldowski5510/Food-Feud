﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomSlider : MonoBehaviour
{
    [SerializeField]
    private Transform startPoint, endPoint, knob;
    [SerializeField]
    private float value;
   // private float totalLength;
    private float rateOfDrag = 5f;
    private bool isAxis = true;
    private bool isHeld;
    [SerializeField]
    private float defaultDrag = 1.0f;
    [SerializeField]
    private float drag;
    [SerializeField]
    private float dragSpeed = 2f;
    [SerializeField]
    private float adjustedValue;
    [SerializeField]
    private Transform coveredAreaSlider;
    private SaveData save;
    private float previousValue;
    public enum typeOfSlider
    {
        music,
        sound,
        master
    }
    public typeOfSlider sliderType;
    private void OnEnable()
    {
        save = GameObject.Find("SaveSystem").GetComponent<SaveData>();
        drag = defaultDrag;
        isHeld = false;
        // totalLength = Vector3.Distance(startPoint.position, endPoint.position);
        //  Debug.Log(totalLength);
        DetectMusicSetting();
        adjustedValue = value / 100;
        knob.transform.position = Vector3.Lerp(startPoint.position, endPoint.position, adjustedValue);
        coveredAreaSlider.transform.localScale = new Vector3(adjustedValue, 1, 1);
    }

    private void DetectMusicSetting()
    {
        if (sliderType == typeOfSlider.master)
        {
            value = SoundManager.Instance.MasterVol * 100;
        }
        else if (sliderType == typeOfSlider.music)
        {
            value = SoundManager.Instance.MusicVol * 100; 
        }
        else if(sliderType == typeOfSlider.sound)
        {
            value = SoundManager.Instance.SoundVol * 100; 
        }
        previousValue = value;
    }

    public void SaveSettings()
    {
        if (sliderType == typeOfSlider.master)
        {
            SoundManager.Instance.SetMasterVol(value / 100);
        }
        else if (sliderType == typeOfSlider.music)
        {
            SoundManager.Instance.SetBGMVol(value/100);
        }
        else if (sliderType == typeOfSlider.sound)
        {
            SoundManager.Instance.SetSFXVol(value / 100);
        }
        save.Save();
    }

    public void CancelChanges()
    {
        value = previousValue;
        adjustedValue = value / 100;
        knob.transform.position = Vector3.Lerp(startPoint.position, endPoint.position, adjustedValue);
        coveredAreaSlider.transform.localScale = new Vector3(adjustedValue, 1, 1);
        SaveSettings();
    }

    private void Update()
    {
        if (Input.GetAxis("Horizontal") > 0.3f)
        {
            value +=  Time.deltaTime * rateOfDrag * drag;
            drag += Time.deltaTime * dragSpeed;
            if(value >= 100)
            {
                value = 100;
            }
            adjustedValue = value / 100;
        }
        else if (Input.GetAxis("Horizontal") < -0.3f)
        {
            value -= Time.deltaTime * rateOfDrag * drag;
            drag += Time.deltaTime * dragSpeed;
            if (value <=0)
            {
                value = 0;
            }
            adjustedValue = value / 100;
        }
        else
        {
            drag = defaultDrag;
        }
        knob.transform.position = Vector3.Lerp(startPoint.position, endPoint.position, adjustedValue);
        coveredAreaSlider.transform.localScale = new Vector3(adjustedValue, 1, 1);
    }

    #region Depricated
    private void PercentageFormula()
    {
       // float percentage = (value / totalLength) * 100;
       // value = percentage;
    }

    private void ReverseCalculation(float percent)
    {
      //  float percentageToValue = (percent / 100) * totalLength;
    }
    #endregion
}
