    using UnityEngine;
    using System.Collections;
     
    public class DayCycle : MonoBehaviour
    {
        [HideInInspector]
        public GameObject sun;
        [HideInInspector]
        public Light sunLight;
     
        public float timeOfDay = 0;
     
        public float secondsPerMinute = 60.0f;
        [HideInInspector]
        public float secondsPerHour;
        [HideInInspector]
        public float secondsPerDay;
     
        private int currentDay = 1;
        public int currectMonth = 1;
        private int currectYear = 1;
     
        public float timeMultiplier = 1;
        public Gradient sunGradient;
        public Gradient fogGradient;
        public Color currentColor;

        void Start()
        {
            sun = gameObject;
            sunLight = gameObject.GetComponent<Light>();
     
            secondsPerHour = secondsPerMinute * 60.0f;
            secondsPerDay = secondsPerHour * 24.0f;
        }
     
        void Update()
        {
            SunUpdate();
     
            timeOfDay += Time.deltaTime * timeMultiplier;
     
            if (timeOfDay >= 360)
            {
                currentDay += 1;
                if(currentDay > 30)
                {
                    currentDay = 1;
                    currectMonth += 1;
                    if(currectMonth > 12)
                    {
                        currectYear += 1;
                        currectMonth = 1;
                    }
                }
                timeOfDay = 0;
            }
        }
     
        public void SunUpdate()
        {
            // print("Date and Time : " + currectYear + "-" + currectMonth + "-" + currentDay + " : time = " + timeOfDay);
     
            //sun.transform.localRotation = Quaternion.Euler((timeOfDay / 24) * 360 - 0, -30, 0);
            sun.transform.localEulerAngles = SunAngle();
            currentColor = sunGradient.Evaluate((timeOfDay+90)/360f);
            RenderSettings.ambientSkyColor = currentColor;
            RenderSettings.fogColor = fogGradient.Evaluate((timeOfDay + 90) / 360f);

        }
     
        private Vector3 SunAngle()
        {
            //30,0,0 = sunrise
            //90,0,0 = High noon
            //180,0,0 = sunset
            //-90,0,0 = Midnight
            //0,90,0 = summer
            //0,40,0 = winter
            int tempMonth;
         
            switch(currectMonth)
            {
                case 8:
                    tempMonth = 6;
                    break;
                case 9:
                    tempMonth = 5;
                    break;
                case 10:
                    tempMonth = 4;
                    break;
                case 11:
                    tempMonth = 3;
                    break;
                case 12:
                    tempMonth = 2;
                    break;
                default:
                    tempMonth = currectMonth;
                    break;
            }
     
            return new Vector3(timeOfDay, (tempMonth * 40 * 0.18f) + 40, 0);
        }
    }
