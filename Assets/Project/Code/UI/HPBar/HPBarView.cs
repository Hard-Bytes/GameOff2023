using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Code.UI
{
    public class HPBarView : ViewBase
    {
        [SerializeField] private GameObject healthBar;
        [SerializeField] private GameObject divideBar;
        [SerializeField] private float scaleSpeed=0.05f;
        private float _percentage;
        private float _percentageDivide;
        // Start is called before the first frame update

        // Update is called once per frame
        void Update()
        {
            float barSize = healthBar.transform.localScale.x;
            if(Mathf.Abs(_percentage - (1-barSize)) > 0.01f)
            {
                float changebar = (_percentage > 1 - barSize ?-scaleSpeed : scaleSpeed) *Time.fixedDeltaTime;
                healthBar.transform.localScale = healthBar.transform.localScale + new Vector3(changebar,0,0);

            }
            float barDivideSize = divideBar.transform.localScale.x;
            if(Mathf.Abs(_percentage - _percentageDivide - (1- barDivideSize)) > 0.01f)
            {
                float changebar = (_percentage > 1 - barSize ?-scaleSpeed : scaleSpeed) *Time.fixedDeltaTime;
                divideBar.transform.localScale = divideBar.transform.localScale + new Vector3(changebar,0,0);

            }
        }

        public void RecalculatePercentage(int newHP, int newDivide, int maxHealth)
        {
            _percentage = (float)newHP / (float)maxHealth;
            _percentageDivide = (float)newDivide / (float)maxHealth;

        }
    }
}
