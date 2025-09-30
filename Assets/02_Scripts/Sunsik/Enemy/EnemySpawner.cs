using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 일정 시간 간격으로 적 캐릭터를 생성하는 역할.
/// 최대 스폰 수, 스폰 반경, 생성된 적 캐릭터 리스트 관리
/// </summary>
public class EnemySpawner : MonoBehaviour
{
    [Header("----- 풀 매니저(읽기 전용) -----")]
    [SerializeField] PoolManager _poolManager; // 풀 매니저

    [Header("----- 동적으로 로드한 적 프리펩 변수(읽기 전용) -----")]
    [SerializeField] GameObject _enemyPrefab;    // Enemy 프리팹
    
    [Header("----- 적 생성 -----")]
    [SerializeField] string _enemyPrefabPath; // Enemy 프리팹 에셋이 저장되어 있는 경로
    [SerializeField] float _spawnSpan;      // 적 생성 간격(초)
    [SerializeField] int _maxSpawnCount;    // 최대 스폰 개수
    [SerializeField] float _spawnRadius;    // 적 스폰 반경
    [SerializeField] Hero _hero;            // Hero

    [Header("----- 적 목록(읽기 전용) -----")]
    [SerializeField] List<Enemy> _enemies = new(); // 생성된 적 캐릭터 목록

    Coroutine _spawnEnemyRoutine;       // 적 생성 코루틴 변수

    private void Start()
    {
        //_enemyPrefab = GameManager.Instance.ResourceManager.LoadPrefab(_enemyPrefabPath);
        //if (_enemyPrefab == null)
        //{
        //    return;
        //}
        _poolManager = GameManager.Instance.PoolManager;

        // 적 생성 코루틴 시작
        _spawnEnemyRoutine = StartCoroutine(SpawnEnemyRoutine());
    }

    void Initialize()
    {
        //GameObject enemyGo = Resources.Load<GameObject>(_enemyPrefabPath);
        //if (enemyGo == null)
        //{
        //    Debug.LogError($"{_enemyPrefabPath} 경로에 Enemy 프리팹이 없습니다.");
        //    return;
        //}

        //_enemyPrefab = enemyGo.GetComponent<Enemy>();
        //if(_enemyPrefab == null)
        //{
        //    Debug.LogError($"{_enemyPrefabPath} 프리팹에 Enemy 컴포넌트가 없습니다.");
        //    return;
        //}
    }

    /// <summary>
    /// 주기적으로 적을 생성하는 코루틴
    /// </summary>
    /// <returns></returns>
    IEnumerator SpawnEnemyRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(_spawnSpan);
            SpawnEnemy();
        }
    }

    ///// <summary>
    ///// 적을 생성하는 함수
    ///// </summary>
    //void SpawnEnemy()
    //{
    //    // 이미 생성된 적 수가 생성 가능한 최대 수보다 크거나 같으면
    //    if(_enemies.Count >= _maxSpawnCount)
    //    {
    //        return;
    //    }
        
    //    // 1. Instantiate로 복제본 생성
    //    GameObject enemyGo = Instantiate(_enemyPrefab, transform);
    //    Enemy enemy = enemyGo.GetComponent<Enemy>();
    //    if (enemy == null) return;

    //    // 2. 복제본의 위치 설정
    //    enemy.transform.position = GetRandomPos();

    //    // 3. 복제본 초기화
    //    enemy.Initialize(_hero.transform);

    //    // 4. 복제본을 리스트에 추가
    //    _enemies.Add(enemy);
        
    //    // (5. 생성된 복제본 이벤트 구독)
    //    enemy.OnRemoved += OnEnemyRemoved; // 적 제거 이벤트 구독
    //    // 여기까지가 템플릿임
    //}
    
    /// <summary>
    /// 적을 생성하는 함수
    /// </summary>
    void SpawnEnemy()
    {
        // 이미 생성된 적 수가 생성 가능한 최대 수보다 크거나 같으면
        if(_enemies.Count >= _maxSpawnCount)
        {
            return;
        }
        
        // 1. Instantiate로 복제본 생성
        GameObject enemyGo = _poolManager.GetFromPool(_enemyPrefabPath);
        if(enemyGo == null)
        {
            Debug.LogError($"풀에서 {_enemyPrefabPath} 경로에 해당하는 적을 가져올 수 없습니다.");
            return;
        }
        Enemy enemy = enemyGo.GetComponent<Enemy>();
        if (enemy == null) return;

        // 2. 복제본의 위치 설정
        enemy.transform.SetParent(transform); // 부모 설정
        enemy.transform.position = GetRandomPos();

        // 3. 복제본 초기화
        //enemy.Initialize(_hero.transform);

        // 4. 복제본을 리스트에 추가
        _enemies.Add(enemy);
        
        // (5. 생성된 복제본 이벤트 구독)
        //enemy.OnRemoved += OnEnemyRemoved; // 적 제거 이벤트 구독
        // 여기까지가 템플릿임
    }

    /// <summary>
    /// 범위 내의 랜덤 위치(포지션)를 반환하는 함수
    /// </summary>
    /// <returns>랜덤한 위치</returns>
    Vector3 GetRandomPos()
    {
        Vector3 pos = transform.position;
        Vector2 randomDir = Random.insideUnitCircle.normalized; // 랜덤한 방향 벡터 생성
        float randomRadius = Random.Range(0, _spawnRadius); // 랜덤한 반경 값 생성
        pos.x += randomDir.x * randomRadius; // x축에 랜덤 방향 벡터를 곱한 값을 더함
        pos.z += randomDir.y * randomRadius; // z축에 랜덤 방향 벡터를 곱한 값을 더함

        return pos;
    }

    /// <summary>
    /// 적이 제거되었을 때 자동으로 호출되는 함수
    /// 해당 Enemy를 Enemy 리스트(_enemies)에서 제거한다.
    /// </summary>
    /// <param name="enemy"></param>
    void OnEnemyRemoved(Enemy enemy)
    {
        _enemies.Remove(enemy);
    }

    private void OnDrawGizmosSelected()
    {
        // 적 생성 반경을 시각적으로 표시
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _spawnRadius);
    }
    
}
