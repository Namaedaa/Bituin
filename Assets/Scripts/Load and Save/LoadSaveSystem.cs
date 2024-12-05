using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;
using System.Runtime.Serialization;
using Unity.VisualScripting;

public  class LoadSaveSystem : MonoBehaviour 
{
    static string save_file_name;
    static string file_path;

    public static PlayerData player_data;

    private static Transform robot_transform;
    private static Transform amelia_transform;

    private void Start()
    {
        save_file_name = "save_000000000000001.sfg";
        file_path = $"{Application.persistentDataPath}\\{save_file_name}";

        robot_transform = GameObject.Find("Robot").GetComponent<Transform>();
        amelia_transform = GameObject.Find("New Amelia AI").GetComponent<Transform>();
    }
    public static void SaveData(InteractableObjects level_data)
    {
        if (level_data == null)
            return;

        PlayerData to_save = new PlayerData(
            robot_x: robot_transform.position.x,
            robot_y: robot_transform.position.y,
            amelia_x: amelia_transform.position.x,
            amelia_y: amelia_transform.position.y,
            current_state: level_data.SaveState,
            current_stage: 0
            );

        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(file_path, FileMode.Create);

        formatter.Serialize(stream, to_save);
        stream.Close();
        stream.Dispose();
    }

    public static void LoadData()
    {
        if(SaveExists())
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(file_path, FileMode.Open);

            try
            {
                player_data = formatter.Deserialize(stream) as PlayerData;
                Debug.Log("Save Loaded!");
            }
            catch (SerializationException)
            {
                stream.Close();
                stream.Dispose();
                return;
            }
            stream.Close();
            stream.Dispose();
        }
        else
        {
            Debug.Log("No Save Data.");
            return;
        }
    }

    public static bool SaveExists()
    {
        if (File.Exists(file_path))
            return true;
        return false;
    }


}


[System.Serializable]
public class PlayerData
{
    public float robot_position_x;
    public float robot_position_y;

    public float amelia_position_x;
    public float amelia_position_y;

    public int level_state;
    public int level_stage;

    /*public PlayerData(Transform robot, Transform amelia, int current_state, int current_stage)
    {
        robot_position_x = robot.position.x;
        robot_position_y = robot.position.y;

        amelia_position_x = amelia.position.x;
        amelia_position_y = amelia.position.y;

        level_state = current_state;
        level_stage = current_stage;
    }*/

    public PlayerData(float robot_x, float robot_y, float amelia_x, float amelia_y, int current_state, int current_stage)
    {
        robot_position_x = robot_x;
        robot_position_y = robot_y;

        amelia_position_x = amelia_x;
        amelia_position_y = amelia_y;

        level_state = current_state;
        level_stage = current_stage;
    }
}
