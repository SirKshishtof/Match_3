using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Match_3
{
    
    static class GameSettings
    {
        private static int timeCount = 240;
        private static int framesPerPeriod = 60;
        private static int drawInterval = 500 / framesPerPeriod;
        private static int gameInterval = 1000;
        private static int matrixSizeX = 8;
        private static int matrixSizeY = 8;
       
        public static int TimeCount => timeCount;
        public static int FramesPerSecond => framesPerPeriod;
        public static int DrawInterval => drawInterval;
        public static int GameInterval => gameInterval;
        public static int MatrixSizeX => matrixSizeX;
        public static int MatrixSizeY => matrixSizeY;
        
    }
}


