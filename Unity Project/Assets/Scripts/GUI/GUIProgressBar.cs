using System;
using UnityEngine;

public class GUIProgressBar : MonoBehaviour
{
    private int maxv;
    public int maxValue { get { return maxv; } set { maxv = value; } }
    public int curValue { get; set; }

    private float ratio;
    public UILabel numericLabel;
    public UISlider slider;

    void Init()
    {
        GetLabel();
        GetSlider();
    }

    private void GetSlider()
    {
        if (slider == null) slider = this.gameObject.GetComponentInChildren<UISlider>();
    }

    private void GetLabel()
    {
        if (numericLabel == null) numericLabel = this.gameObject.GetComponentInChildren<UILabel>();
    }

    public void SetMaxValue(int maxVal)
    {
        maxValue = maxVal;
        UpdateBar();
    }

    public void UpdateValue(int newVal)
    {
        curValue = newVal;
        UpdateBar();
    }

    internal void UpdateBar()
    {
        if (maxValue != 0) ratio = ((float)curValue) / ((float)maxValue);
        else
        {
            throw new DivideByZeroException("Max value of the progress bar was 0!");
        }

        Init();
        slider.sliderValue = ratio;
        numericLabel.text = curValue + " / " + maxValue;
    }
}