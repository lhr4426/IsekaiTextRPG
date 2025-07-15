
public class DungeonEnterScene : GameScene
{
    public override string SceneName =>"던전 입구";
    public override GameScene? StartScene()
    {
        Console.Clear();
        HandleEntranceMenu();
        return EndScene();
    }
    private void HandleEntranceMenu() // 던전 입구 메뉴 처리
    {
        List<string> contents = new()
            {
                "던전을 통해 강화하거나 보스를 토벌할 수 있습니다.",
            };

        UI.DrawTitledBox(SceneName, contents); 
        
    }
    
}

