using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class test_to_save : MonoBehaviour
{
    TextMeshProUGUI txt_box;

    PlayerData loler_test;

    public void Start()
    {
        txt_box = this.transform.GetComponent<TextMeshProUGUI>();
    }

    public void randomize_data()
    {
        float robot_x = Random.Range(0, 1000);
        float robot_y = Random.Range(0, 1000);

        float amelia_x = Random.Range(0, 1000);
        float amelia_y = Random.Range(0, 1000);

        int level_state = Random.Range(0, 1000);
        int level_stage = Random.Range(0, 1000);

        string to_display = $"Robot X: {robot_x}\nRobot Y: {robot_y}\nAmelia X: {amelia_x}\nAmelia Y: {amelia_y}\nLevel State: {level_state}\nLevel Stage: {level_stage}";

        txt_box.text = to_display;

        loler_test = new PlayerData(robot_x, robot_y, amelia_x, amelia_y, level_state, level_stage);
    }

    public void save_data()
    {
        //LoadSaveSystem.SaveData();
    }

    public void load_data()
    {
        LoadSaveSystem.LoadData();

        if (LoadSaveSystem.player_data == null)
            return;
        PlayerData data = LoadSaveSystem.player_data;

        float robot_x = data.robot_position_x;
        float robot_y = data.robot_position_y;

        float amelia_x = data.amelia_position_x;
        float amelia_y = data.amelia_position_y;

        int level_state = data.level_state;
        int level_stage = data.level_stage;

        string to_display = $"Robot X: {robot_x}\nRobot Y: {robot_y}\nAmelia X: {amelia_x}\nAmelia Y: {amelia_y}\nLevel State: {level_state}\nLevel Stage: {level_stage}";

        txt_box.text = to_display;
    }
}
