using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DebuffHandler : MonoBehaviour
{

    [SerializeField]
    private Dictionary<string, ActiveDebuff> activeDebuffs = new Dictionary<string, ActiveDebuff>();
    private MonsterBase monster;
    private NavMeshAgent agent;
    private float originalSpeed;

    private void Awake()
    {
        monster = GetComponent<MonsterBase>();
        agent = GetComponent<NavMeshAgent>();
        originalSpeed = agent.speed;
    }

    public class DebuffData
    {
        public string debuffName;      // 디버프 이름
        public float debuffDuration;   // 지속시간
        public float debuffValue;      // 초당 대미지나 배수값
        public int maxStack = 1;           // 최대 스택

        public DebuffData(string name, float duration, float value, int Maxstack = 1)
        {
            debuffName = name;
            debuffDuration = duration;
            debuffValue = value;
            maxStack = Maxstack;
        }



        /// <summary>
        /// 슬로우 제작 함수
        /// </summary>
        /// <param name="duration"> 슬로우 지속시간</param>
        /// <param name="slowValue"> 슬로우 수치 %단위로 입력할것 (예 : 30% 슬로우 = 30 입력)</param>
        /// <returns></returns>
        public static DebuffData CreateSlow(float duration = 3f, float slowValue = 30f)
        {
            return new DebuffData(Constants.DEBUFF_SLOW, duration, 1 - slowValue / 100);
        }


        /// <summary>
        /// 독 제작 함수
        /// </summary>
        /// <param name="duration"> 독 지속시간</param>
        /// <param name="damagePerSecond"> 독 초당 대미지</param>
        /// <returns></returns>
        public static DebuffData CreatePoison(float duration = 5f, float damagePerSecond = 10f)
        {
            return new DebuffData(Constants.DEBUFF_POISON, duration, damagePerSecond, 5);
        }

        /// <summary>
        /// 과부하 제작 함수
        /// </summary>
        /// <param name="duration"> 과부하 지속 시간</param>
        /// <param name="damagePerSecond"> 과부하 초당 대미지</param>
        /// <returns></returns>
        public static DebuffData CreateOverload(float duration = 4f, float damagePerSecond = 15f)
        {
            return new DebuffData(Constants.DEBUFF_OVERLOAD, duration, damagePerSecond, 5);
        }

        /// <summary>
        /// 스턴 제작 함수
        /// </summary>
        /// <param name="duration"> 스턴 지속시간</param>
        /// <returns></returns>
        public static DebuffData CreateStun(float duration = 2f)
        {
            return new DebuffData(Constants.DEBUFF_STUN, duration, 0f);
        }

        /// <summary>
        /// 혼란 제작 함수.
        /// </summary>
        /// <param name="duration"> 혼란의 지속시간 </param>
        public static DebuffData CreateConfusion(float duration = 3f)
        {
            return new DebuffData(Constants.DEBUFF_CONFUSION, duration, 0f);
        }
    }

    [SerializeField]
    private class ActiveDebuff
    {
        public DebuffData data;
        public float remainingTime;
        public float totalValue;
        public int stackCount;
        public Coroutine routine;
        public Coroutine effectRoutine; // 스턴, 혼란 특별 관리용 코루틴 저장 변수

        public ActiveDebuff(DebuffData data)
        {
            this.data = data;
            remainingTime = data.debuffDuration;
            totalValue = data.debuffValue;
            stackCount = 1;
            this.routine = null;
            this.effectRoutine = null;
        }
    }


    /// <summary>
    /// 디버프 적용 함수
    /// </summary>
    public void ApplyDebuff(DebuffData newDebuff)
    {
        if (monster == null || monster.isDead)
            return;

        if (activeDebuffs.ContainsKey(newDebuff.debuffName))
        {
            ActiveDebuff existing = activeDebuffs[newDebuff.debuffName];

            // 스택형 디버프들 독 / 과부하
            if (newDebuff.debuffName == Constants.DEBUFF_POISON || newDebuff.debuffName == Constants.DEBUFF_OVERLOAD)
            {
                if (existing.stackCount < newDebuff.maxStack)
                {
                    existing.stackCount++;
                    existing.totalValue += newDebuff.debuffValue;
                }
                existing.remainingTime = newDebuff.debuffDuration;
            }

            // 그 외 디버프들. 스턴, 혼란일 경우 기존 코루틴 중단 후 새 코루틴으로 다시 시작
            else
            {
                existing.remainingTime = newDebuff.debuffDuration;
                existing.totalValue = newDebuff.debuffValue;

                if (newDebuff.debuffName == Constants.DEBUFF_STUN)
                {
                    if (existing.effectRoutine != null)
                    {
                        StopCoroutine(existing.effectRoutine);
                    }
                    existing.effectRoutine = StartCoroutine(StunCoroutine(newDebuff.debuffDuration));
                }
                else if (newDebuff.debuffName == Constants.DEBUFF_CONFUSION)
                {
                    if (existing.effectRoutine != null)
                    {
                        StopCoroutine(existing.effectRoutine);
                    }
                    existing.effectRoutine = StartCoroutine(ConfuseCoroutine(newDebuff.debuffDuration));
                }
            }

            return;
        }
        activeDebuffs.Add(newDebuff.debuffName, new ActiveDebuff(newDebuff));
        activeDebuffs[newDebuff.debuffName].routine = StartCoroutine(HandleDebuff(newDebuff));
    }

    private IEnumerator HandleDebuff(DebuffData debuff)
    {
        // 혹시모를 타이밍 오류 방지용 건드리지 말것
        yield return null;
        ActiveDebuff active = activeDebuffs[debuff.debuffName];

        // 시작 시 1회만 받는 효과들
        switch (debuff.debuffName)
        {
            // 슬로우
            case Constants.DEBUFF_SLOW:
                if (agent != null)
                {
                    agent.speed = originalSpeed * active.totalValue;
                }
                break;

            // 스턴
            case Constants.DEBUFF_STUN:
                active.effectRoutine = StartCoroutine(StunCoroutine(debuff.debuffDuration));
                break;

            // 혼란
            case Constants.DEBUFF_CONFUSION:
                active.effectRoutine = StartCoroutine(ConfuseCoroutine(debuff.debuffDuration));
                break;
        }

        while (active.remainingTime > 0f && !monster.isDead)
        {
            switch (debuff.debuffName)
            {
                case Constants.DEBUFF_POISON:
                case Constants.DEBUFF_OVERLOAD:
                    Debug.Log("takeDamage " + active.totalValue);
                    monster.TakeDamage(active.totalValue);
                    break;
            }

            active.remainingTime -= 1f;
            yield return new WaitForSeconds(1f);
        }

        // 해제 시 복원 - 현재 슬로우 이외의 다른 복원 요소 없음
        switch (debuff.debuffName)
        {
            case Constants.DEBUFF_SLOW:
                if (agent != null)
                {
                    agent.speed = originalSpeed;
                }
                break;
        }

        activeDebuffs.Remove(debuff.debuffName);
    }

    private IEnumerator StunCoroutine(float duration)
    {
        monster.isStunned = true;

        if (agent != null)
        {
            agent.isStopped = true;
        }

        yield return new WaitForSeconds(duration);

        if (agent != null && !monster.isDead)
        {
            agent.isStopped = false;
        }

        monster.isStunned = false;
    }

    private IEnumerator ConfuseCoroutine(float duration)
    {
        string originalTag = monster.targetTag;
        monster.targetTag = Constants.TAG_MONSTER;

        yield return new WaitForSeconds(duration);

        monster.targetTag = originalTag;
    }

    public bool CheckDebuff(DebuffData data)
    {
        return activeDebuffs.ContainsKey(data.debuffName);
    }

    public bool HasDebuff(string debuffName)
    {
        return activeDebuffs.ContainsKey(debuffName);
    }

    public int GetActiveDebuffCount()
    {
        int count = 0;

        foreach (KeyValuePair<string, ActiveDebuff> pair in activeDebuffs)
        {
            ActiveDebuff debuff = pair.Value;
            if (debuff != null && debuff.remainingTime > 0f)
            {
                count++;
            }
        }

        return count;
    }

    public int GetStackCount(string debuffName)
    {
        if (activeDebuffs.ContainsKey(debuffName))
        {
            return activeDebuffs[debuffName].stackCount;
        }
        return 0;
    }

    /// <summary>
    /// 현재 걸린 디버프를 제거하는 함수.디버프 이름을 받아 activeDebuffs에서 활성화중인 코루틴을 제거.
    /// </summary>
    public void RemoveDebuff(string debuffName)
    {
        if (activeDebuffs.ContainsKey(debuffName))
        {
            ActiveDebuff debuff = activeDebuffs[debuffName];

            if (debuff.routine != null)
            {
                StopCoroutine(debuff.routine);
            }

            if (debuff.effectRoutine != null)
            {
                StopCoroutine(debuff.effectRoutine);
            }

            // 수동 복원
            switch (debuffName)
            {
                case Constants.DEBUFF_SLOW:
                    if (agent != null)
                    {
                        agent.speed = originalSpeed;
                    }
                    break;

                case Constants.DEBUFF_STUN:
                    if (!monster.isDead)
                    {
                        if (agent != null)
                        {
                            agent.isStopped = false;
                        }
                        monster.isStunned = false;
                    }
                    break;

                case Constants.DEBUFF_CONFUSION:
                    monster.targetTag = Constants.TAG_PLAYER;
                    break;
            }

            activeDebuffs.Remove(debuffName);
        }
    }
}
