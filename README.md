
스파르타 내배캠 유니티 게임개발 11기 3주차
# 7조 : 이세계에 떨어졌조

<p align="center">
<br>
  <img src="./media/team.png">
  <br>
</p>

# Isekai - RPG

## 프로젝트 영상

<p align="center">
<br>
  <img src="./media/game.gif">
  <br>
</p>

## 프로젝트 소개

<p align="center">
<br>
  <img src="./media/s1.JPG">
  <br>
</p>
<p align="center">
<br>
  <img src="./media/s2.JPG">
  <br>
</p>
<p align="center">
<br>
  <img src="./media/s3.JPG">
  <br>
</p>
<p align="center">
<br>
  <img src="./media/s4.JPG">
  <br>
</p>
<p align="center">
<br>
  <img src="./media/s5.JPG">
  <br>
</p>
<p align="center">
<br>
  <img src="./media/s6.JPG">
  <br>
</p>
<p align="center">
<br>
  <img src="./media/s7.JPG">
  <br>
</p>
<p align="center">
<br>
  <img src="./media/s8.JPG">
  <br>
</p>
<p align="center">
<br>
  <img src="./media/s9.JPG">
  <br>
</p>
<p align="center">
<br>
  <img src="./media/s10.JPG">
  <br>
</p>
<p align="center">
<br>
  <img src="./media/s11.JPG">
  <br>
</p>
<p align="center">
<br>
  <img src="./media/s12.JPG">
  <br>
</p>
<p align="center">
<br>
  <img src="./media/s13.JPG">
  <br>
</p>

<br>

## 기술 스택

| C# | .Net |
| :--------: | :--------: |
|   ![csharp]    |   ![dotnet]    |

<br>

## 시스템 구조도

```mermaid
---
config:
  class:
    hideEmptyMembersBox: true
  layout: elk
  look: neo
  theme: redux
---
classDiagram
direction TB
    class JeonJikScene {
    }
    class StatScene {
    }
    class TownScene {
    }
    class FirstScene {
    }
    class GameScene {
    }
    class InvenScene {
    }
    class SkillInvenScene {
    }
    class SkillShopScene {
    }
    class QuestScene {
    }
    class ShopScene {
    }
    class RestScene {
    }
    class DungeonEnterScene {
    }
    class BossDungeonScene {
    }
    class LevelDungeonScene {
    }
    class NormalBattleScene {
    }
    class BossBattleScene {
    }
    class SceneManager {
    }
    class ItemSystem {
    }
    class GameManager {
    }
    class QuestManager {
    }
    class BattleBase {
    }
    class Quest {
    }
    class UI {
    }
    class InputHelper {
    }
    class BattleLoger {
    }
    class Enemy {
    }
    class BossClass {
    }
    class Item {
    }
    class Player {
    }
    class Skill {
    }
    GameScene <|-- FirstScene
    GameScene <|-- TownScene
    GameScene <|-- StatScene
    GameScene <|-- JeonJikScene
    GameScene <|-- InvenScene
    GameScene <|-- SkillInvenScene
    GameScene <|-- SkillShopScene
    GameScene <|-- QuestScene
    GameScene <|-- ShopScene
    GameScene <|-- RestScene
    GameScene <|-- DungeonEnterScene
    GameScene <|-- BossDungeonScene
    GameScene <|-- LevelDungeonScene
    SceneManager -- GameScene
    SceneManager -- ItemSystem
    GameManager -- SceneManager
    GameScene <|-- BattleBase
    BattleBase <|-- NormalBattleScene
    BattleBase <|-- BossBattleScene
    QuestManager o-- Quest
    GameManager -- QuestManager
    Quest -- QuestScene
    ItemSystem -- ShopScene
    BattleLoger -- BattleBase
    Enemy <|-- BossClass
    Enemy -- BattleBase
    GameManager -- Player
    Player -- Item
    Player -- Skill
    Skill -- SkillShopScene
    Item -- InvenScene
    Skill -- SkillInvenScene
    ItemSystem -- Item
    Player -- JeonJikScene

```


<br>

## 소요 기간 : 5일

<!-- Stack Icon Refernces -->

[csharp]: /media/Csharp.png
[dotnet]: /media/Dotnet.png



