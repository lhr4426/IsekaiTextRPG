namespace IsekaiTextRPG
{
    public class DungeonEnterScene : GameScene
    {
        private enum Mode  // 현재 씬이 어떤 모드인지 상태 관리 (던전 입구 / 보스 던전)
        {
            Entrance,
            Boss
        }

        private Mode _currentMode = Mode.Entrance;
        public override string SceneName => _currentMode == Mode.Entrance ? "던전 입구" : "보스 던전";
        public override GameScene? StartScene()
        {
            Console.Clear();
            switch (_currentMode)
            {
                case Mode.Entrance:
                    return HandleEntranceMenu();
                case Mode.Boss:
                    return HandleBossMenu();
                default:
                    return this;
            }
        }
        private GameScene? HandleEntranceMenu() // 던전 입구 메뉴 처리
        {   
            List<string> contents = new()
            {
                "던전을 통해 강화하거나 보스를 토벌할 수 있습니다.",
                "",
                "1. 레벨업 던전 선택",
                "2. 보스 던전 선택",
                "0. 마을로 돌아가기"
            };

            UI.DrawTitledBox(SceneName, contents); // TODO: 이거 잘됐는지 확인 필요
            Console.Write(">> ");
            int? input = InputHelper.InputNumber(0, 2); // 사용자 입력을 받아 숫자로 변환 (0 ~ 2 범위)

            switch (input)
            {
                case 1:
                    Console.WriteLine("레벨업 던전으로 이동합니다."); // TODO: 레벨업 던전 만들어지면 연결
                    Console.ReadKey();
                    return this;
                case 2:
                    _currentMode = Mode.Boss;
                    return this;
                case 0:
                    return GameManager.sceneManager.scenes[SceneManager.SceneType.TownScene];// 마을로 이동
                default:
                    Console.WriteLine("잘못된 입력입니다.");
                    return this;
            }
        }
        private GameScene? HandleBossMenu() // 보스 던전 메뉴 처리
        {
            List<string> contents = new()
            {
                "보스 던전에서는 강력한 적이 등장합니다.",
                "",
                "1. 핑크빈 (난이도: 하)",
                "2. 쿠크세이튼 (난이도: 중)",
                "3. 안톤 (난이도: 상)",
                "0. 던전 입구로 돌아가기"
            };

            UI.DrawTitledBox(SceneName, contents); // TODO: 이거 잘됐는지 확인 필요
            Console.Write(">> ");
            int? input = InputHelper.InputNumber(0, 3);// 사용자 입력을 받아 숫자로 변환 (0 ~ 3 범위)

            switch (input)
            {
                case 1:
                    Console.WriteLine("핑크빈 던전으로 이동합니다."); // TODO: 핑크빈 만들어지면 연결
                    Console.ReadKey();
                    return this;
                case 2:
                    Console.WriteLine("쿠크세이튼 던전으로 이동합니다."); // TODO: 쿠크세이튼 만들어지면 연결
                    Console.ReadKey();
                    return this;
                case 3:
                    Console.WriteLine("안톤 던전으로 이동합니다."); // TODO: 안톤 만들어지면 연결
                    Console.ReadKey();
                    return this;
                case 0:
                    _currentMode = Mode.Entrance; // 던전 입구로 돌아감
                    return this;
                default:
                    Console.WriteLine("잘못된 입력입니다.");
                    return this;
            }
        }
    }
}
