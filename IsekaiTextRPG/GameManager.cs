using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
namespace IsekaiTextRPG
{
    public class GameManager
    {
        public static GameManager instance = new GameManager();

        public static Player player;
        public string path = "PlayerData.json";//임시

        public void NewPlayerData(string playerName)
        {
            player = new Player(playerName);
            SavePlayerData();
        }

        public void LoadPlayerData() //데이터 로드
        {
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                player = JsonSerializer.Deserialize<Player>(json);
            }
            else
            {
                Console.WriteLine("저장된 데이터가 없습니다.");
            }
        }

        public void SavePlayerData() //데이터 저장
        {
            string json = JsonSerializer.Serialize(player, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(path, json);
            Console.WriteLine("플레이어 데이터 저장 완료.");
        }
        public void GameExit()
        {
            Console.WriteLine("게임을 종료합니다.");
            SavePlayerData();
            Environment.Exit(0);
        }
    }

}