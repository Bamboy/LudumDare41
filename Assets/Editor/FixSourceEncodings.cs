/*
Automatically fixes the character encoding of checked-in source files.

Author: fluffy@beesbuzz.biz

Permission is hereby granted, free of charge, to any person obtaining a copy of
this software and associated documentation files (the "Software"), to deal in
the Software without restriction, including without limitation the rights to
use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies
of the Software, and to permit persons to whom the Software is furnished to do
so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

*/

using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;

namespace WS3D.Unity {
class FixSourceEncodings : AssetPostprocessor {
    static void OnPostprocessAllAssets(string[] importedAssets,
                                       string[] deletedAssets,
                                       string[] movedAssets,
                                       string[] movedFromAssetPaths)
    {
        foreach (string asset in importedAssets) {
            if (asset.ToLower().EndsWith(".cs")
                && File.Exists(asset)
                && (File.GetAttributes(asset) & FileAttributes.Directory) == 0) {
                Debug.Log("Found source file: " + asset);

                // File at least appears to be a C# source file; see if it has a UTF-8 BOM
                var bom = new byte[3];
                using (var file = new FileStream(asset, FileMode.Open, FileAccess.Read)) {
                    file.Read(bom, 0, 3);
                }

                if (!(bom[0] == 0xef && bom[1] == 0xbb && bom[2] == 0xbf)) {
                    Debug.Log("Asset " + asset + " BOM doesn't match UTF-8; converting");

                    string fileText = File.ReadAllText(asset);
                    File.WriteAllText(asset, fileText, Encoding.UTF8);
                }
            }
        }
    }

}
}