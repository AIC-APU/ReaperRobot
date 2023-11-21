using SFB;

/*
 Unity Standalone File Browser(SFB) というオープンソースを利用して実装しています
 https://github.com/gkngkc/UnityStandaloneFileBrowser


 SFB はMITライセンスです。以下必要表記。
 
 Copyright (c) 2017 Gökhan Gökçe
 Released under the MIT license
 https://opensource.org/licenses/mit-license.php
*/


namespace Plusplus.ReaperRobot.Scripts.View.Replay
{
    static class OpenDialog
    {
         public static string OpenCSVFile(string title, string defalutDirectory)
        {
            var extensions = new[]
            {
                new ExtensionFilter("csvファイル", "csv"),
            };

            string[] paths = StandaloneFileBrowser.OpenFilePanel(title, defalutDirectory, extensions, false);

            if(paths.Length != 0)
            {
                return paths[0];
            }
            else
            {
                return ""; 
            }
        }
    }
}
