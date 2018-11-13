using UnityEditor;
using UnityEngine;

using SmileLab;
public class UserDeleate {

    [MenuItem("Tools/Game/UserDelete")]
    static void DeleteAll(){
        Debug.Log (Application.persistentDataPath);
        System.IO.File.Delete(System.IO.Path.Combine (Application.persistentDataPath, "aws_cognito_sync.db"));
        PlayerPrefs.DeleteAll();
        var userData = System.IO.Path.Combine (SmileLab.GameSystem.LocalSaveDirectoryPath, "authusernameandpass".ToHashMD5 ());
        if (FileUtility.Exists (userData)) {
            FileUtility.Delete (userData);
        }
        Debug.Log("Delete User Data!!");
    }
}