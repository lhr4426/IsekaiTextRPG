public class JeonJikScene : GameScene
{
    public override string SceneName => "전직소";

    public override GameScene? StartScene()
    {
        Console.Clear();
        PrintJobs();
        return ChangeClass();
        
    }

    public void PrintJobs()
    {
        List<string> strings = new();

        var JobsArray = Enum.GetValues(typeof(Player.Jobs));


        for (int i = 1; i < JobsArray.Length; i++)
        {
            Player.Jobs job = (Player.Jobs)JobsArray.GetValue(i);
            strings.Add($" {i} : " + Player.JobsKorean(job) + $" | {(CanChangeClass(job) ? "전직 가능" : "전직 불가")}");
        }

        UI.DrawTitledBox(SceneName, null);
        UI.DrawLeftAlignedBox(strings);
        Console.Write("전직할 직업을 선택 (0 : 돌아가기) : ");
    }

    private GameScene? ChangeClass()
    {
        List<string> strings = new();
        var JobsArray = Enum.GetValues(typeof(Player.Jobs));
        int? input = InputHelper.InputNumber(0, JobsArray.Length);

        if (input == 0) return prevScene;
        if (input > 0 && input < JobsArray.Length)
        {
            var job = (Player.Jobs)JobsArray.GetValue((int)input);
            if (CanChangeClass(job))
            {
                strings.Add($"{Player.JobsKorean(job)} 직업으로 전직하였습니다!");
                GameManager.player.Job = job;
                UI.DrawBox(strings);
                Console.ReadKey();
                return prevScene;
            }
            else
            {
                strings.Add($"{Player.JobsKorean(job)} 직업으로 전직하기 위한 아이템이 없습니다.");
            }
            return this;
        }
        return this;
    }

    private bool CanChangeClass(Player.Jobs job)
    {
        string? needItemName = Player.JobItemName(job);
        if (needItemName != null)
        {
            bool hasItem = GameManager.player.Inventory.Any(x => x.Name == needItemName);
            return hasItem;
        }
        else
        {
            return true;
        }
    }
}
