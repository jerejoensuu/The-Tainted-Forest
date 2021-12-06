using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

namespace TaintedForest {
    public class ScreenResolutions {

        public List<string> rList = new List<string>();
        public int[,] resolutions;

        public void Initialize() {
            
            rList.Add("1920x1080");
            rList.Add("1680x1050");
            rList.Add("1600x1024");
            rList.Add("1600x900");
            rList.Add("1440x900");
            rList.Add("1366x768");
            rList.Add("1360x768");
            rList.Add("1280x1024");
            rList.Add("1280x960");
            rList.Add("1280x800");
            rList.Add("1280x768");
            rList.Add("1280x720");
            rList.Add("1176x664");
            rList.Add("1152x864");
            rList.Add("1024x768");
            rList.Add("800x600");
            rList.Add("720x576");
            rList.Add("720x480");
            rList.Add("640x480");

            resolutions = new int[rList.Count, rList.Count];

            for (int r = 0; r < rList.Count; r++) {
                string[] split = rList[r].Split('x');
                resolutions[r, 0] = int.Parse(split[0]);
                resolutions[r, 1] = int.Parse(split[1]);
            }
        }

        public ScreenResolutions() {
            Initialize();
        }

        public int[] GetResolution(int index) {
            int[] r = new int[2];
            r[0] = resolutions[index, 0];
            r[1] = resolutions[index, 1];
            return r;
        }

        public List<string> GetResolutionString() {
            return rList;
        }

        public string GetResolutionString(int index) {
            return rList[index];
        }
    }
}
