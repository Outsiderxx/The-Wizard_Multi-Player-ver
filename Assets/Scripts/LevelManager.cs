using System;
using System.Linq;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

public class LevelManager : MonoBehaviourPunCallbacks
{
    public static LevelManager Instance { get; private set; }

    [SerializeField] private ImageEffectManager imageEffectManager;
    [SerializeField] private Cinemachine.CinemachineVirtualCamera virtualCamera;
    [SerializeField] private Transform monsterRoot;
    [SerializeField] private Transform characterRoot;
    [SerializeField] private PlayerUIController playerUI;
    [SerializeField] private GameObject[] characterPrefabs;
    [SerializeField] private GameObject monsterPrefab;
    [SerializeField] private Vector3 defaultCharacterSpawnPosition;
    [SerializeField] private Vector3 defaultCharacterTwoSpawnPosition;

    private CharacterState character;
    private CharacterState otherCharacter;
    private PhotonView view;

    public int alivePlayerCount
    {
        get
        {
            int count = 0;
            if (this.character.isAlive)
            {
                count++;
            }
            if (this.otherCharacter && this.otherCharacter.isAlive)
            {
                count++;
            }
            return count;
        }
    }

    private void Awake()
    {
        LevelManager.Instance = this;
        ExitGames.Client.Photon.Hashtable customProperties = PhotonNetwork.LocalPlayer.CustomProperties;
        customProperties["sceneLoaded"] = true;
        PhotonNetwork.LocalPlayer.SetCustomProperties(customProperties);
        this.view = this.GetComponent<PhotonView>();
        this.StartCoroutine(this.StartWhenAllPlayerLoaded());
    }

    private IEnumerator StartWhenAllPlayerLoaded()
    {
        while (true)
        {
            if (PhotonNetwork.PlayerListOthers.All(player =>
            {
                return player.CustomProperties["sceneLoaded"] != null && (bool)player.CustomProperties["sceneLoaded"];
            }))
            {
                break;
            }
            else
            {
                print("wait");
            }
            yield return null;
        }
        SavedRecord savedRecord = GameManager.Instance.savedRecord;
        if (savedRecord == null)
        {
            this.LoadLevelWithDefaultData();
        }
        else
        {
            this.LoadLevelWithSavedData(savedRecord);
        }
        this.playerUI.BindCharacter(this.character, this.character.GetComponent<CharacterSkillSet>());
        this.character.OnDead += () =>
        {
            if (this.alivePlayerCount == 0)
            {
                this.imageEffectManager.EnableGrayScaleEffect();
                GameManager.Instance.ShowGameResult(false);
            }
            else if (this.otherCharacter)
            {
                this.virtualCamera.Follow = this.otherCharacter.transform;
            }
        };
        this.virtualCamera.Follow = this.character.transform;
        this.imageEffectManager.ShowOpenSceneBlurTransition();
    }

    public void BindOtherCharacter(CharacterState character)
    {
        this.otherCharacter = character;
        this.otherCharacter.OnDead += () =>
        {
            if (this.alivePlayerCount == 0)
            {
                this.imageEffectManager.EnableGrayScaleEffect();
                GameManager.Instance.ShowGameResult(false);
            }
        };
    }

    public void GoToNextStage()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            this.view.RPC("RPC_GoToNextStage", RpcTarget.All);
        }
    }

    [PunRPC]
    public void RPC_GoToNextStage()
    {
        GameManager.Instance.GoToNextStage();
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
        Monster[] monsters = this.monsterRoot.GetComponentsInChildren<Monster>();
        savedLevelData.monsters = monsters.Select((monster) =>
        {
            SavedMonsterData savedMonsterData = new SavedMonsterData();
            Vector3 position = monster.transform.position;
            savedMonsterData.currentHealthPoints = monster.currentHealthPoints;
            savedMonsterData.currentPosition = new float[] { position.x, position.y, position.z };
            return savedMonsterData;
        }).ToArray();

        // character
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
        this.character = this.SpawnCharacter(PhotonNetwork.LocalPlayer);
    }

    private void LoadLevelWithSavedData(SavedRecord savedRecord)
    {
        // spawn player
        SavedCharacterData characterData = savedRecord.characterData;
        Vector3 characterPosition = new Vector3(characterData.currentPositions[0], characterData.currentPositions[1], characterData.currentPositions[2]);
        this.character = this.SpawnCharacter(PhotonNetwork.LocalPlayer, characterPosition);
        this.character.RecoverState(characterData);
        this.character.GetComponent<CharacterSkillSet>().RecoverState(characterData);

        // destroy existed monster
        foreach (Transform child in this.monsterRoot)
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

    private CharacterState SpawnCharacter(Player playerData, Vector3? overridePos = null)
    {
        Vector3 spawnPosition = playerData.IsMasterClient ? this.defaultCharacterSpawnPosition : this.defaultCharacterTwoSpawnPosition;
        if (overridePos != null)
        {
            spawnPosition = (Vector3)overridePos;
        }
        CharacterState character = PhotonNetwork.Instantiate(this.characterPrefabs[(int)playerData.CustomProperties["chooseCharacterIndex"]].name, spawnPosition, Quaternion.identity).GetComponent<CharacterState>();
        if (character == null)
        {
            throw new System.Exception("GameObject doesn't have character state component");
        }
        return character;
    }

    private Monster SpawnMonsters(Vector3 spawnPosition)
    {
        Monster monster = Instantiate(this.monsterPrefab, spawnPosition, Quaternion.identity).GetComponent<Monster>();
        monster.transform.parent = this.monsterRoot;
        if (monster == null)
        {
            throw new System.Exception("GameObject doesn't have monster component");
        }
        return monster;
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (otherPlayer != PhotonNetwork.LocalPlayer)
        {
            this.otherCharacter = null;
        }
    }
}
