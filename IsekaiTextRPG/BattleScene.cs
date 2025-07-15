public class BattleScene : GameScene
{
    public override string SceneName => "전투";

    public override GameScene? StartScene()
    {
        Console.Clear();
        //TODO : 임시로 만든 씬 나중에 내용 추가
        return prevScene;
    }
}
