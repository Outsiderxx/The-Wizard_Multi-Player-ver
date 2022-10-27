using System;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private ImageEffectManager imageEffectManager;
    [SerializeField] private Cinemachine.CinemachineVirtualCamera virtualCamera;
    [SerializeField] private Transform world;
    [SerializeField] private GameObject[] characterPrefabs;
    [SerializeField] private GameObject monsterPrefab;
    [SerializeField] private Vector3 defaultCharacterSpawnPosition;

    private CharacterState character;


    private void Awake()
    {
        SavedRecord savedRecord = GameManager.Instance.savedRecord;
        if (savedRecord == null)
        {
            this.LoadLevelWithDefaultData();
        }
        else
        {
            this.LoadLevelWithSavedData(savedRecord);
        }
    }

    private void Start()
    {
        this.character.OnDead += () =>
        {
            this.imageEffectManager.EnableGrayScaleEffect();
        };
        this.virtualCamera.Follow = this.character.transform;
        this.imageEffectManager.ShowOpenSceneBlurTransition();
    }

    public SavedRecord GetCurrentState()
    {
        SavedRecord savedRecord = new SavedRecord();
        SavedLevelData savedLevelData = new SavedLevelData();
        SavedCharacterData savedCharacterData = new SavedCharacterData();
        savedRecord.levelData = savedLevelData;
        savedRecord.characterData = savedCharacterData;

        // common
        savedRecord.time = (long)DateTimeConverter.ToTimestamp(DateTime.Now);

        // level
        savedLevelData.currentLevelIndex = GameManager.levelNames.Select((name, index) => new { name, index }).First((each) => each.name == SceneManager.GetActiveScene().name).index;
        Monster[] monsters = this.world.GetComponentsInChildren<Monster>();
        savedLevelData.monsters = monsters.Select((monster) =>
        {
            SavedMonsterData savedMonsterData = new SavedMonsterData();
            Vector3 position = monster.transform.position;
            savedMonsterData.currentHealthPoints = monster.currentHealthPoints;
            savedMonsterData.currentPosition = new float[] { position.x, position.y, position.z };
            return savedMonsterData;
        }).ToArray();

        // character
        savedCharacterData.chosenCharacterIndex = GameManager.Instance.chosenCharacterIndex;
        savedCharacterData.currentHealthPoints = this.character.currentHealthPoints;
        savedCharacterData.currentMagicPoints = this.character.currentMagicPoints;
        CharacterSkillSet skillset = this.character.GetComponent<CharacterSkillSet>();
        savedCharacterData.leftNormalAttackCD = skillset.leftNormalAttackCD;
        savedCharacterData.leftSkillACD = skillset.leftSkillACD;
        savedCharacterData.leftSkillBCD = skillset.leftSkillBCD;
        Vector3 position = this.character.transform.position;
        savedCharacterData.currentPositions = new float[] { position.x, position.y, position.z };

        return savedRecord;
    }

    private void LoadLevelWithDefaultData()
    {
        // spawn player
        this.character = this.SpawnCharacter(this.defaultCharacterSpawnPosition);
    }

    private void LoadLevelWithSavedData(SavedRecord savedRecord)
    {
        // spawn player
        SavedCharacterData characterData = savedRecord.characterData;
        Vector3 characterPosition = new Vector3(characterData.currentPositions[0], characterData.currentPositions[1], characterData.currentPositions[2]);
        this.character = this.SpawnCharacter(characterPosition);
        this.character.RecoverState(characterData);
        this.character.GetComponent<CharacterSkillSet>().RecoverState(characterData);

        // destroy existed monster
        foreach (Transform child in this.world)
        {
            if (child.GetComponent<Monster>())
            {
                Destroy(child.gameObject);
            }
        }

        // spawnMonster
        foreach (SavedMonsterData monsterData in savedRecord.levelData.monsters)
        {
            Vector3 monsterPosition = new Vector3(monsterData.currentPosition[0], monsterData.currentPosition[1], monsterData.currentPosition[2]);
            Monster monster = this.SpawnMonsters(monsterPosition);
            monster.RecoverState(monsterData.currentHealthPoints);
        }
    }

    private CharacterState SpawnCharacter(Vector3 spawnPosition)
    {
        CharacterState character = Instantiate(this.characterPrefabs[GameManager.Instance.chosenCharacterIndex], spawnPosition, Quaternion.identity).GetComponent<CharacterState>();
        character.transform.parent = this.world;
        if (character == null)
        {
            throw new System.Exception("GameObject doesn't have character state component");
        }
        return character;
    }

    private Monster SpawnMonsters(Vector3 spawnPosition)
    {
        Monster monster = Instantiate(this.monsterPrefab, spawnPosition, Quaternion.identity).GetComponent<Monster>();
        monster.transform.parent = this.world;
        if (monster == null)
        {
            throw new System.Exception("GameObject doesn't have monster component");
        }
        return monster;
    }
}
