using System;
using System.Collections;
using UnityEngine;

namespace _MainApp.Scripts.Tools
{
    public class CheckAccelerationAccess : MonoBehaviour
    {
        public static CheckAccelerationAccess instance;

        [SerializeField] private bool hasAcceleration;
        private WaitForSeconds waitaSec = new WaitForSeconds(1);

        [field: SerializeField] public bool HasAccessToAcceleration { get; set; }

        private void Start()
        {
            instance = this;
            CheckForAccess();
        }

        private void CheckForAccess()
        {
            // Check if the device supports accelerometer
            if (SystemInfo.supportsAccelerometer && Application.platform == RuntimePlatform.Android
                || SystemInfo.supportsAccelerometer && Application.platform == RuntimePlatform.IPhonePlayer)
            {
                hasAcceleration = true;
                Debug.Log("Accelerometer is supported on this device.");
            }
            else
            {
                hasAcceleration = false;
                Debug.Log("Accelerometer is not supported on this device.");
            }


            // If accelerometer is supported but not enabled, request access
            if (hasAcceleration && !Input.gyro.enabled)
            {
                Debug.Log("Requesting access");
                StartCoroutine(EnableAccelerometer());
            }

            Debug.Log($"Gyroscope enabled? {Input.gyro.enabled}");
        }

        public void AskForGyroAccess()
        {
            // Request access to use the accelerometer
            StartCoroutine(EnableAccelerometer());
        }

        public void ForceGyroAccess()
        {
            // Request access to use the accelerometer
            Input.gyro.enabled = true;
            Debug.Log($"Gyro enabled: {Input.gyro.enabled}.");
        }

        private IEnumerator EnableAccelerometer()
        {
            yield return waitaSec; // Wait for a brief moment before requesting access

            // Request access to use the accelerometer
            Input.gyro.enabled = true;

            // Check if the access was granted
            if (Input.gyro.enabled)
            {
                Debug.Log("Accelerometer access granted.");
            }
            else
            {
                Debug.Log("Failed to grant access to the accelerometer.");
            }
        }
    }
}