using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace IsekaiTextRPG
{
    public class DungeonEnterScene : GameScene
    {
        private enum Mode
        {
            Entrance,
            Boss
        }

        private Mode currentMode = Mode.Entrance;
        public override string SceneName => currentMode == Mode.Entrance ? "던전 입구" : "보스 던전";
        public override GameScene? StartScene()
        {
            Console.Clear();
            switch (currentMode)
            {
                case Mode.Entrance:
                    return HandleEntranceMenu();
                case Mode.Boss:
                    return HandleBossMenu();
                default:
                    return this;
            }
        }
        private GameScene? HandleEntranceMenu()
        {
            List<string> contents = new()
            {
                "던전을 통해 강화하거나 보스를 토벌할 수 있습니다.\n",
                "1. 레벨업 던전 선택",
                "2. 보스 던전 선택",
                "0. 마을로 돌아가기"
            };

            UI.DrawTitledBox(SceneName, contents);
            Console.Write(">> ");
            int? input = InputHelper.InputNumber(0, 2);

            switch (input)
            {
                case 1:
                    Console.WriteLine("레벨업 던전으로 이동합니다."); // TODO: 레벨업 던전 연결
                    Console.ReadKey();
                    return this;
                case 2:
                    currentMode = Mode.Boss;
                    return this;
                case 0:
                    return GameManager.sceneManager.scenes[SceneManager.SceneType.TownScene];
                default:
                    Console.WriteLine("잘못된 입력입니다.");
                    return this;
            }
        }
        private GameScene? HandleBossMenu()
        {
            List<string> contents = new()
            {
                "보스 던전에서는 강력한 적이 등장합니다.\n",
                "1. 핑크빈 (난이도: 하)",
                "2. 쿠크세이튼 (난이도: 중)",
                "3. 안톤 (난이도: 상)",
                "0. 던전 입구로 돌아가기"
            };

            UI.DrawTitledBox(SceneName, contents);
            Console.Write(">> ");
            int? input = InputHelper.InputNumber(0, 4);

            switch (input)
            {
                case 1:
                    Console.WriteLine("핑크빈 던전으로 이동합니다."); // TODO: 핑크빈 연결
                    Console.ReadKey();
                    return this;
                case 2:
                    Console.WriteLine("쿠크세이튼 던전으로 이동합니다."); // TODO: 쿠크세이튼 연결
                    Console.ReadKey();
                    return this;
                case 3:
                    Console.WriteLine("안톤 던전으로 이동합니다."); // TODO: 안톤 연결
                    Console.ReadKey();
                    return this;
                case 0:
                    currentMode = Mode.Entrance;
                    return this;
                default:
                    Console.WriteLine("잘못된 입력입니다.");
                    return this;
            }
        }
    }
}
