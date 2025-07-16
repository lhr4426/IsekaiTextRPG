using System;
using System.Collections.Generic;
using System.Linq;

public class SkillShopScene : GameScene
{
    // 스킬 상태 변경 시 호출할 이벤트
    private event Action? OnSkilltreeUpdated;

    // 스킬 딕셔너리 (ID -> Skill)
    SortedDictionary<int, Skill> skills = SkillManager.Skills;

    // 씬 이름 정의
    public override string SceneName => "스킬 상점";

    // 씬 시작 시 실행되는 메서드
    public override GameScene? StartScene()
    {
        // 스킬 상태 업데이트용 이벤트 설정 및 실행
        SetUpdate();

        // 씬 이름 박스 출력
        UI.DrawTitledBox(SceneName, null);

        // 스킬 목록 출력
        PrintSkills();

        // 사용자 입력 요청
        Console.Write("배울 스킬 입력 (0 : 취소) >> ");

        // 입력 값 받기 (0 ~ 스킬 개수)
        int? input = InputHelper.InputNumber(0, skills.Count);

        // 0 입력 시 이전 씬으로 복귀
        if (input == 0) return prevScene;

        // 올바른 범위 내 입력 시 스킬 습득 시도
        else if (input > 0 && input <= skills.Count)
        {
            LearnSkill((int)input - 1);
        }

        return this;
    }

    // 스킬 상태 갱신 이벤트 구독 및 실행
    private void SetUpdate()
    {
        OnSkilltreeUpdated = null; // 기존 구독 초기화
        if (skills != null)
        {
            foreach (var skill in skills)
            {
                OnSkilltreeUpdated += skill.Value.UpdateLearnState;
            }
        }
        OnSkilltreeUpdated?.Invoke();
    }

    // 스킬 목록을 화면에 출력
    private void PrintSkills()
    {
        List<string> strings = new List<string>();

        if (skills != null)
        {
            foreach (var skill in skills)
            {
                List<string> itemStrings = skill.Value.ToShopString();

                foreach (var str in itemStrings)
                {
                    strings.Add(str);
                }

                strings.Add(""); // 스킬 간 구분용 빈 줄 추가
            }
        }

        UI.DrawLeftAlignedBox(strings);
    }

    // 스킬 습득 처리 (인덱스 기반)
    private void LearnSkill(int idx)
    {
        // 인덱스 범위 검증 (안전하게 키 기반 접근을 위해)
        if (idx < 0 || idx >= skills.Count)
        {
            UI.DrawBox(new List<string> { "잘못된 스킬 번호입니다." });
            return;
        }

        // 인덱스에 해당하는 스킬 키 얻기
        int skillKey = skills.Keys.ElementAt(idx);
        Skill skill = skills[skillKey];

        List<string> strings = new();

        if (skill.learnState == LearnState.Learnable)
        {
            // 스킬 습득 처리
            skill.learnState = LearnState.Learned;
            GameManager.player.Skills.Add(skill);
            strings.Add($"{skill.Name} 스킬을 습득하셨습니다!");

            // 변경된 스킬 상태 저장
            SkillManager.SaveSkills();
            GameManager.player.SaveSkillsToJson();
        }
        else if (skill.learnState == LearnState.NotLearnable)
        {
            strings.Add("스킬을 습득하기 위한 조건을 만족하지 않습니다.");
        }
        else
        {
            strings.Add("이미 해당 스킬을 습득하였습니다.");
        }

        UI.DrawBox(strings);
    }
}