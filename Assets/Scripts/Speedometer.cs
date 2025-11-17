using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Speedometer : MonoBehaviour
{
    public CarController carController;
    public TextMeshProUGUI speedNum;
    public Image needle;
    public float minNeedleAngle = 0f;
    public float maxNeedleAngle = -270f;
    public float maxSpeed = 200f;

    void Update()
    {
        if (carController != null)
        {
            float speed = carController.CurrentSpeed; // km/h
            speed = Mathf.Clamp(speed, 0, maxSpeed);

            if (speedNum != null)
            {
                speedNum.text = Mathf.RoundToInt(speed).ToString();
            }
            if (needle != null)
            {
                float speedNormalized = speed / maxSpeed;
                float targetAngle = Mathf.Lerp(minNeedleAngle, maxNeedleAngle, speedNormalized);
                needle.rectTransform.rotation = Quaternion.Euler(0, 0, targetAngle);
            }
        }
    }
}
