using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BGK.SaveSystem;

namespace BGK.Demo
{
    public class DemoSaveScript : MonoBehaviour
    {
        public DemoSaveClass SaveClass;
        [Range(0, 3)] public int Methode;
        public bool Save;
        public bool Load;

        void Update()
        {
            if (Save)
            {
                SaveData();
                Save = false;
            }

            if (Load)
            {
                LoadData();
                Load = false;
            }
        }

        public void SaveData()
        {
            if (Methode == 0)
            {
                //Save SaveClass to the default path as SaveSystemDemo.dat
                SaveSys.Save(SaveClass, "SaveSystemDemo"); //(Data to save, Save file name)
            }
            else
            {
                //Save SaveClass to a directory named Demo in the default path as SaveSystemDemo.dat
                SaveSys.Save(SaveClass, Application.persistentDataPath + "/Demo", "SaveSystemDemo"); //(Data to save, Save file location, Save file name)
            }
        }

        public void LoadData()
        {
            if (Methode == 0)
            {
                //Load SaveSystemDemo.dat from default path
                SaveClass = SaveSys.Load<DemoSaveClass>("SaveSystemDemo"); //(Save file name)
            }
            else if (Methode == 1)
            {
                //Load SaveSystemDemo.dat from default path
                SaveSys.Load(out SaveClass, "SaveSystemDemo"); //(Output variable, Save file name)
            }
            else if (Methode == 2)
            {
                //Load SaveSystemDemo.dat from the directory named Demo in the default path
                SaveClass = SaveSys.Load<DemoSaveClass>(Application.persistentDataPath + "/Demo", "SaveSystemDemo"); //(Save file location,Save file name)
            }
            else
            {
                //Load SaveSystemDemo.dat from the directory named Demo in the default path
                SaveSys.Load(out SaveClass, Application.persistentDataPath + "/Demo", "SaveSystemDemo"); //(Output variable, Save file location, Save file name)
            }
        }
    }
}
