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
                    
            }
        }
    }

    
}
