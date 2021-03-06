using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wep_Sword : WeaponBase
{
	public Wep_Sword(Player player, Animator animator)
	{
		_Player = player;
		_Animator = animator;
		_isCancelable = false;

		_Property.Name = "Sword";
		_Property.Type = eWeaponType.Melee;
	}

	public override void Attack(eCommands direction, eCommands key)
	{
		if (_Player.GetState() != CharacterBase.eState.Idle && _Player.GetState() != CharacterBase.eState.Move && !_isCancelable) // 서있거나 이동중이 아니고 캔슬이 불가능할 때
			return;
		if (_Player.GetComponent<Rigidbody2D>().velocity.y != 0) // 공중에 있을 때
			return;
		if (_isCancelable && ((direction == eCommands.None) || (direction == eCommands.Front && key == eCommands.Left))) // 캔슬이 가능하지만 입력한 커맨드가 평타일 때
			return;

		bool isAttacked = false;
		switch (direction)
		{
			case eCommands.None:
				if (key == eCommands.Left)
				{
					switch(_ComboCounter)
					{
						case 0: PlayAnimation("Player_Sword_Swing1", out isAttacked); break;
						case 1: PlayAnimation("Player_Sword_Swing2", out isAttacked); break;
						case 2: PlayAnimation("Player_Sword_Swing3", out isAttacked); break;
					}
					_ComboCounter = (_ComboCounter + 1) % 3;
				}
				break;

			case eCommands.Front:
				if (key == eCommands.Right)
					PlayAnimation("Player_Sword_ShieldSlam", out isAttacked);
				else if (key == eCommands.Left)
				{
					switch (_ComboCounter)
					{
						case 0: PlayAnimation("Player_Sword_Swing1", out isAttacked); break;
						case 1: PlayAnimation("Player_Sword_Swing2", out isAttacked); break;
						case 2: PlayAnimation("Player_Sword_Swing3", out isAttacked); break;
					}
					_ComboCounter = (_ComboCounter + 1) % 3;
				}
				break;

			case eCommands.Down:
				if (key == eCommands.Left)
					PlayAnimation("Player_Sword_Parrying_Ready", out isAttacked);
				break;
		}

		if (isAttacked == false && _isCancelable == false) // 입력한 키에 맞는 공격이 없을 때
		{
			_Player.SetState(CharacterBase.eState.Idle);
			return;
		}
		base.Attack(direction, key);
	}
	public override void HandleAnimationEvents(eWeaponEvents weaponEvent)
	{
		base.HandleAnimationEvents(weaponEvent);
	}
}
