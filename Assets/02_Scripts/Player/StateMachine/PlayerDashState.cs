using UnityEngine;

public class PlayerDashState : PlayerStateBase
{
    float _timer;
    float _dashDuration = 0.5f;

    public PlayerDashState(Player player) : base(player)
    {
    }

    public override void Enter()
    {
        _timer = 0f;
        _player.SetIsDashing(true);
        _player.Dash();
    }

    public override void Update()
    {
        _timer += Time.deltaTime;
        if (_timer >= _dashDuration)
        {
            _player.SetToIdle();
        }
    }

    public override void Exit()
    {
        _player.SetIsDashing(false);
    }
}