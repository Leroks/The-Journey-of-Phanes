using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using Firebase.Database;

public class SaveData : MonoBehaviour
{
    // DatabaseReference reference;

    void Start()
    {
        // reference = FirebaseDatabase.DefaultInstance.RottReference;
    }

    void saveUserData(){
        User user = new User();
        string json = JsonUtility.ToJson(user);
        // Save username mail vb here TODO

        // reference.Child("User").Child(user.username).SetRawJsonValueAsync(json).ContinueWith(task => {
        //     if(task.IsCompleted){

        //     }
        //     else{

        //     }
        // }
    }

    void Update()
    {
        
    }

}
